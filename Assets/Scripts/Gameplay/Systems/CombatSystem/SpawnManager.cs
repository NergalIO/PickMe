using System.Collections.Generic;
using PickMe.Gameplay.Data;
using PickMe.Gameplay.Systems.CombatSystem;
using PickMe.Gameplay.Systems.CharacterSystem;
using PickMe.Gameplay.Systems.TowerSystem;
using UnityEngine;

namespace PickMe.Gameplay.Systems.CombatSystem
{
    /// <summary>
    /// Automatically finds spawn points by tags and spawns combat units.
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject characterPrefab;
        [SerializeField] private GameObject enemyPrefab;
        
        private List<Transform> allySpawnPoints = new();
        private List<Transform> enemySpawnPoints = new();
        
        private Transform charactersContainer;
        private Transform enemiesContainer;
        
        private List<CombatCharacter> spawnedCharacters = new();
        private List<CombatEnemy> spawnedEnemies = new();

        private void Awake()
        {
            Debug.Log("SpawnManager: Awake() called");
            FindSpawnPoints();
            FindContainers();
        }
        
        private void Start()
        {
            // Re-search spawn points in case scene wasn't fully loaded during Awake
            if (allySpawnPoints.Count == 0 && enemySpawnPoints.Count == 0)
            {
                Debug.LogWarning("SpawnManager: No spawn points found in Awake(), re-searching in Start()...");
                FindSpawnPoints();
            }
        }
        
        /// <summary>
        /// Re-searches for spawn points. Can be called if spawn points are added dynamically.
        /// </summary>
        public void RefreshSpawnPoints()
        {
            Debug.Log("SpawnManager: Refreshing spawn points...");
            FindSpawnPoints();
        }

        /// <summary>
        /// Finds all spawn points from SpawnPoints -> EnemySpawnPoints/AllySpawnPoints hierarchy.
        /// </summary>
        private void FindSpawnPoints()
        {
            allySpawnPoints.Clear();
            enemySpawnPoints.Clear();
            
            Debug.Log("SpawnManager: Searching for spawn points...");
            
            // Find SpawnPoints parent object
            GameObject spawnPointsParent = GameObject.Find("SpawnPoints");
            if (spawnPointsParent == null)
            {
                Debug.LogWarning("SpawnManager: SpawnPoints parent object not found in scene, trying fallback by tags");
                // Fallback: try to find by tags
                FindSpawnPointsByTags();
                return;
            }
            
            Debug.Log("SpawnManager: Found SpawnPoints parent object");
            
            // Find AllySpawnPoints container
            Transform allySpawnContainer = spawnPointsParent.transform.Find("AllySpawnPoints");
            if (allySpawnContainer != null)
            {
                Debug.Log($"SpawnManager: Found AllySpawnPoints container with {allySpawnContainer.childCount} children");
                // Get all child transforms as spawn points
                for (int i = 0; i < allySpawnContainer.childCount; i++)
                {
                    Transform child = allySpawnContainer.GetChild(i);
                    if (child != null)
                    {
                        allySpawnPoints.Add(child);
                        Debug.Log($"SpawnManager: Added ally spawn point {i + 1}: {child.name} at {child.position}");
                    }
                }
            }
            else
            {
                Debug.LogWarning("SpawnManager: AllySpawnPoints container not found under SpawnPoints");
            }
            
            // Find EnemySpawnPoints container
            Transform enemySpawnContainer = spawnPointsParent.transform.Find("EnemySpawnPoints");
            if (enemySpawnContainer != null)
            {
                Debug.Log($"SpawnManager: Found EnemySpawnPoints container with {enemySpawnContainer.childCount} children");
                // Get all child transforms as spawn points
                for (int i = 0; i < enemySpawnContainer.childCount; i++)
                {
                    Transform child = enemySpawnContainer.GetChild(i);
                    if (child != null)
                    {
                        enemySpawnPoints.Add(child);
                        Debug.Log($"SpawnManager: Added enemy spawn point {i + 1}: {child.name} at {child.position}");
                    }
                }
            }
            else
            {
                Debug.LogWarning("SpawnManager: EnemySpawnPoints container not found under SpawnPoints");
            }
            
            Debug.Log($"SpawnManager: Found {allySpawnPoints.Count} ally spawn points and {enemySpawnPoints.Count} enemy spawn points");
        }
        
        /// <summary>
        /// Fallback: Finds spawn points by tags if hierarchy structure is not found.
        /// </summary>
        private void FindSpawnPointsByTags()
        {
            // Find all objects with AllySpawnPoint tag
            GameObject[] allySpawnObjects = GameObject.FindGameObjectsWithTag("AllySpawnPoint");
            foreach (var obj in allySpawnObjects)
            {
                allySpawnPoints.Add(obj.transform);
            }
            
            // Find all objects with EnemySpawnPoint tag
            GameObject[] enemySpawnObjects = GameObject.FindGameObjectsWithTag("EnemySpawnPoint");
            foreach (var obj in enemySpawnObjects)
            {
                enemySpawnPoints.Add(obj.transform);
            }
        }

        /// <summary>
        /// Finds containers for spawned units by name.
        /// </summary>
        private void FindContainers()
        {
            // Find Characters container
            GameObject charactersObj = GameObject.Find("Characters");
            if (charactersObj != null)
            {
                charactersContainer = charactersObj.transform;
                Debug.Log("SpawnManager: Found Characters container");
            }
            else
            {
                Debug.LogWarning("SpawnManager: Characters container not found. Spawning at root.");
            }
            
            // Find Enemies container
            GameObject enemiesObj = GameObject.Find("Enemies");
            if (enemiesObj != null)
            {
                enemiesContainer = enemiesObj.transform;
                Debug.Log("SpawnManager: Found Enemies container");
            }
            else
            {
                Debug.LogWarning("SpawnManager: Enemies container not found. Spawning at root.");
            }
        }

        /// <summary>
        /// Spawns all characters from the party at ally spawn points.
        /// </summary>
        public List<CombatCharacter> SpawnAllies()
        {
            spawnedCharacters.Clear();
            
            Debug.Log("SpawnManager: SpawnAllies called");
            
            // Re-search for spawn points if none found
            if (allySpawnPoints.Count == 0)
            {
                Debug.LogWarning("SpawnManager: No ally spawn points found, re-searching...");
                RefreshSpawnPoints();
            }
            
            // Use Party instead of Team
            if (!Party.IsInitialized)
            {
                Debug.LogWarning("SpawnManager: Party not initialized");
                return spawnedCharacters;
            }
            
            var party = Party.Instance.Members;
            Debug.Log($"SpawnManager: Party has {party.Count} members");
            
            if (party.Count == 0)
            {
                Debug.LogWarning("SpawnManager: No party members to spawn");
                return spawnedCharacters;
            }

            if (allySpawnPoints.Count == 0)
            {
                Debug.LogWarning("SpawnManager: No ally spawn points found. Check SpawnPoints -> AllySpawnPoints hierarchy in scene.");
                return spawnedCharacters;
            }

            if (characterPrefab == null)
            {
                Debug.LogError("SpawnManager: Character prefab is not assigned!");
                return spawnedCharacters;
            }

            Debug.Log($"SpawnManager: Spawning {Mathf.Min(party.Count, allySpawnPoints.Count)} characters");

            // Spawn characters
            for (int i = 0; i < party.Count && i < allySpawnPoints.Count; i++)
            {
                var spawnPoint = allySpawnPoints[i];
                if (spawnPoint == null)
                {
                    Debug.LogWarning($"SpawnManager: Spawn point {i} is null");
                    continue;
                }

                var charObj = Instantiate(characterPrefab, spawnPoint.position, spawnPoint.rotation);
                
                // Set parent to Characters container if found
                if (charactersContainer != null)
                {
                    charObj.transform.SetParent(charactersContainer);
                }
                
                var combatChar = charObj.GetComponent<CombatCharacter>();
                if (combatChar != null)
                {
                    spawnedCharacters.Add(combatChar);
                    Debug.Log($"SpawnManager: Spawned character {i + 1}/{party.Count} at {spawnPoint.position}");
                }
                else
                {
                    Debug.LogWarning($"SpawnManager: Character prefab doesn't have CombatCharacter component!");
                }
            }

            Debug.Log($"SpawnManager: Successfully spawned {spawnedCharacters.Count} characters");
            return spawnedCharacters;
        }

        /// <summary>
        /// Spawns enemies based on floor data at enemy spawn points.
        /// </summary>
        public List<CombatEnemy> SpawnEnemies(TowerFloorData floor)
        {
            spawnedEnemies.Clear();
            
            // Check if EnemyManager is initialized
            if (!EnemyManager.IsInitialized)
            {
                Debug.LogWarning("SpawnManager: EnemyManager not initialized, cannot spawn enemies. Make sure EnemyManager is initialized before calling SpawnEnemies.");
                return spawnedEnemies;
            }
            
            // Re-search for spawn points if none found
            if (enemySpawnPoints.Count == 0)
            {
                Debug.LogWarning("SpawnManager: No enemy spawn points found, re-searching...");
                RefreshSpawnPoints();
            }
            
            if (enemySpawnPoints.Count == 0)
            {
                Debug.LogWarning("SpawnManager: No enemy spawn points found. Check SpawnPoints -> EnemySpawnPoints hierarchy in scene.");
                return spawnedEnemies;
            }

            int spawnPointIndex = 0;
            
            // Spawn enemies
            foreach (var enemySpawn in floor.enemies)
            {
                var enemyData = EnemyManager.Instance.GetEnemyData(enemySpawn.enemyId);
                if (enemyData == null)
                {
                    Debug.LogWarning($"SpawnManager: Enemy data not found for ID: {enemySpawn.enemyId}");
                    continue;
                }

                for (int i = 0; i < enemySpawn.count; i++)
                {
                    var spawnPoint = enemySpawnPoints[spawnPointIndex % enemySpawnPoints.Count];
                    if (spawnPoint == null) continue;

                    var enemyObj = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                    
                    // Set parent to Enemies container if found
                    if (enemiesContainer != null)
                    {
                        enemyObj.transform.SetParent(enemiesContainer);
                    }
                    
                    var combatEnemy = enemyObj.GetComponent<CombatEnemy>();
                    if (combatEnemy != null)
                    {
                        spawnedEnemies.Add(combatEnemy);
                    }
                    
                    spawnPointIndex++;
                }
            }

            return spawnedEnemies;
        }

        /// <summary>
        /// Initializes all spawned characters with their data and enemy lists.
        /// </summary>
        public void InitializeCharacters(List<CombatEnemy> enemies)
        {
            if (!Party.IsInitialized)
            {
                Debug.LogWarning("SpawnManager: Party not initialized, cannot initialize characters");
                return;
            }
            
            var party = Party.Instance.Members;
            
            for (int i = 0; i < spawnedCharacters.Count && i < party.Count; i++)
            {
                spawnedCharacters[i].Initialize(party[i], enemies);
            }
            
            // Update character enemy lists after all enemies are spawned
            foreach (var character in spawnedCharacters)
            {
                character.UpdateEnemyList(enemies);
            }
        }

        /// <summary>
        /// Initializes all spawned enemies with their data and character lists.
        /// </summary>
        public void InitializeEnemies(TowerFloorData floor, List<CombatCharacter> characters)
        {
            int enemyIndex = 0;
            
            foreach (var enemySpawn in floor.enemies)
            {
                var enemyData = EnemyManager.Instance.GetEnemyData(enemySpawn.enemyId);
                if (enemyData == null) continue;

                for (int i = 0; i < enemySpawn.count; i++)
                {
                    if (enemyIndex < spawnedEnemies.Count)
                    {
                        spawnedEnemies[enemyIndex].Initialize(enemyData, characters);
                        enemyIndex++;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all spawned characters.
        /// </summary>
        public List<CombatCharacter> GetSpawnedCharacters()
        {
            return spawnedCharacters;
        }

        /// <summary>
        /// Gets all spawned enemies.
        /// </summary>
        public List<CombatEnemy> GetSpawnedEnemies()
        {
            return spawnedEnemies;
        }

        /// <summary>
        /// Clears all spawned units.
        /// </summary>
        public void ClearSpawnedUnits()
        {
            foreach (var character in spawnedCharacters)
            {
                if (character != null)
                {
                    Destroy(character.gameObject);
                }
            }
            
            foreach (var enemy in spawnedEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy.gameObject);
                }
            }
            
            spawnedCharacters.Clear();
            spawnedEnemies.Clear();
        }
    }
}

