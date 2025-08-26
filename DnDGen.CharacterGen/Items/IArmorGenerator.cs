using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;
using DnDGen.TreasureGen.Items;

namespace DnDGen.CharacterGen.Items
{
    internal interface IArmorGenerator
    {
        Armor GenerateArmorFrom(FeatCollections feats, CharacterClass characterClass, Race race);
        Armor GenerateShieldFrom(FeatCollections feats, CharacterClass characterClass, Race race);
    }
}
