namespace PickMe.Infrastructure
{
    /// <summary>
    /// Event payloads for audio settings changes.
    /// </summary>
    public readonly struct MusicVolumeChanged
    {
        public float Value { get; }
        public MusicVolumeChanged(float value) => Value = value;
    }

    public readonly struct SfxVolumeChanged
    {
        public float Value { get; }
        public SfxVolumeChanged(float value) => Value = value;
    }
}

