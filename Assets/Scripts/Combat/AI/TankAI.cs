using UnityEngine;
namespace PickMe.Combat.AI
{
    public class TankAI : CharacterAI
    {
        [Header("Tank Settings")]
        [SerializeField] private float aggroRange = 5f;
        public override void UpdateAI()
        {
            base.UpdateAI();
            AggroEnemies();
        }
        private void AggroEnemies()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance <= aggroRange)
                {
                    // TODO: Установить танка как цель для врага
                }
            }
        }
        protected override void FindTarget()
        {
            base.FindTarget();
        }
    }
}
