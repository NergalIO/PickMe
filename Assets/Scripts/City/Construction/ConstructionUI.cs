using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PickMe.Data;
using PickMe.City.Buildings;
using PickMe.City.Construction;
namespace PickMe.UI.City
{
    public class ConstructionUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject constructionPanel;
        [SerializeField] private TextMeshProUGUI buildingNameText;
        [SerializeField] private TextMeshProUGUI buildingDescriptionText;
        [SerializeField] private Button buildButton;
        [SerializeField] private Button cancelButton;
        private Building targetBuilding;
        private ConstructionManager constructionManager;
        private void Start()
        {
            if (buildButton != null)
            {
                buildButton.onClick.AddListener(OnBuildClicked);
            }
            if (cancelButton != null)
            {
                cancelButton.onClick.AddListener(OnCancelClicked);
            }
        }
        public void OpenConstruction(Building building, BuildingData buildingData)
        {
            targetBuilding = building;
            if (constructionPanel != null)
            {
                constructionPanel.SetActive(true);
            }
            if (buildingNameText != null)
            {
                buildingNameText.text = buildingData.buildingName;
            }
            if (buildingDescriptionText != null)
            {
                buildingDescriptionText.text = buildingData.description;
            }
            // TODO: Показать стоимость постройки
        }
        private void OnBuildClicked()
        {
            if (targetBuilding == null || constructionManager == null) return;
            bool success = constructionManager.BuildBuilding(targetBuilding, targetBuilding.BuildingType);
            if (success)
            {
                CloseConstruction();
            }
        }
        private void OnCancelClicked()
        {
            CloseConstruction();
        }
        private void CloseConstruction()
        {
            if (constructionPanel != null)
            {
                constructionPanel.SetActive(false);
            }
            targetBuilding = null;
        }
    }
}
