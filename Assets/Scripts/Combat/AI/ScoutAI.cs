using UnityEngine;
namespace PickMe.Combat.AI
{
    public class ScoutAI : CharacterAI
    {
        protected override void FindTarget()
        {
            Transform unaggroedEnemy = FindUnaggroedEnemy();
            if (unaggroedEnemy != null)
            {
                currentTarget = unaggroedEnemy;
                return;
            }
            base.FindTarget();
        }
        private Transform FindUnaggroedEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float nearestDistance = float.MaxValue;
            Transform nearestUnaggroed = null;
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                // TODO: Проверить, заагрен ли враг на танка
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance && distance <= detectionRange)
                {
                    nearestDistance = distance;
                    nearestUnaggroed = enemy.transform;
                }
            }
            return nearestUnaggroed;
        }
    }
}
