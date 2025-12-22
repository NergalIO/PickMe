using System.Collections.Generic;
using System.Linq;
using PickMe.Gameplay.Data;
using PickMe.Core.Infrastructure;
using PickMe.Core.Services;
using UnityEngine;

namespace PickMe.Gameplay.Systems.TowerSystem
{
    /// <summary>
    /// Manages enemy configurations loaded from Resources/Config/enemy.json
    /// </summary>
    public class EnemyManager : PersistentSingleton<EnemyManager>
    {
        private readonly Dictionary<string, EnemyData> _enemyConfigs = new();

        protected override System.Collections.IEnumerator OnInitialized()
        {
            yield return EventController.WaitUntilInitialized();
            LoadEnemyConfigs();
        }

        /// <summary>
        /// Gets enemy data by ID.
        /// </summary>
        public EnemyData GetEnemyData(string enemyId)
        {
            return _enemyConfigs.TryGetValue(enemyId, out var data) ? data : null;
        }

        private void LoadEnemyConfigs()
        {
            var asset = Resources.Load<TextAsset>("Config/enemy");
            if (asset == null)
            {
                Debug.LogWarning("EnemyManager: Config/enemy.json not found");
                return;
            }

            var wrapper = JsonUtility.FromJson<EnemyConfigWrapper>(asset.text);
            if (wrapper != null && wrapper.enemies != null)
            {
                foreach (var enemy in wrapper.enemies)
                {
                    if (!string.IsNullOrEmpty(enemy.id))
                    {
                        _enemyConfigs[enemy.id] = enemy;
                    }
                }
                Debug.Log($"EnemyManager: Loaded {_enemyConfigs.Count} enemy configs");
            }
            else
            {
                Debug.LogWarning("EnemyManager: Failed to parse enemy config");
            }
        }

        [System.Serializable]
        private class EnemyConfigWrapper
        {
            public List<EnemyData> enemies;
        }
    }
}

