using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PickMe.UI
{
    /// <summary>
    /// Simple toast notification system for displaying temporary messages.
    /// </summary>
    public class ToastNotification : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField] private TMP_Text _messageText;
        
        [Header("Timing")]
        [SerializeField] private float _displayDuration = 2f;
        [SerializeField] private float _fadeInDuration = 0.3f;
        [SerializeField] private float _fadeOutDuration = 0.3f;
        
        private CanvasGroup _canvasGroup;
        private Coroutine _currentCoroutine;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            // Auto-find message text if not assigned
            if (_messageText == null)
            {
                _messageText = GetComponentInChildren<TMP_Text>();
                if (_messageText == null)
                {
                    _messageText = GetComponent<TMP_Text>();
                }
            }
            
            // Start hidden
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Shows a toast message for the specified duration.
        /// </summary>
        public void Show(string message, float duration = -1f)
        {
            // Stop current coroutine if running
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }

            // Activate object first to ensure components are ready
            gameObject.SetActive(true);

            // Set text after activation to ensure it updates properly
            if (_messageText != null)
            {
                _messageText.text = message; // Use .text instead of SetText for more reliable updates
            }
            else
            {
                Debug.LogWarning("ToastNotification: _messageText is not assigned!");
            }

            _currentCoroutine = StartCoroutine(ShowCoroutine(duration < 0 ? _displayDuration : duration));
        }

        private IEnumerator ShowCoroutine(float duration)
        {
            _canvasGroup.blocksRaycasts = false;

            // Fade in
            float elapsed = 0f;
            while (elapsed < _fadeInDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Clamp01(elapsed / _fadeInDuration);
                yield return null;
            }
            _canvasGroup.alpha = 1f;

            // Wait
            yield return new WaitForSeconds(duration);

            // Fade out
            elapsed = 0f;
            while (elapsed < _fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / _fadeOutDuration);
                yield return null;
            }
            _canvasGroup.alpha = 0f;

            gameObject.SetActive(false);
            _currentCoroutine = null;
        }

        /// <summary>
        /// Hides the toast immediately.
        /// </summary>
        public void Hide()
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }
            _canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }
    }
}

