using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Races.Randomizers
{
    public interface RaceRandomizer
    {
        string Randomize(Alignment alignment, CharacterClassPrototype characterClass);
        IEnumerable<string> GetAllPossible(Alignment alignment, CharacterClassPrototype characterClass);
    }
}