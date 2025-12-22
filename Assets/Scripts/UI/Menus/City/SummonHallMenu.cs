using System.Collections;
using System.Collections.Generic;
using PickMe.Gameplay.Data;
using PickMe.Gameplay.Systems.ResourceSystem;
using PickMe.Gameplay.Systems.CharacterSystem;
using PickMe.Core.Services;
using PickMe.Core.Managers;
using PickMe.UI.Controllers;
using PickMe.UI.Menus.Base;
using PickMe.Utils;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PickMe.UI.Menus.City
{
    /// <summary>
    /// Summon Hall menu - summon characters with tickets or gems.
    /// </summary>
    public class SummonHallMenu : Menu
    {
        [Header("Buttons")]
        [SerializeField] private Button ticketSummonButton;
        [SerializeField] private Button gemSummonButton;
        
        [Header("Button Labels")]
        [SerializeField] private TMP_Text ticketSummonButtonText;
        [SerializeField] private TMP_Text gemSummonButtonText;
        
        [Header("Resource Display")]
        [SerializeField] private TMP_Text ticketsCountText;
        [SerializeField] private TMP_Text rubiesCountText;
        
        private const int TicketCost = 1;
        private const int GemCost = 100; // Placeholder

        public override void Awake()
        {
            base.Awake();
            
            // Auto-find button text if not assigned
            if (ticketSummonButtonText == null && ticketSummonButton != null)
            {
                ticketSummonButtonText = ticketSummonButton.GetComponentInChildren<TMP_Text>();
            }
            
            if (gemSummonButtonText == null && gemSummonButton != null)
            {
                gemSummonButtonText = gemSummonButton.GetComponentInChildren<TMP_Text>();
            }
            
            // Connect button click handlers - remove old listeners first to avoid duplicates
            if (ticketSummonButton != null)
            {
                ticketSummonButton.onClick.RemoveAllListeners();
                ticketSummonButton.onClick.AddListener(OnTicketSummonClicked);
                Debug.Log($"SummonHallMenu: Connected ticket button handler. Button active: {ticketSummonButton.gameObject.activeSelf}, interactable: {ticketSummonButton.interactable}");
            }
            else
            {
                Debug.LogWarning("SummonHallMenu: ticketSummonButton is null!");
            }
            
            if (gemSummonButton != null)
            {
                gemSummonButton.onClick.RemoveAllListeners();
                gemSummonButton.onClick.AddListener(OnGemSummonClicked);
            }
        }

        public override void OnFocus()
        {
            base.OnFocus();
            SubscribeEvents();
            
            // Update resources immediately if managers are ready
            if (ResourceManager.IsInitialized)
            {
                UpdateResourceDisplay();
            }
            
            // Wait for managers to initialize if needed, then update buttons
            StartCoroutine(WaitAndUpdateButtons());
        }

        private void UpdateResourceDisplay()
        {
            if (!ResourceManager.IsInitialized) return;
            
            int currentTickets = ResourceManager.Instance.Get(ResourceType.Tickets);
            int currentRubies = ResourceManager.Instance.Get(ResourceType.Rubies);
            
            if (ticketsCountText != null)
            {
                ticketsCountText.text = currentTickets.ToString();
            }
            
            if (rubiesCountText != null)
            {
                rubiesCountText.text = currentRubies.ToString();
            }
        }

        private IEnumerator WaitAndUpdateButtons()
        {
            // Wait for managers to be ready
            if (!SummonManager.IsInitialized)
            {
                yield return SummonManager.WaitUntilInitialized();
            }
            
            if (!ResourceManager.IsInitialized)
            {
                yield return ResourceManager.WaitUntilInitialized();
            }
            
            if (!CharacterManager.IsInitialized)
            {
                yield return CharacterManager.WaitUntilInitialized();
            }
            
            UpdateButtons();
        }

        public override void OnBlur()
        {
            base.OnBlur();
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            EventSubscriptionHelper.SubscribeIfReady<ResourceChanged>(OnResourceChanged);
            EventSubscriptionHelper.SubscribeIfReady<SummonCompleted>(OnSummonCompleted);
        }

        private void UnsubscribeEvents()
        {
            EventSubscriptionHelper.UnsubscribeIfReady<ResourceChanged>(OnResourceChanged);
            EventSubscriptionHelper.UnsubscribeIfReady<SummonCompleted>(OnSummonCompleted);
        }

        private void OnResourceChanged(ResourceChanged evt)
        {
            // Update resource display when resources change
            if (evt.Type == ResourceType.Tickets || evt.Type == ResourceType.Rubies)
            {
                UpdateResourceDisplay();
            }
            UpdateButtons();
        }

        private void OnSummonCompleted(SummonCompleted evt)
        {
            UpdateButtons();
            
            Debug.Log($"SummonHallMenu: OnSummonCompleted called with {evt.Characters?.Count ?? 0} characters");
            
            // Show summon result menu with summoned characters
            if (UIController.IsInitialized && evt.Characters != null && evt.Characters.Count > 0)
            {
                // Use UIController to start coroutine since it's always active
                // This ensures the coroutine runs even if SummonHallMenu becomes inactive
                UIController.Instance.StartCoroutine(SetSummonResultAfterOpen(evt.Characters));
            }
            else
            {
                Debug.LogWarning($"SummonHallMenu: Cannot show result menu. UIController initialized: {UIController.IsInitialized}, Characters: {evt.Characters?.Count ?? 0}");
            }
        }

        private IEnumerator SetSummonResultAfterOpen(IReadOnlyList<CharacterData> characters)
        {
            Debug.Log($"SummonHallMenu: SetSummonResultAfterOpen called with {characters?.Count ?? 0} characters");
            yield return MenuUtils.OpenMenuAndSetData<SummonResultMenu>("SummonResultMenu", menu =>
            {
                var characterList = new List<CharacterData>(characters);
                Debug.Log($"SummonHallMenu: Setting {characterList.Count} characters to SummonResultMenu");
                menu.SetSummonedCharacters(characterList);
            });
        }

        private void UpdateButtons()
        {
            // Update resource display texts
            if (ResourceManager.IsInitialized)
            {
                int currentTickets = ResourceManager.Instance.Get(ResourceType.Tickets);
                int currentRubies = ResourceManager.Instance.Get(ResourceType.Rubies);
                
                if (ticketsCountText != null)
                {
                    ticketsCountText.text = currentTickets.ToString();
                }
                
                if (rubiesCountText != null)
                {
                    rubiesCountText.text = currentRubies.ToString();
                }
            }
            
            // Update ticket summon button
            if (ticketSummonButton != null)
            {
                bool canSummon = false;
                int currentTickets = 0;
                
                if (SummonManager.IsInitialized && ResourceManager.IsInitialized && CharacterManager.IsInitialized)
                {
                    currentTickets = ResourceManager.Instance.Get(ResourceType.Tickets);
                    canSummon = SummonManager.Instance.CanSummon();
                    Debug.Log($"SummonHallMenu: UpdateButtons - Tickets: {currentTickets}, CanSummon: {canSummon}");
                }
                else
                {
                    Debug.Log($"SummonHallMenu: UpdateButtons - Managers not ready. SummonManager: {SummonManager.IsInitialized}, ResourceManager: {ResourceManager.IsInitialized}, CharacterManager: {CharacterManager.IsInitialized}");
                }
                
                // Кнопка всегда активна - проверка будет при клике
                // Это позволяет пользователю видеть, что кнопка работает, даже если менеджеры еще не готовы
                ticketSummonButton.interactable = true;
                Debug.Log($"SummonHallMenu: Button interactable set to: true (canSummon: {canSummon})");
                
                // Update button text
                if (ticketSummonButtonText != null)
                {
                    ticketSummonButtonText.text = $"ПРИЗЫВ ЗА {TicketCost}";
                }
            }
            else
            {
                Debug.LogWarning("SummonHallMenu: ticketSummonButton is null in UpdateButtons!");
            }

            // Update gem summon button
            if (gemSummonButton != null)
            {
                // Кнопка за рубины - неактивная (только визуал)
                gemSummonButton.interactable = false;
                
                // Update button text
                if (gemSummonButtonText != null)
                {
                    gemSummonButtonText.text = $"ПРИЗЫВ ЗА {GemCost}";
                }
            }
        }

        public void OnTicketSummonClicked()
        {
            Debug.Log("SummonHallMenu: OnTicketSummonClicked called!");
            
            // Always show something to confirm the click is working
            if (ToastManager.IsInitialized)
            {
                ToastManager.Instance.ShowToast("Обработка призыва...", 1f);
            }
            
            if (!SummonManager.IsInitialized)
            {
                Debug.LogWarning("SummonHallMenu: SummonManager is not initialized");
                if (ToastManager.IsInitialized)
                {
                    ToastManager.Instance.ShowToast("Система призыва не готова", 2f);
                }
                return;
            }
            
            if (!ResourceManager.IsInitialized)
            {
                Debug.LogWarning("SummonHallMenu: ResourceManager is not initialized");
                if (ToastManager.IsInitialized)
                {
                    ToastManager.Instance.ShowToast("Система ресурсов не готова", 2f);
                }
                return;
            }
            
            if (!CharacterManager.IsInitialized)
            {
                Debug.LogWarning("SummonHallMenu: CharacterManager is not initialized");
                if (ToastManager.IsInitialized)
                {
                    ToastManager.Instance.ShowToast("Система персонажей не готова", 2f);
                }
                return;
            }
            
            int tickets = ResourceManager.Instance.Get(ResourceType.Tickets);
            bool canSummon = SummonManager.Instance.CanSummon();
            
            Debug.Log($"SummonHallMenu: Attempting summon. Tickets: {tickets}, CanSummon: {canSummon}");
            
            if (!canSummon)
            {
                string errorMsg = tickets < 1 ? "Недостаточно билетов" : "Хранилище переполнено";
                Debug.LogWarning($"SummonHallMenu: Cannot summon - {errorMsg}");
                if (ToastManager.IsInitialized)
                {
                    ToastManager.Instance.ShowToast(errorMsg, 2f);
                }
                return;
            }
            
            bool success = SummonManager.Instance.SummonWithTickets();
            if (success)
            {
                UpdateButtons();
                
                // Summon result menu will be shown automatically via SummonCompleted event
                // No need to show toast here, the result menu will display the characters
            }
            else
            {
                // Show error message - use already declared tickets variable
                string errorMessage = "Недостаточно билетов или места в хранилище";
                
                if (tickets < 1)
                {
                    errorMessage = "Недостаточно билетов";
                }
                else if (CharacterManager.IsInitialized && !CharacterManager.Instance.HasFreeSlots(3))
                {
                    errorMessage = "Хранилище переполнено";
                }
                
                Debug.LogWarning($"SummonHallMenu: Summon failed - {errorMessage}");
                if (ToastManager.IsInitialized)
                {
                    ToastManager.Instance.ShowToast(errorMessage, 2f);
                }
            }
        }

        public void OnGemSummonClicked()
        {
            // Кнопка за рубины - неактивная (только визуал)
            // Показываем сообщение "SOON" при попытке нажать
            if (ToastManager.IsInitialized)
            {
                ToastManager.Instance.ShowSoon();
            }
            else
            {
                Debug.Log("SummonHallMenu: Gem summon not available yet");
            }
        }

        public override void OnCancel()
        {
            base.OnCancel();
            MenuUtils.CloseCurrentMenu();
        }
    }
}

