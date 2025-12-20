using System.Collections.Generic;
using PickMe.Gameplay;

namespace PickMe.Infrastructure
{
    /// <summary>
    /// Handles summon logic using tickets.
    /// </summary>
    public class SummonManager : PersistentSingleton<SummonManager>
    {
        private const int CharactersPerSummon = 3;
        private const int TicketCost = 1;

        protected override System.Collections.IEnumerator OnInitialized()
        {
            yield return EventController.WaitUntilInitialized();
            yield return ResourceManager.WaitUntilInitialized();
            yield return CharacterManager.WaitUntilInitialized();
        }

        public bool CanSummon()
        {
            return ResourceManager.Instance.Get(ResourceType.Tickets) >= TicketCost
                   && CharacterManager.Instance.HasFreeSlots(CharactersPerSummon);
        }

        public bool SummonWithTickets()
        {
            if (!CanSummon()) return false;

            if (!ResourceManager.Instance.Spend(ResourceType.Tickets, TicketCost))
            {
                return false;
            }

            List<CharacterData> newChars = CharacterManager.Instance.GenerateCharacters(CharactersPerSummon);
            CharacterManager.Instance.AddToCollection(newChars);
            if (EventController.IsInitialized)
            {
                EventController.Instance.Publish(new SummonCompleted(newChars));
            }
            return true;
        }
    }
}

