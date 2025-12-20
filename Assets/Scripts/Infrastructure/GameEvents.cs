using System.Collections.Generic;
using PickMe.Gameplay;

namespace PickMe.Infrastructure
{
    public readonly struct ResourceChanged
    {
        public ResourceType Type { get; }
        public int NewValue { get; }
        public ResourceChanged(ResourceType type, int newValue)
        {
            Type = type;
            NewValue = newValue;
        }
    }

    public readonly struct GameStateChanged
    {
        public GameState State { get; }
        public GameStateChanged(GameState state) => State = state;
    }

    public readonly struct SummonCompleted
    {
        public IReadOnlyList<CharacterData> Characters { get; }
        public SummonCompleted(IReadOnlyList<CharacterData> characters) => Characters = characters;
    }

    public readonly struct CombatCompleted
    {
        public CombatResultType Result { get; }
        public IReadOnlyList<ResourceReward> Rewards { get; }
        public CombatCompleted(CombatResultType result, IReadOnlyList<ResourceReward> rewards)
        {
            Result = result;
            Rewards = rewards;
        }
    }

    public readonly struct BuildingBuilt
    {
        public BuildingType Type { get; }
        public BuildingBuilt(BuildingType type) => Type = type;
    }

    // UI input events
    public readonly struct UiNavigate
    {
        public UnityEngine.Vector2 Direction { get; }
        public UiNavigate(UnityEngine.Vector2 direction) => Direction = direction;
    }

    public readonly struct UiSubmit
    {
    }

    public readonly struct UiCancel
    {
    }

    public readonly struct UiClick
    {
        public UnityEngine.Vector2 Position { get; }
        public UiClick(UnityEngine.Vector2 position) => Position = position;
    }

    public readonly struct UiPoint
    {
        public UnityEngine.Vector2 Position { get; }
        public UiPoint(UnityEngine.Vector2 position) => Position = position;
    }

    public readonly struct UiScroll
    {
        public UnityEngine.Vector2 Delta { get; }
        public UiScroll(UnityEngine.Vector2 delta) => Delta = delta;
    }
}

