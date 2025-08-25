using DnDGen.CharacterGen.Alignments;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Alignments.Randomizers
{
    internal class SetAlignmentRandomizer : ISetAlignmentRandomizer
    {
        public Alignment SetAlignment { get; set; }

        public SetAlignmentRandomizer()
        {
            SetAlignment = new Alignment();
        }

        public Alignment Randomize()
        {
            return SetAlignment;
        }

        public IEnumerable<Alignment> GetAllPossibleResults()
        {
            return new[] { SetAlignment };
        }
    }
}