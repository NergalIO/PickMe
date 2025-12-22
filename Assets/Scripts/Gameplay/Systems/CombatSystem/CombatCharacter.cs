using System.Collections.Generic;
using System.Linq;
using PickMe.Gameplay.Data;
using PickMe.Core.Infrastructure;
using UnityEngine;
using UnityEngine.AI;

namespace PickMe.Gameplay.Systems.CombatSystem
{
    /// <summary>
    /// Combat component for player characters with class-specific AI behavior.
    /// </summary>
    public class CombatCharacter : CombatUnit
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Character Data")]
        [SerializeField] private CharacterData characterData;
        
        [Header("Character Sprites")]
        [SerializeField] private Sprite warriorSprite;
        [SerializeField] private Sprite scoutSprite;
        [SerializeField] private Sprite tankSprite;
        [SerializeField] private Sprite mageSprite;
        
        private List<CombatEnemy> allEnemies = new();
        private CombatCharacter tankInTeam;
        private float abilityCooldown;
        private bool abilityActive;
        private float abilityEndTime;
        private float attackMultiplier = 1f;
        
        public CharacterData CharacterData => characterData;

        public void Initialize(CharacterData data, List<CombatEnemy> enemies)
        {
            characterData = data;
            allEnemies = enemies ?? new List<CombatEnemy>();
            
            // Set stats from character data
            maxHp = data.base_hp;
            currentHp = data.current_hp > 0 ? data.current_hp : data.base_hp;
            attack = data.base_atk;
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
                    Debug.LogWarning($"{data.ch_name}: Not on NavMesh at position {transform.position}. Make sure NavMesh is baked in the scene!");
                }
            }
            
            // Set sprite based on class
            SetCharacterSprite(data.class_tag);
            
            // Initialize HP bar as ally (green)
            SetIsAlly(true);
            
            // Find tank in team
            FindTankInTeam();
        }

        /// <summary>
        /// Sets the character sprite based on class tag.
        /// </summary>
        private void SetCharacterSprite(CharacterClassTag classTag)
        {
            Sprite sprite = GetSpriteForClass(classTag);
            if (sprite != null)
            {
                spriteRenderer.sprite = sprite;
            }
            else
            {
                Debug.LogWarning($"CombatCharacter: No sprite found for class {classTag}");
            }
        }

        /// <summary>
        /// Gets the sprite for the specified class tag.
        /// </summary>
        private Sprite GetSpriteForClass(CharacterClassTag classTag)
        {
            return classTag switch
            {
                CharacterClassTag.Warrior => warriorSprite,
                CharacterClassTag.Scout => scoutSprite,
                CharacterClassTag.Tank => tankSprite,
                CharacterClassTag.Mage => mageSprite,
                _ => null
            };
        }

        /// <summary>
        /// Updates the enemy list (called after all enemies are spawned).
        /// </summary>
        public void UpdateEnemyList(List<CombatEnemy> enemies)
        {
            allEnemies = enemies ?? new List<CombatEnemy>();
        }

        private void FindTankInTeam()
        {
            var combatCharacters = FindObjectsByType<CombatCharacter>(FindObjectsSortMode.None);
            tankInTeam = combatCharacters.FirstOrDefault(c => 
                c != this && 
                c.characterData != null && 
                c.characterData.class_tag == CharacterClassTag.Tank &&
                !c.IsDead);
        }

        protected override void UpdateCombat()
        {
            if (isDead) return;
            
            // Update ability
            UpdateAbility();
            
            // Find target based on class
            SelectTarget();
            
            // Move and attack
            if (currentTarget != null)
            {
                MoveTowardsTarget();
                
                // Always try to attack if in range (even if moving)
                AttackTarget();
            }
        }

        private void SelectTarget()
        {
            // Remove dead enemies from list
            allEnemies.RemoveAll(e => e == null || e.IsDead);
            
            if (allEnemies.Count == 0)
            {
                currentTarget = null;
                return;
            }

            switch (characterData?.class_tag)
            {
                case CharacterClassTag.Tank:
                    SelectTargetAsTank();
                    break;
                case CharacterClassTag.Warrior:
                    SelectTargetAsWarrior();
                    break;
                case CharacterClassTag.Scout:
                case CharacterClassTag.Mage:
                    SelectTargetAsRanged();
                    break;
                default:
                    // Fallback: select nearest enemy
                    SelectTargetAsTank();
                    break;
            }
        }

        private void SelectTargetAsTank()
        {
            // Tank: go forward and aggro enemies
            // Find nearest enemy
            CombatEnemy nearest = null;
            float nearestDistance = float.MaxValue;
            
            foreach (var enemy in allEnemies)
            {
                if (enemy.IsDead) continue;
                
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = enemy;
                }
            }
            
            currentTarget = nearest;
        }

        private void SelectTargetAsWarrior()
        {
            // Warrior: prioritize enemies aggroed on tank, else nearest
            CombatEnemy target = null;
            
            // First, try to find enemies targeting the tank
            if (tankInTeam != null && !tankInTeam.IsDead)
            {
                foreach (var enemy in allEnemies)
                {
                    if (enemy.IsDead) continue;
                    if (enemy.CurrentTarget == tankInTeam)
                    {
                        target = enemy;
                        break;
                    }
                }
            }
            
            // If no enemy targeting tank, find nearest
            if (target == null)
            {
                float nearestDistance = float.MaxValue;
                foreach (var enemy in allEnemies)
                {
                    if (enemy.IsDead) continue;
                    
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        target = enemy;
                    }
                }
            }
            
            currentTarget = target;
        }

        private void SelectTargetAsRanged()
        {
            // Scout/Mage: attack enemies NOT aggroed on tank
            CombatEnemy target = null;
            float nearestDistance = float.MaxValue;
            
            foreach (var enemy in allEnemies)
            {
                if (enemy.IsDead) continue;
                
                // Skip if enemy is targeting tank
                if (tankInTeam != null && !tankInTeam.IsDead && enemy.CurrentTarget == tankInTeam)
                {
                    continue;
                }
                
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    target = enemy;
                }
            }
            
            // If all enemies are on tank, attack nearest anyway
            if (target == null)
            {
                foreach (var enemy in allEnemies)
                {
                    if (enemy.IsDead) continue;
                    
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        target = enemy;
                    }
                }
            }
            
            currentTarget = target;
        }

        private void UpdateAbility()
        {
            if (!characterData.has_ability || characterData.ability == null) return;
            
            // Only Blood Sacrifice for vertical slice
            if (characterData.ability.ability_id != "blood_sacrifice") return;
            
            float currentTime = Time.time;
            
            // Check if ability is on cooldown
            if (currentTime < abilityCooldown) return;
            
            // Check activation condition: HP <= 50%
            float hpPercent = currentHp / maxHp;
            if (hpPercent <= 0.5f && !abilityActive)
            {
                ActivateBloodSacrifice();
            }
            
            // Check if ability duration ended
            if (abilityActive && currentTime >= abilityEndTime)
            {
                DeactivateBloodSacrifice();
            }
        }

        private void ActivateBloodSacrifice()
        {
            abilityActive = true;
            abilityEndTime = Time.time + characterData.ability.ability_duration;
            abilityCooldown = Time.time + characterData.ability.ability_cooldown;
            
            // ATK +50%
            attackMultiplier = 1.5f;
            attack = characterData.base_atk * attackMultiplier;
            
            // HP -10%
            float hpLoss = maxHp * 0.1f;
            currentHp = Mathf.Max(1, currentHp - hpLoss);
        }

        private void DeactivateBloodSacrifice()
        {
            abilityActive = false;
            attackMultiplier = 1f;
            attack = characterData.base_atk;
        }

        protected override void AttackTarget()
        {
            if (currentTarget == null || currentTarget.IsDead || !IsInAttackRange(currentTarget))
            {
                currentTarget = null;
                return;
            }

            float currentTime = Time.time;
            if (currentTime - lastAttackTime >= 1f / attackSpeed)
            {
                // Attack already includes multiplier (set in ActivateBloodSacrifice)
                currentTarget.TakeDamage(attack);
                lastAttackTime = currentTime;
            }
        }
    }
}

