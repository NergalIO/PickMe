using System.Collections;
using PickMe.Gameplay;
using UnityEngine;

namespace PickMe.Infrastructure
{
    /// <summary>
    /// Controls high-level game state transitions.
    /// </summary>
    public class GameManager : PersistentSingleton<GameManager>
    {
        public GameState CurrentState { get; private set; } = GameState.City;

        protected override IEnumerator OnInitialized()
        {
            yield return EventController.WaitUntilInitialized();
            
            // Wait for SaveSystem to initialize
            if (!SaveSystem.IsInitialized)
            {
                yield return SaveSystem.WaitUntilInitialized();
            }
            
            // Wait for all managers needed for loading save
            yield return ResourceManager.WaitUntilInitialized();
            yield return CharacterManager.WaitUntilInitialized();
            yield return TowerManager.WaitUntilInitialized();
            yield return CityManager.WaitUntilInitialized();
            
            // Load save data if available
            LoadGameProgress();
            
            PublishState();
        }

        private void LoadGameProgress()
        {
            if (SaveSystem.IsInitialized)
            {
                bool loaded = SaveSystem.Instance.LoadGame();
                if (loaded)
                {
                    Debug.Log("GameManager: Game progress loaded successfully");
                }
                else
                {
                    Debug.Log("GameManager: Starting new game (no save file found)");
                }
            }
        }

        public void EnterCity()
        {
            SetState(GameState.City);
        }

        public void EnterSummon()
        {
            SetState(GameState.Summon);
        }

        public void EnterCollection()
        {
            SetState(GameState.Collection);
        }

        public void EnterCombat()
        {
            SetState(GameState.Combat);
        }

        private void SetState(GameState state)
        {
            CurrentState = state;
            PublishState();
        }

        private void PublishState()
        {
            if (EventController.IsInitialized)
            {
                EventController.Instance.Publish(new GameStateChanged(CurrentState));
            }
        }
    }
}

