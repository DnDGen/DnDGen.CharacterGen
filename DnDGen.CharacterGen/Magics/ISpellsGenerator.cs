using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Magics
{
    internal interface ISpellsGenerator
    {
        IEnumerable<SpellQuantity> GeneratePerDay(CharacterClass characterClass, Dictionary<string, Ability> abilities);
        IEnumerable<Spell> GenerateKnown(CharacterClass characterClass, Dictionary<string, Ability> abilities);
        IEnumerable<Spell> GeneratePrepared(CharacterClass characterClass, IEnumerable<Spell> knownSpells, IEnumerable<SpellQuantity> spellsPerDay);
    }
}
