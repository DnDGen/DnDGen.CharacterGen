using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System.Linq;

namespace DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames
{
    internal class AnyPlayerClassNameRandomizer : BaseClassNameRandomizer
    {
        private readonly ICollectionSelector collectionsSelector;

        public AnyPlayerClassNameRandomizer(IPercentileSelector percentileResultSelector, ICollectionSelector collectionsSelector)
            : base(percentileResultSelector, collectionsSelector)
        {
            this.collectionsSelector = collectionsSelector;
        }

        protected override bool CharacterClassIsAllowed(string className, Alignment alignment)
        {
            var players = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.Players);
            var alignmentClasses = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, alignment.ToString());
            var allowedClasses = players.Intersect(alignmentClasses);

            return allowedClasses.Contains(className);
        }
    }
}