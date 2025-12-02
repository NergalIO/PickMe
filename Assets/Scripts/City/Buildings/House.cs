using UnityEngine;
using PickMe.Managers;
namespace PickMe.City.Buildings
{
    public class House : Building
    {
        public override void OnBuildingClicked()
        {
            base.OnBuildingClicked();
            if (GameManager.HasInstance)
            {
                GameManager.Instance.ChangeState(GameState.Collection);
            }
            // TODO: Открыть UI коллекции
            Debug.Log("[House] Открытие коллекции персонажей");
        }
    }
}
