using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System.Linq;

namespace DnDGen.CharacterGen.Races.Randomizers.BaseRaces
{
    internal class NonMonsterBaseRaceRandomizer : BaseRaceRandomizerBase
    {
        private readonly ICollectionSelector collectionsSelector;

        public NonMonsterBaseRaceRandomizer(IPercentileSelector percentileResultSelector, ICollectionSelector collectionsSelector)
            : base(percentileResultSelector, collectionsSelector)
        {
            this.collectionsSelector = collectionsSelector;
        }

        protected override bool BaseRaceIsAllowedByRandomizer(string baseRace)
        {
            var monsters = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters);
            return !monsters.Contains(baseRace);
        }
    }
}
