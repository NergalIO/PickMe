using UnityEngine;
namespace PickMe.Managers
{
    public class CityManager : PersistentSingleton<CityManager>
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            InitializeCity();
        }
        private void InitializeCity()
        {
            Debug.Log("[CityManager] Инициализация города");
            // TODO: Инициализация зданий, проверка их состояния
        }
        public void OpenBuildingUI(BuildingType buildingType)
        {
            Debug.Log($"[CityManager] Открытие UI здания: {buildingType}");
            switch (buildingType)
            {
                case BuildingType.House:
                    // TODO: Открыть UI дома (коллекция персонажей)
                    break;
                case BuildingType.SummonHall:
                    // TODO: Открыть UI зала призыва
                    GameManager.Instance.ChangeState(GameState.Summon);
                    break;
                case BuildingType.Tower:
                    // TODO: Открыть UI башни (выбор этажа)
                    GameManager.Instance.ChangeState(GameState.Combat);
                    break;
                case BuildingType.Portal:
                case BuildingType.Merge:
                    Debug.Log($"[CityManager] Здание {buildingType} пока недоступно (SOON)");
                    // TODO: Показать уведомление "SOON"
                    break;
            }
        }
        public void ReturnToCity()
        {
            Debug.Log("[CityManager] Возврат в город");
            GameManager.Instance.ChangeState(GameState.City);
            // TODO: Загрузка сцены города, если нужно
        }
    }
    public enum BuildingType
    {
        House,
        SummonHall,
        Tower,
        Portal,
        Merge
    }
}
