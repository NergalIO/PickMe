using System.Collections.Generic;
using UnityEngine;
using PickMe.Data;
namespace PickMe.Combat.Abilities
{
    public class AbilitySystem : MonoBehaviour
    {
        private Dictionary<int, Ability> activeAbilities = new Dictionary<int, Ability>();
        private Dictionary<int, AbilityData> abilityDatabase = new Dictionary<int, AbilityData>();
        private void Update()
        {
            foreach (var ability in activeAbilities.Values)
            {
                ability.Update(Time.deltaTime);
            }
        }
        public void RegisterAbility(CharacterData character, AbilityData abilityData)
        {
            if (character == null || abilityData == null) return;
            if (!character.has_ability) return;
            Ability ability = CreateAbility(abilityData);
            if (ability != null)
            {
                ability.Initialize(abilityData, character);
                activeAbilities[character.id] = ability;
            }
        }
        private Ability CreateAbility(AbilityData abilityData)
        {
            // TODO: Определить тип способности по ability_id или ability_name
            if (abilityData.ability_name.ToLower().Contains("кровавая") || 
                abilityData.ability_name.ToLower().Contains("blood"))
            {
                return new BloodSacrifice();
            }
            return null;
        }
        public void CheckAndActivateAbilities(CharacterData character)
        {
            if (character == null || !character.has_ability) return;
            if (!activeAbilities.ContainsKey(character.id)) return;
            Ability ability = activeAbilities[character.id];
            ability.Activate();
        }
        public Ability GetAbility(int characterId)
        {
            if (activeAbilities.ContainsKey(characterId))
            {
                return activeAbilities[characterId];
            }
            return null;
        }
    }
}
