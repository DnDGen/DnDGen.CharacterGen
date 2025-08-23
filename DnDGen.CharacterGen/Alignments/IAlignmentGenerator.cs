using DnDGen.CharacterGen.Randomizers.Alignments;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Alignments
{
    internal interface IAlignmentGenerator
    {
        Alignment GenerateWith(Alignment alignmentPrototype);
        Alignment GeneratePrototype(IAlignmentRandomizer alignmentRandomizer);
        IEnumerable<Alignment> GeneratePrototypes(IAlignmentRandomizer alignmentRandomizer);
    }
}