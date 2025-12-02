using System.Collections.Generic;
using UnityEngine;
using PickMe.Data;
using PickMe.Managers;
namespace PickMe.Combat.Tower
{
    public class TowerRewards : MonoBehaviour
    {
        public void GiveRewards(List<RewardData> rewards)
        {
            if (rewards == null || rewards.Count == 0)
            {
                Debug.LogWarning("[TowerRewards] Нет наград для выдачи");
                return;
            }
            foreach (var reward in rewards)
            {
                GiveReward(reward);
            }
            if (SaveManager.HasInstance)
            {
                SaveManager.Instance.SaveGame();
            }
        }
        private void GiveReward(RewardData reward)
        {
            switch (reward.rewardType.ToLower())
            {
                case "tickets":
                    // TODO: Добавить билеты игроку
                    Debug.Log($"[TowerRewards] Выдано {reward.count} билетов");
                    break;
                case "rubies":
                    // TODO: Добавить рубины игроку
                    Debug.Log($"[TowerRewards] Выдано {reward.count} рубинов");
                    break;
                default:
                    Debug.LogWarning($"[TowerRewards] Неизвестный тип награды: {reward.rewardType}");
                    break;
            }
        }
    }
}
