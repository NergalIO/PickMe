using System.Collections.Generic;
using UnityEngine;
using PickMe.Data;
using PickMe.Managers;
namespace PickMe.Characters.Summon
{
    public class SummonManager : PersistentSingleton<SummonManager>
    {
        [Header("Summon Settings")]
        [SerializeField] private int charactersPerSummon = 3;
        [SerializeField] private int ticketsPerSummon = 1;
        private SummonLogic summonLogic;
        public int CharactersPerSummon => charactersPerSummon;
        public int TicketsPerSummon => ticketsPerSummon;
        protected override void OnAwake()
        {
            base.OnAwake();
            summonLogic = new SummonLogic();
        }
        public List<CharacterData> SummonWithTickets()
        {
            // TODO: Проверка наличия билетов через PlayerData
            Debug.Log($"[SummonManager] Призыв {charactersPerSummon} персонажей за {ticketsPerSummon} билет(ов)");
            List<CharacterData> summonedCharacters = summonLogic.SummonCharacters(charactersPerSummon);
            if (CharacterManager.HasInstance)
            {
                CharacterManager.Instance.AddCharacters(summonedCharacters);
            }
            // TODO: Списать билеты
            return summonedCharacters;
        }
        public List<CharacterData> SummonWithRubies()
        {
            Debug.LogWarning("[SummonManager] Призыв за рубины пока недоступен");
            return null;
        }
    }
}
