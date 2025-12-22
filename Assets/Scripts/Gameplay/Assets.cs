using System.Collections.Generic;
using PickMe.Gameplay.Data;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PickMe.Gameplay.Assets
{
    /// <summary>
    /// ScriptableObject definition for a tower floor, including scene reference.
    /// </summary>
    [CreateAssetMenu(fileName = "TowerFloor", menuName = "PickMe/Tower Floor", order = 0)]
    public class TowerFloorAsset : ScriptableObject
    {
        public int level = 1;
        [Tooltip("Scene path inside the project (used at runtime).")]
        public string scenePath;

#if UNITY_EDITOR
        [SerializeField] private SceneAsset sceneAsset;

        private void OnValidate()
        {
            if (sceneAsset != null)
            {
                var path = AssetDatabase.GetAssetPath(sceneAsset);
                if (!string.IsNullOrEmpty(path) && path != scenePath)
                {
                    scenePath = path;
                }
            }
        }
#endif

        public List<EnemySpawn> enemies = new();
        public List<ResourceReward> rewards = new();

        public TowerFloorData ToData()
        {
            return new TowerFloorData
            {
                level = level,
                name = $"Floor {level}",
                scenePath = scenePath,
                available = true, // ScriptableObjects are always available
                enemies = CloneEnemies(enemies),
                rewards = CloneRewards(rewards)
            };
        }

        private static List<EnemySpawn> CloneEnemies(List<EnemySpawn> source)
        {
            var list = new List<EnemySpawn>(source.Count);
            foreach (var e in source)
            {
                list.Add(new EnemySpawn { enemyId = e.enemyId, count = e.count });
            }
            return list;
        }

        private static List<ResourceReward> CloneRewards(List<ResourceReward> source)
        {
            var list = new List<ResourceReward>(source.Count);
            foreach (var r in source)
            {
                list.Add(new ResourceReward { resourceType = r.resourceType, amount = r.amount });
            }
            return list;
        }
    }
}

