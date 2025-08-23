using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Randomizers.Races;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Randomizers.Races.Metaraces
{
    internal class NoMetaraceRandomizer : RaceRandomizer
    {
        public string Randomize(Alignment alignment, CharacterClassPrototype characterClass)
        {
            return RaceConstants.Metaraces.None;
        }

        public IEnumerable<string> GetAllPossible(Alignment alignment, CharacterClassPrototype characterClass)
        {
            return new[] { RaceConstants.Metaraces.None };
        }
    }
}