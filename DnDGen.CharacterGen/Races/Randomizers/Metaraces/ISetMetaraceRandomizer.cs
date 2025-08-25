using DnDGen.CharacterGen.Races.Randomizers;

namespace DnDGen.CharacterGen.Races.Randomizers.Metaraces
{
    public interface ISetMetaraceRandomizer : RaceRandomizer
    {
        string SetMetarace { get; set; }
    }
}