using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PickMe.Gameplay.Data;
using PickMe.Gameplay.Systems.CharacterSystem;
using PickMe.Gameplay.Systems.ResourceSystem;
using PickMe.Gameplay.Systems.TowerSystem;
using PickMe.Core.Infrastructure;
using PickMe.Core.Managers;
using PickMe.Core.Services;
using PickMe.UI.Menus.Base;
using PickMe.UI.Menus.Combat;
using PickMe.Utils;
using UnityEngine;

namespace PickMe.Gameplay.Systems.CombatSystem
{
    /// <summary>
    /// Manages real-time combat with NavMeshAgent AI for characters and enemies.
    /// </summary>
    public class CombatManager : PersistentSingleton<CombatManager>
    {
        private SpawnManager spawnManager;
        private List<CombatCharacter> spawnedCharacters = new();
        private List<CombatEnemy> spawnedEnemies = new();
        private int currentFloorLevel;
        private bool combatActive;
        private TowerFloorData currentFloor;

        protected override void Awake()
        {
            Debug.Log($"CombatManager: Awake() called, Instance exists: {Instance != null}, IsInitialized: {IsInitialized}");
            base.Awake();
            Debug.Log($"CombatManager: After base.Awake(), Instance: {Instance != null}, IsInitialized: {IsInitialized}");
        }
        
        /// <summary>
        /// Forces re-initialization if stuck. Should only be called if initialization is stuck.
        /// </summary>
        public void ForceReinitialize()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("CombatManager: Force re-initializing...");
                StartCoroutine(OnInitialized());
            }
        }

        protected override IEnumerator OnInitialized()
        {
            Debug.Log("CombatManager: Starting initialization...");
            
            Debug.Log("CombatManager: Waiting for EventController...");
            yield return EventController.WaitUntilInitialized();
            Debug.Log("CombatManager: EventController initialized");
            
            Debug.Log("CombatManager: Waiting for ResourceManager...");
            yield return ResourceManager.WaitUntilInitialized();
            Debug.Log("CombatManager: ResourceManager initialized");
            
            Debug.Log("CombatManager: Waiting for CharacterManager...");
            yield return CharacterManager.WaitUntilInitialized();
            Debug.Log("CombatManager: CharacterManager initialized");
            
            Debug.Log("CombatManager: Waiting for Party...");
            yield return Party.WaitUntilInitialized();
            Debug.Log("CombatManager: Party initialized");
            
            Debug.Log("CombatManager: Waiting for TowerManager...");
            yield return TowerManager.WaitUntilInitialized();
            Debug.Log("CombatManager: TowerManager initialized");
            
            Debug.Log("CombatManager: Waiting for EnemyManager...");
            yield return EnemyManager.WaitUntilInitialized();
            Debug.Log("CombatManager: EnemyManager initialized");
            
            Debug.Log("CombatManager: Initialization complete!");
        }

        public void StartCombat(int floorLevel)
        {
            Debug.Log($"CombatManager: StartCombat called for floor {floorLevel}");
            
            currentFloorLevel = floorLevel;
            var floor = TowerManager.Instance.GetFloor(floorLevel);
            if (floor == null)
            {
                Debug.LogWarning($"CombatManager: No floor data for level {floorLevel}");
                return;
            }
            
            // Refresh party before combat
            if (Party.IsInitialized)
            {
                Party.Instance.RefreshParty();
                Debug.Log($"CombatManager: Party refreshed, {Party.Instance.Count} members");
            }
            else
            {
                Debug.LogWarning("CombatManager: Party not initialized!");
            }
            
            // Find SpawnManager in scene
            if (spawnManager == null)
            {
                spawnManager = FindFirstObjectByType<SpawnManager>();
                if (spawnManager == null)
                {
                    Debug.LogError("CombatManager: SpawnManager not found in scene!");
                    return;
                }
                Debug.Log("CombatManager: SpawnManager found");
            }
            
            currentFloor = floor;
            StartCoroutine(SpawnCombatUnitsCoroutine(floor));
        }

        private IEnumerator SpawnCombatUnitsCoroutine(TowerFloorData floor)
        {
            // Wait for EnemyManager to be initialized
            if (!EnemyManager.IsInitialized)
            {
                Debug.Log("CombatManager: Waiting for EnemyManager initialization...");
                yield return EnemyManager.WaitUntilInitialized();
            }
            
            // Clear previous spawns
            spawnManager.ClearSpawnedUnits();
            
            // Spawn allies
            spawnedCharacters = spawnManager.SpawnAllies();
            
            // Spawn enemies
            spawnedEnemies = spawnManager.SpawnEnemies(floor);
            
            // Initialize enemies with character list
            spawnManager.InitializeEnemies(floor, spawnedCharacters);
            
            // Initialize characters with enemy list
            spawnManager.InitializeCharacters(spawnedEnemies);
            
            combatActive = true;
            Debug.Log($"CombatManager: Combat started, spawned {spawnedCharacters.Count} characters and {spawnedEnemies.Count} enemies");
        }

        private void Update()
        {
            if (!combatActive) return;

            CheckCombatEnd();
        }

        private void CheckCombatEnd()
        {
            // Remove dead units
            spawnedCharacters.RemoveAll(c => c == null || c.IsDead);
            spawnedEnemies.RemoveAll(e => e == null || e.IsDead);

            // Check victory: all enemies dead
            if (spawnedEnemies.Count == 0 && spawnedCharacters.Count > 0)
            {
                EndCombat(CombatResultType.Victory);
                return;
            }

            // Check defeat: all characters dead
            if (spawnedCharacters.Count == 0)
            {
                EndCombat(CombatResultType.Defeat);
                return;
            }
        }

        private void EndCombat(CombatResultType result)
        {
            combatActive = false;

            if (result == CombatResultType.Victory)
            {
                GrantRewards(currentFloor.rewards);
                TowerManager.Instance.UnlockNext(currentFloorLevel);
            }
            else
            {
                // Mark dead heroes
                var teamToMark = CharacterManager.Instance.Team;
                foreach (var hero in teamToMark)
                {
                    CharacterManager.Instance.MarkDead(hero.id);
                }
            }

            // Get fallen units (characters that died in combat)
            List<CharacterData> fallenUnits = new();
            var allTeam = CharacterManager.Instance.Team;
            foreach (var character in allTeam)
            {
                if (character.is_dead)
                {
                    fallenUnits.Add(character);
                }
            }

            // Update character HP in CharacterManager
            foreach (var character in spawnedCharacters)
            {
                if (character.CharacterData != null)
                {
                    character.CharacterData.current_hp = character.CurrentHp;
                    character.CharacterData.is_dead = character.IsDead;
                }
            }
            
            // Save game after combat completion
            if (SaveSystem.IsInitialized && !SaveSystem.Instance.IsLoading)
            {
                SaveSystem.Instance.SaveGame();
            }

            // Open result menu with data
            if (UIController.IsInitialized)
            {
                string menuId = result == CombatResultType.Victory ? "VictoryMenu" : "DefeatedMenu";
                StartCoroutine(OpenResultMenu(menuId, result, fallenUnits));
            }

            if (EventController.IsInitialized)
            {
                EventController.Instance.Publish(new CombatCompleted(result, currentFloor.rewards));
            }
        }

        private IEnumerator OpenResultMenu(string menuId, CombatResultType result, List<CharacterData> fallenUnits)
        {
            yield return MenuUtils.OpenMenuAndSetData<Menu>(menuId, menu =>
            {
                if (result == CombatResultType.Victory && menu is PickMe.UI.Menus.Combat.VictoryMenu victoryMenu)
                {
                    victoryMenu.SetData(currentFloor.rewards, fallenUnits);
                }
                else if (result == CombatResultType.Defeat && menu is PickMe.UI.Menus.Combat.DefeatedMenu defeatedMenu)
                {
                    defeatedMenu.SetFallenUnits(fallenUnits);
                }
            });
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

