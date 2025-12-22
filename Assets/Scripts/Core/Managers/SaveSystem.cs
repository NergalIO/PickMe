using System;
using System.Collections.Generic;
using System.IO;
using PickMe.Gameplay.Data;
using PickMe.Gameplay.Systems.CharacterSystem;
using PickMe.Gameplay.Systems.ResourceSystem;
using PickMe.Gameplay.Systems.TowerSystem;
using PickMe.Gameplay.Systems.CitySystem;
using PickMe.Core.Infrastructure;
using UnityEngine;

namespace PickMe.Core.Managers
{
    /// <summary>
    /// Manages saving and loading player progress.
    /// </summary>
    public class SaveSystem : PersistentSingleton<SaveSystem>
    {
        private const string SaveFileName = "player_save.json";
        private string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);
        private bool _isLoading = false;
        private bool _hasLoadedOnce = false; // Prevents saving before first load

        protected override System.Collections.IEnumerator OnInitialized()
        {
            yield return null; // No dependencies needed
        }

        /// <summary>
        /// Checks if save system is currently loading data.
        /// </summary>
        public bool IsLoading => _isLoading;

        /// <summary>
        /// Saves current game state to file.
        /// </summary>
        public void SaveGame()
        {
            // Don't save if we're currently loading or haven't loaded once yet
            if (_isLoading)
            {
                Debug.LogWarning("SaveSystem: SaveGame called during loading, ignoring");
                return;
            }
            
            if (!_hasLoadedOnce)
            {
                Debug.LogWarning("SaveSystem: SaveGame called before first load, ignoring to prevent overwriting save data");
                return;
            }
            
            try
            {
                var saveData = CollectSaveData();
                string json = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(SaveFilePath, json);
                Debug.Log($"SaveSystem: Game saved to {SaveFilePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveSystem: Failed to save game: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads game state from file.
        /// </summary>
        public bool LoadGame()
        {
            try
            {
                _isLoading = true;
                
                if (!File.Exists(SaveFilePath))
                {
                    Debug.Log("SaveSystem: No save file found, starting new game");
                    _hasLoadedOnce = true; // Mark as loaded even if no file exists
                    return false;
                }

                string json = File.ReadAllText(SaveFilePath);
                var saveData = JsonUtility.FromJson<SaveData>(json);
                
                if (saveData == null)
                {
                    Debug.LogWarning("SaveSystem: Save file is corrupted or empty");
                    _hasLoadedOnce = true; // Mark as loaded even if corrupted
                    return false;
                }

                ApplySaveData(saveData);
                Debug.Log($"SaveSystem: Game loaded from {SaveFilePath}");
                _hasLoadedOnce = true; // Mark that we've successfully loaded
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveSystem: Failed to load game: {ex.Message}");
                _hasLoadedOnce = true; // Mark as loaded even on error to allow saving
                return false;
            }
            finally
            {
                _isLoading = false;
            }
        }

        /// <summary>
        /// Deletes the save file.
        /// </summary>
        public void DeleteSave()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    File.Delete(SaveFilePath);
                    Debug.Log("SaveSystem: Save file deleted");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveSystem: Failed to delete save file: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a save file exists.
        /// </summary>
        public bool SaveExists()
        {
            return File.Exists(SaveFilePath);
        }

        private SaveData CollectSaveData()
        {
            var data = new SaveData();

            // Save tower progress
            if (TowerManager.IsInitialized)
            {
                data.highestUnlockedFloor = TowerManager.Instance.HighestUnlocked;
            }

            // Save resources
            if (ResourceManager.IsInitialized)
            {
                var resourceList = new List<ResourceEntry>();
                foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
                {
                    int value = ResourceManager.Instance.Get(type);
                    if (value > 0) // Only save non-zero resources
                    {
                        resourceList.Add(new ResourceEntry { key = type.ToString(), value = value });
                    }
                }
                data.resources = resourceList.ToArray();
            }

            // Save characters
            if (CharacterManager.IsInitialized)
            {
                var characterList = new List<CharacterData>();
                foreach (var character in CharacterManager.Instance.Collection)
                {
                    if (character != null)
                    {
                        characterList.Add(character);
                    }
                }
                data.characters = characterList.ToArray();
                data.storageCapacity = CharacterManager.Instance.StorageCapacity;
            }

            // Save buildings
            if (CityManager.IsInitialized)
            {
                var buildingList = new List<BuildingData>();
                foreach (var building in CityManager.Instance.Buildings)
                {
                    if (building != null)
                    {
                        buildingList.Add(building);
                    }
                }
                data.buildings = buildingList.ToArray();
            }

            data.saveVersion = 1;
            data.saveTimestamp = DateTime.Now.ToBinary();

            return data;
        }

        private void ApplySaveData(SaveData saveData)
        {
            // Load tower progress
            if (TowerManager.IsInitialized && saveData.highestUnlockedFloor > 0)
            {
                // TowerManager doesn't have a public setter, so we need to add one or use reflection
                // For now, we'll add a method to TowerManager
                TowerManager.Instance.SetHighestUnlocked(saveData.highestUnlockedFloor);
            }

            // Load resources
            if (ResourceManager.IsInitialized && saveData.resources != null)
            {
                foreach (var entry in saveData.resources)
                {
                    if (Enum.TryParse<ResourceType>(entry.key, out var resourceType))
                    {
                        ResourceManager.Instance.Set(resourceType, entry.value);
                    }
                }
            }

            // Load characters
            if (CharacterManager.IsInitialized && saveData.characters != null)
            {
                var characterList = new List<CharacterData>(saveData.characters);
                CharacterManager.Instance.LoadCollection(characterList, saveData.storageCapacity);
            }

            // Load buildings
            if (CityManager.IsInitialized && saveData.buildings != null && saveData.buildings.Length > 0)
            {
                var buildingList = new List<BuildingData>(saveData.buildings);
                Debug.Log($"SaveSystem: Loading {buildingList.Count} buildings from save");
                CityManager.Instance.LoadBuildings(buildingList);
            }
            else
            {
                Debug.Log("SaveSystem: No buildings in save data or CityManager not initialized");
                // If no save data, sync house capacity after marking as loaded
                if (CityManager.IsInitialized)
                {
                    CityManager.Instance.SyncHouseCapacityAfterLoad();
                }
            }
        }
    }

    /// <summary>
    /// Serializable data structure for save file.
    /// Unity's JsonUtility doesn't support Dictionary directly, so we use arrays.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public int saveVersion;
        public long saveTimestamp;
        public int highestUnlockedFloor;
        public ResourceEntry[] resources;
        public CharacterData[] characters;
        public int storageCapacity;
        public BuildingData[] buildings;
    }

    /// <summary>
    /// Helper class for serializing resource dictionary.
    /// </summary>
    [Serializable]
    public class ResourceEntry
    {
        public string key;
        public int value;
    }
}

