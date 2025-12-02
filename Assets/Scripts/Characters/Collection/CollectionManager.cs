using System.Collections.Generic;
using UnityEngine;
using PickMe.Data;
using PickMe.Managers;
namespace PickMe.Characters.Collection
{
    public class CollectionManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform characterCardContainer;
        [SerializeField] private GameObject characterCardPrefab;
        private List<CharacterCardUI> characterCards = new List<CharacterCardUI>();
        private void Start()
        {
            LoadCollection();
        }
        public void LoadCollection()
        {
            if (!CharacterManager.HasInstance) return;
            ClearCollection();
            var characters = CharacterManager.Instance.CharacterCollection;
            foreach (var character in characters)
            {
                CreateCharacterCard(character);
            }
            Debug.Log($"[CollectionManager] Загружено {characters.Count} персонажей");
        }
        private void CreateCharacterCard(CharacterData character)
        {
            if (characterCardPrefab == null || characterCardContainer == null) return;
            GameObject cardObject = Instantiate(characterCardPrefab, characterCardContainer);
            CharacterCardUI cardUI = cardObject.GetComponent<CharacterCardUI>();
            if (cardUI != null)
            {
                cardUI.Initialize(character);
                characterCards.Add(cardUI);
            }
        }
        private void ClearCollection()
        {
            foreach (var card in characterCards)
            {
                if (card != null)
                {
                    Destroy(card.gameObject);
                }
            }
            characterCards.Clear();
        }
        public void RefreshCollection()
        {
            LoadCollection();
        }
    }
}
