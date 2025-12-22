using System.Collections;
using System.Linq;
using PickMe.Gameplay.Data;
using PickMe.Core.Managers;
using PickMe.Core.Services;
using PickMe.UI.Controllers;
using PickMe.UI.Menus.Combat;
using PickMe.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace PickMe.Gameplay.Systems.CitySystem
{
    /// <summary>
    /// Building component - handles click interaction and opens appropriate menu.
    /// </summary>
    public class Building : MonoBehaviour
    {
        [Header("Building Settings")]
        [SerializeField] private BuildingType buildingType;
        [SerializeField] private string constructionMenuId = "ConstructionMenu";
        [SerializeField] private string buildingMenuId = ""; // Will be set based on building type if empty

        [Header("Interaction")]
        [SerializeField] private Button button;

        private void Awake()
        {
            // Auto-find button if not assigned
            if (button == null)
            {
                button = GetComponent<Button>();
                if (button == null)
                {
                    button = GetComponentInChildren<Button>();
                }
            }
        }

        private void Start()
        {
            // Always use the correct menu ID based on building type (override Inspector value if needed)
            var expectedId = GetMenuIdForBuildingType(buildingType);
            if (!string.IsNullOrEmpty(expectedId))
            {
                if (buildingMenuId != expectedId)
                {
                    if (!string.IsNullOrEmpty(buildingMenuId))
                    {
                        Debug.LogWarning($"Building: Menu ID mismatch for {buildingType}. Inspector has '{buildingMenuId}', but expected '{expectedId}'. Using '{expectedId}'.", this);
                    }
                    buildingMenuId = expectedId;
                }
            }
            
            Debug.Log($"Building: {buildingType} initialized with menuId='{buildingMenuId}'");
            
            // Setup button listener
            if (button != null)
            {
                button.onClick.RemoveListener(OnBuildingClicked); // Remove if already added
                button.onClick.AddListener(OnBuildingClicked);
            }
            else
            {
                Debug.LogWarning($"Building: No Button component found for {buildingType}. Click won't work!", this);
            }
        }

        public void OnBuildingClicked()
        {
            Debug.Log($"Building: OnBuildingClicked called for {buildingType}");
            
            // Try immediate processing first (if managers are ready)
            if (CityManager.IsInitialized && UIController.IsInitialized)
            {
                ProcessBuildingClickImmediate();
                return;
            }
            
            // Otherwise, start coroutine to wait for managers
            StartCoroutine(ProcessBuildingClick());
        }
        
        private void ProcessBuildingClickImmediate()
        {
            if (!CityManager.IsInitialized || !UIController.IsInitialized)
            {
                Debug.LogWarning($"Building: Managers not ready for immediate processing, falling back to coroutine");
                StartCoroutine(ProcessBuildingClick());
                return;
            }
            
            // Get fresh data from CityManager (don't cache, as status might change)
            var buildingData = CityManager.Instance.Buildings.FirstOrDefault(b => b.type == buildingType);
            if (buildingData == null)
            {
                Debug.LogWarning($"Building: No data found for {buildingType} in CityManager. Available buildings: {string.Join(", ", CityManager.Instance.Buildings.Select(b => $"{b.type}({b.status})"))}");
                return;
            }
            
            Debug.Log($"Building: Processing click immediately for {buildingType}, status from CityManager: {buildingData.status}, menuId: '{buildingMenuId}'");
            ProcessBuildingClickLogic(buildingData);
        }

        private IEnumerator ProcessBuildingClick()
        {
            // Wait for managers to initialize with timeout
            if (!CityManager.IsInitialized)
            {
                Debug.Log($"Building: Waiting for CityManager to initialize for {buildingType}...");
                
                // Check if CityManager instance exists
                if (CityManager.Instance == null)
                {
                    Debug.LogError($"Building: CityManager.Instance is null! Make sure CityManager is on the scene.");
                    yield break;
                }
                
                float timeout = 5f;
                float elapsed = 0f;
                while (!CityManager.IsInitialized && elapsed < timeout)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                
                if (!CityManager.IsInitialized)
                {
                    Debug.LogError($"Building: CityManager initialization timeout after {timeout}s for {buildingType}");
                    yield break;
                }
            }
            
            if (!UIController.IsInitialized)
            {
                Debug.Log($"Building: Waiting for UIController to initialize for {buildingType}...");
                
                // Check if UIController instance exists
                if (UIController.Instance == null)
                {
                    Debug.LogError($"Building: UIController.Instance is null! Make sure UIController is on the scene.");
                    yield break;
                }
                
                // Check EventController (UIController depends on it)
                if (EventController.Instance == null)
                {
                    Debug.LogError($"Building: EventController.Instance is null! UIController cannot initialize without EventController.");
                    yield break;
                }
                
                if (!EventController.IsInitialized)
                {
                    Debug.Log($"Building: EventController not initialized yet, waiting...");
                    float eventTimeout = 3f;
                    float eventElapsed = 0f;
                    while (!EventController.IsInitialized && eventElapsed < eventTimeout)
                    {
                        eventElapsed += Time.deltaTime;
                        yield return null;
                    }
                    
                    if (!EventController.IsInitialized)
                    {
                        Debug.LogError($"Building: EventController initialization timeout after {eventTimeout}s");
                        yield break;
                    }
                }
                
                // Now wait for UIController
                float timeout = 5f;
                float elapsed = 0f;
                int checkCount = 0;
                while (!UIController.IsInitialized && elapsed < timeout)
                {
                    elapsed += Time.deltaTime;
                    checkCount++;
                    if (checkCount % 60 == 0) // Log every ~1 second at 60fps
                    {
                        Debug.Log($"Building: Still waiting for UIController... (elapsed: {elapsed:F1}s, " +
                                 $"UIController.Instance exists: {UIController.Instance != null}, " +
                                 $"EventController initialized: {EventController.IsInitialized}, " +
                                 $"EventController.Instance exists: {EventController.Instance != null})");
                    }
                    yield return null;
                }
                
                if (!UIController.IsInitialized)
                {
                    Debug.LogError($"Building: UIController initialization timeout after {timeout}s for {buildingType}. " +
                                 $"UIController.Instance exists: {UIController.Instance != null}, " +
                                 $"EventController initialized: {EventController.IsInitialized}, " +
                                 $"EventController.Instance exists: {EventController.Instance != null}");
                    yield break;
                }
                
                Debug.Log($"Building: UIController initialized successfully for {buildingType}");
            }

            // Now process the click - get fresh data from CityManager
            var buildingData = CityManager.Instance.Buildings.FirstOrDefault(b => b.type == buildingType);
            if (buildingData == null)
            {
                Debug.LogWarning($"Building: No data found for {buildingType} in CityManager. Available buildings: {string.Join(", ", CityManager.Instance.Buildings.Select(b => $"{b.type}({b.status})"))}");
                yield break;
            }
            
            Debug.Log($"Building: Processing click for {buildingType}, status from CityManager: {buildingData.status}, menuId: '{buildingMenuId}'");
            
            ProcessBuildingClickLogic(buildingData);
        }
        
        private void ProcessBuildingClickLogic(BuildingData buildingData)
        {
            // Check if building is built
            if (buildingData.status == BuildingStatus.Built)
            {
                // Special handling for ExpeditionPortal and MergeLab - show "SOON" even when built
                if (buildingType == BuildingType.ExpeditionPortal || buildingType == BuildingType.MergeLab)
                {
                    if (ToastManager.IsInitialized)
                    {
                        ToastManager.Instance.ShowSoon();
                    }
                    else
                    {
                        Debug.Log($"Building: {buildingType} - SOON");
                    }
                    return;
                }

                // Open building menu for other buildings
                if (!string.IsNullOrEmpty(buildingMenuId))
                {
                    Debug.Log($"Building: Opening menu '{buildingMenuId}' for {buildingType}");
                    
                    // List all registered menus for debugging
                    var allMenuIds = string.Join(", ", UIController.Instance.GetAllMenuIds());
                    Debug.Log($"Building: Available menu IDs in UIController: [{allMenuIds}]");
                    
                    bool menuExists = UIController.Instance.HasMenu(buildingMenuId);
                    if (menuExists)
                    {
                        UIController.Instance.Open(buildingMenuId);
                    }
                    else
                    {
                        // Try to find menu by alternative name
                        var expectedId = GetMenuIdForBuildingType(buildingType);
                        if (expectedId != buildingMenuId && UIController.Instance.HasMenu(expectedId))
                        {
                            Debug.LogWarning($"Building: Menu '{buildingMenuId}' not found, but found '{expectedId}'. Using '{expectedId}' instead.");
                            UIController.Instance.Open(expectedId);
                        }
                        else
                        {
                            Debug.LogWarning($"Building: Menu '{buildingMenuId}' not found in UIController for {buildingType}. Expected: '{expectedId}'");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"Building: No menu ID set for {buildingType}");
                }
            }
            else if (buildingData.status == BuildingStatus.Available)
            {
                // Open construction menu
                Debug.Log($"Building: Opening construction menu for {buildingType}");
                bool menuExists = UIController.Instance.HasMenu(constructionMenuId);
                if (menuExists)
                {
                    UIController.Instance.Open(constructionMenuId);
                    
                    // Pass building data to construction menu
                    StartCoroutine(SetConstructionMenuAfterOpen(buildingType, buildingData));
                }
                else
                {
                    Debug.LogWarning($"Building: Construction menu '{constructionMenuId}' not found in UIController");
                }
            }
            else
            {
                // Building is locked/unavailable
                // Show "SOON" notification for ExpeditionPortal and MergeLab
                if (buildingType == BuildingType.ExpeditionPortal || buildingType == BuildingType.MergeLab)
                {
                    if (ToastManager.IsInitialized)
                    {
                        ToastManager.Instance.ShowSoon();
                    }
                    else
                    {
                        Debug.Log($"Building: {buildingType} - SOON");
                    }
                }
                else
                {
                    Debug.Log($"Building: {buildingType} is not available yet");
                }
            }
        }

        private IEnumerator SetConstructionMenuAfterOpen(BuildingType type, BuildingData data)
        {
            yield return PickMe.Utils.MenuUtils.OpenMenuAndSetData<ConstructionMenu>(constructionMenuId, menu =>
            {
                menu.SetBuilding(type, data);
            });
        }

        private string GetMenuIdForBuildingType(BuildingType type)
        {
            return type switch
            {
                BuildingType.House => "HouseMenu",
                BuildingType.SummonHall => "SummonHallMenu",
                BuildingType.Tower => "TowerMenu",
                BuildingType.ExpeditionPortal => "TopLeftMenu", // Placeholder
                BuildingType.MergeLab => "TopLeftMenu", // Placeholder
                _ => ""
            };
        }
    }
}

