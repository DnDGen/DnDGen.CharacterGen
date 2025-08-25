using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Races.Randomizers;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Races.Randomizers.Metaraces
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