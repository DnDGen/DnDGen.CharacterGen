using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Verifiers.Exceptions;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames
{
    internal abstract class BaseClassNameRandomizer : IClassNameRandomizer
    {
        private readonly IPercentileSelector percentileSelector;
        private readonly ICollectionSelector collectionsSelector;

        public BaseClassNameRandomizer(IPercentileSelector percentileSelector, ICollectionSelector collectionsSelector)
        {
            this.percentileSelector = percentileSelector;
            this.collectionsSelector = collectionsSelector;
        }

        public string Randomize(Alignment alignment)
        {
            var possibleClassNames = GetAllPossibleResults(alignment);
            if (possibleClassNames.Any() == false)
                throw new IncompatibleRandomizersException();

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.GOODNESSCharacterClasses, alignment.Goodness);
            var className = percentileSelector.SelectFrom(Config.Name, tableName);

            if (possibleClassNames.Contains(className))
                return className;

            return collectionsSelector.SelectRandomFrom(possibleClassNames);
        }

        protected abstract bool CharacterClassIsAllowed(string className, Alignment alignment);

        public IEnumerable<string> GetAllPossibleResults(Alignment alignment)
        {
            var tableName = string.Format(TableNameConstants.Formattable.Percentile.GOODNESSCharacterClasses, alignment.Goodness);
            var classNames = percentileSelector.SelectAllFrom(Config.Name, tableName);
            return classNames.Where(c => CharacterClassIsAllowed(c, alignment));
        }
    }
}