using UnityEngine;
using PickMe.Infrastructure;
using PickMe.Gameplay;

public class AddTicketsAtStart : MonoBehaviour
{
    [SerializeField] private int ticketsToAdd = 100;
    void Start()
    {
        ResourceManager.Instance.Add(ResourceType.Tickets, ticketsToAdd);
    }
}
