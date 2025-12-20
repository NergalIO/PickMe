using System.Collections;
using System.Collections.Generic;
using PickMe.Gameplay;
using UnityEngine;

namespace PickMe.Infrastructure
{
    /// <summary>
    /// Simplified combat loop for vertical slice: resolves instantly based on team presence.
    /// </summary>
    public class CombatManager : PersistentSingleton<CombatManager>
    {
        protected override IEnumerator OnInitialized()
        {
            yield return EventController.WaitUntilInitialized();
            yield return ResourceManager.WaitUntilInitialized();
            yield return CharacterManager.WaitUntilInitialized();
            yield return TowerManager.WaitUntilInitialized();
        }

        public void StartCombat(int floorLevel)
        {
            StartCoroutine(ResolveCombatRoutine(floorLevel));
        }

        private IEnumerator ResolveCombatRoutine(int floorLevel)
        {
            var floor = TowerManager.Instance.GetFloor(floorLevel);
            if (floor == null)
            {
                Debug.LogWarning($"No floor data for level {floorLevel}");
                yield break;
            }

            // Placeholder auto-resolution: if team exists -> victory, else defeat.
            var hasTeam = CharacterManager.Instance.Team.Count > 0;
            var result = hasTeam ? CombatResultType.Victory : CombatResultType.Defeat;

            yield return null; // simulate frame

            if (result == CombatResultType.Victory)
            {
                GrantRewards(floor.rewards);
                TowerManager.Instance.UnlockNext(floorLevel);
            }

            if (EventController.IsInitialized)
            {
                EventController.Instance.Publish(new CombatCompleted(result, floor.rewards));
            }

            // Mark dead heroes if defeat (placeholder: all die)
            if (result == CombatResultType.Defeat)
            {
                foreach (var hero in CharacterManager.Instance.Team)
                {
                    CharacterManager.Instance.MarkDead(hero.id);
                }
            }
        }

        private void GrantRewards(IEnumerable<ResourceReward> rewards)
        {
            foreach (var reward in rewards)
            {
                ResourceManager.Instance.Add(reward.resourceType, reward.amount);
            }
        }
    }
}

