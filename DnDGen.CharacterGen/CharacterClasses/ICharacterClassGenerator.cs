using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Randomizers.CharacterClasses.ClassNames;
using DnDGen.CharacterGen.Randomizers.CharacterClasses.Levels;
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