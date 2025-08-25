using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Selectors;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Abilities.Selectors
{
    internal class AbilityAdjustmentsSelector : IAbilityAdjustmentsSelector
    {
        private readonly IAdjustmentsSelector innerSelector;
        private readonly ICollectionSelector collectionsSelector;

        public AbilityAdjustmentsSelector(IAdjustmentsSelector innerSelector, ICollectionSelector collectionsSelector)
        {
            this.innerSelector = innerSelector;
            this.collectionsSelector = collectionsSelector;
        }

        public Dictionary<string, int> SelectFor(Race race)
        {
            var adjustments = new Dictionary<string, int>();
            var abilities = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.AbilityGroups, GroupConstants.All);

            var tableName = string.Format(TableNameConstants.Formattable.Adjustments.AGEAbilityAdjustments, race.Age.Description);
            var agingEffects = innerSelector.SelectAllFrom(tableName);

            tableName = string.Format(TableNameConstants.Formattable.Adjustments.RACEAbilityAdjustments, race.BaseRace);
            var baseRaceAdjustments = innerSelector.SelectAllFrom(tableName);

            tableName = string.Format(TableNameConstants.Formattable.Adjustments.RACEAbilityAdjustments, race.Metarace);
            var metaraceAdjustments = innerSelector.SelectAllFrom(tableName);

            foreach (var ability in abilities)
            {
                adjustments[ability] = baseRaceAdjustments[ability];
                adjustments[ability] += metaraceAdjustments[ability];
                adjustments[ability] += agingEffects[ability];
            }

            return adjustments;
        }
    }
}