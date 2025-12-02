using UnityEngine;
using PickMe.Data;
namespace PickMe.Combat.Tower
{
    public class TowerFloor : MonoBehaviour
    {
        [Header("Floor Data")]
        [SerializeField] private int floorNumber;
        [SerializeField] private TowerFloorData floorData;
        public int FloorNumber => floorNumber;
        public TowerFloorData FloorData => floorData;
        public void Initialize(int floor, TowerFloorData data)
        {
            floorNumber = floor;
            floorData = data;
        }
    }
}
