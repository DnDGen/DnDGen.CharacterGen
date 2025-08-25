using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats.Selectors;
using DnDGen.CharacterGen.Skills;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Feats
{
    internal interface IFeatFocusGenerator
    {
        string GenerateFrom(string feat, string focusType, IEnumerable<Skill> skills, IEnumerable<RequiredFeatSelection> requiredFeats, IEnumerable<Feat> otherFeat, CharacterClass characterClass);
        string GenerateFrom(string feat, string focusType, IEnumerable<Skill> skills);
        string GenerateAllowingFocusOfAllFrom(string feat, string focusType, IEnumerable<Skill> skills, IEnumerable<RequiredFeatSelection> requiredFeats, IEnumerable<Feat> otherFeat, CharacterClass characterClass);
        string GenerateAllowingFocusOfAllFrom(string feat, string focusType, IEnumerable<Skill> skills);
    }
}