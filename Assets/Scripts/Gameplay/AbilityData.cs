using System;

namespace PickMe.Gameplay.Data
{
    [Serializable]
    public class AbilityData
    {
        public string ability_id;
        public string ability_name;
        public string activate;
        public string ability_effect;
        public float ability_duration;
        public float ability_cooldown;
    }
}

