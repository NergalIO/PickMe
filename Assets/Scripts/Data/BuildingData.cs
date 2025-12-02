using System;
using UnityEngine;
using PickMe.Managers;
namespace PickMe.Data
{
    [Serializable]
    public class BuildingData
    {
        public BuildingType buildingType;
        public bool isBuilt;
        public bool isUnlocked;
        public string buildingName;
        public string description;
        public BuildingData()
        {
            buildingType = BuildingType.House;
            isBuilt = true;
            isUnlocked = true;
            buildingName = "";
            description = "";
        }
    }
}
