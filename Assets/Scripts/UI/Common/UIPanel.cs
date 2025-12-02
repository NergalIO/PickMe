using UnityEngine;
namespace PickMe.UI.Common
{
    public class UIPanel : MonoBehaviour
    {
        [Header("Panel Settings")]
        [SerializeField] protected GameObject panelObject;
        [SerializeField] protected bool startVisible = false;
        protected bool isOpen = false;
        protected virtual void Start()
        {
            if (panelObject != null)
            {
                panelObject.SetActive(startVisible);
                isOpen = startVisible;
            }
        }
        public virtual void Open()
        {
            if (panelObject != null)
            {
                panelObject.SetActive(true);
            }
            isOpen = true;
            OnOpened();
        }
        public virtual void Close()
        {
            if (panelObject != null)
            {
                panelObject.SetActive(false);
            }
            isOpen = false;
            OnClosed();
        }
        public virtual void Toggle()
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
        protected virtual void OnOpened() { }
        protected virtual void OnClosed() { }
    }
}
