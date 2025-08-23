namespace DnDGen.CharacterGen.Randomizers.CharacterClasses.ClassNames
{
    public interface ISetClassNameRandomizer : IClassNameRandomizer
    {
        string SetClassName { get; set; }
    }
}