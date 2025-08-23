using DnDGen.CharacterGen.Alignments;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;

namespace DnDGen.CharacterGen.Randomizers.Alignments
{
    internal class NonChaoticAlignmentRandomizer : BaseAlignmentRandomizer
    {
        public NonChaoticAlignmentRandomizer(IPercentileSelector innerSelector, ICollectionSelector collectionsSelector)
            : base(innerSelector, collectionsSelector)
        { }

        protected override bool AlignmentIsAllowed(Alignment alignment)
        {
            return alignment.Lawfulness != AlignmentConstants.Chaotic;
        }
    }
}