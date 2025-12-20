using System.Linq;
using PickMe.Gameplay;
using PickMe.Infrastructure;
using UnityEngine;

namespace PickMe.City
{
    /// <summary>
    /// Controls visual representation of buildings (ruined vs built state).
    /// Listens to BuildingBuilt events to update visuals.
    /// </summary>
    public class BuildingVisualController : MonoBehaviour
    {
        [Header("Building")]
        [SerializeField] private BuildingType _buildingType;
        
        [Header("Visuals")]
        [SerializeField] private GameObject _ruinedVisual;
        [SerializeField] private GameObject _builtVisual;

        private void Start()
        {
            // Wait for CityManager to initialize before updating visuals
            StartCoroutine(InitializeVisuals());
        }

        private System.Collections.IEnumerator InitializeVisuals()
        {
            // Wait for CityManager to be ready
            if (!CityManager.IsInitialized)
            {
                yield return CityManager.WaitUntilInitialized();
            }
            
            // Subscribe to building built events
            if (EventController.IsInitialized)
            {
                EventController.Instance.Subscribe<BuildingBuilt>(OnBuildingBuilt);
            }
            else
            {
                // Wait for EventController if not ready
                yield return EventController.WaitUntilInitialized();
                EventController.Instance.Subscribe<BuildingBuilt>(OnBuildingBuilt);
            }
            
            // Update visuals after everything is ready
            UpdateVisuals();
        }

        private void OnEnable()
        {
            // Try to subscribe if not already subscribed
            if (EventController.IsInitialized)
            {
                EventController.Instance.Subscribe<BuildingBuilt>(OnBuildingBuilt);
            }
            
            // Update visuals if CityManager is ready
            if (CityManager.IsInitialized)
            {
                UpdateVisuals();
            }
        }

        private void OnDisable()
        {
            if (EventController.IsInitialized)
            {
                EventController.Instance.Unsubscribe<BuildingBuilt>(OnBuildingBuilt);
            }
        }

        private void OnBuildingBuilt(BuildingBuilt evt)
        {
            if (evt.Type == _buildingType)
            {
                UpdateVisuals();
            }
        }

        private void UpdateVisuals()
        {
            if (!CityManager.IsInitialized) return;

            var buildingData = CityManager.Instance.Buildings
                .FirstOrDefault(b => b.type == _buildingType);
            
            if (buildingData == null) return;

            bool isBuilt = buildingData.status == BuildingStatus.Built;

            // Show/hide visuals based on building status
            if (_ruinedVisual != null)
            {
                _ruinedVisual.SetActive(!isBuilt);
            }

            if (_builtVisual != null)
            {
                _builtVisual.SetActive(isBuilt);
            }
        }
    }
}

