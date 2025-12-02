using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
namespace PickMe.Managers
{
    public class InputController : PersistentSingleton<InputController>
    {
        [Header("Input Settings")]
        [SerializeField] private InputSystem_Actions inputActions;
        [SerializeField] private bool enableTouchSupport = true;
        [SerializeField] private bool enableEnhancedTouch = true;
        [Header("Touch Settings")]
        [SerializeField] private float tapTimeThreshold = 0.2f;
        [SerializeField] private float swipeDistanceThreshold = 50f;
        public System.Action<Vector2> OnTap;
        public System.Action<Vector2, Vector2> OnSwipe;
        public System.Action<Vector2> OnTouchStart;
        public System.Action<Vector2> OnTouchEnd;
        public System.Action<Vector2> OnTouchMove;
        private Vector2 touchStartPosition;
        private float touchStartTime;
        private bool isTouching = false;
        protected override void OnAwake()
        {
            base.OnAwake();
            InitializeInput();
        }
        private void OnEnable()
        {
            if (enableEnhancedTouch)
            {
                EnhancedTouchSupport.Enable();
            }
            RegisterInputCallbacks();
        }
        private void OnDisable()
        {
            UnregisterInputCallbacks();
            if (enableEnhancedTouch)
            {
                EnhancedTouchSupport.Disable();
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (inputActions != null)
            {
                inputActions.Dispose();
            }
        }
        private void InitializeInput()
        {
            if (inputActions == null)
            {
                inputActions = new InputSystem_Actions();
            }
            if (enableTouchSupport)
            {
                EnableTouchSupport();
            }
        }
        private void EnableTouchSupport()
        {
            if (enableEnhancedTouch && !EnhancedTouchSupport.enabled)
            {
                EnhancedTouchSupport.Enable();
            }
        }
        private void RegisterInputCallbacks()
        {
            if (inputActions == null) return;
            inputActions.Player.Attack.performed += OnAttackPerformed;
            inputActions.Player.Interact.performed += OnInteractPerformed;
            inputActions.Player.Move.performed += OnMovePerformed;
            inputActions.Player.Move.canceled += OnMoveCanceled;
            inputActions.UI.Click.performed += OnUIClickPerformed;
            inputActions.UI.Point.performed += OnUIPointPerformed;
        }
        private void UnregisterInputCallbacks()
        {
            if (inputActions == null) return;
            inputActions.Player.Attack.performed -= OnAttackPerformed;
            inputActions.Player.Interact.performed -= OnInteractPerformed;
            inputActions.Player.Move.performed -= OnMovePerformed;
            inputActions.Player.Move.canceled -= OnMoveCanceled;
            inputActions.UI.Click.performed -= OnUIClickPerformed;
            inputActions.UI.Point.performed -= OnUIPointPerformed;
        }
        private void Update()
        {
            if (enableEnhancedTouch && enableTouchSupport)
            {
                ProcessTouchInput();
            }
        }
        private void ProcessTouchInput()
        {
            if (Touch.activeTouches.Count > 0)
            {
                Touch activeTouch = Touch.activeTouches[0];
                Vector2 touchPosition = activeTouch.screenPosition;
                switch (activeTouch.phase)
                {
                    case UnityEngine.InputSystem.TouchPhase.Began:
                        OnTouchBegan(touchPosition);
                        break;
                    case UnityEngine.InputSystem.TouchPhase.Moved:
                        OnTouchMoved(touchPosition);
                        break;
                    case UnityEngine.InputSystem.TouchPhase.Ended:
                    case UnityEngine.InputSystem.TouchPhase.Canceled:
                        OnTouchEnded(touchPosition);
                        break;
                }
            }
        }
        private void OnTouchBegan(Vector2 position)
        {
            isTouching = true;
            touchStartPosition = position;
            touchStartTime = Time.time;
            OnTouchStart?.Invoke(position);
        }
        private void OnTouchMoved(Vector2 position)
        {
            if (isTouching)
            {
                OnTouchMove?.Invoke(position);
            }
        }
        private void OnTouchEnded(Vector2 position)
        {
            if (!isTouching) return;
            float touchDuration = Time.time - touchStartTime;
            float touchDistance = Vector2.Distance(touchStartPosition, position);
            OnTouchEnd?.Invoke(position);
            if (touchDuration <= tapTimeThreshold && touchDistance < swipeDistanceThreshold)
            {
                OnTap?.Invoke(position);
            }
            else if (touchDistance >= swipeDistanceThreshold)
            {
                OnSwipe?.Invoke(touchStartPosition, position);
            }
            isTouching = false;
        }
        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            if (context.control.device is Touchscreen)
            {
                Vector2 position = inputActions.UI.Point.ReadValue<Vector2>();
                OnTap?.Invoke(position);
            }
        }
        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            // TODO: Обработка взаимодействия
        }
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            // TODO: Обработка движения
        }
        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            // TODO: Обработка остановки движения
        }
        private void OnUIClickPerformed(InputAction.CallbackContext context)
        {
            Vector2 position = inputActions.UI.Point.ReadValue<Vector2>();
            OnTap?.Invoke(position);
        }
        private void OnUIPointPerformed(InputAction.CallbackContext context)
        {
        }
        public Vector2 GetPointerPosition()
        {
            if (inputActions != null)
            {
                return inputActions.UI.Point.ReadValue<Vector2>();
            }
            return Vector2.zero;
        }
        public bool IsTouching()
        {
            return isTouching || (enableEnhancedTouch && Touch.activeTouches.Count > 0);
        }
        public int GetTouchCount()
        {
            if (enableEnhancedTouch)
            {
                return Touch.activeTouches.Count;
            }
            return 0;
        }
    }
}
