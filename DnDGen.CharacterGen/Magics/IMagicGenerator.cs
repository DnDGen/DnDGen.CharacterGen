using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Items;
using DnDGen.CharacterGen.Races;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Magics
{
    internal interface IMagicGenerator
    {
        Magic GenerateWith(Alignment alignment, CharacterClass characterClass, Race race, Dictionary<string, Ability> abilities, IEnumerable<Feat> feats, Equipment equipment);
    }
}
