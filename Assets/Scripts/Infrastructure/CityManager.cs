using System.Collections.Generic;
using System.Linq;
using PickMe.Gameplay;
using UnityEngine;

namespace PickMe.Infrastructure
{
    /// <summary>
    /// Manages buildings in the city and construction flow.
    /// </summary>
    public class CityManager : PersistentSingleton<CityManager>
    {
        [Header("Buildings")]
        [SerializeField] private List<BuildingData> buildings = new();

        public IReadOnlyList<BuildingData> Buildings => buildings;

        protected override System.Collections.IEnumerator OnInitialized()
        {
            yield return EventController.WaitUntilInitialized();
            yield return ResourceManager.WaitUntilInitialized();
            yield return CharacterManager.WaitUntilInitialized();

            // Log initial state before ensuring buildings
            Debug.Log($"CityManager: Initializing. Buildings in Inspector: {buildings.Count}");
            foreach (var b in buildings)
            {
                Debug.Log($"CityManager: Inspector building {b.type} has status {b.status}");
            }

            // Ensure required starting buildings exist
            // If building already exists in Inspector, preserve its status and other properties
            // Only set defaults if building doesn't exist
            EnsureBuildingPreservingInspector(BuildingType.House, BuildingStatus.Built, storageCapacity: 50);
            EnsureBuildingPreservingInspector(BuildingType.SummonHall, BuildingStatus.Built);
            EnsureBuildingPreservingInspector(BuildingType.Tower, BuildingStatus.Built);
            EnsureBuildingPreservingInspector(BuildingType.ExpeditionPortal, BuildingStatus.Built);
            EnsureBuildingPreservingInspector(BuildingType.MergeLab, BuildingStatus.Built);

            // Log final state
            Debug.Log($"CityManager: After initialization. Final building statuses:");
            foreach (var b in buildings)
            {
                Debug.Log($"CityManager: {b.type} = {b.status}, storageCapacity = {b.storageCapacity}, buildCost = {b.buildCost}");
            }

            SyncHouseCapacity();
        }

        /// <summary>
        /// Ensures building exists. If it already exists in Inspector, preserves its status and properties.
        /// Only sets defaults if building doesn't exist.
        /// </summary>
        private void EnsureBuildingPreservingInspector(BuildingType type, BuildingStatus defaultStatus, int storageCapacity = 0, int cost = 0)
        {
            var existing = buildings.FirstOrDefault(b => b.type == type);
            if (existing == null)
            {
                // Building doesn't exist - create with defaults
                var newBuilding = new BuildingData
                {
                    type = type,
                    status = defaultStatus,
                    storageCapacity = storageCapacity,
                    buildCost = cost
                };
                buildings.Add(newBuilding);
                Debug.Log($"CityManager: Created building {type} with status {defaultStatus}");
            }
            else
            {
                // Building exists in Inspector - preserve its status and only update missing values
                Debug.Log($"CityManager: Building {type} already exists in Inspector with status {existing.status}. Preserving Inspector values.");
                
                // Only update storageCapacity if it's 0 and we have a value to set
                if (existing.storageCapacity == 0 && storageCapacity > 0)
                {
                    existing.storageCapacity = storageCapacity;
                    Debug.Log($"CityManager: Updated {type} storageCapacity to {storageCapacity}");
                }
                
                // Only update buildCost if it's 0 and we have a value to set
                if (existing.buildCost == 0 && cost > 0)
                {
                    existing.buildCost = cost;
                    Debug.Log($"CityManager: Updated {type} buildCost to {cost}");
                }
            }
        }

        /// <summary>
        /// Legacy method - always sets status. Use EnsureBuildingPreservingInspector for new code.
        /// </summary>
        private void EnsureBuilding(BuildingType type, BuildingStatus status, int storageCapacity = 0, int cost = 0)
        {
            var existing = buildings.FirstOrDefault(b => b.type == type);
            if (existing == null)
            {
                var newBuilding = new BuildingData
                {
                    type = type,
                    status = status,
                    storageCapacity = storageCapacity,
                    buildCost = cost
                };
                buildings.Add(newBuilding);
                Debug.Log($"CityManager: Created building {type} with status {status}");
            }
            else
            {
                var oldStatus = existing.status;
                existing.status = status;
                if (storageCapacity > 0) existing.storageCapacity = storageCapacity;
                if (cost > 0) existing.buildCost = cost;
                
                if (oldStatus != status)
                {
                    Debug.Log($"CityManager: Updated building {type} status from {oldStatus} to {status}");
                }
            }
        }

        public bool CanBuild(BuildingType type)
        {
            var b = buildings.FirstOrDefault(x => x.type == type);
            if (b == null || b.status != BuildingStatus.Available) return false;
            return ResourceManager.Instance.Get(ResourceType.Construction) >= b.buildCost;
        }

        public bool Build(BuildingType type)
        {
            var b = buildings.FirstOrDefault(x => x.type == type);
            if (b == null || b.status != BuildingStatus.Available) return false;

            if (!ResourceManager.Instance.Spend(ResourceType.Construction, b.buildCost))
            {
                return false;
            }

            b.status = BuildingStatus.Built;

            if (b.type == BuildingType.House && b.storageCapacity > 0)
            {
                CharacterManager.Instance.SetStorageCapacity(b.storageCapacity);
            }

            if (EventController.IsInitialized)
            {
                EventController.Instance.Publish(new BuildingBuilt(type));
            }
            return true;
        }

        private void SyncHouseCapacity()
        {
            var house = buildings.FirstOrDefault(b => b.type == BuildingType.House);
            if (house != null && house.storageCapacity > 0)
            {
                CharacterManager.Instance.SetStorageCapacity(house.storageCapacity);
            }
        }
    }
}

