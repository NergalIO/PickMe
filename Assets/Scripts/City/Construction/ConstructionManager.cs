using UnityEngine;
using PickMe.Data;
using PickMe.Managers;
using PickMe.City.Buildings;
namespace PickMe.City.Construction
{
    public class ConstructionManager : MonoBehaviour
    {
        public bool CanBuild(BuildingType buildingType)
        {
            // TODO: Проверить условия разблокировки
            // TODO: Проверить наличие ресурсов
            return true;
        }
        public bool BuildBuilding(Building building, BuildingType buildingType)
        {
            if (building == null) return false;
            if (!CanBuild(buildingType))
            {
                Debug.LogWarning($"[ConstructionManager] Невозможно построить здание {buildingType}");
                return false;
            }
            // TODO: Списать ресурсы
            // TODO: Проверить условия
            if (building.BuildingData != null)
            {
                building.BuildingData.isBuilt = true;
                building.Initialize(building.BuildingData);
            }
            Debug.Log($"[ConstructionManager] Здание {buildingType} построено!");
            ShowBuildNotification(buildingType);
            return true;
        }
        private void ShowBuildNotification(BuildingType buildingType)
        {
            // TODO: Показать UI уведомление "Здание построено!"
            Debug.Log($"[ConstructionManager] Здание {buildingType} построено!");
        }
    }
}
