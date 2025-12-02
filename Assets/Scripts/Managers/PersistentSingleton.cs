using UnityEngine;
namespace PickMe.Managers
{
    public abstract class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        private static bool _isApplicationQuitting = false;
        private static readonly object _lock = new object();
        public static T Instance
        {
            get
            {
                if (_isApplicationQuitting)
                {
                    Debug.LogWarning($"[PersistentSingleton] Instance '{typeof(T)}' уже уничтожен. " +
                                   "Возвращается null из-за выхода приложения.");
                    return null;
                }
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();
                        if (_instance == null)
                        {
                            GameObject singletonObject = new GameObject();
                            singletonObject.name = typeof(T).Name;
                            _instance = singletonObject.AddComponent<T>();
                            Debug.Log($"[PersistentSingleton] Создан новый экземпляр '{typeof(T)}'");
                        }
                    }
                    return _instance;
                }
            }
        }
        public static bool HasInstance => _instance != null && !_isApplicationQuitting;
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
                OnAwake();
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"[PersistentSingleton] Обнаружен дубликат '{typeof(T)}'. Уничтожается дубликат.");
                Destroy(gameObject);
            }
        }
        protected virtual void OnApplicationQuit()
        {
            _isApplicationQuitting = true;
        }
        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
        protected virtual void OnAwake() { }
    }
}
