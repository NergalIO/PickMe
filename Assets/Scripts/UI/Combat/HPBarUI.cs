using UnityEngine;
using UnityEngine.UI;
namespace PickMe.UI.Combat
{
    public class HPBarUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Slider hpSlider;
        [SerializeField] private Text hpText;
        private int maxHP;
        private int currentHP;
        public void Initialize(int maxHealth)
        {
            maxHP = maxHealth;
            currentHP = maxHealth;
            UpdateHPBar();
        }
        public void UpdateHP(int newHP)
        {
            currentHP = Mathf.Clamp(newHP, 0, maxHP);
            UpdateHPBar();
        }
        private void UpdateHPBar()
        {
            if (hpSlider != null)
            {
                hpSlider.value = maxHP > 0 ? (float)currentHP / maxHP : 0f;
            }
            if (hpText != null)
            {
                hpText.text = $"{currentHP}/{maxHP}";
            }
        }
    }
}
