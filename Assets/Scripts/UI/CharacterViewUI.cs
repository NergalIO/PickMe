using System.Collections;
using PickMe.Gameplay;
using PickMe.Infrastructure;
using PickMe.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PickMe.UI
{
    /// <summary>
    /// UI component for displaying character view with class icon, name, and sprite.
    /// </summary>
    public class CharacterViewUI : MonoBehaviour
    {
        [Header("Character Display")]
        [SerializeField] private Image classIconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image characterSpriteImage;
        
        [Header("Interaction")]
        [SerializeField] private Button button;
        
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
        
        private CharacterData _character;

        private void Awake()
        {
            // Setup button click handler
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnButtonClicked);
            }
        }

        /// <summary>
        /// Sets the character to display.
        /// </summary>
        public void SetCharacter(CharacterData character)
        {
            _character = character;
            RefreshDisplay();
        }

        /// <summary>
        /// Disables button interaction. Used when CharacterViewUI is used in non-interactive contexts.
        /// </summary>
        public void DisableButton()
        {
            if (button != null)
            {
                button.interactable = false;
            }
        }

        /// <summary>
        /// Gets the current class icon sprite being displayed.
        /// </summary>
        public Sprite GetClassIconSprite()
        {
            if (classIconImage != null && classIconImage.enabled)
            {
                return classIconImage.sprite;
            }
            return null;
        }

        /// <summary>
        /// Handles button click - opens CharacterCardMenu with current character.
        /// </summary>
        private void OnButtonClicked()
        {
            if (_character == null) return;
            
            // Open character card menu
            if (UIController.IsInitialized)
            {
                UIController.Instance.Open("CharacterCardMenu");
                
                // Find the opened menu and set character
                // Use UIController to start coroutine since this object might be inactive
                UIController.Instance.StartCoroutine(SetCharacterAfterOpen(_character));
            }
        }

        private IEnumerator SetCharacterAfterOpen(CharacterData character)
        {
            yield return MenuUtils.OpenMenuAndSetData<CharacterCardUI>("CharacterCardMenu", menu =>
            {
                menu.Setup(character);
            });
        }

        /// <summary>
        /// Refreshes the display with current character data.
        /// </summary>
        private void RefreshDisplay()
        {
            if (_character == null) return;
            
            // Set character name
            if (nameText != null)
            {
                nameText.text = _character.ch_name;
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
            
            // Set character sprite
            if (characterSpriteImage != null)
            {
                Sprite sprite = GetCharacterSprite(_character.class_tag);
                if (sprite != null)
                {
                    characterSpriteImage.sprite = sprite;
                    characterSpriteImage.enabled = true;
                }
                else
                {
                    characterSpriteImage.enabled = false;
                }
            }
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
    }
}

