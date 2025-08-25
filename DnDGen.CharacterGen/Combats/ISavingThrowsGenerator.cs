using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Combats
{
    internal interface ISavingThrowsGenerator
    {
        SavingThrows GenerateWith(CharacterClass characterClass, IEnumerable<Feat> feats, Dictionary<string, Ability> abilities);
    }
}