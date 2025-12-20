using System.Collections;
using PickMe.Input;
using UnityEngine;

namespace PickMe.Infrastructure
{
    /// <summary>
    /// Bridges Unity Input System to EventController (UI actions only).
    /// </summary>
    public class InputManager : PersistentSingleton<InputManager>, UnityInput.IUIActions
    {
        private UnityInput _input;
        private Vector2 _lastPointPosition;

        protected override IEnumerator OnInitialized()
        {
            yield return EventController.WaitUntilInitialized();

            _input = new UnityInput();
            _input.UI.SetCallbacks(this);
            _input.UI.Enable();
        }

        public void EnableUI() => _input?.UI.Enable();
        public void DisableUI() => _input?.UI.Disable();

        #region UI Callbacks
        public void OnNavigate(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (!context.performed || !EventController.IsInitialized) return;
            EventController.Instance.Publish(new UiNavigate(context.ReadValue<Vector2>()));
        }

        public void OnSubmit(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (!context.performed || !EventController.IsInitialized) return;
            EventController.Instance.Publish(new UiSubmit());
        }

        public void OnCancel(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (!context.performed || !EventController.IsInitialized) return;
            EventController.Instance.Publish(new UiCancel());
        }

        public void OnPoint(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (!EventController.IsInitialized) return;
            
            if (context.performed || context.started)
            {
                _lastPointPosition = context.ReadValue<Vector2>();
                EventController.Instance.Publish(new UiPoint(_lastPointPosition));
            }
        }

        public void OnClick(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (!context.performed || !EventController.IsInitialized) return;
            // Use last known point position since Click action returns float, not Vector2
            EventController.Instance.Publish(new UiClick(_lastPointPosition));
        }

        public void OnScrollWheel(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (!context.performed || !EventController.IsInitialized) return;
            EventController.Instance.Publish(new UiScroll(context.ReadValue<Vector2>()));
        }

        public void OnMiddleClick(UnityEngine.InputSystem.InputAction.CallbackContext context) { }
        public void OnRightClick(UnityEngine.InputSystem.InputAction.CallbackContext context) { }
        public void OnTrackedDeviceOrientation(UnityEngine.InputSystem.InputAction.CallbackContext context) { }
        public void OnTrackedDevicePosition(UnityEngine.InputSystem.InputAction.CallbackContext context) { }
        #endregion

        private void OnDestroy()
        {
            _input?.Dispose();
        }
    }
}

