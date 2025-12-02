using UnityEngine;
using PickMe.Data;
namespace PickMe.Combat.Enemies
{
    public class Goblin : EnemyController
    {
        private void Start()
        {
            if (EnemyData == null)
            {
                EnemyData defaultData = new EnemyData
                {
                    enemy_type = "goblin",
                    base_hp = 50,
                    base_atk = 8,
                    atk_range = 1.0f,
                    atk_speed = 1.0f,
                    move_speed = 2.0f
                };
                InitializeEnemy(defaultData);
            }
        }
    }
}
