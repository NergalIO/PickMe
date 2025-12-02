using UnityEngine;
namespace PickMe.Combat.Enemies
{
    public class EnemyAI : MonoBehaviour
    {
        [Header("AI Settings")]
        [SerializeField] private float detectionRange = 10f;
        [SerializeField] private float attackRange = 1.5f;
        private EnemyController enemyController;
        private Transform currentTarget;
        private UnityEngine.AI.NavMeshAgent navAgent;
        private void Awake()
        {
            navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }
        public void Initialize(EnemyController controller)
        {
            enemyController = controller;
            if (navAgent != null && enemyController.EnemyData != null)
            {
                navAgent.speed = enemyController.EnemyData.move_speed;
            }
        }
        public void UpdateAI()
        {
            if (enemyController == null || !enemyController.IsAlive) return;
            if (currentTarget == null || !IsTargetValid(currentTarget))
            {
                FindNearestTarget();
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
        private void FindNearestTarget()
        {
            // TODO: Найти всех персонажей игрока на сцене
            GameObject[] characters = GameObject.FindGameObjectsWithTag("PlayerCharacter");
            float nearestDistance = float.MaxValue;
            Transform nearestTarget = null;
            foreach (var character in characters)
            {
                if (character == null) continue;
                float distance = Vector3.Distance(transform.position, character.transform.position);
                if (distance < nearestDistance && distance <= detectionRange)
                {
                    nearestDistance = distance;
                    nearestTarget = character.transform;
                }
            }
            currentTarget = nearestTarget;
        }
        private bool IsTargetValid(Transform target)
        {
            if (target == null) return false;
            // TODO: Проверить, жив ли персонаж
            float distance = Vector3.Distance(transform.position, target.position);
            return distance <= detectionRange;
        }
        private void MoveToTarget()
        {
            if (navAgent != null && currentTarget != null)
            {
                navAgent.SetDestination(currentTarget.position);
            }
        }
        private void Attack()
        {
            if (navAgent != null)
            {
                navAgent.isStopped = true;
            }
            if (enemyController != null && currentTarget != null)
            {
                enemyController.AttackTarget(currentTarget);
            }
        }
    }
}
