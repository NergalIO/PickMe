using System;

namespace PickMe.Gameplay.Data
{
    [Serializable]
    public class BuildingData
    {
        public BuildingType type;
        public BuildingStatus status;
        public int buildCost; // construction resource cost
        public int unlockFloor; // future use

        // specific: house capacity
        public int storageCapacity;
    }
}

