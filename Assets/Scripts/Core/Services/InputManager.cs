using System.Collections;
using PickMe.Core.Infrastructure;
using PickMe.Input;
using UnityEngine;

namespace PickMe.Core.Services
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

        /// <summary>
        /// Gets current mouse position in screen coordinates.
        /// </summary>
        public Vector2 GetMousePosition()
        {
            var mouse = UnityEngine.InputSystem.Mouse.current;
            return mouse != null ? mouse.position.ReadValue() : Vector2.zero;
        }

        /// <summary>
        /// Checks if left mouse button is pressed.
        /// </summary>
        public bool IsMouseButtonPressed(int buttonIndex)
        {
            var mouse = UnityEngine.InputSystem.Mouse.current;
            if (mouse == null) return false;
            
            return buttonIndex switch
            {
                0 => mouse.leftButton.isPressed,
                1 => mouse.rightButton.isPressed,
                2 => mouse.middleButton.isPressed,
                _ => false
            };
        }

        /// <summary>
        /// Gets touch input if available. Returns true if touch is active.
        /// </summary>
        public bool GetTouchInput(out Vector2 position, out bool isPressed)
        {
            var touchscreen = UnityEngine.InputSystem.Touchscreen.current;
            if (touchscreen != null)
            {
                var touch = touchscreen.primaryTouch;
                isPressed = touch.press.isPressed;
                
                if (isPressed || touch.phase.ReadValue() != UnityEngine.InputSystem.TouchPhase.None)
                {
                    position = touch.position.ReadValue();
                    return true;
                }
            }
            
            position = Vector2.zero;
            isPressed = false;
            return false;
        }

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

        protected override void OnDestroy()
        {
            _input?.Dispose();
            base.OnDestroy();
        }
    }
}

