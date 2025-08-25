using DnDGen.CharacterGen.Races.Randomizers;

namespace DnDGen.CharacterGen.Races.Randomizers.BaseRaces
{
    public interface ISetBaseRaceRandomizer : RaceRandomizer
    {
        string SetBaseRace { get; set; }
    }
}