using PickMe.Core.Services;
using UnityEngine;

namespace PickMe.Camera
{
    /// <summary>
    /// Controls camera horizontal movement on tap/drag or mouse drag.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float dragSensitivity = 0.01f;
        
        [Header("Bounds")]
        [SerializeField] private bool useBounds = true;
        [SerializeField] private float minX = -10f;
        [SerializeField] private float maxX = 10f;
        [SerializeField] private SpriteRenderer boundsSprite;
        [SerializeField] private string boundsSpriteName = "Battlefield";
        
        private UnityEngine.Camera cam;
        private Vector2 lastMousePosition;
        private Vector2 lastTouchPosition;
        private bool isDragging;
        
        private void Awake()
        {
            cam = GetComponent<UnityEngine.Camera>();
            if (cam == null)
            {
                cam = UnityEngine.Camera.main;
            }
        }

        private void Start()
        {
            CalculateBoundsFromSprite();
        }

        private void CalculateBoundsFromSprite()
        {
            if (!useBounds) return;

            SpriteRenderer sprite = boundsSprite;
            
            // If not assigned, try to find by name
            if (sprite == null && !string.IsNullOrEmpty(boundsSpriteName))
            {
                var go = GameObject.Find(boundsSpriteName);
                if (go != null)
                {
                    sprite = go.GetComponent<SpriteRenderer>();
                }
            }
            
            // If still not found, find any SpriteRenderer in scene
            if (sprite == null)
            {
                sprite = FindFirstObjectByType<SpriteRenderer>();
            }
            
            if (sprite != null)
            {
                // Calculate bounds from sprite
                Bounds spriteBounds = sprite.bounds;
                minX = spriteBounds.min.x;
                maxX = spriteBounds.max.x;
                
                Debug.Log($"CameraController: Bounds calculated from sprite '{sprite.name}': minX={minX}, maxX={maxX}");
            }
            else
            {
                Debug.LogWarning("CameraController: No sprite found for bounds calculation. Using default values.");
            }
        }

        private void Update()
        {
            if (!InputManager.IsInitialized) return;
            
            HandleMouseInput();
            HandleTouchInput();
        }

        private void HandleMouseInput()
        {
            var inputManager = InputManager.Instance;
            bool isPressed = inputManager.IsMouseButtonPressed(0);
            Vector2 currentPosition = inputManager.GetMousePosition();
            
            if (isPressed && !isDragging)
            {
                // Mouse button just pressed
                lastMousePosition = currentPosition;
                isDragging = true;
            }
            else if (isPressed && isDragging)
            {
                // Mouse button held - drag camera
                Vector2 delta = currentPosition - lastMousePosition;
                MoveCameraHorizontal(delta.x);
                lastMousePosition = currentPosition;
            }
            else if (!isPressed && isDragging)
            {
                // Mouse button released
                isDragging = false;
            }
        }

        private void HandleTouchInput()
        {
            var inputManager = InputManager.Instance;
            
            if (inputManager.GetTouchInput(out Vector2 touchPosition, out bool isPressed))
            {
                if (isPressed && !isDragging)
                {
                    // Touch just started
                    lastTouchPosition = touchPosition;
                    isDragging = true;
                }
                else if (isPressed && isDragging)
                {
                    // Touch moved - drag camera
                    Vector2 delta = touchPosition - lastTouchPosition;
                    MoveCameraHorizontal(delta.x);
                    lastTouchPosition = touchPosition;
                }
                else if (!isPressed && isDragging)
                {
                    // Touch ended
                    isDragging = false;
                }
            }
        }

        private void MoveCameraHorizontal(float deltaX)
        {
            // Convert screen delta to world movement
            // Move camera horizontally in world space (only X axis)
            // deltaX is in screen pixels, convert to world units
            float worldDeltaX = deltaX * dragSensitivity;
            
            // Move along camera's right vector projected onto world XZ plane
            Vector3 rightDirection = transform.right;
            rightDirection.y = 0; // Keep movement horizontal only
            rightDirection.Normalize();
            
            Vector3 movement = rightDirection * worldDeltaX;
            Vector3 newPosition = transform.position + movement;
            
            // Apply bounds if enabled
            if (useBounds)
            {
                newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            }
            
            transform.position = newPosition;
        }
    }
}

