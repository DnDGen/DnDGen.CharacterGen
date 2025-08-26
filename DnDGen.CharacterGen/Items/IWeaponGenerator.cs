using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;
using DnDGen.TreasureGen.Items;

namespace DnDGen.CharacterGen.Items
{
    internal interface IWeaponGenerator
    {
        Weapon GenerateFrom(FeatCollections feats, CharacterClass characterClass, Race race);
        Weapon GenerateAmmunition(CharacterClass characterClass, Race race, string ammunitionType);
        Weapon GenerateMeleeFrom(FeatCollections feats, CharacterClass characterClass, Race race);
        Weapon GenerateOneHandedMeleeFrom(FeatCollections feats, CharacterClass characterClass, Race race);
        Weapon GenerateRangedFrom(FeatCollections feats, CharacterClass characterClass, Race race);
    }
}