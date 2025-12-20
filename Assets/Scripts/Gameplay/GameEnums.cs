namespace PickMe.Gameplay
{
    public enum CharacterClassTag
    {
        Warrior,
        Scout,
        Tank,
        Mage
    }

    public enum BuildingType
    {
        House,
        SummonHall,
        Tower,
        ExpeditionPortal,
        MergeLab
    }

    public enum BuildingStatus
    {
        Built,
        Available,
        Locked
    }

    public enum ResourceType
    {
        Tickets,
        Construction,
        Rubies
    }

    public enum GameState
    {
        City,
        Combat,
        Summon,
        Collection
    }

    public enum CombatResultType
    {
        Victory,
        Defeat
    }
}

