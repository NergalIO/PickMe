using UnityEngine;
namespace PickMe.City.Buildings
{
    public class TowerBuilding : Building
    {
        public override void OnBuildingClicked()
        {
            base.OnBuildingClicked();
            // TODO: Открыть UI выбора этажа башни
            Debug.Log("[TowerBuilding] Открытие башни");
        }
    }
}
