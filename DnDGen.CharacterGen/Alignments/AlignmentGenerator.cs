using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Randomizers.Alignments;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Generators.Alignments
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