using System.Collections.Generic;
using PickMe.Gameplay.Data;
using UnityEngine;

namespace PickMe.Gameplay.Systems.CharacterSystem
{
    /// <summary>
    /// Provides ability data instances. Creates random abilities for characters.
    /// </summary>
    public static class AbilityFactory
    {
        private static readonly List<System.Func<AbilityData>> _abilityCreators = new()
        {
            CreateBloodSacrifice,
            CreateSwiftStrike,
            CreateShieldWall,
            CreateFireball,
            CreatePoisonDart,
            CreateHeal,
            CreateBerserkerRage,
            CreateShadowStep
        };

        /// <summary>
        /// Creates a random ability from the available pool.
        /// </summary>
        public static AbilityData CreateRandomAbility()
        {
            if (_abilityCreators.Count == 0)
            {
                return CreateBloodSacrifice(); // Fallback
            }
            
            int index = Random.Range(0, _abilityCreators.Count);
            return _abilityCreators[index]();
        }

        public static AbilityData CreateBloodSacrifice()
        {
            return new AbilityData
            {
                ability_id = "blood_sacrifice",
                ability_name = "Кровавая жертва",
                activate = "HP <= 50%",
                ability_effect = "АТК +50% на 5с, HP -10%",
                ability_duration = 5f,
                ability_cooldown = 12f
            };
        }

        public static AbilityData CreateSwiftStrike()
        {
            return new AbilityData
            {
                ability_id = "swift_strike",
                ability_name = "Быстрый удар",
                activate = "При атаке",
                ability_effect = "АТК +30%, скорость атаки +20% на 3с",
                ability_duration = 3f,
                ability_cooldown = 8f
            };
        }

        public static AbilityData CreateShieldWall()
        {
            return new AbilityData
            {
                ability_id = "shield_wall",
                ability_name = "Стена щитов",
                activate = "HP <= 30%",
                ability_effect = "Защита +40% на 6с",
                ability_duration = 6f,
                ability_cooldown = 15f
            };
        }

        public static AbilityData CreateFireball()
        {
            return new AbilityData
            {
                ability_id = "fireball",
                ability_name = "Огненный шар",
                activate = "При атаке",
                ability_effect = "Урон +60% на 4с, область поражения",
                ability_duration = 4f,
                ability_cooldown = 10f
            };
        }

        public static AbilityData CreatePoisonDart()
        {
            return new AbilityData
            {
                ability_id = "poison_dart",
                ability_name = "Отравленный дротик",
                activate = "При атаке",
                ability_effect = "Яд: 15% урона в секунду на 5с",
                ability_duration = 5f,
                ability_cooldown = 12f
            };
        }

        public static AbilityData CreateHeal()
        {
            return new AbilityData
            {
                ability_id = "heal",
                ability_name = "Лечение",
                activate = "HP <= 40%",
                ability_effect = "Восстанавливает 30% HP",
                ability_duration = 0f,
                ability_cooldown = 20f
            };
        }

        public static AbilityData CreateBerserkerRage()
        {
            return new AbilityData
            {
                ability_id = "berserker_rage",
                ability_name = "Ярость берсерка",
                activate = "HP <= 25%",
                ability_effect = "АТК +80%, скорость +50% на 8с",
                ability_duration = 8f,
                ability_cooldown = 25f
            };
        }

        public static AbilityData CreateShadowStep()
        {
            return new AbilityData
            {
                ability_id = "shadow_step",
                ability_name = "Теневой шаг",
                activate = "При получении урона",
                ability_effect = "Уклонение +70% на 3с",
                ability_duration = 3f,
                ability_cooldown = 15f
            };
        }
    }
}

