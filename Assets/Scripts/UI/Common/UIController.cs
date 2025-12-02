using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
namespace PickMe.UI.Common
{
    public class UIController : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private InputSystem_Actions inputActions;
        [SerializeField] private bool autoInitializeInput = true;
        [Header("UI Settings")]
        [SerializeField] private GameObject defaultSelectedObject;
        [SerializeField] private bool useEventSystem = true;
        private Stack<MenuBase> menuStack = new Stack<MenuBase>();
        private MenuBase currentMenu;
        public MenuBase CurrentMenu => currentMenu;
        public bool HasOpenMenu => menuStack.Count > 0;
        private void Awake()
        {
            if (autoInitializeInput)
            {
                InitializeInput();
            }
        }
        private void OnEnable()
        {
            if (inputActions != null)
            {
                EnableInput();
            }
        }
        private void OnDisable()
        {
            if (inputActions != null)
            {
                DisableInput();
            }
        }
        private void OnDestroy()
        {
            if (inputActions != null)
            {
                inputActions.Dispose();
            }
        }
        public void InitializeInput()
        {
            if (inputActions == null)
            {
                inputActions = new InputSystem_Actions();
            }
            RegisterInputCallbacks();
        }
        private void RegisterInputCallbacks()
        {
            if (inputActions == null) return;
            inputActions.UI.Cancel.performed += OnCancelPerformed;
            inputActions.UI.Submit.performed += OnSubmitPerformed;
            inputActions.UI.Navigate.performed += OnNavigatePerformed;
            inputActions.UI.Point.performed += OnPointPerformed;
            inputActions.UI.Click.performed += OnClickPerformed;
        }
        private void UnregisterInputCallbacks()
        {
            if (inputActions == null) return;
            inputActions.UI.Cancel.performed -= OnCancelPerformed;
            inputActions.UI.Submit.performed -= OnSubmitPerformed;
            inputActions.UI.Navigate.performed -= OnNavigatePerformed;
            inputActions.UI.Point.performed -= OnPointPerformed;
            inputActions.UI.Click.performed -= OnClickPerformed;
        }
        public void EnableInput()
        {
            if (inputActions != null)
            {
                inputActions.UI.Enable();
            }
        }
        public void DisableInput()
        {
            if (inputActions != null)
            {
                inputActions.UI.Disable();
            }
        }
        public void OpenMenu(MenuBase menu)
        {
            if (menu == null) return;
            if (currentMenu != null)
            {
                currentMenu.Close();
                menuStack.Push(currentMenu);
            }
            currentMenu = menu;
            menu.Open();
        }
        public void CloseCurrentMenu()
        {
            if (currentMenu == null) return;
            currentMenu.Close();
            currentMenu = null;
            if (menuStack.Count > 0)
            {
                MenuBase previousMenu = menuStack.Pop();
                currentMenu = previousMenu;
                currentMenu.Open();
            }
        }
        public void CloseAllMenus()
        {
            if (currentMenu != null)
            {
                currentMenu.Close();
                currentMenu = null;
            }
            while (menuStack.Count > 0)
            {
                MenuBase menu = menuStack.Pop();
                if (menu != null)
                {
                    menu.Close();
                }
            }
        }
        private void OnCancelPerformed(InputAction.CallbackContext context)
        {
            if (HasOpenMenu)
            {
                CloseCurrentMenu();
            }
        }
        private void OnSubmitPerformed(InputAction.CallbackContext context)
        {
            if (useEventSystem && EventSystem.current != null)
            {
                GameObject selected = EventSystem.current.currentSelectedGameObject;
                if (selected != null)
                {
                    ExecuteEvents.Execute(selected, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                }
            }
        }
        private void OnNavigatePerformed(InputAction.CallbackContext context)
        {
            if (!useEventSystem || EventSystem.current == null) return;
            Vector2 navigation = context.ReadValue<Vector2>();
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
            if (currentSelected == null && defaultSelectedObject != null)
            {
                EventSystem.current.SetSelectedGameObject(defaultSelectedObject);
            }
        }
        private void OnPointPerformed(InputAction.CallbackContext context)
        {
            Vector2 position = context.ReadValue<Vector2>();
            // TODO: Обработка позиции указателя для Touch
        }
        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            if (useEventSystem && EventSystem.current != null)
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = inputActions.UI.Point.ReadValue<Vector2>();
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);
                if (results.Count > 0)
                {
                    GameObject clickedObject = results[0].gameObject;
                    ExecuteEvents.Execute(clickedObject, pointerData, ExecuteEvents.pointerClickHandler);
                }
            }
        }
    }
}
