using System.Collections.Generic;
using System.Linq;
using PickMe.Gameplay.Data;
using PickMe.Core.Infrastructure;
using PickMe.Core.Services;
using UnityEngine;

namespace PickMe.Gameplay.Systems.CharacterSystem
{
    /// <summary>
    /// Manages the party of characters for combat.
    /// Automatically fills with up to 5 random characters from collection.
    /// </summary>
    public class Party : PersistentSingleton<Party>
    {
        private const int MaxPartySize = 5;
        
        private readonly List<CharacterData> _partyMembers = new();
        
        public IReadOnlyList<CharacterData> Members => _partyMembers;
        public int Count => _partyMembers.Count;

        protected override System.Collections.IEnumerator OnInitialized()
        {
            yield return EventController.WaitUntilInitialized();
            yield return CharacterManager.WaitUntilInitialized();
            
            // Auto-fill party on initialization
            RefreshParty();
        }

        /// <summary>
        /// Refreshes the party with random characters from collection (up to 5).
        /// </summary>
        public void RefreshParty()
        {
            _partyMembers.Clear();
            
            if (!CharacterManager.IsInitialized)
            {
                Debug.LogWarning("Party: CharacterManager not initialized, cannot refresh party");
                return;
            }
            
            var collection = CharacterManager.Instance.Collection;
            if (collection == null || collection.Count == 0)
            {
                Debug.LogWarning("Party: No characters in collection to form party");
                return;
            }
            
            // Get all alive characters
            var aliveCharacters = collection.Where(c => c != null && !c.is_dead).ToList();
            
            if (aliveCharacters.Count == 0)
            {
                Debug.LogWarning("Party: No alive characters in collection");
                return;
            }
            
            // Shuffle and take up to MaxPartySize
            var shuffled = aliveCharacters.OrderBy(x => Random.Range(0f, 1f)).ToList();
            int countToTake = Mathf.Min(MaxPartySize, shuffled.Count);
            
            for (int i = 0; i < countToTake; i++)
            {
                _partyMembers.Add(shuffled[i]);
            }
            
            Debug.Log($"Party: Refreshed with {_partyMembers.Count} members");
        }

        /// <summary>
        /// Manually sets party members (for testing or specific selection).
        /// </summary>
        public void SetPartyMembers(IEnumerable<CharacterData> members)
        {
            _partyMembers.Clear();
            if (members != null)
            {
                _partyMembers.AddRange(members.Take(MaxPartySize));
            }
        }

        /// <summary>
        /// Clears the party.
        /// </summary>
        public void Clear()
        {
            _partyMembers.Clear();
        }
    }
}

