using UnityEngine;
using PickMe.Data;
using PickMe.Managers;
namespace PickMe.City.Buildings
{
    public class Building : MonoBehaviour
    {
        [Header("Building Data")]
        [SerializeField] protected BuildingData buildingData;
        [Header("Visuals")]
        [SerializeField] protected GameObject builtVisual;
        [SerializeField] protected GameObject ruinsVisual;
        public BuildingData BuildingData => buildingData;
        public BuildingType BuildingType => buildingData.buildingType;
        public bool IsBuilt => buildingData.isBuilt;
        protected virtual void Start()
        {
            UpdateVisuals();
        }
        public virtual void Initialize(BuildingData data)
        {
            buildingData = data;
            UpdateVisuals();
        }
        protected virtual void UpdateVisuals()
        {
            if (builtVisual != null)
            {
                builtVisual.SetActive(buildingData.isBuilt);
            }
            if (ruinsVisual != null)
            {
                ruinsVisual.SetActive(!buildingData.isBuilt);
            }
        }
        protected virtual void OnMouseDown()
        {
            OnBuildingClicked();
        }
        public virtual void OnBuildingClicked()
        {
            if (CityManager.HasInstance)
            {
                CityManager.Instance.OpenBuildingUI(buildingData.buildingType);
            }
        }
    }
}
