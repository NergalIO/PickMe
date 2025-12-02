using UnityEngine;
using PickMe.Data;
using PickMe.Combat.Abilities;
namespace PickMe.Combat.AI
{
    public class CharacterCombatController : MonoBehaviour
    {
        [Header("Character Data")]
        [SerializeField] private CharacterData characterData;
        [Header("Combat")]
        [SerializeField] private int currentHP;
        [SerializeField] private float attackCooldown = 0f;
        private AbilitySystem abilitySystem;
        private Ability characterAbility;
        public CharacterData CharacterData => characterData;
        public int CurrentHP => currentHP;
        public bool IsAlive => currentHP > 0;
        private void Start()
        {
            InitializeCharacter();
        }
        private void Update()
        {
            if (!IsAlive) return;
            attackCooldown -= Time.deltaTime;
            if (characterData.has_ability && abilitySystem != null)
            {
                abilitySystem.CheckAndActivateAbilities(characterData);
            }
        }
        public void InitializeCharacter(CharacterData data = null)
        {
            if (data != null)
            {
                characterData = data;
            }
            if (characterData != null)
            {
                characterData.InitializeForCombat();
                currentHP = characterData.current_hp;
            }
            if (characterData != null && characterData.has_ability)
            {
                abilitySystem = FindObjectOfType<AbilitySystem>();
                if (abilitySystem != null)
                {
                    // TODO: Загрузить AbilityData из конфига
                }
            }
        }
        public void TakeDamage(int damage)
        {
            currentHP = Mathf.Max(0, currentHP - damage);
            if (currentHP <= 0)
            {
                Die();
            }
        }
        public void AttackTarget(Transform targetTransform)
        {
            if (attackCooldown > 0f || !IsAlive) return;
            if (targetTransform == null) return;
            // TODO: Нанести урон цели
            Debug.Log($"[CharacterCombatController] {characterData.ch_name} атакует цель");
            attackCooldown = characterData.atk_speed;
        }
        private void Die()
        {
            Debug.Log($"[CharacterCombatController] {characterData.ch_name} повержен");
            characterData.is_dead = true;
            // TODO: Анимация смерти, эффекты
            // TODO: Уведомить CombatManager о смерти персонажа
        }
    }
}
