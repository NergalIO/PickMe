using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PickMe.Data;
namespace PickMe.Characters.Collection
{
    public class CharacterCardUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI classText;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI atkText;
        [SerializeField] private Image portraitImage;
        [SerializeField] private GameObject abilityBlock;
        [SerializeField] private TextMeshProUGUI abilityNameText;
        [SerializeField] private TextMeshProUGUI abilityEffectText;
        [SerializeField] private Image deadOverlay;
        private CharacterData characterData;
        public void Initialize(CharacterData character)
        {
            characterData = character;
            UpdateUI();
        }
        private void UpdateUI()
        {
            if (characterData == null) return;
            if (nameText != null)
            {
                nameText.text = characterData.ch_name;
            }
            if (classText != null)
            {
                classText.text = characterData.class_tag.ToUpper();
            }
            if (hpText != null)
            {
                hpText.text = $"HP: {characterData.base_hp}";
            }
            if (atkText != null)
            {
                atkText.text = $"ATK: {characterData.base_atk}";
            }
            if (abilityBlock != null)
            {
                abilityBlock.SetActive(characterData.has_ability);
            }
            if (characterData.has_ability)
            {
                // TODO: Загрузить данные способности из AbilitySystem
                if (abilityNameText != null)
                {
                    abilityNameText.text = "Способность"; // TODO: Реальное название
                }
                if (abilityEffectText != null)
                {
                    abilityEffectText.text = "Эффект"; // TODO: Реальное описание
                }
            }
            if (deadOverlay != null)
            {
                deadOverlay.gameObject.SetActive(characterData.is_dead);
            }
            if (characterData.is_dead)
            {
                // TODO: Применить серый фильтр или изменить цвет карточки
            }
        }
        public void OnCardClicked()
        {
            // TODO: Показать детальную информацию о персонаже
            Debug.Log($"[CharacterCardUI] Клик по карточке: {characterData.ch_name}");
        }
    }
}
