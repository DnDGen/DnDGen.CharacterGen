namespace DnDGen.CharacterGen.Skills.Selectors
{
    internal interface ISkillSelector
    {
        SkillSelection SelectFor(string skill);
    }
}