using System.Collections.Generic;
using PickMe.Core.Infrastructure;
using PickMe.Core.Services;
using PickMe.UI.Menus.Base;
using UnityEngine;

namespace PickMe.Core.Managers
{
    /// <summary>
    /// Manages focus and input routing to active menu.
    /// </summary>
    public class UIController : PersistentSingleton<UIController>
    {
        [Header("Menu Registration")]
        [Tooltip("If enabled, automatically finds all Menu objects in the scene. Manual list will be merged with found menus.")]
        [SerializeField] private bool _autoFindMenus = true;
        
        [Tooltip("Manually assigned menus (will be merged with auto-found menus if auto-find is enabled).")]
        [SerializeField] private List<Menu> _manualMenus = new();

        private readonly Dictionary<string, Menu> _registry = new();
        private readonly Stack<Menu> _stack = new();
        private Menu _current;

        #region Initialization

        protected override System.Collections.IEnumerator OnInitialized()
        {
            yield return EventController.WaitUntilInitialized();
            RegisterMenus();
            SubscribeInput();
        }

        private void RegisterMenus()
        {
            _registry.Clear();
            var menusToRegister = new HashSet<Menu>();

            // Add manually assigned menus
            foreach (var menu in _manualMenus)
            {
                if (menu != null)
                {
                    menusToRegister.Add(menu);
                }
            }

            // Auto-find menus from scene if enabled
            if (_autoFindMenus)
            {
                var foundMenus = FindAllMenusInScene();
                foreach (var menu in foundMenus)
                {
                    if (menu != null)
                    {
                        menusToRegister.Add(menu);
                    }
                }
            }

            // Register all collected menus
            foreach (var menu in menusToRegister)
            {
                if (string.IsNullOrEmpty(menu.Id))
                {
                    Debug.LogWarning($"UIController: Menu '{menu.name}' has empty or null Id, skipping registration.", menu);
                    continue;
                }

                // Check for duplicate IDs
                if (_registry.ContainsKey(menu.Id))
                {
                    Debug.LogWarning($"UIController: Duplicate menu ID '{menu.Id}' found. Menu '{menu.name}' will override '{_registry[menu.Id].name}'.", menu);
                }

                _registry[menu.Id] = menu;
                menu.gameObject.SetActive(false);
            }

            Debug.Log($"UIController: Registered {_registry.Count} menu(s).");
        }

        /// <summary>
        /// Finds all Menu components in the current scene.
        /// </summary>
        private List<Menu> FindAllMenusInScene()
        {
            var foundMenus = new List<Menu>();
            
            // Find all Menu components in the scene (including inactive)
            var allMenus = FindObjectsByType<Menu>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foundMenus.AddRange(allMenus);

            return foundMenus;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Opens a menu by its ID.
        /// </summary>
        public void Open(string id)
        {
            if (!_registry.TryGetValue(id, out var menu))
            {
                Debug.LogWarning($"UIController: Menu with ID '{id}' not found in registry.");
                return;
            }
            SetCurrent(menu);
        }

        /// <summary>
        /// Closes the current menu and returns to the previous one if available.
        /// </summary>
        public void CloseCurrent()
        {
            if (_current != null)
            {
                _current.Hide();
                _current = null;
                if (_stack.Count > 0)
                {
                    var previous = _stack.Pop();
                    _current = previous;
                    _current.Show();
                }
                else
                {
                    // No more menus in stack - ensure _current is null so input doesn't get blocked
                    _current = null;
                }
            }
        }

        /// <summary>
        /// Gets a menu by its ID.
        /// </summary>
        public Menu GetMenu(string id)
        {
            return _registry.TryGetValue(id, out var menu) ? menu : null;
        }

        /// <summary>
        /// Checks if a menu with the given ID is registered.
        /// </summary>
        public bool HasMenu(string id)
        {
            return _registry.ContainsKey(id);
        }

        /// <summary>
        /// Gets all registered menu IDs (for debugging).
        /// </summary>
        public IEnumerable<string> GetAllMenuIds()
        {
            return _registry.Keys;
        }

        /// <summary>
        /// Re-registers all menus in the current scene. Useful after loading a new scene.
        /// </summary>
        public void RefreshMenus()
        {
            Debug.Log("UIController: Refreshing menu registry...");
            RegisterMenus();
        }

        #endregion

        #region Private Methods

        private void SetCurrent(Menu menu)
        {
            if (_current == menu) return;
            if (_current != null)
            {
                // Push current to stack before switching
                _stack.Push(_current);
                _current.Hide();
            }

            _current = menu;
            _current.Show();
        }

        private void SubscribeInput()
        {
            EventController.Instance.Subscribe<UiNavigate>(OnNavigate);
            EventController.Instance.Subscribe<UiSubmit>(OnSubmit);
            EventController.Instance.Subscribe<UiCancel>(OnCancel);
            EventController.Instance.Subscribe<UiClick>(OnClick);
            EventController.Instance.Subscribe<UiPoint>(OnPoint);
            EventController.Instance.Subscribe<UiScroll>(OnScroll);
        }

        #endregion

        #region Input Handlers

        private void OnNavigate(UiNavigate evt)
        {
            // Only handle input if there's an active menu
            if (_current != null && _current.IsVisible)
            {
                _current.OnNavigate(evt.Direction);
            }
        }

        private void OnSubmit(UiSubmit evt)
        {
            if (_current != null && _current.IsVisible)
            {
                _current.OnSubmit();
            }
        }

        private void OnCancel(UiCancel evt)
        {
            if (_current != null && _current.IsVisible)
            {
                _current.OnCancel();
            }
        }

        private void OnClick(UiClick evt)
        {
            // Don't block clicks if no menu is active - let them pass through to buttons
            if (_current != null && _current.IsVisible)
            {
                _current.OnClick(evt.Position);
            }
        }

        private void OnPoint(UiPoint evt)
        {
            if (_current != null && _current.IsVisible)
            {
                _current.OnPoint(evt.Position);
            }
        }

        private void OnScroll(UiScroll evt)
        {
            if (_current != null && _current.IsVisible)
            {
                _current.OnScroll(evt.Delta);
            }
        }

        #endregion

        #region Cleanup

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (EventController.IsInitialized)
            {
                EventController.Instance.Unsubscribe<UiNavigate>(OnNavigate);
                EventController.Instance.Unsubscribe<UiSubmit>(OnSubmit);
                EventController.Instance.Unsubscribe<UiCancel>(OnCancel);
                EventController.Instance.Unsubscribe<UiClick>(OnClick);
                EventController.Instance.Unsubscribe<UiPoint>(OnPoint);
                EventController.Instance.Unsubscribe<UiScroll>(OnScroll);
            }
        }

        #endregion
    }
}

