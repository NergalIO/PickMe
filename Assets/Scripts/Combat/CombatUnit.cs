using UnityEngine;
using UnityEngine.AI;

namespace PickMe.Combat
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
            
            navAgent.speed = moveSpeed;
            navAgent.stoppingDistance = attackRange * 0.9f; // Stop slightly before attack range
        }

        protected virtual void Start()
        {
            currentHp = maxHp;
            isDead = false;
            
            // Initialize HP bar if not assigned
            if (hpBar == null)
            {
                hpBar = GetComponentInChildren<HpBar>();
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
            
            float distance = Vector3.Distance(transform.position, target.transform.position);
            return distance <= attackRange;
        }

        protected virtual void AttackTarget()
        {
            if (currentTarget == null || currentTarget.IsDead || !IsInAttackRange(currentTarget))
            {
                currentTarget = null;
                return;
            }

            float currentTime = Time.time;
            if (currentTime - lastAttackTime >= 1f / attackSpeed)
            {
                currentTarget.TakeDamage(attack);
                lastAttackTime = currentTime;
            }
        }

        protected void MoveTowardsTarget()
        {
            if (currentTarget == null || currentTarget.IsDead)
            {
                navAgent.isStopped = true;
                return;
            }

            if (IsInAttackRange(currentTarget))
            {
                navAgent.isStopped = true;
                transform.LookAt(currentTarget.transform);
            }
            else
            {
                navAgent.isStopped = false;
                navAgent.SetDestination(currentTarget.transform.position);
            }
        }
    }
}

