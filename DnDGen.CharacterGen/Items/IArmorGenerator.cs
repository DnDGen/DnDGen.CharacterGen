using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;
using System.Collections.Generic;
using DnDGen.TreasureGen.Items;

namespace DnDGen.CharacterGen.Items
{
    internal interface IArmorGenerator
    {
        Armor GenerateArmorFrom(IEnumerable<Feat> feats, CharacterClass characterClass, Race race);
        Armor GenerateShieldFrom(IEnumerable<Feat> feats, CharacterClass characterClass, Race race);
    }
}
