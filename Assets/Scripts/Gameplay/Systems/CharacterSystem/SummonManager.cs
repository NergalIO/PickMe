using System.Collections.Generic;
using PickMe.Gameplay.Data;
using PickMe.Gameplay.Systems.ResourceSystem;
using PickMe.Core.Infrastructure;
using PickMe.Core.Services;
using PickMe.Core.Managers;
using UnityEngine;

namespace PickMe.Gameplay.Systems.CharacterSystem
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
            Debug.Log($"SummonManager: Generated {newChars?.Count ?? 0} characters");
            CharacterManager.Instance.AddToCollection(newChars);
            if (EventController.IsInitialized)
            {
                Debug.Log($"SummonManager: Publishing SummonCompleted event with {newChars?.Count ?? 0} characters");
                EventController.Instance.Publish(new SummonCompleted(newChars));
            }
            else
            {
                Debug.LogWarning("SummonManager: EventController not initialized, cannot publish SummonCompleted event");
            }
            return true;
        }
    }
}

