namespace DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels
{
    public interface ISetLevelRandomizer : ILevelRandomizer
    {
        int SetLevel { get; set; }
    }
}