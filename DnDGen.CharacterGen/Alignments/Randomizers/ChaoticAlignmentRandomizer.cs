using DnDGen.CharacterGen.Alignments;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;

namespace DnDGen.CharacterGen.Alignments.Randomizers
{
    internal class ChaoticAlignmentRandomizer : BaseAlignmentRandomizer
    {
        public ChaoticAlignmentRandomizer(IPercentileSelector innerSelector, ICollectionSelector collectionsSelector)
            : base(innerSelector, collectionsSelector)
        { }

        protected override bool AlignmentIsAllowed(Alignment alignment)
        {
            return alignment.Lawfulness == AlignmentConstants.Chaotic;
        }
    }
}