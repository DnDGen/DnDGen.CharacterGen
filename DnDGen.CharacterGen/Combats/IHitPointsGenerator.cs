using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Combats
{
    internal interface IHitPointsGenerator
    {
        int GenerateWith(CharacterClass characterClass, int constitutionBonus, Race race, IEnumerable<Feat> feats);
    }
}