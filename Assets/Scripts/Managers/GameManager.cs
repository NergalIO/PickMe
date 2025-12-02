using UnityEngine;
namespace PickMe.Managers
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.City;
        public GameState CurrentState => currentState;
        protected override void OnAwake()
        {
            base.OnAwake();
            InitializeGame();
        }
        private void InitializeGame()
        {
            Debug.Log("[GameManager] Инициализация игры");
        }
        public void ChangeState(GameState newState)
        {
            if (currentState == newState) return;
            Debug.Log($"[GameManager] Изменение состояния: {currentState} -> {newState}");
            currentState = newState;
            OnStateChanged(currentState);
        }
        private void OnStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.City:
                    break;
                case GameState.Combat:
                    break;
                case GameState.Summon:
                    break;
            }
        }
    }
    public enum GameState
    {
        City,
        Combat,
        Summon,
        Collection
    }
}
