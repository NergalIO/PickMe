using System;
using UnityEngine;

namespace PickMe.Gameplay.Data
{
    [Serializable]
    public struct CombatStats
    {
        [Min(0f)]
        public float baseHp;
        
        [Min(0f)]
        public float baseAtk;
        
        [Min(0f)]
        public float attackRange;
        
        [Min(0f)]
        public float attackSpeed;
        
        [Min(0f)]
        public float moveSpeed;

        public CombatStats(float baseHp = 100f, float baseAtk = 10f, float attackRange = 1.5f, float attackSpeed = 1f, float moveSpeed = 3f)
        {
            this.baseHp = baseHp;
            this.baseAtk = baseAtk;
            this.attackRange = attackRange;
            this.attackSpeed = attackSpeed;
            this.moveSpeed = moveSpeed;
        }

        public float BaseHp => baseHp;
        public float BaseAtk => baseAtk;
        public float AttackRange => attackRange;
        public float AttackSpeed => attackSpeed;
        public float MoveSpeed => moveSpeed;
    }
}

