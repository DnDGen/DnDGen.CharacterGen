namespace DnDGen.CharacterGen.Randomizers.Races
{
    public interface ISetBaseRaceRandomizer : RaceRandomizer
    {
        string SetBaseRace { get; set; }
    }
}