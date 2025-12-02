using System;
using UnityEngine;
namespace PickMe.Data
{
    [Serializable]
    public class EnemyData
    {
        public string enemy_type;
        public int base_hp;
        public int base_atk;
        public float atk_range;
        public float atk_speed;
        public float move_speed;
        [NonSerialized] public int current_hp;
        [NonSerialized] public int current_atk;
        public EnemyData()
        {
            enemy_type = "goblin";
            base_hp = 50;
            base_atk = 8;
            atk_range = 1.0f;
            atk_speed = 1.0f;
            move_speed = 2.0f;
        }
        public void InitializeForCombat()
        {
            current_hp = base_hp;
            current_atk = base_atk;
        }
    }
    public enum EnemyType
    {
        Goblin
    }
}
