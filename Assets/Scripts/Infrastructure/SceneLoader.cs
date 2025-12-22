using System.Collections;
using PickMe.Gameplay;
using PickMe.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PickMe.Infrastructure
{
    /// <summary>
    /// Manages scene loading and transitions.
    /// </summary>
    public class SceneLoader : PersistentSingleton<SceneLoader>
    {
        private string _mainSceneName;
        private string _currentCombatScenePath;
        private int _currentFloorLevel;
        
        protected override IEnumerator OnInitialized()
        {
            yield return EventController.WaitUntilInitialized();
            
            // Store main scene name
            _mainSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            // Subscribe to combat completion to return to main scene
            EventController.Instance.Subscribe<CombatCompleted>(OnCombatCompleted);
        }

        /// <summary>
        /// Loads a combat scene for the specified floor.
        /// </summary>
        public void LoadCombatScene(TowerFloorData floor)
        {
            if (floor == null || string.IsNullOrEmpty(floor.scenePath))
            {
                Debug.LogWarning($"SceneLoader: Floor {floor?.level} has no scene path");
                return;
            }
            
            _currentCombatScenePath = floor.scenePath;
            _currentFloorLevel = floor.level;
            StartCoroutine(LoadSceneCoroutine(floor.scenePath));
        }

        /// <summary>
        /// Returns to the main scene.
        /// </summary>
        public void ReturnToMainScene()
        {
            if (string.IsNullOrEmpty(_mainSceneName))
            {
                Debug.LogWarning("SceneLoader: Main scene name not set");
                return;
            }
            
            StartCoroutine(LoadSceneCoroutine(_mainSceneName));
        }

        private IEnumerator LoadSceneCoroutine(string scenePathOrName)
        {
            // Extract scene name from path if it's a full path
            string sceneName = ExtractSceneName(scenePathOrName);
            
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning($"SceneLoader: Invalid scene path/name: {scenePathOrName}");
                yield break;
            }
            
            // Load scene
            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            // Wait for scene to fully initialize
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.1f);
            
            // Refresh UIController menus after any scene load to register menus from the new scene
            if (UIController.IsInitialized)
            {
                yield return null; // Wait a frame for menus to be created
                UIController.Instance.RefreshMenus();
                Debug.Log($"SceneLoader: Refreshed UIController menus after loading scene '{sceneName}'");
            }
            
            // If this is a combat scene, start combat after loading
            if (!string.IsNullOrEmpty(_currentCombatScenePath) && 
                ExtractSceneName(_currentCombatScenePath) == sceneName)
            {
                yield return new WaitForEndOfFrame(); // Wait for scene to fully initialize
                
                // Wait for managers to initialize in the new scene
                yield return new WaitForSeconds(0.1f);
                
                // Ensure all required managers exist
                EnsureManagerExists<EventController>("EventController");
                EnsureManagerExists<ResourceManager>("ResourceManager");
                EnsureManagerExists<CharacterManager>("CharacterManager");
                EnsureManagerExists<Party>("Party");
                EnsureManagerExists<TowerManager>("TowerManager");
                EnsureManagerExists<EnemyManager>("EnemyManager");
                
                // Ensure CombatManager exists
                if (CombatManager.Instance == null)
                {
                    Debug.Log("SceneLoader: Creating CombatManager in combat scene");
                    GameObject combatManagerObj = new GameObject("CombatManager");
                    var combatManager = combatManagerObj.AddComponent<CombatManager>();
                    Debug.Log($"SceneLoader: CombatManager created, instance: {combatManager != null}");
                    
                    // Wait a frame for Awake to be called
                    yield return null;
                    Debug.Log($"SceneLoader: After frame wait, Instance: {CombatManager.Instance != null}, IsInitialized: {CombatManager.IsInitialized}");
                }
                else
                {
                    Debug.Log($"SceneLoader: CombatManager already exists, IsInitialized: {CombatManager.IsInitialized}");
                }
                
                // Wait for CombatManager to be initialized with timeout
                if (!CombatManager.IsInitialized)
                {
                    Debug.Log("SceneLoader: Waiting for CombatManager initialization...");
                    
                    // Check status of all dependent managers
                    Debug.Log($"SceneLoader: Manager status - EventController: {EventController.IsInitialized}, ResourceManager: {ResourceManager.IsInitialized}, CharacterManager: {CharacterManager.IsInitialized}, Party: {Party.IsInitialized}, TowerManager: {TowerManager.IsInitialized}, EnemyManager: {EnemyManager.IsInitialized}");
                    
                    float timeout = 10f; // Increased timeout
                    float elapsed = 0f;
                    
                    while (!CombatManager.IsInitialized && elapsed < timeout)
                    {
                        yield return null;
                        elapsed += Time.deltaTime;
                        
                        // Log progress every second with manager status
                        if (Mathf.FloorToInt(elapsed) != Mathf.FloorToInt(elapsed - Time.deltaTime))
                        {
                            Debug.Log($"SceneLoader: Still waiting... ({Mathf.FloorToInt(elapsed)}s elapsed) - EventController: {EventController.IsInitialized}, ResourceManager: {ResourceManager.IsInitialized}, CharacterManager: {CharacterManager.IsInitialized}, Party: {Party.IsInitialized}, TowerManager: {TowerManager.IsInitialized}, EnemyManager: {EnemyManager.IsInitialized}");
                        }
                    }
                    
                    if (!CombatManager.IsInitialized)
                    {
                        Debug.LogError($"SceneLoader: CombatManager initialization timeout after {timeout} seconds! Instance exists: {CombatManager.Instance != null}");
                        Debug.LogError($"SceneLoader: Final manager status - EventController: {EventController.IsInitialized}, ResourceManager: {ResourceManager.IsInitialized}, CharacterManager: {CharacterManager.IsInitialized}, Party: {Party.IsInitialized}, TowerManager: {TowerManager.IsInitialized}, EnemyManager: {EnemyManager.IsInitialized}");
                        
                        // Try to force re-initialization
                        Debug.LogWarning("SceneLoader: Attempting to force CombatManager re-initialization...");
                        CombatManager.Instance.ForceReinitialize();
                        
                        // Wait a bit more
                        yield return new WaitForSeconds(2f);
                        
                        if (!CombatManager.IsInitialized)
                        {
                            Debug.LogError("SceneLoader: Force re-initialization failed, aborting combat start");
                            yield break;
                        }
                    }
                }
                
                // Note: UIController menus already refreshed above for all scene loads
                
                if (CombatManager.IsInitialized)
                {
                    Debug.Log($"SceneLoader: Starting combat for floor {_currentFloorLevel}");
                    CombatManager.Instance.StartCombat(_currentFloorLevel);
                }
                else
                {
                    Debug.LogError("SceneLoader: CombatManager failed to initialize!");
                }
            }
        }

        private string ExtractSceneName(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath)) return null;
            
            // If it's already just a scene name, return it
            if (!scenePath.Contains("/"))
            {
                return scenePath;
            }
            
            // Extract scene name from path (e.g., "Assets/Scenes/Combat1.unity" -> "Combat1")
            int lastSlash = scenePath.LastIndexOf('/');
            string fileName = scenePath.Substring(lastSlash + 1);
            
            // Remove .unity extension if present
            if (fileName.EndsWith(".unity"))
            {
                fileName = fileName.Substring(0, fileName.Length - 6);
            }
            
            return fileName;
        }

        private void OnCombatCompleted(CombatCompleted evt)
        {
            // Return to main scene after combat completes
            ReturnToMainScene();
        }

        /// <summary>
        /// Ensures a manager exists, creating it if it doesn't.
        /// </summary>
        private void EnsureManagerExists<T>(string name) where T : MonoBehaviour
        {
            var managerType = typeof(T);
            var instanceProperty = managerType.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var isInitializedProperty = managerType.GetProperty("IsInitialized", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            
            if (instanceProperty != null)
            {
                var instance = instanceProperty.GetValue(null);
                if (instance == null)
                {
                    Debug.Log($"SceneLoader: Creating {name} in combat scene");
                    GameObject obj = new GameObject(name);
                    obj.AddComponent<T>();
                }
            }
        }

        protected override void OnDestroy()
        {
            if (EventController.IsInitialized)
            {
                EventController.Instance.Unsubscribe<CombatCompleted>(OnCombatCompleted);
            }
            base.OnDestroy();
        }
    }
}

