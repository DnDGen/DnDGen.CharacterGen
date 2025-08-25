using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System.Linq;

namespace DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames
{
    internal class DivineSpellcasterClassNameRandomizer : BaseClassNameRandomizer
    {
        private readonly ICollectionSelector collectionsSelector;

        public DivineSpellcasterClassNameRandomizer(IPercentileSelector percentileResultSelector, ICollectionSelector collectionsSelector)
            : base(percentileResultSelector, collectionsSelector)
        {
            this.collectionsSelector = collectionsSelector;
        }

        protected override bool CharacterClassIsAllowed(string className, Alignment alignment)
        {
            var divineSpellcasters = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Divine);
            var alignmentClasses = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, alignment.ToString());
            var allowedClasses = divineSpellcasters.Intersect(alignmentClasses);

            return allowedClasses.Contains(className);
        }
    }
}