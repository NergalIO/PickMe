using System;
using System.Collections.Generic;

namespace PickMe.Gameplay.Data
{
    [Serializable]
    public class EnemySpawn
    {
        public string enemyId;
        public int count;
    }

    [Serializable]
    public class ResourceReward
    {
        public ResourceType resourceType;
        public int amount;
    }

    [Serializable]
    public class TowerFloorData
    {
        public int level;
        public string name;
        public string scenePath;
        public bool available = true;
        public List<EnemySpawn> enemies = new();
        public List<ResourceReward> rewards = new();
    }
}

