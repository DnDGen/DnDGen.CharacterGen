using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items;
using DnDGen.CharacterGen.Races;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Combats
{
    internal interface ICombatGenerator
    {
        Combat GenerateWith(BaseAttack baseAttack, CharacterClass characterClass, Race race, IEnumerable<Feat> feats, Dictionary<string, Ability> abilities, Equipment equipment);
        BaseAttack GenerateBaseAttackWith(CharacterClass characterClass, Race race, Dictionary<string, Ability> abilities);
    }
}