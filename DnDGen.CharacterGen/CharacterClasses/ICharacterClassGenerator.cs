using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using DnDGen.CharacterGen.Races;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.CharacterClasses
{
    internal interface ICharacterClassGenerator
    {
        CharacterClassPrototype GeneratePrototype(Alignment alignmentPrototype, IClassNameRandomizer classNameRandomizer, ILevelRandomizer levelRandomizer);
        IEnumerable<CharacterClassPrototype> GeneratePrototypes(Alignment alignmentPrototype, IClassNameRandomizer classNameRandomizer, ILevelRandomizer levelRandomizer);
        CharacterClass GenerateWith(Alignment alignment, CharacterClassPrototype classPrototype, RacePrototype racePrototype);
    }
}