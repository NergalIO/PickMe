using PickMe.Gameplay;
using PickMe.Infrastructure;
using PickMe.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PickMe.UI
{
    /// <summary>
    /// Construction menu - shows building info and allows construction.
    /// </summary>
    public class ConstructionMenu : Menu
    {
        [Header("Display")]
        [SerializeField] private TMP_Text buildingNameText;
        [SerializeField] private TMP_Text costText;
        
        [Header("Buttons")]
        [SerializeField] private Button buildButton;
        [SerializeField] private Button cancelButton;
        
        private BuildingType _currentBuildingType;
        private BuildingData _currentBuildingData;

        public override void Awake()
        {
            base.Awake();
            
            // Connect button click handlers
            if (buildButton != null)
            {
                buildButton.onClick.AddListener(OnBuildClicked);
            }
            
            if (cancelButton != null)
            {
                cancelButton.onClick.AddListener(OnCancelClicked);
            }
        }

        public void SetBuilding(BuildingType type, BuildingData data)
        {
            if (data == null)
            {
                Debug.LogWarning($"ConstructionMenu: Cannot set building - data is null for type {type}");
                return;
            }

            _currentBuildingType = type;
            _currentBuildingData = data;
            RefreshDisplay();
        }

        public override void OnFocus()
        {
            base.OnFocus();
            SubscribeEvents();
            RefreshDisplay();
        }

        public override void OnBlur()
        {
            base.OnBlur();
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            EventSubscriptionHelper.SubscribeIfReady<ResourceChanged>(OnResourceChanged);
            EventSubscriptionHelper.SubscribeIfReady<BuildingBuilt>(OnBuildingBuilt);
        }

        private void UnsubscribeEvents()
        {
            EventSubscriptionHelper.UnsubscribeIfReady<ResourceChanged>(OnResourceChanged);
            EventSubscriptionHelper.UnsubscribeIfReady<BuildingBuilt>(OnBuildingBuilt);
        }

        private void OnResourceChanged(ResourceChanged evt)
        {
            if (evt.Type == ResourceType.Construction)
            {
                RefreshDisplay();
            }
        }

        private void OnBuildingBuilt(BuildingBuilt evt)
        {
            if (evt.Type == _currentBuildingType)
            {
                // Building was built, close menu
                OnCancelClicked();
            }
        }

        private void RefreshDisplay()
        {
            if (_currentBuildingData == null) return;

            if (buildingNameText != null)
            {
                buildingNameText.text = $"Постройка {_currentBuildingType}";
            }

            if (costText != null)
            {
                int currentResources = ResourceManager.IsInitialized 
                    ? ResourceManager.Instance.Get(ResourceType.Construction) 
                    : 0;
                bool canAfford = currentResources >= _currentBuildingData.buildCost;
                costText.text = $"Стоимость: {_currentBuildingData.buildCost}\n";
                costText.color = canAfford ? Color.white : Color.red;
            }

            if (buildButton != null)
            {
                bool canBuild = CityManager.IsInitialized && 
                               CityManager.Instance.CanBuild(_currentBuildingType);
                buildButton.interactable = canBuild;
            }
        }

        public void OnBuildClicked()
        {
            if (!CityManager.IsInitialized)
            {
                Debug.LogWarning("ConstructionMenu: CityManager is not initialized");
                return;
            }

            if (_currentBuildingData == null)
            {
                Debug.LogWarning("ConstructionMenu: No building data set");
                return;
            }

            bool success = CityManager.Instance.Build(_currentBuildingType);
            if (success)
            {
                // Close construction menu
                if (UIController.IsInitialized)
                {
                    UIController.Instance.CloseCurrent();
                }
                
                // Show "Building built!" notification
                if (ToastManager.IsInitialized)
                {
                    ToastManager.Instance.ShowBuildingBuilt();
                }
                else
                {
                    Debug.Log($"ConstructionMenu: {_currentBuildingType} built successfully!");
                }
            }
            else
            {
                // Show error message via toast if available
                if (ToastManager.IsInitialized)
                {
                    ToastManager.Instance.ShowToast($"Недостаточно ресурсов для постройки", 2f);
                }
                Debug.LogWarning($"ConstructionMenu: Cannot build {_currentBuildingType}");
            }
        }

        public void OnCancelClicked()
        {
            MenuUtils.CloseCurrentMenu();
        }

        public override void OnCancel()
        {
            base.OnCancel();
            OnCancelClicked();
        }
    }
}

