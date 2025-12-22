using UnityEngine;
using PickMe.Gameplay.Systems.ResourceSystem;
using PickMe.Gameplay.Data;

public class AddTicketsAtStart : MonoBehaviour
{
    [SerializeField] private int ticketsToAdd = 100;
    void Start()
    {
        ResourceManager.Instance.Add(ResourceType.Tickets, ticketsToAdd);
    }
}
