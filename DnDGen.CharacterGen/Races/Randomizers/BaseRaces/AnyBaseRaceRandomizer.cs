using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;

namespace DnDGen.CharacterGen.Races.Randomizers.BaseRaces
{
    internal class AnyBaseRaceRandomizer : BaseRaceRandomizerBase
    {
        public AnyBaseRaceRandomizer(IPercentileSelector percentileResultSelector, ICollectionSelector collectionSelector)
            : base(percentileResultSelector, collectionSelector) { }

        protected override bool BaseRaceIsAllowedByRandomizer(string baseRace)
        {
            return true;
        }
    }
}