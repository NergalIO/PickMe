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
            LoadFromScriptableObjects();
            LoadFromResourcesJsonFallback();
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

        private void LoadFromScriptableObjects()
        {
            if (floors != null && floors.Count > 0) return;

            var assets = Resources.LoadAll<PickMe.Gameplay.Assets.TowerFloorAsset>("TowerFloors");
            if (assets != null && assets.Length > 0)
            {
                floors = assets
                    .Select(a => a.ToData())
                    .OrderBy(f => f.level)
                    .ToList();
                return;
            }
        }

        private void LoadFromResourcesJsonFallback()
        {
            // If already configured in inspector, keep it.
            if (floors != null && floors.Count > 0) return;

            var asset = Resources.Load<TextAsset>("Config/tower_floors");
            if (asset == null)
            {
                Debug.LogWarning("TowerManager: no floors found. Provide ScriptableObjects in Resources/TowerFloors or Config/tower_floors.json");
                return;
            }

            var wrapper = JsonUtility.FromJson<TowerFloorsWrapper>(asset.text);
            if (wrapper != null && wrapper.floors != null && wrapper.floors.Count > 0)
            {
                floors = wrapper.floors;
            }
            else
            {
                Debug.LogWarning("TowerManager: tower_floors.json parsed but empty");
            }
        }

        [System.Serializable]
        private class TowerFloorsWrapper
        {
            public List<TowerFloorData> floors;
        }
    }
}

