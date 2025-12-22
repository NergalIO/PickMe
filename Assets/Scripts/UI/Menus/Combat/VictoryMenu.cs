using System.Collections.Generic;
using PickMe.Gameplay.Data;
using PickMe.Core.Infrastructure;
using PickMe.Core.Managers;
using PickMe.UI.Controllers;
using PickMe.UI.Menus.Base;
using PickMe.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PickMe.UI.Menus.Combat
{
    /// <summary>
    /// Victory menu - displayed when player wins combat.
    /// Shows rewards and fallen units.
    /// </summary>
    public class VictoryMenu : Menu
    {
        [Header("Rewards")]
        [SerializeField] private Transform rewardsContainer;
        [SerializeField] private GameObject rewardItemPrefab;
        [SerializeField] private TMP_Text rewardsTitleText;
        
        [Header("Fallen Units")]
        [SerializeField] private Transform fallenUnitsContainer;
        [SerializeField] private GameObject characterCardPrefab;
        [SerializeField] private GameObject noFallenUnitsText;
        
        [Header("Buttons")]
        [SerializeField] private Button continueButton;
        
        private List<CharacterData> _fallenUnits = new();
        private List<ResourceReward> _rewards = new();
        
        public override void Awake()
        {
            base.Awake();
            
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(OnContinueClicked);
            }
        }

        /// <summary>
        /// Sets the rewards and fallen units to display.
        /// </summary>
        public void SetData(List<ResourceReward> rewards, List<CharacterData> fallenUnits)
        {
            _rewards = rewards ?? new List<ResourceReward>();
            _fallenUnits = fallenUnits ?? new List<CharacterData>();
            RefreshDisplay();
        }

        public override void OnFocus()
        {
            base.OnFocus();
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            RefreshRewards();
            RefreshFallenUnits();
        }

        private void RefreshRewards()
        {
            // Clear existing rewards
            if (rewardsContainer != null)
            {
                MenuUtils.ClearContainer(rewardsContainer);
            }
            
            if (_rewards == null || _rewards.Count == 0)
            {
                if (rewardsTitleText != null)
                {
                    rewardsTitleText.gameObject.SetActive(false);
                }
                return;
            }
            
            if (rewardsTitleText != null)
            {
                rewardsTitleText.gameObject.SetActive(true);
            }
            
            // Create reward items
            if (rewardItemPrefab != null && rewardsContainer != null)
            {
                foreach (var reward in _rewards)
                {
                    var rewardObj = Instantiate(rewardItemPrefab, rewardsContainer);
                    var text = rewardObj.GetComponentInChildren<TMP_Text>();
                    if (text != null)
                    {
                        string resourceName = GetResourceName(reward.resourceType);
                        text.text = $"{resourceName}: +{reward.amount}";
                    }
                }
            }
        }

        private void RefreshFallenUnits()
        {
            // Clear existing cards
            if (fallenUnitsContainer != null)
            {
                MenuUtils.ClearContainer(fallenUnitsContainer);
            }
            
            if (_fallenUnits == null || _fallenUnits.Count == 0)
            {
                if (noFallenUnitsText != null)
                {
                    noFallenUnitsText.SetActive(true);
                }
                return;
            }
            
            if (noFallenUnitsText != null)
            {
                noFallenUnitsText.SetActive(false);
            }
            
            // Create character cards for fallen units
            if (characterCardPrefab != null && fallenUnitsContainer != null)
            {
                foreach (var character in _fallenUnits)
                {
                    var cardObj = Instantiate(characterCardPrefab, fallenUnitsContainer);
                    var card = cardObj.GetComponent<CharacterCardUI>();
                    if (card != null)
                    {
                        card.Setup(character);
                    }
                }
            }
        }

        private string GetResourceName(ResourceType type)
        {
            return type switch
            {
                ResourceType.Tickets => "Билеты",
                ResourceType.Construction => "Стройматериалы",
                ResourceType.Rubies => "Рубины",
                _ => type.ToString()
            };
        }

        private void OnContinueClicked()
        {
            // Close menu and return to city
            MenuUtils.CloseCurrentMenu();
            
            // Return to main scene after menu closes
            if (SceneLoader.IsInitialized)
            {
                SceneLoader.Instance.ReturnToMainScene();
            }
        }

        public override void OnCancel()
        {
            base.OnCancel();
            // Base OnCancel already closes the menu through UIController
        }
    }
}

