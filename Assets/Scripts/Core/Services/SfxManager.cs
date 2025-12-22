using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PickMe.Core.Infrastructure;
using PickMe.Core.Services;
using UnityEngine;

namespace PickMe.Core.Services
{
    /// <summary>
    /// Handles sound effects playback with volume driven by SettingsManager.
    /// Supports multiple simultaneous sound sources using a pool system.
    /// </summary>
    public class SfxManager : PersistentSingleton<SfxManager>
    {
        [Header("Pool Settings")]
        [SerializeField] private int _maxConcurrentSounds = 16;
        [SerializeField] private int _initialPoolSize = 4;

        private readonly Dictionary<string, AudioClip> _cache = new();
        private readonly List<AudioSource> _audioSourcePool = new();
        private readonly List<AudioSource> _activeSources = new();
        private Transform _sourcesRoot;
        private float _currentVolume = 1f;

        protected override IEnumerator OnInitialized()
        {
            yield return EventController.WaitUntilInitialized();
            yield return SettingsManager.WaitUntilInitialized();

            _currentVolume = SettingsManager.Instance.SfxVolume;
            InitializePool();
            EventController.Instance.Subscribe<SfxVolumeChanged>(HandleVolumeChanged);
        }

        private void InitializePool()
        {
            _sourcesRoot = new GameObject("SFX Sources").transform;
            _sourcesRoot.SetParent(transform);
            _sourcesRoot.localPosition = Vector3.zero;

            for (int i = 0; i < _initialPoolSize; i++)
            {
                CreatePooledSource();
            }
        }

        private AudioSource CreatePooledSource()
        {
            var go = new GameObject($"SFX_Source_{_audioSourcePool.Count}");
            go.transform.SetParent(_sourcesRoot);
            go.transform.localPosition = Vector3.zero;

            var audioSource = go.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = go.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.volume = _currentVolume;
            audioSource.spatialBlend = 0f; // 2D sound

            _audioSourcePool.Add(audioSource);
            return audioSource;
        }

        private AudioSource GetAvailableSource()
        {
            // Try to find an inactive source in the pool
            var available = _audioSourcePool.FirstOrDefault(source => source != null && !source.isPlaying);
            
            if (available != null)
            {
                return available;
            }

            // If pool is not at max capacity, create a new source
            if (_audioSourcePool.Count < _maxConcurrentSounds)
            {
                return CreatePooledSource();
            }

            // If at max capacity, reuse the oldest playing source
            if (_activeSources.Count > 0)
            {
                var oldest = _activeSources[0];
                if (oldest != null && oldest.isPlaying)
                {
                    oldest.Stop();
                }
                _activeSources.RemoveAt(0);
                return oldest;
            }

            return null;
        }

        private void LateUpdate()
        {
            // Clean up finished sources from active list
            for (int i = _activeSources.Count - 1; i >= 0; i--)
            {
                if (_activeSources[i] == null || !_activeSources[i].isPlaying)
                {
                    _activeSources.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Plays a sound effect using an available audio source from the pool.
        /// </summary>
        public void PlaySfx(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null) return;

            var source = GetAvailableSource();
            if (source == null)
            {
                Debug.LogWarning("SfxManager: No available audio source. Max concurrent sounds reached.");
                return;
            }

            var finalVolume = Mathf.Clamp01(_currentVolume * volumeScale);
            source.volume = finalVolume;
            source.PlayOneShot(clip, 1f); // volumeScale is already applied to source.volume

            if (!_activeSources.Contains(source))
            {
                _activeSources.Add(source);
            }
        }

        /// <summary>
        /// Loads an AudioClip from Resources and plays it.
        /// Example path: "Audio/SFX/click".
        /// </summary>
        public void PlaySfxByName(string resourcePath, float volumeScale = 1f)
        {
            var clip = LoadClip(resourcePath);
            PlaySfx(clip, volumeScale);
        }

        private AudioClip LoadClip(string resourcePath)
        {
            if (string.IsNullOrWhiteSpace(resourcePath)) return null;
            if (_cache.TryGetValue(resourcePath, out var cached)) return cached;

            var clip = Resources.Load<AudioClip>(resourcePath);
            if (clip == null)
            {
                Debug.LogWarning($"SfxManager: clip not found at Resources/{resourcePath}");
                return null;
            }

            _cache[resourcePath] = clip;
            return clip;
        }

        private void HandleVolumeChanged(SfxVolumeChanged evt)
        {
            _currentVolume = evt.Value;
            UpdateAllSourcesVolume();
        }

        private void UpdateAllSourcesVolume()
        {
            foreach (var source in _audioSourcePool)
            {
                if (source != null)
                {
                    source.volume = _currentVolume;
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (EventController.IsInitialized)
            {
                EventController.Instance.Unsubscribe<SfxVolumeChanged>(HandleVolumeChanged);
            }
        }
    }
}

