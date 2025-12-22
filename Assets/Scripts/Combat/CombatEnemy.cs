using System.Collections.Generic;
using System.Linq;
using PickMe.Gameplay;
using PickMe.Infrastructure;
using UnityEngine;

namespace PickMe.Combat
{
    /// <summary>
    /// Combat component for enemies (goblins).
    /// </summary>
    public class CombatEnemy : CombatUnit
    {
        [Header("Enemy Data")]
        [SerializeField] private EnemyData enemyData;
        
        private List<CombatCharacter> allCharacters = new();

        public void Initialize(EnemyData data, List<CombatCharacter> characters)
        {
            enemyData = data;
            allCharacters = characters;
            
            // Set stats from enemy data
            maxHp = data.hp;
            currentHp = data.hp;
            attack = data.atk;
            attackRange = data.atk_range;
            attackSpeed = data.atk_speed;
            moveSpeed = data.move_speed;
            
            // Initialize HP bar as enemy (red)
            SetIsAlly(false);
        }

        protected override void UpdateCombat()
        {
            if (isDead) return;
            
            // Remove dead characters from list
            allCharacters.RemoveAll(c => c == null || c.IsDead);
            
            // Select nearest character as target
            SelectNearestTarget();
            
            // Move and attack
            if (currentTarget != null)
            {
                MoveTowardsTarget();
                AttackTarget();
            }
        }

        private void SelectNearestTarget()
        {
            if (allCharacters.Count == 0)
            {
                currentTarget = null;
                return;
            }

            // Find nearest character
            CombatCharacter nearest = null;
            float nearestDistance = float.MaxValue;
            
            foreach (var character in allCharacters)
            {
                if (character.IsDead) continue;
                
                float distance = Vector3.Distance(transform.position, character.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = character;
                }
            }
            
            currentTarget = nearest;
        }
    }
}

