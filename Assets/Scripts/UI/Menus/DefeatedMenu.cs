using System.Collections.Generic;
using PickMe.Gameplay;
using PickMe.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace PickMe.UI
{
    /// <summary>
    /// Defeated menu - displayed when player loses combat.
    /// Shows fallen units.
    /// </summary>
    public class DefeatedMenu : Menu
    {
        [Header("Fallen Units")]
        [SerializeField] private Transform fallenUnitsContainer;
        [SerializeField] private GameObject characterCardPrefab;
        [SerializeField] private GameObject noFallenUnitsText;
        
        [Header("Buttons")]
        [SerializeField] private Button continueButton;
        
        private List<CharacterData> _fallenUnits = new();
        
        public override void Awake()
        {
            base.Awake();
            
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(OnContinueClicked);
            }
        }

        /// <summary>
        /// Sets the fallen units to display.
        /// </summary>
        public void SetFallenUnits(List<CharacterData> fallenUnits)
        {
            _fallenUnits = fallenUnits ?? new List<CharacterData>();
            RefreshDisplay();
        }

        public override void OnFocus()
        {
            base.OnFocus();
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            RefreshFallenUnits();
        }

        private void RefreshFallenUnits()
        {
            // Clear existing cards
            if (fallenUnitsContainer != null)
            {
                MenuUtils.ClearContainer(fallenUnitsContainer);
            }
            
            if (_fallenUnits == null || _fallenUnits.Count == 0)
            {
                if (noFallenUnitsText != null)
                {
                    noFallenUnitsText.SetActive(true);
                }
                return;
            }
            
            if (noFallenUnitsText != null)
            {
                noFallenUnitsText.SetActive(false);
            }
            
            // Create character cards for fallen units
            if (characterCardPrefab != null && fallenUnitsContainer != null)
            {
                foreach (var character in _fallenUnits)
                {
                    var cardObj = Instantiate(characterCardPrefab, fallenUnitsContainer);
                    var card = cardObj.GetComponent<CharacterCardUI>();
                    if (card != null)
                    {
                        card.Setup(character);
                    }
                }
            }
        }

        private void OnContinueClicked()
        {
            // Close menu and return to city (handled by SceneLoader)
            MenuUtils.CloseCurrentMenu();
        }

        public override void OnCancel()
        {
            base.OnCancel();
            // Base OnCancel already closes the menu through UIController
        }
    }
}

