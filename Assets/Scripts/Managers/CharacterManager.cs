using System.Collections.Generic;
using UnityEngine;
using PickMe.Data;
namespace PickMe.Managers
{
    public class CharacterManager : PersistentSingleton<CharacterManager>
    {
        [Header("Storage Settings")]
        [SerializeField] private int maxStorageCapacity = 100;
        private List<CharacterData> characterCollection = new List<CharacterData>();
        private List<CharacterData> currentSquad = new List<CharacterData>();
        public List<CharacterData> CharacterCollection => characterCollection;
        public List<CharacterData> CurrentSquad => currentSquad;
        public int MaxStorageCapacity => maxStorageCapacity;
        public int CurrentStorageCount => characterCollection.Count;
        public bool IsStorageFull => characterCollection.Count >= maxStorageCapacity;
        protected override void OnAwake()
        {
            base.OnAwake();
            LoadCharacters();
        }
        public bool AddCharacter(CharacterData character)
        {
            if (IsStorageFull)
            {
                Debug.LogWarning("[CharacterManager] Хранилище заполнено! Невозможно добавить персонажа.");
                return false;
            }
            if (character == null)
            {
                Debug.LogError("[CharacterManager] Попытка добавить null персонажа!");
                return false;
            }
            characterCollection.Add(character);
            Debug.Log($"[CharacterManager] Персонаж {character.ch_name} добавлен в коллекцию. Всего: {characterCollection.Count}");
            SaveCharacters();
            return true;
        }
        public void AddCharacters(List<CharacterData> characters)
        {
            foreach (var character in characters)
            {
                if (!IsStorageFull)
                {
                    AddCharacter(character);
                }
                else
                {
                    Debug.LogWarning("[CharacterManager] Хранилище заполнено! Остальные персонажи не добавлены.");
                    break;
                }
            }
        }
        public bool RemoveCharacter(CharacterData character)
        {
            if (characterCollection.Remove(character))
            {
                Debug.Log($"[CharacterManager] Персонаж {character.ch_name} удален из коллекции.");
                SaveCharacters();
                return true;
            }
            return false;
        }
        public CharacterData GetCharacterById(int id)
        {
            return characterCollection.Find(c => c.id == id);
        }
        public List<CharacterData> GetAliveCharacters()
        {
            return characterCollection.FindAll(c => !c.is_dead);
        }
        public List<CharacterData> GetCharactersByClass(CharacterClass characterClass)
        {
            return characterCollection.FindAll(c => c.class_tag == characterClass.ToString().ToLower());
        }
        public void SetSquad(List<CharacterData> squad)
        {
            if (squad == null || squad.Count == 0)
            {
                Debug.LogWarning("[CharacterManager] Попытка установить пустой отряд!");
                return;
            }
            currentSquad = new List<CharacterData>(squad);
            Debug.Log($"[CharacterManager] Отряд установлен. Количество персонажей: {currentSquad.Count}");
        }
        public void ClearSquad()
        {
            currentSquad.Clear();
            Debug.Log("[CharacterManager] Отряд очищен.");
        }
        public void MarkCharacterAsDead(int characterId)
        {
            var character = GetCharacterById(characterId);
            if (character != null)
            {
                character.is_dead = true;
                Debug.Log($"[CharacterManager] Персонаж {character.ch_name} отмечен как погибший.");
                SaveCharacters();
            }
        }
        private void LoadCharacters()
        {
            // TODO: Реализовать загрузку из SaveManager
            Debug.Log("[CharacterManager] Загрузка персонажей...");
        }
        private void SaveCharacters()
        {
            // TODO: Реализовать сохранение через SaveManager
            Debug.Log("[CharacterManager] Сохранение персонажей...");
        }
    }
}
