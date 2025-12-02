using UnityEngine;
namespace PickMe.City.Buildings
{
    public class SummonHall : Building
    {
        public override void OnBuildingClicked()
        {
            base.OnBuildingClicked();
            // TODO: Открыть UI зала призыва
            Debug.Log("[SummonHall] Открытие зала призыва");
        }
    }
}
