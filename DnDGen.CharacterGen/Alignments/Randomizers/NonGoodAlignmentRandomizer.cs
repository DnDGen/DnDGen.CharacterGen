using DnDGen.CharacterGen.Alignments;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;

namespace DnDGen.CharacterGen.Alignments.Randomizers
{
    internal class NonGoodAlignmentRandomizer : BaseAlignmentRandomizer
    {
        public NonGoodAlignmentRandomizer(IPercentileSelector innerSelector, ICollectionSelector collectionsSelector)
            : base(innerSelector, collectionsSelector)
        { }

        protected override bool AlignmentIsAllowed(Alignment alignment)
        {
            return alignment.Goodness != AlignmentConstants.Good;
        }
    }
}