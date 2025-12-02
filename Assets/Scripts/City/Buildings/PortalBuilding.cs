using UnityEngine;
namespace PickMe.City.Buildings
{
    public class PortalBuilding : Building
    {
        public override void OnBuildingClicked()
        {
            base.OnBuildingClicked();
            Debug.Log("[PortalBuilding] Портал экспедиций скоро будет доступен!");
            // TODO: Показать UI уведомления "SOON"
        }
    }
}
