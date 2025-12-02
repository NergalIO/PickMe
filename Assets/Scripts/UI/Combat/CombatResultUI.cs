using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PickMe.Managers;
using PickMe.Data;
namespace PickMe.UI.Combat
{
    public class CombatResultUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private Button returnToCityButton;
        [SerializeField] private Transform rewardsContainer;
        [SerializeField] private GameObject rewardItemPrefab;
        private void Start()
        {
            if (returnToCityButton != null)
            {
                returnToCityButton.onClick.AddListener(OnReturnToCityClicked);
            }
        }
        public void ShowVictory(System.Collections.Generic.List<RewardData> rewards)
        {
            if (resultPanel != null)
            {
                resultPanel.SetActive(true);
            }
            if (resultText != null)
            {
                resultText.text = "ПОБЕДА!";
                resultText.color = Color.green;
            }
            ShowRewards(rewards);
        }
        public void ShowDefeat()
        {
            if (resultPanel != null)
            {
                resultPanel.SetActive(true);
            }
            if (resultText != null)
            {
                resultText.text = "ПОРАЖЕНИЕ";
                resultText.color = Color.red;
            }
            if (rewardsContainer != null)
            {
                rewardsContainer.gameObject.SetActive(false);
            }
        }
        private void ShowRewards(System.Collections.Generic.List<RewardData> rewards)
        {
            if (rewardsContainer == null || rewardItemPrefab == null) return;
            foreach (var reward in rewards)
            {
                GameObject rewardItem = Instantiate(rewardItemPrefab, rewardsContainer);
                TextMeshProUGUI rewardText = rewardItem.GetComponentInChildren<TextMeshProUGUI>();
                if (rewardText != null)
                {
                    rewardText.text = $"{reward.rewardType}: {reward.count}";
                }
            }
        }
        private void OnReturnToCityClicked()
        {
            if (CityManager.HasInstance)
            {
                CityManager.Instance.ReturnToCity();
            }
            if (resultPanel != null)
            {
                resultPanel.SetActive(false);
            }
        }
    }
}
