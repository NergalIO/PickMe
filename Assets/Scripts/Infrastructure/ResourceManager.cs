using System.Collections.Generic;
using PickMe.Gameplay;
using UnityEngine;

namespace PickMe.Infrastructure
{
    /// <summary>
    /// Tracks player resources and broadcasts changes.
    /// </summary>
    public class ResourceManager : PersistentSingleton<ResourceManager>
    {
        private readonly Dictionary<ResourceType, int> _resources = new();

        protected override System.Collections.IEnumerator OnInitialized()
        {
            yield return EventController.WaitUntilInitialized();
            
            // initialize defaults
            foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
            {
                if (!_resources.ContainsKey(type))
                {
                    _resources[type] = 0;
                }
            }
            PublishAll();
        }

        public int Get(ResourceType type) => _resources.TryGetValue(type, out var value) ? value : 0;

        public void Add(ResourceType type, int amount)
        {
            if (amount == 0) return;
            _resources[type] = Get(type) + amount;
            if (EventController.IsInitialized)
            {
                EventController.Instance.Publish(new ResourceChanged(type, _resources[type]));
            }
            AutoSave();
        }

        public bool Spend(ResourceType type, int amount)
        {
            if (amount < 0) return false;
            var current = Get(type);
            if (current < amount) return false;
            _resources[type] = current - amount;
            if (EventController.IsInitialized)
            {
                EventController.Instance.Publish(new ResourceChanged(type, _resources[type]));
            }
            AutoSave();
            return true;
        }

        /// <summary>
        /// Sets resource value directly (used for loading save data).
        /// </summary>
        public void Set(ResourceType type, int value)
        {
            _resources[type] = Mathf.Max(0, value);
            if (EventController.IsInitialized)
            {
                EventController.Instance.Publish(new ResourceChanged(type, _resources[type]));
            }
        }

        private void AutoSave()
        {
            if (SaveSystem.IsInitialized && !SaveSystem.Instance.IsLoading)
            {
                SaveSystem.Instance.SaveGame();
            }
        }

        private void PublishAll()
        {
            if (!EventController.IsInitialized) return;
            
            foreach (var kvp in _resources)
            {
                EventController.Instance.Publish(new ResourceChanged(kvp.Key, kvp.Value));
            }
        }
    }
}

