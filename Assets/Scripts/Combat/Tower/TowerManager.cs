using System.Collections.Generic;
using UnityEngine;
using PickMe.Data;
using PickMe.Managers;
namespace PickMe.Combat.Tower
{
    public class TowerManager : MonoBehaviour
    {
        [Header("Tower Settings")]
        [SerializeField] private int maxUnlockedFloor = 1;
        private Dictionary<int, TowerFloorData> towerFloors = new Dictionary<int, TowerFloorData>();
        public int MaxUnlockedFloor => maxUnlockedFloor;
        private void Start()
        {
            LoadTowerFloors();
        }
        private void LoadTowerFloors()
        {
            // TODO: Загрузить из ConfigManager
            Debug.Log("[TowerManager] Загрузка этажей башни");
        }
        public TowerFloorData GetFloorData(int floorNumber)
        {
            if (towerFloors.ContainsKey(floorNumber))
            {
                return towerFloors[floorNumber];
            }
            return null;
        }
        public bool IsFloorUnlocked(int floorNumber)
        {
            return floorNumber <= maxUnlockedFloor;
        }
        public void UnlockNextFloor(int completedFloor)
        {
            if (completedFloor >= maxUnlockedFloor)
            {
                maxUnlockedFloor = completedFloor + 1;
                Debug.Log($"[TowerManager] Открыт этаж {maxUnlockedFloor}");
            }
        }
        public void StartFloorCombat(int floorNumber)
        {
            if (!IsFloorUnlocked(floorNumber))
            {
                Debug.LogWarning($"[TowerManager] Этаж {floorNumber} еще не открыт!");
                return;
            }
            TowerFloorData floorData = GetFloorData(floorNumber);
            if (floorData == null)
            {
                Debug.LogError($"[TowerManager] Данные этажа {floorNumber} не найдены!");
                return;
            }
            Debug.Log($"[TowerManager] Запуск боя на этаже {floorNumber}");
            if (CombatManager.HasInstance)
            {
                CombatManager.Instance.StartCombat(floorNumber);
            }
            // TODO: Загрузить сцену боя или инициализировать боевую арену
        }
    }
}
