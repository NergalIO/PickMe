using UnityEngine;
using PickMe.Managers;
namespace PickMe.Characters.Summon
{
    public class SummonController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject summonWithTicketsButton;
        [SerializeField] private GameObject summonWithRubiesButton;
        private SummonManager summonManager;
        private void Start()
        {
            summonManager = SummonManager.Instance;
            InitializeUI();
        }
        private void InitializeUI()
        {
            if (summonWithTicketsButton != null)
            {
                // TODO: Настроить UI кнопки
            }
            if (summonWithRubiesButton != null)
            {
                // TODO: Отключить интерактивность, показать как визуал
            }
        }
        public void OnSummonWithTicketsClicked()
        {
            if (summonManager == null) return;
            var summonedCharacters = summonManager.SummonWithTickets();
            if (summonedCharacters != null && summonedCharacters.Count > 0)
            {
                Debug.Log($"[SummonController] Призвано {summonedCharacters.Count} персонажей");
                // TODO: Показать UI с результатами призыва
                ShowSummonResults(summonedCharacters);
            }
        }
        public void OnSummonWithRubiesClicked()
        {
            Debug.Log("[SummonController] Призыв за рубины пока недоступен");
            // TODO: Показать уведомление о недоступности
        }
        private void ShowSummonResults(System.Collections.Generic.List<PickMe.Data.CharacterData> characters)
        {
            // TODO: Открыть UI с карточками призванных персонажей
            Debug.Log("[SummonController] Показ результатов призыва");
        }
    }
}
