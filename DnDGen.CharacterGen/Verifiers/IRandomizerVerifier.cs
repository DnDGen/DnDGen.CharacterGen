using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Randomizers.Alignments;
using DnDGen.CharacterGen.Randomizers.CharacterClasses.ClassNames;
using DnDGen.CharacterGen.Randomizers.CharacterClasses.Levels;
using DnDGen.CharacterGen.Randomizers.Races;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Verifiers
{
    public interface IRandomizerVerifier
    {
        bool VerifyCompatibility(IAlignmentRandomizer alignmentRandomizer, IClassNameRandomizer classNameRandomizer, ILevelRandomizer levelRandomizer, RaceRandomizer baseRaceRandomizer, RaceRandomizer metaraceRandomizer);
        bool VerifyAlignmentCompatibility(Alignment alignment, IClassNameRandomizer classNameRandomizer, ILevelRandomizer levelRandomizer, RaceRandomizer baseRaceRandomizer, RaceRandomizer metaraceRandomizer);
        IEnumerable<Alignment> FilterAlignments(
            IEnumerable<Alignment> alignments,
            IClassNameRandomizer classNameRandomizer,
            ILevelRandomizer levelRandomizer,
            RaceRandomizer baseRaceRandomizer,
            RaceRandomizer metaraceRandomizer);
        bool VerifyCharacterClassCompatibility(Alignment alignment, CharacterClassPrototype characterClass, RaceRandomizer baseRaceRandomizer, RaceRandomizer metaraceRandomizer);
        IEnumerable<CharacterClassPrototype> FilterCharacterClasses(
            IEnumerable<CharacterClassPrototype> characterClasses,
            Alignment alignment,
            RaceRandomizer baseRaceRandomizer,
            RaceRandomizer metaraceRandomizer);
        bool VerifyRaceCompatibility(Alignment alignment, CharacterClassPrototype characterClass, RacePrototype race);
        IEnumerable<RacePrototype> FilterRaces(IEnumerable<RacePrototype> races, Alignment alignment, CharacterClassPrototype characterClass);
    }
}