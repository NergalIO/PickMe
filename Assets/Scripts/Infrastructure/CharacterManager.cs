using System;
using System.Collections.Generic;
using System.Linq;
using PickMe.Gameplay;
using UnityEngine;

namespace PickMe.Infrastructure
{
    /// <summary>
    /// Handles player collection, team, and summon generation.
    /// </summary>
    public class CharacterManager : PersistentSingleton<CharacterManager>
    {
        [Header("Storage")]
        [SerializeField] private int baseStorageCapacity = 50;
        
        [Header("Character Generation")]
        [SerializeField] private List<string> names = new()
        {
            "Alex","Jordan","Casey","Riley","Taylor",
            "Sam","Quinn","Morgan","Cameron","Sky"
        };

        [Header("Class Templates")]
        [SerializeField] private List<ClassTemplate> classTemplates = new()
        {
            new ClassTemplate(CharacterClassTag.Warrior, 120, 20, 1.5f, 1.2f, 3.5f, 0.3f),
            new ClassTemplate(CharacterClassTag.Scout,   80,  18, 2.5f, 1.6f, 4.2f, 0.25f),
            new ClassTemplate(CharacterClassTag.Tank,   160, 15, 1.2f, 1.0f, 3.0f, 0.2f),
            new ClassTemplate(CharacterClassTag.Mage,    90,  25, 2.8f, 1.4f, 3.3f, 0.35f),
        };

        private readonly List<CharacterData> _collection = new();
        private readonly List<CharacterData> _team = new();
        private int _storageCapacity;

        public IReadOnlyList<CharacterData> Collection => _collection;
        public IReadOnlyList<CharacterData> Team => _team;
        public int StorageCapacity => _storageCapacity;

        protected override System.Collections.IEnumerator OnInitialized()
        {
            yield return EventController.WaitUntilInitialized();
            _storageCapacity = baseStorageCapacity;
        }

        public bool HasFreeSlots(int incomingCount) => _collection.Count + incomingCount <= _storageCapacity;

        public void SetStorageCapacity(int capacity)
        {
            _storageCapacity = Mathf.Max(0, capacity);
            AutoSave();
        }

        /// <summary>
        /// Sets storage capacity without triggering save (used during load).
        /// </summary>
        public void SetStorageCapacityDirect(int capacity)
        {
            _storageCapacity = Mathf.Max(0, capacity);
        }

        public List<CharacterData> GenerateCharacters(int count)
        {
            var result = new List<CharacterData>(count);
            for (int i = 0; i < count; i++)
            {
                result.Add(CreateRandomCharacter());
            }
            return result;
        }

        public void AddToCollection(IEnumerable<CharacterData> characters)
        {
            int countBefore = _collection.Count;
            _collection.AddRange(characters);
            int countAfter = _collection.Count;
            Debug.Log($"CharacterManager: Added {countAfter - countBefore} characters to collection. Total: {countAfter}");
            AutoSave();
        }

        /// <summary>
        /// Loads collection from save data (used for loading save file).
        /// </summary>
        public void LoadCollection(List<CharacterData> characters, int storageCapacity)
        {
            _collection.Clear();
            if (characters != null)
            {
                _collection.AddRange(characters);
                Debug.Log($"CharacterManager: Loaded {characters.Count} characters from save");
            }
            else
            {
                Debug.Log("CharacterManager: No characters in save data");
            }
            _storageCapacity = storageCapacity > 0 ? storageCapacity : baseStorageCapacity;
            Debug.Log($"CharacterManager: Storage capacity set to {_storageCapacity}");
        }

        private void AutoSave()
        {
            if (SaveSystem.IsInitialized && !SaveSystem.Instance.IsLoading)
            {
                Debug.Log("CharacterManager: Triggering auto-save");
                SaveSystem.Instance.SaveGame();
            }
            else
            {
                Debug.LogWarning($"CharacterManager: Cannot auto-save. SaveSystem initialized: {SaveSystem.IsInitialized}, IsLoading: {SaveSystem.Instance?.IsLoading ?? false}");
            }
        }

        public void SetTeam(IEnumerable<CharacterData> teamMembers)
        {
            _team.Clear();
            _team.AddRange(teamMembers.Where(c => c != null));
        }

        public void MarkDead(string characterId)
        {
            var ch = _collection.FirstOrDefault(c => c.id == characterId);
            if (ch != null)
            {
                ch.is_dead = true;
                AutoSave();
            }
        }

        private CharacterData CreateRandomCharacter()
        {
            var template = GetRandomTemplate();
            var data = new CharacterData
            {
                id = Guid.NewGuid().ToString(),
                ch_name = GetRandomName(),
                class_tag = template.classTag,
                base_hp = template.baseHp,
                base_atk = template.baseAtk,
                atk_range = template.atkRange,
                atk_speed = template.atkSpeed,
                move_speed = template.moveSpeed,
                ability_chance = template.abilityChance,
                has_ability = UnityEngine.Random.value <= template.abilityChance,
                is_dead = false,
                current_hp = template.baseHp
            };

            // Assign random ability if character has ability (based on ability_chance)
            if (data.has_ability)
            {
                data.ability = AbilityFactory.CreateRandomAbility();
            }

            return data;
        }

        private ClassTemplate GetRandomTemplate()
        {
            var idx = UnityEngine.Random.Range(0, classTemplates.Count);
            return classTemplates[idx];
        }

        private string GetRandomName()
        {
            if (names == null || names.Count == 0) return "Hero";
            var idx = UnityEngine.Random.Range(0, names.Count);
            return names[idx];
        }
    }

    [Serializable]
    public class ClassTemplate
    {
        public CharacterClassTag classTag;
        public float baseHp;
        public float baseAtk;
        public float atkRange;
        public float atkSpeed;
        public float moveSpeed;
        public float abilityChance;

        public ClassTemplate(CharacterClassTag classTag, float baseHp, float baseAtk, float atkRange, float atkSpeed, float moveSpeed, float abilityChance)
        {
            this.classTag = classTag;
            this.baseHp = baseHp;
            this.baseAtk = baseAtk;
            this.atkRange = atkRange;
            this.atkSpeed = atkSpeed;
            this.moveSpeed = moveSpeed;
            this.abilityChance = abilityChance;
        }
    }
}

