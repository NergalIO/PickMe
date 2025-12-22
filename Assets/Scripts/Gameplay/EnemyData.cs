using System;
using UnityEngine;

namespace PickMe.Gameplay.Data
{
    [Serializable]
    public class EnemyData
    {
        public string id;
        public string name;
        public float hp;
        public float atk;
        public float atk_range;
        public float atk_speed;
        public float move_speed;
    }
}

