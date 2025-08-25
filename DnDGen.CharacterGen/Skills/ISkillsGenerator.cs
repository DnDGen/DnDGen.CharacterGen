using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items;
using DnDGen.CharacterGen.Races;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Skills
{
    internal interface ISkillsGenerator
    {
        IEnumerable<Skill> GenerateWith(CharacterClass characterClass, Race race, Dictionary<string, Ability> abilities);
        IEnumerable<Skill> UpdateSkillsFromFeats(IEnumerable<Skill> skills, IEnumerable<Feat> feats);
        IEnumerable<Skill> UpdateSkillsFromEquipment(IEnumerable<Skill> skills, Equipment equipment);
    }
}