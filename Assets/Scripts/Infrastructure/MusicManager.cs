using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PickMe.Infrastructure
{
    /// <summary>
    /// Handles background music playback with volume driven by SettingsManager.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : PersistentSingleton<MusicManager>
    {
        private AudioSource _audioSource;
        private readonly Dictionary<string, AudioClip> _cache = new();

        protected override IEnumerator OnInitialized()
        {
            // Wait for event bus and settings to be ready
            yield return EventController.WaitUntilInitialized();
            yield return SettingsManager.WaitUntilInitialized();

            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = true;
            _audioSource.volume = SettingsManager.Instance.MusicVolume;
            EventController.Instance.Subscribe<MusicVolumeChanged>(HandleVolumeChanged);
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null) return;

            _audioSource.loop = loop;
            _audioSource.clip = clip;
            _audioSource.volume = SettingsManager.Instance.MusicVolume;
            _audioSource.Play();
        }

        /// <summary>
        /// Loads an AudioClip from Resources and plays it.
        /// Example path: "Audio/Music/track01" (without extension).
        /// </summary>
        public void PlayMusicByName(string resourcePath, bool loop = true)
        {
            var clip = LoadClip(resourcePath);
            PlayMusic(clip, loop);
        }

        private AudioClip LoadClip(string resourcePath)
        {
            if (string.IsNullOrWhiteSpace(resourcePath)) return null;
            if (_cache.TryGetValue(resourcePath, out var cached)) return cached;

            var clip = Resources.Load<AudioClip>(resourcePath);
            if (clip == null)
            {
                Debug.LogWarning($"MusicManager: clip not found at Resources/{resourcePath}");
                return null;
            }

            _cache[resourcePath] = clip;
            return clip;
        }

        public void StopMusic()
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }
        }

        private void HandleVolumeChanged(MusicVolumeChanged evt)
        {
            _audioSource.volume = evt.Value;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (EventController.IsInitialized)
            {
                EventController.Instance.Unsubscribe<MusicVolumeChanged>(HandleVolumeChanged);
            }
        }
    }
}

