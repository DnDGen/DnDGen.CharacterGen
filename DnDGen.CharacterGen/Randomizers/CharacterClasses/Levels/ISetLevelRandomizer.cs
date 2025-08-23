namespace DnDGen.CharacterGen.Randomizers.CharacterClasses.Levels
{
    public interface ISetLevelRandomizer : ILevelRandomizer
    {
        int SetLevel { get; set; }
    }
}