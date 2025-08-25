namespace DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames
{
    public interface ISetClassNameRandomizer : IClassNameRandomizer
    {
        string SetClassName { get; set; }
    }
}