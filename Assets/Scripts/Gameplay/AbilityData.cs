using System;

namespace PickMe.Gameplay
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

