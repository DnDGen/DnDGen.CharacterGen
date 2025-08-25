using DnDGen.CharacterGen.Alignments.Randomizers;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Alignments
{
    internal class AlignmentGenerator : IAlignmentGenerator
    {
        public Alignment GeneratePrototype(IAlignmentRandomizer alignmentRandomizer)
        {
            return alignmentRandomizer.Randomize();
        }

        public IEnumerable<Alignment> GeneratePrototypes(IAlignmentRandomizer alignmentRandomizer)
        {
            return alignmentRandomizer.GetAllPossibleResults();
        }

        public Alignment GenerateWith(Alignment alignmentPrototype)
        {
            return alignmentPrototype;
        }
    }
}