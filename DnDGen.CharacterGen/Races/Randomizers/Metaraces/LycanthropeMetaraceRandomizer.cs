using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System.Linq;

namespace DnDGen.CharacterGen.Races.Randomizers.Metaraces
{
    internal class LycanthropeMetaraceRandomizer : ForcableMetaraceBase
    {
        private readonly ICollectionSelector collectionsSelector;

        public LycanthropeMetaraceRandomizer(IPercentileSelector percentileResultSelector, ICollectionSelector collectionsSelector)
            : base(percentileResultSelector, collectionsSelector)
        {
            this.collectionsSelector = collectionsSelector;
        }

        protected override bool MetaraceIsAllowed(string metarace)
        {
            var metaraces = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, GroupConstants.Lycanthrope);
            return metaraces.Contains(metarace);
        }
    }
}