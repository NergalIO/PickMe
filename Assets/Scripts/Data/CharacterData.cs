using System;
using UnityEngine;
namespace PickMe.Data
{
    [Serializable]
    public class CharacterData
    {
        public int id;
        public string class_tag;
        public int base_hp;
        public int base_atk;
        public float atk_range;
        public float atk_speed;
        public float move_speed;
        public float ability_chance;
        public bool has_ability;
        public bool is_dead;
        public string ch_name;
        [NonSerialized] public int current_hp;
        [NonSerialized] public int current_atk;
        public CharacterData()
        {
            id = 0;
            class_tag = "";
            base_hp = 100;
            base_atk = 10;
            atk_range = 1.5f;
            atk_speed = 1.0f;
            move_speed = 2.0f;
            ability_chance = 0.0f;
            has_ability = false;
            is_dead = false;
            ch_name = "";
        }
        public void InitializeForCombat()
        {
            current_hp = base_hp;
            current_atk = base_atk;
        }
        public CharacterClass GetCharacterClass()
        {
            if (Enum.TryParse<CharacterClass>(class_tag, true, out CharacterClass result))
            {
                return result;
            }
            return CharacterClass.Warrior;
        }
    }
    public enum CharacterClass
    {
        Warrior,
        Scout,
        Tank,
        Mage
    }
}
