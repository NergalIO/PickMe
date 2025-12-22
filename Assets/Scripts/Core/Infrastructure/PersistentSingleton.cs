using System.Collections;
using UnityEngine;

namespace PickMe.Core.Infrastructure
{
    /// <summary>
    /// Base persistent singleton for managers that must survive scene loads.
    /// Provides unified initialization flow supporting both sync and async initialization.
    /// </summary>
    /// <typeparam name="T">Manager type.</typeparam>
    public abstract class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool _isQuitting;
        private static bool _isInitialized;
        private static T _instance;

        public static T Instance => _instance;
        public static bool IsInitialized => _isInitialized && _instance != null;

        /// <summary>
        /// Override if you need to keep duplicate instances instead of destroying them.
        /// </summary>
        protected virtual bool DestroyOnDuplicate => true;

        protected virtual void Awake()
        {
            if (_isQuitting) return;

            if (_instance != null && _instance != this)
            {
                if (DestroyOnDuplicate)
                {
                    Destroy(gameObject);
                }
                return;
            }

            _instance = this as T;
            DontDestroyOnLoad(gameObject);

            if (!_isInitialized)
            {
                // Support both sync and async initialization
                var asyncInit = OnInitialized();
                if (asyncInit != null)
                {
                    StartCoroutine(InitializeInternal(asyncInit));
                }
                else
                {
                    // Fallback to sync initialization for backward compatibility
                    OnSingletonInitialized();
                    _isInitialized = true;
                }
            }
        }

        private IEnumerator InitializeInternal(IEnumerator asyncInit)
        {
            yield return asyncInit;
            _isInitialized = true;
        }

        /// <summary>
        /// Override to perform async initialization for the singleton.
        /// Return null for synchronous initialization (will call OnSingletonInitialized instead).
        /// </summary>
        protected virtual IEnumerator OnInitialized()
        {
            // Default: use sync initialization for backward compatibility
            return null;
        }

        /// <summary>
        /// Override for synchronous initialization (backward compatibility).
        /// Called automatically if OnInitialized returns null.
        /// </summary>
        protected virtual void OnSingletonInitialized()
        {
        }

        /// <summary>
        /// Waits until the singleton is created and finished initializing.
        /// Use in coroutines before accessing managers if their initialization is not guaranteed.
        /// </summary>
        public static IEnumerator WaitUntilInitialized()
        {
            while (!IsInitialized)
            {
                yield return null;
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                _isInitialized = false;
            }
        }
    }
}

