using UnityEngine;
using PickMe.Data;
namespace PickMe.Combat.Enemies
{
    [RequireComponent(typeof(EnemyAI))]
    public class EnemyController : MonoBehaviour
    {
        [Header("Enemy Data")]
        [SerializeField] private EnemyData enemyData;
        [Header("Combat")]
        [SerializeField] private int currentHP;
        [SerializeField] private float attackCooldown = 0f;
        private EnemyAI enemyAI;
        private Transform target;
        public EnemyData EnemyData => enemyData;
        public int CurrentHP => currentHP;
        public bool IsAlive => currentHP > 0;
        private void Awake()
        {
            enemyAI = GetComponent<EnemyAI>();
        }
        private void Start()
        {
            InitializeEnemy();
        }
        private void Update()
        {
            if (!IsAlive) return;
            attackCooldown -= Time.deltaTime;
            if (enemyAI != null)
            {
                enemyAI.UpdateAI();
            }
        }
        public void InitializeEnemy(EnemyData data = null)
        {
            if (data != null)
            {
                enemyData = data;
            }
            if (enemyData != null)
            {
                enemyData.InitializeForCombat();
                currentHP = enemyData.current_hp;
            }
            if (enemyAI != null)
            {
                enemyAI.Initialize(this);
            }
        }
        public void TakeDamage(int damage)
        {
            currentHP = Mathf.Max(0, currentHP - damage);
            if (currentHP <= 0)
            {
                Die();
            }
        }
        public void AttackTarget(Transform targetTransform)
        {
            if (attackCooldown > 0f || !IsAlive) return;
            if (targetTransform == null) return;
            // TODO: Нанести урон цели
            Debug.Log($"[EnemyController] Враг атакует цель");
            attackCooldown = enemyData.atk_speed;
        }
        private void Die()
        {
            Debug.Log("[EnemyController] Враг повержен");
            // TODO: Анимация смерти, эффекты
            // TODO: Уведомить CombatManager о смерти врага
        }
    }
}
