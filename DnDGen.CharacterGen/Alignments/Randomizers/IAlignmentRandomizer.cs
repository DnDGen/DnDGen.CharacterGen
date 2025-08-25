using System.Collections.Generic;
using DnDGen.CharacterGen.Alignments;

namespace DnDGen.CharacterGen.Alignments.Randomizers
{
    public interface IAlignmentRandomizer
    {
        Alignment Randomize();
        IEnumerable<Alignment> GetAllPossibleResults();
    }
}