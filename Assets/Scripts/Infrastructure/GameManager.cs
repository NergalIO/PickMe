using System.Collections;
using PickMe.Gameplay;

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
            PublishState();
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

