using System.Collections.Generic;
using PickMe.Gameplay;
using PickMe.Infrastructure;
using PickMe.Utils;
using UnityEngine;

namespace PickMe.UI
{
    /// <summary>
    /// Tower menu - select floor, view details, start combat.
    /// </summary>
    public class TowerMenu : Menu
    {
        [Header("Floor List")]
        [SerializeField] private Transform floorListContainer;
        [SerializeField] private GameObject floorButtonPrefab;
        
        private readonly List<FloorButtonUI> _floorButtons = new();
        private TowerFloorData _selectedFloor;

        public override void OnFocus()
        {
            base.OnFocus();
            RefreshFloors();
        }

        private void RefreshFloors()
        {
            ClearButtons();
            
            if (!TowerManager.IsInitialized) return;
            
            var floors = TowerManager.Instance.Floors;
            int highestUnlocked = TowerManager.Instance.HighestUnlocked;
            
            foreach (var floor in floors)
            {
                bool isUnlocked = floor.level <= highestUnlocked;
                CreateFloorButton(floor, isUnlocked);
            }
        }

        private void CreateFloorButton(TowerFloorData floor, bool isUnlocked)
        {
            if (floorButtonPrefab == null || floorListContainer == null) return;
            
            var buttonObj = Instantiate(floorButtonPrefab, floorListContainer);
            var button = buttonObj.GetComponent<FloorButtonUI>();
            if (button != null)
            {
                button.Setup(floor, isUnlocked, OnFloorSelected, OnFloorDetails);
                _floorButtons.Add(button);
            }
        }

        private void OnFloorSelected(TowerFloorData floor)
        {
            if (floor == null) return;
            
            // Load combat scene
            if (SceneLoader.IsInitialized)
            {
                // Close menu before loading scene
                if (UIController.IsInitialized)
                {
                    UIController.Instance.CloseCurrent();
                }
                
                // Enter combat state
                if (GameManager.IsInitialized)
                {
                    GameManager.Instance.EnterCombat();
                }
                
                // Load the combat scene
                SceneLoader.Instance.LoadCombatScene(floor);
            }
            else
            {
                Debug.LogWarning("TowerMenu: SceneLoader not initialized, cannot load combat scene");
            }
        }

        private void OnFloorDetails(TowerFloorData floor)
        {
            if (floor == null) return;
            
            _selectedFloor = floor;
            
            // Open floor detail menu (can reuse CharacterMenu pattern or create separate)
            // For now, just log details
            Debug.Log($"TowerMenu: Floor {floor.level} details - " +
                     $"Enemies: {floor.enemies.Count}, Rewards: {floor.rewards.Count}");
            
            // TODO: Open floor detail popup/menu
        }

        private void ClearButtons()
        {
            // Clear list
            _floorButtons.Clear();
            
            // Destroy all child objects in container to prevent duplicates
            MenuUtils.ClearContainer(floorListContainer);
        }

        public override void OnCancel()
        {
            base.OnCancel();
            MenuUtils.CloseCurrentMenu();
        }
    }
}

