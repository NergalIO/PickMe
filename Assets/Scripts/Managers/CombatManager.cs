using UnityEngine;
namespace PickMe.Managers
{
    public class CombatManager : PersistentSingleton<CombatManager>
    {
        [Header("Combat State")]
        [SerializeField] private bool isCombatActive = false;
        [SerializeField] private int currentFloor = 1;
        public bool IsCombatActive => isCombatActive;
        public int CurrentFloor => currentFloor;
        protected override void OnAwake()
        {
            base.OnAwake();
        }
        public void StartCombat(int floorNumber)
        {
            if (isCombatActive)
            {
                Debug.LogWarning("[CombatManager] Бой уже активен!");
                return;
            }
            currentFloor = floorNumber;
            isCombatActive = true;
            Debug.Log($"[CombatManager] Начало боя на этаже {currentFloor}");
            // TODO: Инициализация боевой сцены, спавн персонажей и врагов
        }
        public void EndCombatWithVictory()
        {
            if (!isCombatActive) return;
            isCombatActive = false;
            Debug.Log($"[CombatManager] Победа на этаже {currentFloor}!");
            // TODO: Выдача наград, открытие следующего этажа
            OnCombatVictory();
        }
        public void EndCombatWithDefeat()
        {
            if (!isCombatActive) return;
            isCombatActive = false;
            Debug.Log($"[CombatManager] Поражение на этаже {currentFloor}!");
            // TODO: Обработка поражения
            OnCombatDefeat();
        }
        private void OnCombatVictory()
        {
            // TODO: Показать окно результатов с наградами
        }
        private void OnCombatDefeat()
        {
            // TODO: Показать окно поражения
        }
        public void CheckCombatConditions()
        {
            if (!isCombatActive) return;
            // TODO: Проверка живых персонажей и врагов
        }
    }
}
