using System.Collections.Generic;
using System.Linq;
using PickMe.Gameplay.Data;
using PickMe.Gameplay.Assets;
using PickMe.Core.Infrastructure;
using PickMe.Core.Services;
using PickMe.Core.Managers;
using UnityEngine;

namespace PickMe.Gameplay.Systems.TowerSystem
{
    /// <summary>
    /// Manages tower floors, unlocking, and rewards.
    /// </summary>
    public class TowerManager : PersistentSingleton<TowerManager>
    {
        [SerializeField] private List<TowerFloorData> floors = new();
        private int _highestUnlocked = 1;

        public IReadOnlyList<TowerFloorData> Floors => floors;
        public int HighestUnlocked => _highestUnlocked;

        protected override System.Collections.IEnumerator OnInitialized()
        {
            yield return EventController.WaitUntilInitialized();
            LoadFromResourcesJson();
            // Fallback to ScriptableObjects if JSON not found
            if (floors == null || floors.Count == 0)
            {
                LoadFromScriptableObjects();
            }
        }

        public TowerFloorData GetFloor(int level)
        {
            return floors.FirstOrDefault(f => f.level == level);
        }

        public void UnlockNext(int completedLevel)
        {
            _highestUnlocked = Mathf.Max(_highestUnlocked, completedLevel + 1);
            AutoSave();
        }

        /// <summary>
        /// Sets the highest unlocked floor (used for loading save data).
        /// </summary>
        public void SetHighestUnlocked(int floor)
        {
            _highestUnlocked = Mathf.Max(1, floor);
        }

        private void AutoSave()
        {
            if (SaveSystem.IsInitialized && !SaveSystem.Instance.IsLoading)
            {
                SaveSystem.Instance.SaveGame();
            }
        }

        private void LoadFromResourcesJson()
        {
            var asset = Resources.Load<TextAsset>("Config/tower_floors");
            if (asset == null)
            {
                Debug.LogWarning("TowerManager: Config/tower_floors.json not found, will try ScriptableObjects fallback");
                return;
            }

            var wrapper = JsonUtility.FromJson<TowerFloorsWrapper>(asset.text);
            if (wrapper != null && wrapper.floors != null && wrapper.floors.Count > 0)
            {
                // Filter out unavailable floors (available == false)
                floors = wrapper.floors
                    .Where(f => f.available)
                    .OrderBy(f => f.level)
                    .ToList();
                Debug.Log($"TowerManager: Loaded {floors.Count} available floors from JSON (filtered out unavailable floors)");
            }
            else
            {
                Debug.LogWarning("TowerManager: tower_floors.json parsed but empty");
            }
        }

        private void LoadFromScriptableObjects()
        {
            var assets = Resources.LoadAll<PickMe.Gameplay.Assets.TowerFloorAsset>("TowerFloors");
            if (assets != null && assets.Length > 0)
            {
                floors = assets
                    .Select(a => a.ToData())
                    .Where(f => f.available) // Also filter ScriptableObjects for consistency
                    .OrderBy(f => f.level)
                    .ToList();
                Debug.Log($"TowerManager: Loaded {floors.Count} floors from ScriptableObjects");
            }
            else
            {
                Debug.LogWarning("TowerManager: No floors found. Provide ScriptableObjects in Resources/TowerFloors or Config/tower_floors.json");
            }
        }

        [System.Serializable]
        private class TowerFloorsWrapper
        {
            public List<TowerFloorData> floors;
        }
    }
}

