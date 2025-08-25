using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Combats;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Skills;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Feats
{
    internal interface IFeatsGenerator
    {
        FeatCollections GenerateWith(CharacterClass characterClass, Race race, Dictionary<string, Ability> abilities, IEnumerable<Skill> skills, BaseAttack baseAttack);
    }
}