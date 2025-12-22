using PickMe.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PickMe.UI
{
    /// <summary>
    /// Menu for displaying a character card in collection.
    /// Displays character portrait, name, class icon, stats and abilities.
    /// </summary>
    public class CharacterCardUI : Menu
    {
        [Header("Character Display")]
        [SerializeField] private Image portraitImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image classIconImage;
        
        [Header("Visuals")]
        [SerializeField] private TMP_Text classText;
        [SerializeField] private GameObject deadOverlay;
        [SerializeField] private TMP_Text hpText;
        [SerializeField] private TMP_Text atkText;
        
        [Header("Class Icons")]
        [SerializeField] private Sprite warriorIcon;
        [SerializeField] private Sprite scoutIcon;
        [SerializeField] private Sprite archerIcon;
        [SerializeField] private Sprite mageIcon;
        
        [Header("Character Sprites")]
        [SerializeField] private Sprite warriorSprite;
        [SerializeField] private Sprite scoutSprite;
        [SerializeField] private Sprite archerSprite;
        [SerializeField] private Sprite mageSprite;
        
        [Header("Ability")]
        [SerializeField] private GameObject abilityEmpty;
        [SerializeField] private GameObject abilityViewPrefab;
        [SerializeField] private Transform abilityContainer;
        
        private AbilityView _abilityView;
        private CharacterData _character;

        public override void Awake()
        {
            base.Awake();
        }

        public void Setup(CharacterData character)
        {
            if (character == null)
            {
                Debug.LogWarning("CharacterCardUI: Setup called with null character");
                return;
            }
            
            Debug.Log($"CharacterCardUI: Setup called for character {character.ch_name} (Class: {character.class_tag}, HP: {character.base_hp}, ATK: {character.base_atk})");
            _character = character;
            RefreshVisuals();
        }

        private void RefreshVisuals()
        {
            if (_character == null)
            {
                Debug.LogWarning("CharacterCardUI: RefreshVisuals called but _character is null");
                return;
            }
            
            Debug.Log($"CharacterCardUI: Refreshing visuals for {_character.ch_name}");
            
            // Set character name
            if (nameText != null)
            {
                nameText.text = _character.ch_name;
                Debug.Log($"CharacterCardUI: Set name text to '{_character.ch_name}'");
            }
            else
            {
                Debug.LogWarning("CharacterCardUI: nameText is null");
            }
            
            // Set portrait sprite
            if (portraitImage != null)
            {
                Sprite sprite = GetCharacterSprite(_character.class_tag);
                if (sprite != null)
                {
                    portraitImage.sprite = sprite;
                    portraitImage.enabled = true;
                }
                else
                {
                    portraitImage.enabled = false;
                }
            }
            
            // Set class icon
            if (classIconImage != null)
            {
                Sprite iconSprite = GetClassIcon(_character.class_tag);
                if (iconSprite != null)
                {
                    classIconImage.sprite = iconSprite;
                    classIconImage.enabled = true;
                }
                else
                {
                    classIconImage.enabled = false;
                }
            }
            
            // Set class text
            if (classText != null)
            {
                classText.text = _character.class_tag.ToString();
            }
            
            // Set HP text
            if (hpText != null)
            {
                hpText.text = $"HP: {_character.base_hp}";
            }
            
            // Set ATK text
            if (atkText != null)
            {
                atkText.text = $"ATK: {_character.base_atk}";
            }
            
            // Set dead overlay
            if (deadOverlay != null)
            {
                deadOverlay.SetActive(_character.is_dead);
            }
            
            // Handle ability display
            RefreshAbility();
        }

        /// <summary>
        /// Gets the class icon sprite based on class tag.
        /// </summary>
        private Sprite GetClassIcon(CharacterClassTag classTag)
        {
            return classTag switch
            {
                CharacterClassTag.Warrior => warriorIcon,
                CharacterClassTag.Scout => scoutIcon,
                CharacterClassTag.Tank => archerIcon, // Tank uses archer icon
                CharacterClassTag.Mage => mageIcon,
                _ => null
            };
        }

        /// <summary>
        /// Gets the character sprite based on class tag.
        /// </summary>
        private Sprite GetCharacterSprite(CharacterClassTag classTag)
        {
            return classTag switch
            {
                CharacterClassTag.Warrior => warriorSprite,
                CharacterClassTag.Scout => scoutSprite,
                CharacterClassTag.Tank => archerSprite, // Tank uses archer sprite
                CharacterClassTag.Mage => mageSprite,
                _ => null
            };
        }

        private void RefreshAbility()
        {
            if (_character == null) return;
            
            // Show empty if no ability
            if (abilityEmpty != null)
            {
                abilityEmpty.SetActive(!_character.has_ability || _character.ability == null);
            }
            
            // Show ability view if ability exists
            if (_character.has_ability && _character.ability != null)
            {
                // Create AbilityView instance if not exists
                if (_abilityView == null && abilityViewPrefab != null && abilityContainer != null)
                {
                    var viewObj = Instantiate(abilityViewPrefab, abilityContainer);
                    _abilityView = viewObj.GetComponent<AbilityView>();
                    
                    // Setup RectTransform for proper UI scaling
                    var rectTransform = viewObj.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.localScale = Vector3.one;
                        rectTransform.anchoredPosition = Vector2.zero;
                        // Set to stretch if container is RectTransform
                        var containerRect = abilityContainer as RectTransform;
                        if (containerRect != null)
                        {
                            rectTransform.anchorMin = Vector2.zero;
                            rectTransform.anchorMax = Vector2.one;
                            rectTransform.sizeDelta = Vector2.zero;
                        }
                    }
                }
                
                // Set ability data
                if (_abilityView != null)
                {
                    _abilityView.SetAbility(_character.ability);
                }
            }
            else
            {
                // Destroy ability view if exists and character has no ability
                if (_abilityView != null)
                {
                    Destroy(_abilityView.gameObject);
                    _abilityView = null;
                }
            }
        }
    }
}

