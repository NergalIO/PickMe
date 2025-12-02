using System.Collections.Generic;
using UnityEngine;
using PickMe.Combat.Tower;
using PickMe.Managers;
namespace PickMe.UI.Combat
{
    public class TowerUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform floorContainer;
        [SerializeField] private GameObject floorButtonPrefab;
        [SerializeField] private TowerManager towerManager;
        private List<FloorButtonUI> floorButtons = new List<FloorButtonUI>();
        private void Start()
        {
            LoadFloors();
        }
        private void LoadFloors()
        {
            if (towerManager == null) return;
            // TODO: Загрузить список этажей из TowerManager
        }
        private void CreateFloorButton(int floorNumber, bool isUnlocked)
        {
            if (floorButtonPrefab == null || floorContainer == null) return;
            GameObject buttonObject = Instantiate(floorButtonPrefab, floorContainer);
            FloorButtonUI buttonUI = buttonObject.GetComponent<FloorButtonUI>();
            if (buttonUI != null)
            {
                buttonUI.Initialize(floorNumber, isUnlocked, OnFloorSelected);
                floorButtons.Add(buttonUI);
            }
        }
        private void OnFloorSelected(int floorNumber)
        {
            if (towerManager != null)
            {
                towerManager.StartFloorCombat(floorNumber);
            }
        }
    }
    public class FloorButtonUI : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Button button;
        [SerializeField] private TMPro.TextMeshProUGUI floorNumberText;
        [SerializeField] private TMPro.TextMeshProUGUI floorNameText;
        private int floorNumber;
        private System.Action<int> onFloorSelected;
        public void Initialize(int floor, bool isUnlocked, System.Action<int> callback)
        {
            floorNumber = floor;
            onFloorSelected = callback;
            if (floorNumberText != null)
            {
                floorNumberText.text = $"Этаж {floor}";
            }
            if (button != null)
            {
                button.interactable = isUnlocked;
                button.onClick.AddListener(() => onFloorSelected?.Invoke(floorNumber));
            }
        }
    }
}
