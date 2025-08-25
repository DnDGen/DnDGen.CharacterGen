using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Races.Randomizers;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Verifiers.Exceptions;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Races.Randomizers.BaseRaces
{
    internal abstract class BaseRaceRandomizerBase : RaceRandomizer
    {
        private readonly IPercentileSelector percentileSelector;
        private readonly ICollectionSelector collectionSelector;

        public BaseRaceRandomizerBase(IPercentileSelector percentileSelector, ICollectionSelector collectionSelector)
        {
            this.percentileSelector = percentileSelector;
            this.collectionSelector = collectionSelector;
        }

        public string Randomize(Alignment alignment, CharacterClassPrototype characterClass)
        {
            var allowedBaseRaces = GetAllPossible(alignment, characterClass);
            if (!allowedBaseRaces.Any())
                throw new IncompatibleRandomizersException();

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.GOODNESSCLASSBaseRaces, alignment.Goodness, characterClass.Name);
            var baseRace = percentileSelector.SelectFrom(Config.Name, tableName);

            if (allowedBaseRaces.Contains(baseRace))
                return baseRace;

            return collectionSelector.SelectRandomFrom(allowedBaseRaces);
        }

        private bool RaceIsAllowed(string baseRace, Alignment alignment)
        {
            return !string.IsNullOrEmpty(baseRace)
                && BaseRaceCanBeAlignment(baseRace, alignment)
                && BaseRaceIsAllowedByRandomizer(baseRace);
        }

        private bool BaseRaceCanBeAlignment(string baseRace, Alignment alignment)
        {
            var alignmentRaces = collectionSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, alignment.Full);
            return alignmentRaces.Contains(baseRace);
        }

        protected abstract bool BaseRaceIsAllowedByRandomizer(string baseRace);

        public IEnumerable<string> GetAllPossible(Alignment alignment, CharacterClassPrototype characterClass)
        {
            var tableName = string.Format(TableNameConstants.Formattable.Percentile.GOODNESSCLASSBaseRaces, alignment.Goodness, characterClass.Name);
            var baseRaces = percentileSelector.SelectAllFrom(Config.Name, tableName);

            return baseRaces.Where(r => RaceIsAllowed(r, alignment));
        }
    }
}