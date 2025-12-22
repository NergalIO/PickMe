using UnityEngine;
using UnityEngine.UI;

namespace PickMe.Gameplay.Systems.CombatSystem
{
    /// <summary>
    /// Displays HP bar for combat units. Green for allies, red for enemies.
    /// Attached to prefab as child of CombatUnit.
    /// </summary>
    public class HpBar : MonoBehaviour
    {
        [Header("HP Bar Components")]
        [SerializeField] private Image fillImage;
        [SerializeField] private GameObject hpBarContainer;
        
        [Header("Colors")]
        [SerializeField] private Color allyColor = Color.green;
        [SerializeField] private Color enemyColor = Color.red;
        
        [Header("Visual Settings")]
        [SerializeField] private SpriteRenderer unitSpriteRenderer;
        [SerializeField] private float darkenAmount = 0.3f;
        [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);
        
        private CombatUnit combatUnit;
        private Color originalSpriteColor;
        private bool isAlly;
        private Canvas worldCanvas;

        private void Awake()
        {
            // Find CombatUnit in parent
            combatUnit = GetComponentInParent<CombatUnit>();
            
            // Find Canvas in parent hierarchy
            worldCanvas = GetComponentInParent<Canvas>();
            
            // Find sprite renderer in parent
            if (unitSpriteRenderer == null && combatUnit != null)
            {
                unitSpriteRenderer = combatUnit.GetComponent<SpriteRenderer>();
            }
            
            // Store original sprite color
            if (unitSpriteRenderer != null)
            {
                originalSpriteColor = unitSpriteRenderer.color;
            }
        }

        /// <summary>
        /// Initializes HP bar for a combat unit.
        /// </summary>
        public void Initialize(CombatUnit unit, bool isAllyUnit)
        {
            combatUnit = unit ?? GetComponentInParent<CombatUnit>();
            isAlly = isAllyUnit;
            
            // Set color based on unit type
            if (fillImage != null)
            {
                fillImage.color = isAlly ? allyColor : enemyColor;
            }
            
            // Find sprite renderer if not assigned
            if (unitSpriteRenderer == null && combatUnit != null)
            {
                unitSpriteRenderer = combatUnit.GetComponent<SpriteRenderer>();
            }
            
            // Store original sprite color
            if (unitSpriteRenderer != null)
            {
                originalSpriteColor = unitSpriteRenderer.color;
            }
            
            // Update HP bar
            UpdateHpBar();
        }

        private void Start()
        {
            // Auto-initialize if combat unit found
            if (combatUnit != null)
            {
                // Try to determine if ally or enemy
                bool isAllyUnit = combatUnit is CombatCharacter;
                Initialize(combatUnit, isAllyUnit);
            }
        }

        private void LateUpdate()
        {
            if (combatUnit == null) return;
            
            // Update position to follow unit
            if (worldCanvas != null && combatUnit.transform != null)
            {
                worldCanvas.transform.position = combatUnit.transform.position + offset;
                worldCanvas.transform.rotation = Quaternion.LookRotation(
                    UnityEngine.Camera.main != null ? UnityEngine.Camera.main.transform.forward : Vector3.forward);
            }
            
            UpdateHpBar();
            UpdateSpriteColor();
        }

        private void UpdateHpBar()
        {
            if (combatUnit == null || fillImage == null) return;
            
            // Calculate HP percentage
            float hpPercent = combatUnit.MaxHp > 0 ? combatUnit.CurrentHp / combatUnit.MaxHp : 0f;
            
            // Update fill amount (requires Image Type = Filled)
            fillImage.fillAmount = hpPercent;
            
            // Hide HP bar if unit is dead or HP is 0
            if (hpBarContainer != null)
            {
                bool shouldShow = !combatUnit.IsDead && hpPercent > 0f;
                hpBarContainer.SetActive(shouldShow);
            }
        }

        private void UpdateSpriteColor()
        {
            if (unitSpriteRenderer == null || combatUnit == null) return;
            
            // Darken sprite if unit is dead
            if (combatUnit.IsDead)
            {
                Color darkenedColor = originalSpriteColor * darkenAmount;
                darkenedColor.a = originalSpriteColor.a; // Preserve alpha
                unitSpriteRenderer.color = darkenedColor;
            }
            else
            {
                unitSpriteRenderer.color = originalSpriteColor;
            }
        }
    }
}

