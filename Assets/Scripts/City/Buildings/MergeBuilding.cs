using UnityEngine;
namespace PickMe.City.Buildings
{
    public class MergeBuilding : Building
    {
        public override void OnBuildingClicked()
        {
            base.OnBuildingClicked();
            Debug.Log("[MergeBuilding] Здание мерджа скоро будет доступно!");
            // TODO: Показать UI уведомления "SOON"
        }
    }
}
