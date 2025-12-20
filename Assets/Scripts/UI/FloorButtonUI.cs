using PickMe.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PickMe.UI
{
    /// <summary>
    /// UI component for tower floor button.
    /// </summary>
    public class FloorButtonUI : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField] private TMP_Text levelText;
        
        [Header("Buttons")]
        [SerializeField] private Button selectButton;
        
        [Header("Visuals")]
        [SerializeField] private GameObject lockedOverlay;
        
        private TowerFloorData _floor;
        private System.Action<TowerFloorData> _onSelected;
        private System.Action<TowerFloorData> _onDetails;

        public void Setup(TowerFloorData floor, bool isUnlocked, 
                         System.Action<TowerFloorData> onSelected,
                         System.Action<TowerFloorData> onDetails)
        {
            _floor = floor;
            _onSelected = onSelected;
            _onDetails = onDetails;
            
            RefreshDisplay(isUnlocked);
            
            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(() => _onSelected?.Invoke(_floor));
                selectButton.interactable = isUnlocked;
            }
        }

        private void RefreshDisplay(bool isUnlocked)
        {
            if (_floor == null) return;
            
            if (levelText != null) levelText.text = $"Этаж №{_floor.level}";
            
            int totalEnemies = 0;
            if (_floor.enemies != null)
            {
                foreach (var enemy in _floor.enemies)
                {
                    totalEnemies += enemy.count;
                }
            }
            
            lockedOverlay?.SetActive(!isUnlocked);
        }
    }
}

