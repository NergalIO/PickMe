using UnityEngine;
using PickMe.Managers;
namespace PickMe.Characters.Collection
{
    public class CollectionUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject collectionPanel;
        [SerializeField] private CollectionManager collectionManager;
        private bool isOpen = false;
        public void OpenCollection()
        {
            if (isOpen) return;
            if (collectionPanel != null)
            {
                collectionPanel.SetActive(true);
            }
            isOpen = true;
            GameManager.Instance.ChangeState(GameState.Collection);
            if (collectionManager != null)
            {
                collectionManager.RefreshCollection();
            }
            Debug.Log("[CollectionUI] Коллекция открыта");
        }
        public void CloseCollection()
        {
            if (!isOpen) return;
            if (collectionPanel != null)
            {
                collectionPanel.SetActive(false);
            }
            isOpen = false;
            GameManager.Instance.ChangeState(GameState.City);
            Debug.Log("[CollectionUI] Коллекция закрыта");
        }
        public void ToggleCollection()
        {
            if (isOpen)
            {
                CloseCollection();
            }
            else
            {
                OpenCollection();
            }
        }
    }
}
