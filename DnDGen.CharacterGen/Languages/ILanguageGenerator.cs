using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Skills;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Generators.Languages
{
    internal interface ILanguageGenerator
    {
        IEnumerable<string> GenerateWith(Race race, CharacterClass characterClass, Dictionary<string, Ability> abilities, IEnumerable<Skill> skills);
    }
}