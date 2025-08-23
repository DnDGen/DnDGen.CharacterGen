namespace DnDGen.CharacterGen.Randomizers.Races.Metaraces
{
    public interface IForcableMetaraceRandomizer : RaceRandomizer
    {
        bool ForceMetarace { get; set; }
    }
}