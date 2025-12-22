using System;
using UnityEngine;

namespace PickMe.Gameplay.Data
{
    [Serializable]
    public class CharacterData
    {
        public string id;
        public string ch_name;
        public CharacterClassTag class_tag;
        public float base_hp;
        public float base_atk;
        public float atk_range;
        public float atk_speed;
        public float move_speed;
        public float ability_chance;
        public bool has_ability;
        public bool is_dead;

        // runtime
        public float current_hp;
        public AbilityData ability;

        public CharacterData Clone()
        {
            return (CharacterData)MemberwiseClone();
        }
    }
}

