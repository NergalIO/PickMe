using System;
using System.Collections.Generic;
using UnityEngine;
namespace PickMe.Data
{
    [Serializable]
    public class TowerFloorData
    {
        public int level;
        public string enemy;
        public string rewards;
        public TowerFloorData()
        {
            level = 1;
            enemy = "";
            rewards = "";
        }
        public List<EnemySpawnData> ParseEnemies()
        {
            List<EnemySpawnData> enemies = new List<EnemySpawnData>();
            if (string.IsNullOrEmpty(enemy)) return enemies;
            string[] enemyPairs = enemy.Split(',');
            foreach (string pair in enemyPairs)
            {
                string[] parts = pair.Split(':');
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[1], out int count))
                    {
                        enemies.Add(new EnemySpawnData
                        {
                            enemyType = parts[0].Trim(),
                            count = count
                        });
                    }
                }
            }
            return enemies;
        }
        public List<RewardData> ParseRewards()
        {
            List<RewardData> rewardsList = new List<RewardData>();
            if (string.IsNullOrEmpty(rewards)) return rewardsList;
            string[] rewardPairs = rewards.Split(',');
            foreach (string pair in rewardPairs)
            {
                string[] parts = pair.Split(':');
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[1], out int count))
                    {
                        rewardsList.Add(new RewardData
                        {
                            rewardType = parts[0].Trim(),
                            count = count
                        });
                    }
                }
            }
            return rewardsList;
        }
    }
    [Serializable]
    public class EnemySpawnData
    {
        public string enemyType;
        public int count;
    }
    [Serializable]
    public class RewardData
    {
        public string rewardType;
        public int count;
    }
}
