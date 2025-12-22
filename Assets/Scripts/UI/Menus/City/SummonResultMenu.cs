using System.Collections;
using System.Collections.Generic;
using PickMe.Gameplay.Data;
using PickMe.Core.Infrastructure;
using PickMe.UI.Controllers;
using PickMe.UI.Menus.Base;
using PickMe.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace PickMe.UI.Menus.City
{
    /// <summary>
    /// Summon result menu - displays summoned characters.
    /// </summary>
    public class SummonResultMenu : Menu
    {
        [Header("Character List")]
        [SerializeField] private Transform characterListContainer;
        [SerializeField] private GameObject characterCardPrefab;
        
        [Header("Buttons")]
        [SerializeField] private Button continueButton;
        
        private readonly List<CharacterViewUI> _cards = new();
        private List<CharacterData> _summonedCharacters;

        public override void Awake()
        {
            base.Awake();
            
            // Connect continue button handler
            // closeButton is handled by base Menu class
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(OnContinueClicked);
            }
        }

        public void SetSummonedCharacters(List<CharacterData> characters)
        {
            Debug.Log($"SummonResultMenu: SetSummonedCharacters called with {characters?.Count ?? 0} characters");
            _summonedCharacters = characters;
            RefreshDisplay();
        }

        public override void OnFocus()
        {
            base.OnFocus();
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            ClearCards();
            
            if (_summonedCharacters == null || _summonedCharacters.Count == 0)
            {
                Debug.LogWarning("SummonResultMenu: No characters to display");
                return;
            }
            
            foreach (var character in _summonedCharacters)
            {
                CreateCharacterCard(character);
            }
        }

        private void CreateCharacterCard(CharacterData character)
        {
            if (characterCardPrefab == null || characterListContainer == null)
            {
                Debug.LogWarning("SummonResultMenu: characterCardPrefab or characterListContainer is null");
                return;
            }
            
            if (character == null)
            {
                Debug.LogWarning("SummonResultMenu: character is null");
                return;
            }
            
            Debug.Log($"SummonResultMenu: Creating card for character {character.ch_name} (Class: {character.class_tag}, HP: {character.base_hp}, ATK: {character.base_atk})");
            
            var cardObj = Instantiate(characterCardPrefab, characterListContainer);
            
            // Ensure the object is active so components can initialize
            cardObj.SetActive(true);
            
            // Wait one frame to ensure all components are initialized (especially if CharacterCardUI is a Menu)
            StartCoroutine(SetupCardAfterFrame(cardObj, character));
        }
        
        private IEnumerator SetupCardAfterFrame(GameObject cardObj, CharacterData character)
        {
            yield return null; // Wait one frame for initialization
            
            var card = cardObj.GetComponent<CharacterViewUI>();
            if (card != null)
            {
                Debug.Log($"SummonResultMenu: Found CharacterViewUI component, calling SetCharacter");
                card.SetCharacter(character);
                // Disable button interaction for summon result display
                card.DisableButton();
                _cards.Add(card);
            }
            else
            {
                Debug.LogError($"SummonResultMenu: CharacterViewUI component not found on prefab {characterCardPrefab.name}");
                Destroy(cardObj);
            }
        }

        private void ClearCards()
        {
            foreach (var card in _cards)
            {
                if (card != null) Destroy(card.gameObject);
            }
            _cards.Clear();
            
            // Also clear all child objects in container to prevent duplicates
            MenuUtils.ClearContainer(characterListContainer);
        }

        private void OnContinueClicked()
        {
            // Same as close - just closes the result menu
            MenuUtils.CloseCurrentMenu();
        }

        public override void OnCancel()
        {
            base.OnCancel();
            // Base OnCancel already closes the menu through UIController
        }
    }
}

