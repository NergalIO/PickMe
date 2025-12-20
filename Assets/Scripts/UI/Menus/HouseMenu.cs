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
    /// House menu - view character collection and open character details.
    /// </summary>
    public class HouseMenu : Menu
    {
        [Header("Character List")]
        [SerializeField] private Transform _characterListContainer;
        [SerializeField] private GameObject _characterCardPrefab;
        [SerializeField] private ScrollRect _scrollRect;
        
        private readonly List<CharacterCardUI> _cards = new();
        private int _lastCollectionCount = -1;
        private bool _isRefreshing;

        private void Start()
        {
            // Auto-find ScrollRect if not assigned
            if (_scrollRect == null)
            {
                _scrollRect = GetComponentInChildren<ScrollRect>();
            }
        }

        public override void OnFocus()
        {
            base.OnFocus();
            // Always refresh collection when menu opens
            RefreshCollection();
        }

        public override void OnBlur()
        {
            base.OnBlur();
            // Clear cards when menu closes to prevent stale data
            ClearCards();
            // Reset last count so next open will refresh
            _lastCollectionCount = -1;
        }

        public override void OnScroll(Vector2 delta)
        {
            // Don't call base to prevent any potential refresh triggers
            // ScrollRect will handle scrolling automatically via Unity's UI system
        }

        private void RefreshCollectionIfNeeded()
        {
            if (!CharacterManager.IsInitialized) return;
            
            var collection = CharacterManager.Instance.Collection;
            int currentCount = collection.Count;
            
            // Only refresh if collection count changed
            if (currentCount != _lastCollectionCount && !_isRefreshing)
            {
                StartCoroutine(RefreshCollectionCoroutine());
            }
        }

        private IEnumerator RefreshCollectionCoroutine()
        {
            if (_isRefreshing) yield break;
            _isRefreshing = true;

            // Wait for CharacterManager if not ready
            if (!CharacterManager.IsInitialized)
            {
                yield return CharacterManager.WaitUntilInitialized();
            }
            
            if (!CharacterManager.IsInitialized)
            {
                _isRefreshing = false;
                yield break;
            }

            // Save scroll position
            float savedScrollPosition = 0f;
            if (_scrollRect != null)
            {
                savedScrollPosition = _scrollRect.verticalNormalizedPosition;
            }

            // Clear and recreate cards
            ClearCards();
            
            var collection = CharacterManager.Instance.Collection;
            _lastCollectionCount = collection.Count;
            
            foreach (var character in collection)
            {
                CreateCharacterCard(character);
            }

            // Wait for layout to update
            yield return null;
            yield return null; // Wait one more frame for layout calculation

            // Restore scroll position
            if (_scrollRect != null)
            {
                _scrollRect.verticalNormalizedPosition = savedScrollPosition;
            }

            _isRefreshing = false;
        }

        /// <summary>
        /// Forces a refresh of the character collection display.
        /// Can be called externally to update the list.
        /// </summary>
        public void RefreshCollection()
        {
            if (_isRefreshing) return;
            _lastCollectionCount = -1; // Force refresh
            StartCoroutine(RefreshCollectionCoroutine());
        }

        private void CreateCharacterCard(CharacterData character)
        {
            if (_characterCardPrefab == null || _characterListContainer == null) return;
            
            var cardObj = Instantiate(_characterCardPrefab, _characterListContainer);
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
            // Destroy all card objects
            foreach (var card in _cards)
            {
                if (card != null) Destroy(card.gameObject);
            }
            _cards.Clear();
            
            // Also clear all child objects in container to prevent duplicates
            MenuUtils.ClearContainer(_characterListContainer);
        }

        public override void OnCancel()
        {
            base.OnCancel();
            MenuUtils.CloseCurrentMenu();
        }
    }
}

