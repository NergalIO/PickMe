using UnityEngine;
using PickMe.Data;
using PickMe.Managers;
using PickMe.City.Buildings;
using PickMe.UI.City;
namespace PickMe.City.Construction
{
    public class BuildingPlace : MonoBehaviour
    {
        [Header("Building Settings")]
        [SerializeField] private BuildingType buildingType;
        [SerializeField] private BuildingData buildingData;
        [Header("UI")]
        [SerializeField] private ConstructionUI constructionUI;
        private Building building;
        private void Start()
        {
            InitializeBuildingPlace();
        }
        private void InitializeBuildingPlace()
        {
            if (buildingData == null)
            {
                buildingData = new BuildingData
                {
                    buildingType = buildingType,
                    isBuilt = false,
                    isUnlocked = true,
                    buildingName = GetBuildingName(buildingType),
                    description = GetBuildingDescription(buildingType)
                };
            }
        }
        private void OnMouseDown()
        {
            if (buildingData.isBuilt)
            {
                if (building != null)
                {
                    building.OnBuildingClicked();
                }
            }
            else
            {
                OpenConstructionUI();
            }
        }
        private void OpenConstructionUI()
        {
            if (constructionUI != null)
            {
                if (building == null)
                {
                    GameObject buildingObj = new GameObject("Building");
                    buildingObj.transform.SetParent(transform);
                    building = buildingObj.AddComponent<Building>();
                    building.Initialize(buildingData);
                }
                constructionUI.OpenConstruction(building, buildingData);
            }
        }
        private string GetBuildingName(BuildingType type)
        {
            switch (type)
            {
                case BuildingType.House: return "Дом";
                case BuildingType.SummonHall: return "Зал призыва";
                case BuildingType.Tower: return "Башня";
                case BuildingType.Portal: return "Портал экспедиций";
                case BuildingType.Merge: return "Здание мерджа";
                default: return "Здание";
            }
        }
        private string GetBuildingDescription(BuildingType type)
        {
            switch (type)
            {
                case BuildingType.House: return "Хранилище персонажей";
                case BuildingType.SummonHall: return "Призыв новых героев";
                case BuildingType.Tower: return "PvE контент";
                case BuildingType.Portal: return "Экспедиции (скоро)";
                case BuildingType.Merge: return "Объединение персонажей (скоро)";
                default: return "";
            }
        }
    }
}
