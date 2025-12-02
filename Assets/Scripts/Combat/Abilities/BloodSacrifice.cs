using UnityEngine;
using PickMe.Data;
namespace PickMe.Combat.Abilities
{
    public class BloodSacrifice : Ability
    {
        private float originalAtk;
        private float atkMultiplier = 1.5f; // TODO: Загрузить из ability_effect
        public override bool CheckActivateCondition()
        {
            if (owner == null) return false;
            float hpPercentage = (float)owner.current_hp / owner.base_hp;
            return hpPercentage <= 0.3f;
        }
        protected override void OnActivate()
        {
            if (owner == null) return;
            originalAtk = owner.current_atk;
            owner.current_atk = Mathf.RoundToInt(owner.current_atk * atkMultiplier);
            UnityEngine.Debug.Log($"[BloodSacrifice] Способность активирована! Атака увеличена до {owner.current_atk}");
        }
        protected override void OnDeactivate()
        {
            if (owner == null) return;
            owner.current_atk = Mathf.RoundToInt(originalAtk);
            UnityEngine.Debug.Log($"[BloodSacrifice] Способность деактивирована. Атака восстановлена до {owner.current_atk}");
        }
    }
}
