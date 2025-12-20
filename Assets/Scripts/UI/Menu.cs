using PickMe.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace PickMe.UI
{
    /// <summary>
    /// Base class for UI menus. Derive and override handlers as needed.
    /// </summary>
    public class Menu : MonoBehaviour
    {
        [SerializeField] private string menuId = "Menu";
        [SerializeField] private Button closeButton;
        public string Id => menuId;

        public bool IsVisible => gameObject.activeSelf;

        public virtual void Awake()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseButtonClicked);
            }
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            OnFocus();
        }

        public virtual void Hide()
        {
            OnBlur();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Handles close button click - closes menu through UIController if available.
        /// </summary>
        private void OnCloseButtonClicked()
        {
            // Try to close through UIController first (proper way)
            if (UIController.IsInitialized && IsVisible)
            {
                // If menu is visible, it's likely the current menu - close through UIController
                UIController.Instance.CloseCurrent();
            }
            else
            {
                // Fallback: just hide if UIController is not available or menu is not visible
                Hide();
            }
        }

        public virtual void OnFocus() { }
        public virtual void OnBlur() { }

        public virtual void OnNavigate(Vector2 dir) { }
        public virtual void OnSubmit() { }
        public virtual void OnCancel() { }
        public virtual void OnClick(Vector2 position) { }
        public virtual void OnPoint(Vector2 position) { }
        public virtual void OnScroll(Vector2 delta) { }
    }
}

