using PickMe.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace PickMe.UI
{
    /// <summary>
    /// UI component for displaying a character card in collection.
    /// </summary>
    public class CharacterCardUI : MonoBehaviour
    {
        [Header("Character Info")]
        [SerializeField] private Text nameText;
        [SerializeField] private Text classText;
        
        [Header("Stats")]
        [SerializeField] private Text hpText;
        [SerializeField] private Text atkText;
        
        [Header("Visuals")]
        [SerializeField] private Image portraitImage;
        [SerializeField] private GameObject abilityBlock;
        [SerializeField] private GameObject deadOverlay;
        
        [Header("Interaction")]
        [SerializeField] private Button cardButton;
        
        private CharacterData _character;
        private System.Action<CharacterData> _onSelected;

        public void Setup(CharacterData character, System.Action<CharacterData> onSelected)
        {
            _character = character;
            _onSelected = onSelected;
            
            RefreshDisplay();
            
            if (cardButton != null)
            {
                cardButton.onClick.RemoveAllListeners();
                cardButton.onClick.AddListener(() => _onSelected?.Invoke(_character));
            }
        }

        private void RefreshDisplay()
        {
            if (_character == null) return;
            
            if (nameText != null) nameText.text = _character.ch_name;
            if (classText != null) classText.text = _character.class_tag.ToString();
            if (hpText != null) hpText.text = $"HP: {_character.base_hp}";
            if (atkText != null) atkText.text = $"ATK: {_character.base_atk}";
            
            // TODO: Set portrait image based on character class/name
            
            if (abilityBlock != null)
            {
                abilityBlock.SetActive(_character.has_ability);
            }
            
            if (deadOverlay != null)
            {
                deadOverlay.SetActive(_character.is_dead);
            }
        }
    }
}

