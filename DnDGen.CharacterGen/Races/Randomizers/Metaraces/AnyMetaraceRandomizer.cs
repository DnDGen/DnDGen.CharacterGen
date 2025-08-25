using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;

namespace DnDGen.CharacterGen.Races.Randomizers.Metaraces
{
    internal class AnyMetaraceRandomizer : ForcableMetaraceBase
    {
        public AnyMetaraceRandomizer(IPercentileSelector percentileResultSelector, ICollectionSelector collectionSelector)
            : base(percentileResultSelector, collectionSelector)
        { }

        protected override bool MetaraceIsAllowed(string metarace)
        {
            return true;
        }
    }
}