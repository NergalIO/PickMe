using PickMe.Gameplay;
using TMPro;
using UnityEngine;

namespace PickMe.UI
{
    /// <summary>
    /// UI component for displaying ability information.
    /// </summary>
    public class AbilityView : MonoBehaviour
    {
        [Header("Ability Display")]
        [SerializeField] private TMP_Text abilityNameText;
        [SerializeField] private TMP_Text abilityDescriptionText;
        
        private AbilityData _ability;

        /// <summary>
        /// Sets the ability to display.
        /// </summary>
        public void SetAbility(AbilityData ability)
        {
            _ability = ability;
            RefreshDisplay();
        }

        /// <summary>
        /// Refreshes the display with current ability data.
        /// </summary>
        private void RefreshDisplay()
        {
            if (_ability == null) return;
            
            // Set ability name
            if (abilityNameText != null)
            {
                abilityNameText.text = _ability.ability_name;
            }
            
            // Set ability description (using ability_effect)
            if (abilityDescriptionText != null)
            {
                abilityDescriptionText.text = _ability.ability_effect;
            }
        }
    }
}

