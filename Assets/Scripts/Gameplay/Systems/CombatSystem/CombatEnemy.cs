using System.Collections.Generic;
using System.Linq;
using PickMe.Gameplay.Data;
using PickMe.Core.Infrastructure;
using UnityEngine;
using UnityEngine.AI;

namespace PickMe.Gameplay.Systems.CombatSystem
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
            allCharacters = characters ?? new List<CombatCharacter>();
            
            // Set stats from enemy data
            maxHp = data.hp;
            currentHp = data.hp;
            attack = data.atk;
            attackRange = data.atk_range * 10f; // Increase attack range by 10x
            attackSpeed = data.atk_speed;
            moveSpeed = data.move_speed;
            
            // Update NavMeshAgent settings after data is loaded
            if (navAgent != null)
            {
                navAgent.enabled = true;
                navAgent.speed = moveSpeed;
                // Set stoppingDistance based on attackRange: we want to stop when we can attack
                // stoppingDistance should be: agentRadius + targetRadius + attackRange
                // But we'll set it dynamically in MoveTowardsTarget, so set a reasonable default
                navAgent.stoppingDistance = 0.5f; // Will be updated dynamically
                navAgent.radius = 0.5f; // Set agent radius for obstacle avoidance
                navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
                navAgent.updateRotation = false; // Disable rotation for 2D movement
                navAgent.updateUpAxis = false; // Keep Z axis fixed
                
                // Ensure agent is on NavMesh
                if (NavMesh.SamplePosition(transform.position, out var hit, 5f, NavMesh.AllAreas))
                {
                    navAgent.Warp(hit.position);
                }
                else
                {
                    Debug.LogWarning($"Enemy {data.name}: Not on NavMesh at position {transform.position}. Make sure NavMesh is baked in the scene!");
                }
            }
            
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
                
                // Always try to attack if in range (even if moving)
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

