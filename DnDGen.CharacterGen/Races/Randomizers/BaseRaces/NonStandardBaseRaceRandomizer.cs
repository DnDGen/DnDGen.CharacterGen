using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System.Linq;

namespace DnDGen.CharacterGen.Races.Randomizers.BaseRaces
{
    internal class NonStandardBaseRaceRandomizer : BaseRaceRandomizerBase
    {
        private readonly ICollectionSelector collectionsSelector;

        public NonStandardBaseRaceRandomizer(IPercentileSelector percentileResultSelector, ICollectionSelector collectionsSelector)
            : base(percentileResultSelector, collectionsSelector)
        {
            this.collectionsSelector = collectionsSelector;
        }

        protected override bool BaseRaceIsAllowedByRandomizer(string baseRace)
        {
            var standardBaseRaces = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Standard);
            return !standardBaseRaces.Contains(baseRace);
        }
    }
}