using PickMe.Gameplay.Data;
using PickMe.Core.Infrastructure;
using PickMe.UI.Menus.Base;
using PickMe.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace PickMe.UI.Menus.Character
{
    /// <summary>
    /// Character detail menu - displays character stats and abilities (empty for now).
    /// </summary>
    public class CharacterMenu : Menu
    {
        [Header("Character Info")]
        [SerializeField] private Text nameText;
        [SerializeField] private Text classText;
        
        [Header("Stats")]
        [SerializeField] private Text hpText;
        [SerializeField] private Text atkText;
        [SerializeField] private Text rangeText;
        [SerializeField] private Text speedText;
        
        [Header("Ability")]
        [SerializeField] private GameObject abilityBlock;
        [SerializeField] private Text abilityNameText;
        [SerializeField] private Text abilityEffectText;
        
        private CharacterData _currentCharacter;

        public void SetCharacter(CharacterData character)
        {
            _currentCharacter = character;
            RefreshDisplay();
        }

        public override void OnFocus()
        {
            base.OnFocus();
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            if (_currentCharacter == null) return;
            
            if (nameText != null) nameText.text = _currentCharacter.ch_name;
            if (classText != null) classText.text = $"Class: {_currentCharacter.class_tag}";
            if (hpText != null) hpText.text = $"HP: {_currentCharacter.base_hp}";
            if (atkText != null) atkText.text = $"ATK: {_currentCharacter.base_atk}";
            if (rangeText != null) rangeText.text = $"Range: {_currentCharacter.atk_range}";
            if (speedText != null) speedText.text = $"Speed: {_currentCharacter.move_speed}";
            
            if (abilityBlock != null)
            {
                abilityBlock.SetActive(_currentCharacter.has_ability);
            }
            
            if (_currentCharacter.has_ability && _currentCharacter.ability != null)
            {
                if (abilityNameText != null) abilityNameText.text = _currentCharacter.ability.ability_name;
                if (abilityEffectText != null) abilityEffectText.text = _currentCharacter.ability.ability_effect;
            }
        }

        public override void OnCancel()
        {
            base.OnCancel();
            MenuUtils.CloseCurrentMenu();
        }
    }
}

