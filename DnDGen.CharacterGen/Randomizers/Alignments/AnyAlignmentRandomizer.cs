using DnDGen.CharacterGen.Alignments;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;

namespace DnDGen.CharacterGen.Generators.Randomizers.Alignments
{
    internal class AnyAlignmentRandomizer : BaseAlignmentRandomizer
    {
        public AnyAlignmentRandomizer(IPercentileSelector innerSelector, ICollectionSelector collectionsSelector)
            : base(innerSelector, collectionsSelector)
        { }

        protected override bool AlignmentIsAllowed(Alignment alignment)
        {
            return true;
        }
    }
}