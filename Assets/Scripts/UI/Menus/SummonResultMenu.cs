using System.Collections;
using System.Collections.Generic;
using PickMe.Gameplay;
using PickMe.Infrastructure;
using PickMe.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace PickMe.UI
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
        
        private readonly List<CharacterCardUI> _cards = new();
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
            if (characterCardPrefab == null || characterListContainer == null) return;
            
            var cardObj = Instantiate(characterCardPrefab, characterListContainer);
            var card = cardObj.GetComponent<CharacterCardUI>();
            if (card != null)
            {
                card.Setup(character, OnCharacterSelected);
                _cards.Add(card);
            }
        }

        private void OnCharacterSelected(CharacterData character)
        {
            if (character == null) return;
            
            // Open character detail menu
            if (UIController.IsInitialized)
            {
                UIController.Instance.Open("CharacterMenu");
                
                // Find the opened menu and set character
                StartCoroutine(SetCharacterAfterOpen(character));
            }
        }

        private IEnumerator SetCharacterAfterOpen(CharacterData character)
        {
            yield return MenuUtils.OpenMenuAndSetData<CharacterMenu>("CharacterMenu", menu =>
            {
                menu.SetCharacter(character);
            });
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

