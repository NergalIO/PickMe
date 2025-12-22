using UnityEngine;
using UnityEngine.AI;

namespace PickMe.Gameplay.Systems.CombatSystem
{
    /// <summary>
    /// Base class for combat units (characters and enemies) with NavMeshAgent.
    /// </summary>
    public abstract class CombatUnit : MonoBehaviour
    {
        [Header("Combat Stats")]
        [SerializeField] protected float maxHp;
        [SerializeField] protected float currentHp;
        [SerializeField] protected float attack;
        [SerializeField] protected float attackRange;
        [SerializeField] protected float attackSpeed;
        [SerializeField] protected float moveSpeed;
        
        [Header("HP Bar")]
        [SerializeField] protected HpBar hpBar;
        
        protected NavMeshAgent navAgent;
        protected CombatUnit currentTarget;
        protected float lastAttackTime;
        protected bool isDead;
        
        public float CurrentHp => currentHp;
        public float MaxHp => maxHp;
        public bool IsDead => isDead;
        public CombatUnit CurrentTarget => currentTarget;

        protected virtual void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();
            if (navAgent == null)
            {
                navAgent = gameObject.AddComponent<NavMeshAgent>();
            }
            
            // Ensure NavMeshAgent is enabled
            navAgent.enabled = true;
            
            // Configure NavMeshAgent for 2D movement (XY only, no rotation)
            navAgent.speed = moveSpeed;
            navAgent.stoppingDistance = 0.5f; // Will be updated dynamically based on attackRange
            navAgent.acceleration = 8f;
            navAgent.angularSpeed = 0f; // Disable rotation
            navAgent.updateRotation = false; // Disable automatic rotation
            navAgent.updateUpAxis = false; // Keep Z axis fixed
            navAgent.radius = 0.5f; // Set agent radius for obstacle avoidance
            navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        }

        protected virtual void Start()
        {
            currentHp = maxHp;
            isDead = false;
            lastAttackTime = 0f; // Initialize attack timer
            
            // Initialize HP bar if not assigned
            if (hpBar == null)
            {
                hpBar = GetComponentInChildren<HpBar>();
            }
            
            // Ensure NavMeshAgent is enabled and on NavMesh
            // Note: This will be called before Initialize(), so moveSpeed might still be 0
            // The actual speed will be set in Initialize() method
            if (navAgent != null)
            {
                navAgent.enabled = true;
                navAgent.updateRotation = false; // Disable rotation for 2D movement
                navAgent.updateUpAxis = false; // Keep Z axis fixed
                
                // Warp to current position to ensure agent is on NavMesh
                if (NavMesh.SamplePosition(transform.position, out var hit, 5f, NavMesh.AllAreas))
                {
                    navAgent.Warp(hit.position);
                }
                else
                {
                    Debug.LogWarning($"CombatUnit: {gameObject.name} is not on NavMesh at position {transform.position}. NavMesh may not be baked!");
                }
            }
            else
            {
                Debug.LogError($"CombatUnit: {gameObject.name} has no NavMeshAgent!");
            }
        }
        
        /// <summary>
        /// Sets whether this unit is an ally (for HP bar color).
        /// </summary>
        public virtual void SetIsAlly(bool isAlly)
        {
            if (hpBar != null)
            {
                hpBar.Initialize(this, isAlly);
            }
        }

        protected virtual void Update()
        {
            if (isDead) return;
            
            UpdateCombat();
        }

        protected abstract void UpdateCombat();

        /// <summary>
        /// Takes damage and returns true if unit died.
        /// </summary>
        public virtual bool TakeDamage(float damage)
        {
            if (isDead) return false;
            
            currentHp = Mathf.Max(0, currentHp - damage);
            
            if (currentHp <= 0)
            {
                Die();
                return true;
            }
            
            return false;
        }

        protected virtual void Die()
        {
            isDead = true;
            currentHp = 0;
            
            if (navAgent != null)
            {
                navAgent.enabled = false;
            }
            
            // Disable collider and visual
            var collider = GetComponent<Collider>();
            if (collider != null) collider.enabled = false;
            
            gameObject.SetActive(false);
        }

        protected bool IsInAttackRange(CombatUnit target)
        {
            if (target == null || target.IsDead) return false;
            
            // Calculate distance between centers
            float distance = Vector3.Distance(transform.position, target.transform.position);
            
            // Account for agent radii
            float agentRadius = navAgent != null ? navAgent.radius : 0.5f;
            float targetRadius = target.navAgent != null ? target.navAgent.radius : 0.5f;
            float combinedRadius = agentRadius + targetRadius;
            
            // If agents are touching or overlapping (distance <= combined radius), they're definitely in range
            // This handles melee combat where units can overlap
            if (distance <= combinedRadius + 0.1f) // Add small tolerance
            {
                return true;
            }
            
            // Otherwise check if distance (minus radii) is within attack range
            float effectiveDistance = distance - combinedRadius;
            
            // Ensure attackRange is at least 0.5 for melee units
            float minAttackRange = Mathf.Max(attackRange, 0.5f);
            bool inRange = effectiveDistance <= minAttackRange;
            
            return inRange;
        }

        protected virtual void AttackTarget()
        {
            if (currentTarget == null || currentTarget.IsDead)
            {
                return;
            }
            
            bool inRange = IsInAttackRange(currentTarget);
            if (!inRange)
            {
                return;
            }

            float currentTime = Time.time;
            // Ensure attackSpeed is at least 0.5 (2 attacks per second max) to prevent infinite cooldown
            float safeAttackSpeed = Mathf.Max(attackSpeed, 0.5f);
            float attackCooldown = 1f / safeAttackSpeed;
            
            if (currentTime - lastAttackTime >= attackCooldown)
            {
                bool targetDied = currentTarget.TakeDamage(attack);
                lastAttackTime = currentTime;
                
                if (targetDied)
                {
                    currentTarget = null;
                }
            }
        }

        protected void MoveTowardsTarget()
        {
            if (navAgent == null || !navAgent.enabled)
            {
                Debug.LogWarning($"CombatUnit: NavMeshAgent is null or disabled on {gameObject.name}");
                return;
            }
            
            if (currentTarget == null || currentTarget.IsDead)
            {
                navAgent.isStopped = true;
                return;
            }

            // Calculate distance to target
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
            float agentRadius = navAgent != null ? navAgent.radius : 0.5f;
            float targetRadius = currentTarget.navAgent != null ? currentTarget.navAgent.radius : 0.5f;
            float combinedRadius = agentRadius + targetRadius;
            
            // Calculate the distance we need to be at to attack
            // We want to be at: combinedRadius + attackRange (or closer)
            float desiredDistance = combinedRadius + attackRange;
            
            // Set stoppingDistance to minimum - we'll check range manually
            // This prevents NavMeshAgent from stopping too early
            navAgent.stoppingDistance = 0.1f;
            
            // Check if we're in attack range
            bool inRange = IsInAttackRange(currentTarget);
            
            if (inRange)
            {
                // Stop moving and attack
                navAgent.isStopped = true;
                // No rotation - characters move in 2D (XY plane only)
            }
            else
            {
                // Check if we're close enough that NavMeshAgent might stop
                // If distance is greater than desiredDistance, we need to keep moving
                if (distance > desiredDistance + 0.5f) // Add small buffer
                {
                    // Continue moving towards target
                    navAgent.isStopped = false;
                    
                    if (currentTarget.transform != null)
                    {
                        Vector3 targetPos = currentTarget.transform.position;
                        
                        // Check if NavMeshAgent is on NavMesh
                        if (!navAgent.isOnNavMesh)
                        {
                            // Try to warp to NavMesh
                            if (NavMesh.SamplePosition(transform.position, out var hit, 5f, NavMesh.AllAreas))
                            {
                                navAgent.Warp(hit.position);
                            }
                        }
                        
                        // Set destination directly to target position
                        // NavMeshAgent will try to get as close as possible (stoppingDistance = 0.1f)
                        navAgent.SetDestination(targetPos);
                    }
                }
                else
                {
                    // We're close but not in range - might be blocked or need to get closer
                    // Try to move closer manually
                    navAgent.isStopped = false;
                    if (currentTarget.transform != null)
                    {
                        Vector3 targetPos = currentTarget.transform.position;
                        Vector3 direction = (targetPos - transform.position).normalized;
                        // Try to get even closer
                        Vector3 closerDestination = targetPos - direction * (combinedRadius + 0.1f);
                        navAgent.SetDestination(closerDestination);
                    }
                }
            }
        }
    }
}

