using DnDGen.CharacterGen.Races.Randomizers;

namespace DnDGen.CharacterGen.Races.Randomizers.Metaraces
{
    public interface IForcableMetaraceRandomizer : RaceRandomizer
    {
        bool ForceMetarace { get; set; }
    }
}