using UnityEngine;
using PickMe.Data;
namespace PickMe.Combat.Abilities
{
    public abstract class Ability
    {
        protected AbilityData abilityData;
        protected CharacterData owner;
        protected float cooldownTimer = 0f;
        protected float durationTimer = 0f;
        protected bool isActive = false;
        public AbilityData AbilityData => abilityData;
        public bool IsOnCooldown => cooldownTimer > 0f;
        public bool IsActive => isActive;
        public virtual void Initialize(AbilityData data, CharacterData character)
        {
            abilityData = data;
            owner = character;
        }
        public abstract bool CheckActivateCondition();
        public virtual void Activate()
        {
            if (IsOnCooldown || isActive) return;
            if (CheckActivateCondition())
            {
                isActive = true;
                durationTimer = abilityData.ability_duration;
                OnActivate();
            }
        }
        public virtual void Update(float deltaTime)
        {
            if (cooldownTimer > 0f)
            {
                cooldownTimer -= deltaTime;
            }
            if (isActive)
            {
                durationTimer -= deltaTime;
                if (durationTimer <= 0f)
                {
                    Deactivate();
                }
            }
        }
        public virtual void Deactivate()
        {
            isActive = false;
            cooldownTimer = abilityData.ability_cooldown;
            OnDeactivate();
        }
        protected virtual void OnActivate() { }
        protected virtual void OnDeactivate() { }
    }
}
