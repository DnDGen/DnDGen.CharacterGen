using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Items
{
    internal interface IEquipmentGenerator
    {
        Equipment GenerateWith(IEnumerable<Feat> feats, CharacterClass characterClass, Race race);
    }
}