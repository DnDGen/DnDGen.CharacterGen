using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;
using System.Collections.Generic;
using DnDGen.TreasureGen.Items;

namespace DnDGen.CharacterGen.Items
{
    internal interface IWeaponGenerator
    {
        Weapon GenerateFrom(IEnumerable<Feat> feats, CharacterClass characterClass, Race race);
        Weapon GenerateAmmunition(CharacterClass characterClass, Race race, string ammunitionType);
        Weapon GenerateMeleeFrom(IEnumerable<Feat> feats, CharacterClass characterClass, Race race);
        Weapon GenerateOneHandedMeleeFrom(IEnumerable<Feat> feats, CharacterClass characterClass, Race race);
        Weapon GenerateRangedFrom(IEnumerable<Feat> feats, CharacterClass characterClass, Race race);
    }
}