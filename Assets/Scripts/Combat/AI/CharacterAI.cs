using UnityEngine;
namespace PickMe.Combat.AI
{
    [RequireComponent(typeof(CharacterCombatController))]
    public class CharacterAI : MonoBehaviour
    {
        [Header("AI Settings")]
        [SerializeField] protected float detectionRange = 10f;
        [SerializeField] protected float attackRange = 1.5f;
        protected CharacterCombatController characterController;
        protected Transform currentTarget;
        protected UnityEngine.AI.NavMeshAgent navAgent;
        protected virtual void Awake()
        {
            navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            characterController = GetComponent<CharacterCombatController>();
        }
        protected virtual void Start()
        {
            if (characterController != null && characterController.CharacterData != null)
            {
                if (navAgent != null)
                {
                    navAgent.speed = characterController.CharacterData.move_speed;
                }
            }
        }
        public virtual void UpdateAI()
        {
            if (characterController == null || !characterController.IsAlive) return;
            if (currentTarget == null || !IsTargetValid(currentTarget))
            {
                FindTarget();
            }
            if (currentTarget != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
                if (distanceToTarget <= attackRange)
                {
                    Attack();
                }
                else if (distanceToTarget <= detectionRange)
                {
                    MoveToTarget();
                }
            }
        }
        protected virtual void FindTarget()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float nearestDistance = float.MaxValue;
            Transform nearestTarget = null;
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance && distance <= detectionRange)
                {
                    nearestDistance = distance;
                    nearestTarget = enemy.transform;
                }
            }
            currentTarget = nearestTarget;
        }
        protected virtual bool IsTargetValid(Transform target)
        {
            if (target == null) return false;
            // TODO: Проверить, жив ли враг
            float distance = Vector3.Distance(transform.position, target.position);
            return distance <= detectionRange;
        }
        protected virtual void MoveToTarget()
        {
            if (navAgent != null && currentTarget != null)
            {
                navAgent.SetDestination(currentTarget.position);
            }
        }
        protected virtual void Attack()
        {
            if (navAgent != null)
            {
                navAgent.isStopped = true;
            }
            if (characterController != null && currentTarget != null)
            {
                characterController.AttackTarget(currentTarget);
            }
        }
    }
}
