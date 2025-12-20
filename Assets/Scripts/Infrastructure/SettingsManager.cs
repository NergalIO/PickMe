using System.Collections;
using UnityEngine;

namespace PickMe.Infrastructure
{
    /// <summary>
    /// Stores and persists user settings such as audio volumes.
    /// All changes are broadcast through EventController.
    /// </summary>
    public class SettingsManager : PersistentSingleton<SettingsManager>
    {
        private static class PrefKeys
        {
            public const string MusicVolume = "settings_music_volume";
            public const string SfxVolume = "settings_sfx_volume";
        }

        private const float DefaultVolume = 1f;

        [Header("Default Values")]
        [Range(0f, 1f)]
        [SerializeField] private float _defaultMusicVolume = DefaultVolume;
        [Range(0f, 1f)]
        [SerializeField] private float _defaultSfxVolume = DefaultVolume;

        public float MusicVolume { get; private set; }
        public float SfxVolume { get; private set; }

        #region Initialization

        protected override IEnumerator OnInitialized()
        {
            yield return EventController.WaitUntilInitialized();
            LoadSettings();
            PublishSettings();
        }

        private void LoadSettings()
        {
            MusicVolume = PlayerPrefs.GetFloat(PrefKeys.MusicVolume, _defaultMusicVolume);
            SfxVolume = PlayerPrefs.GetFloat(PrefKeys.SfxVolume, _defaultSfxVolume);
            
            // Clamp loaded values to valid range
            MusicVolume = Mathf.Clamp01(MusicVolume);
            SfxVolume = Mathf.Clamp01(SfxVolume);
        }

        private void PublishSettings()
        {
            if (!EventController.IsInitialized) return;
            
            EventController.Instance.Publish(new MusicVolumeChanged(MusicVolume));
            EventController.Instance.Publish(new SfxVolumeChanged(SfxVolume));
        }

        #endregion

        #region Public API

        /// <summary>
        /// Sets the music volume and persists the change.
        /// </summary>
        public void SetMusicVolume(float value)
        {
            value = Mathf.Clamp01(value);
            
            // Skip if value hasn't changed
            if (Mathf.Approximately(MusicVolume, value))
            {
                return;
            }

            MusicVolume = value;
            PlayerPrefs.SetFloat(PrefKeys.MusicVolume, MusicVolume);
            SavePreferences();

            if (EventController.IsInitialized)
            {
                EventController.Instance.Publish(new MusicVolumeChanged(MusicVolume));
            }
        }

        /// <summary>
        /// Sets the SFX volume and persists the change.
        /// </summary>
        public void SetSfxVolume(float value)
        {
            value = Mathf.Clamp01(value);
            
            // Skip if value hasn't changed
            if (Mathf.Approximately(SfxVolume, value))
            {
                return;
            }

            SfxVolume = value;
            PlayerPrefs.SetFloat(PrefKeys.SfxVolume, SfxVolume);
            SavePreferences();

            if (EventController.IsInitialized)
            {
                EventController.Instance.Publish(new SfxVolumeChanged(SfxVolume));
            }
        }

        /// <summary>
        /// Resets all settings to default values.
        /// </summary>
        public void ResetToDefaults()
        {
            SetMusicVolume(_defaultMusicVolume);
            SetSfxVolume(_defaultSfxVolume);
        }

        #endregion

        #region Helpers

        private void SavePreferences()
        {
            PlayerPrefs.Save();
        }

        #endregion
    }
}

