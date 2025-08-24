namespace DnDGen.CharacterGen.Abilities.Randomizers
{
    public interface ISetAbilitiesRandomizer : IAbilitiesRandomizer
    {
        int SetStrength { get; set; }
        int SetDexterity { get; set; }
        int SetConstitution { get; set; }
        int SetIntelligence { get; set; }
        int SetWisdom { get; set; }
        int SetCharisma { get; set; }
        bool AllowAdjustments { get; set; }
    }
}