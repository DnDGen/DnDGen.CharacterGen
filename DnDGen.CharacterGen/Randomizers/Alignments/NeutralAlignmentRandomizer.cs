using DnDGen.CharacterGen.Alignments;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;

namespace DnDGen.CharacterGen.Randomizers.Alignments
{
    internal class NeutralAlignmentRandomizer : BaseAlignmentRandomizer
    {
        public NeutralAlignmentRandomizer(IPercentileSelector innerSelector, ICollectionSelector collectionsSelector)
            : base(innerSelector, collectionsSelector)
        { }

        protected override bool AlignmentIsAllowed(Alignment alignment)
        {
            return alignment.Goodness == AlignmentConstants.Neutral || alignment.Lawfulness == AlignmentConstants.Neutral;
        }
    }
}