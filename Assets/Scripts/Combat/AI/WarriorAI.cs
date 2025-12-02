using UnityEngine;
namespace PickMe.Combat.AI
{
    public class WarriorAI : CharacterAI
    {
        protected override void FindTarget()
        {
            Transform aggroedEnemy = FindAggroedEnemy();
            if (aggroedEnemy != null)
            {
                currentTarget = aggroedEnemy;
                return;
            }
            base.FindTarget();
        }
        private Transform FindAggroedEnemy()
        {
            // TODO: Найти танка в отряде
            GameObject tank = GameObject.FindGameObjectWithTag("Tank");
            if (tank == null) return null;
            // TODO: Найти врагов, которые атакуют танка
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                // TODO: Проверить, атакует ли враг танка
            }
            return null;
        }
    }
}
