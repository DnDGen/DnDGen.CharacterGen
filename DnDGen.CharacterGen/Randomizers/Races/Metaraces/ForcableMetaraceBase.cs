using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Verifiers.Exceptions;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Randomizers.Races.Metaraces
{
    internal abstract class ForcableMetaraceBase : IForcableMetaraceRandomizer
    {
        public bool ForceMetarace { get; set; }

        private readonly ICollectionSelector collectionSelector;
        private readonly IPercentileSelector percentileSelector;

        public ForcableMetaraceBase(IPercentileSelector percentileSelector, ICollectionSelector collectionSelector)
        {
            this.percentileSelector = percentileSelector;
            this.collectionSelector = collectionSelector;
        }

        public string Randomize(Alignment alignment, CharacterClassPrototype characterClass)
        {
            var allowedMetaraces = GetAllPossible(alignment, characterClass);
            if (!allowedMetaraces.Any())
                throw new IncompatibleRandomizersException();

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.GOODNESSCLASSMetaraces, alignment.Goodness, characterClass.Name);
            var metarace = percentileSelector.SelectFrom(Config.Name, tableName);

            if (allowedMetaraces.Contains(metarace))
                return metarace;

            return GetDefaultMetarace(allowedMetaraces);
        }

        private string GetDefaultMetarace(IEnumerable<string> allowedMetaraces)
        {
            if (allowedMetaraces.Contains(RaceConstants.Metaraces.None))
                return RaceConstants.Metaraces.None;

            return collectionSelector.SelectRandomFrom(allowedMetaraces);
        }

        public IEnumerable<string> GetAllPossible(Alignment alignment, CharacterClassPrototype characterClass)
        {
            var tableName = string.Format(TableNameConstants.Formattable.Percentile.GOODNESSCLASSMetaraces, alignment.Goodness, characterClass.Name);
            var metaraces = percentileSelector.SelectAllFrom(Config.Name, tableName);
            return metaraces.Where(r => RaceIsAllowed(r, alignment, characterClass));
        }

        private bool RaceIsAllowed(string metarace, Alignment alignment, CharacterClassPrototype characterClass)
        {
            if (metarace == RaceConstants.Metaraces.None)
                return !ForceMetarace;

            return MetaraceCanBeAlignment(metarace, alignment)
                && MetaraceCanBeClass(metarace, characterClass)
                && MetaraceIsAllowed(metarace);
        }

        private bool MetaraceCanBeClass(string metarace, CharacterClassPrototype characterClass)
        {
            var classRaces = collectionSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, characterClass.Name);
            return classRaces.Contains(metarace);
        }

        private bool MetaraceCanBeAlignment(string metarace, Alignment alignment)
        {
            var alignmentRaces = collectionSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, alignment.Full);
            return alignmentRaces.Contains(metarace);
        }

        protected abstract bool MetaraceIsAllowed(string metarace);
    }
}