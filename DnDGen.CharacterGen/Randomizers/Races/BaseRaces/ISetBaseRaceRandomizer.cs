namespace DnDGen.CharacterGen.Randomizers.Races.BaseRaces
{
    public interface ISetBaseRaceRandomizer : RaceRandomizer
    {
        string SetBaseRace { get; set; }
    }
}