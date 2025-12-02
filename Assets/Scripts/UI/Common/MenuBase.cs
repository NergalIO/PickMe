using UnityEngine;
using UnityEngine.InputSystem;
namespace PickMe.UI.Common
{
    public abstract class MenuBase : UIPanel
    {
        [Header("Input Settings")]
        [SerializeField] protected bool useInputSystem = true;
        [SerializeField] protected bool closeOnCancel = true;
        [SerializeField] protected bool selectFirstOnOpen = true;
        protected InputSystem_Actions inputActions;
        protected virtual void Awake()
        {
            if (useInputSystem)
            {
                InitializeInput();
            }
        }
        protected virtual void OnEnable()
        {
            if (useInputSystem && inputActions != null)
            {
                EnableInput();
            }
        }
        protected virtual void OnDisable()
        {
            if (useInputSystem && inputActions != null)
            {
                DisableInput();
            }
        }
        protected virtual void OnDestroy()
        {
            if (inputActions != null)
            {
                inputActions.Dispose();
            }
        }
        protected virtual void InitializeInput()
        {
            inputActions = new InputSystem_Actions();
            RegisterInputCallbacks();
        }
        protected virtual void RegisterInputCallbacks()
        {
            if (inputActions == null) return;
            inputActions.UI.Cancel.performed += OnCancelPerformed;
            inputActions.UI.Submit.performed += OnSubmitPerformed;
            inputActions.UI.Navigate.performed += OnNavigatePerformed;
        }
        protected virtual void UnregisterInputCallbacks()
        {
            if (inputActions == null) return;
            inputActions.UI.Cancel.performed -= OnCancelPerformed;
            inputActions.UI.Submit.performed -= OnSubmitPerformed;
            inputActions.UI.Navigate.performed -= OnNavigatePerformed;
        }
        protected virtual void EnableInput()
        {
            if (inputActions != null)
            {
                inputActions.UI.Enable();
            }
        }
        protected virtual void DisableInput()
        {
            if (inputActions != null)
            {
                inputActions.UI.Disable();
            }
        }
        public override void Open()
        {
            base.Open();
            if (useInputSystem)
            {
                EnableInput();
            }
            if (selectFirstOnOpen)
            {
                SelectFirstElement();
            }
        }
        public override void Close()
        {
            if (useInputSystem)
            {
                DisableInput();
            }
            base.Close();
        }
        protected virtual void SelectFirstElement()
        {
            // TODO: Реализовать выбор первого UI элемента
        }
        protected virtual void OnCancelPerformed(InputAction.CallbackContext context)
        {
            if (closeOnCancel && isOpen)
            {
                Close();
            }
        }
        protected virtual void OnSubmitPerformed(InputAction.CallbackContext context)
        {
        }
        protected virtual void OnNavigatePerformed(InputAction.CallbackContext context)
        {
            Vector2 navigation = context.ReadValue<Vector2>();
            // TODO: Реализовать навигацию по UI элементам
        }
    }
}
