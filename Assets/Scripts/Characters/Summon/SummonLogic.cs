using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PickMe.Data;
namespace PickMe.Characters.Summon
{
    public class SummonLogic
    {
        private static int nextCharacterId = 1;
        private readonly string[] characterClasses = { "warrior", "scout", "tank", "mage" };
        // TODO: Загружать из конфига
        private Dictionary<string, CharacterClassConfig> classConfigs = new Dictionary<string, CharacterClassConfig>
        {
            { "warrior", new CharacterClassConfig { baseHp = 100, baseAtk = 15, atkRange = 1.5f, atkSpeed = 1.2f, moveSpeed = 2.5f, abilityChance = 0.3f } },
            { "scout", new CharacterClassConfig { baseHp = 80, baseAtk = 20, atkRange = 3.0f, atkSpeed = 1.5f, moveSpeed = 3.0f, abilityChance = 0.25f } },
            { "tank", new CharacterClassConfig { baseHp = 150, baseAtk = 10, atkRange = 1.0f, atkSpeed = 0.8f, moveSpeed = 1.5f, abilityChance = 0.2f } },
            { "mage", new CharacterClassConfig { baseHp = 70, baseAtk = 25, atkRange = 4.0f, atkSpeed = 1.0f, moveSpeed = 2.0f, abilityChance = 0.35f } }
        };
        public List<CharacterData> SummonCharacters(int count)
        {
            List<CharacterData> characters = new List<CharacterData>();
            for (int i = 0; i < count; i++)
            {
                CharacterData character = CreateRandomCharacter();
                characters.Add(character);
            }
            return characters;
        }
        private CharacterData CreateRandomCharacter()
        {
            CharacterData character = new CharacterData();
            character.id = nextCharacterId++;
            string randomClass = characterClasses[Random.Range(0, characterClasses.Length)];
            character.class_tag = randomClass;
            if (classConfigs.ContainsKey(randomClass))
            {
                CharacterClassConfig config = classConfigs[randomClass];
                character.base_hp = config.baseHp;
                character.base_atk = config.baseAtk;
                character.atk_range = config.atkRange;
                character.atk_speed = config.atkSpeed;
                character.move_speed = config.moveSpeed;
                character.ability_chance = config.abilityChance;
            }
            character.has_ability = Random.Range(0f, 1f) <= character.ability_chance;
            character.ch_name = GetRandomName();
            return character;
        }
        private string GetRandomName()
        {
            // TODO: Загружать из Data/Names/names.json
            string[] names = {
                "Alex", "Jordan", "Taylor", "Casey", "Morgan",
                "Riley", "Avery", "Quinn", "Sage", "River"
            };
            return names[Random.Range(0, names.Length)];
        }
    }
    public class CharacterClassConfig
    {
        public int baseHp;
        public int baseAtk;
        public float atkRange;
        public float atkSpeed;
        public float moveSpeed;
        public float abilityChance;
    }
}
