using PickMe.Core.Infrastructure;
using PickMe.Core.Services;
using UnityEngine;

namespace PickMe.UI.Controllers
{
    /// <summary>
    /// Manages toast notifications across the game.
    /// </summary>
    public class ToastManager : PersistentSingleton<ToastManager>
    {
        [Header("Toast Settings")]
        [SerializeField] private ToastNotification _toastPrefab;
        [SerializeField] private Transform _toastParent;
        
        private ToastNotification _currentToast;

        protected override System.Collections.IEnumerator OnInitialized()
        {
            yield return EventController.WaitUntilInitialized();
            
            // Create toast parent if not assigned
            if (_toastParent == null)
            {
                var canvas = FindFirstObjectByType<Canvas>();
                if (canvas != null)
                {
                    var go = new GameObject("Toast Container");
                    go.transform.SetParent(canvas.transform, false);
                    var rectTransform = go.AddComponent<RectTransform>();
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    rectTransform.sizeDelta = new Vector2(400, 100);
                    rectTransform.anchoredPosition = new Vector2(0, -200);
                    _toastParent = go.transform;
                }
            }
        }

        /// <summary>
        /// Shows a toast notification message.
        /// </summary>
        public void ShowToast(string message, float duration = 2f)
        {
            if (_toastPrefab == null)
            {
                Debug.LogWarning("ToastManager: Toast prefab not assigned!");
                return;
            }

            // Hide current toast if showing
            if (_currentToast != null)
            {
                if (_currentToast.gameObject.activeSelf)
                {
                    _currentToast.Hide();
                }
            }

            // Create toast if needed
            if (_currentToast == null)
            {
                _currentToast = Instantiate(_toastPrefab, _toastParent != null ? _toastParent : transform);
            }

            // Show with new message - this will update the text
            _currentToast.Show(message, duration);
        }

        /// <summary>
        /// Shows a "SOON" notification for locked features.
        /// </summary>
        public void ShowSoon()
        {
            ShowToast("SOON", 1.5f);
        }

        /// <summary>
        /// Shows a building built notification.
        /// </summary>
        public void ShowBuildingBuilt()
        {
            ShowToast("Здание построено!", 2f);
        }
    }
}

