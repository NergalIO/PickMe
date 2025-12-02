using System;
using UnityEngine;
namespace PickMe.Data
{
    [Serializable]
    public class AbilityData
    {
        public int ability_id;
        public string ability_name;
        public string activate;
        public string ability_effect;
        public float ability_duration;
        public float ability_cooldown;
        public AbilityData()
        {
            ability_id = 0;
            ability_name = "";
            activate = "";
            ability_effect = "";
            ability_duration = 0f;
            ability_cooldown = 0f;
        }
    }
    [Serializable]
    public class AbilityActivateCondition
    {
        public string stat;
        public ComparisonOperator comparison;
        public float value;
        public bool CheckCondition(float currentStatValue)
        {
            switch (comparison)
            {
                case ComparisonOperator.LessThan:
                    return currentStatValue < value;
                case ComparisonOperator.LessThanOrEqual:
                    return currentStatValue <= value;
                case ComparisonOperator.Equal:
                    return Mathf.Approximately(currentStatValue, value);
                case ComparisonOperator.GreaterThanOrEqual:
                    return currentStatValue >= value;
                case ComparisonOperator.GreaterThan:
                    return currentStatValue > value;
                default:
                    return false;
            }
        }
    }
    public enum ComparisonOperator
    {
        LessThan,
        LessThanOrEqual,
        Equal,
        GreaterThanOrEqual,
        GreaterThan
    }
}
