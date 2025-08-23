using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Randomizers.Races;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Verifiers.Exceptions;
using DnDGen.Infrastructure.Selectors.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Randomizers.Races.BaseRaces
{
    //INFO: We are not using the base class here, as aquatic characters will never appear randomly (which assumes mostly on land)
    internal class AquaticBaseRaceRandomizer : RaceRandomizer
    {
        private readonly ICollectionSelector collectionSelector;

        public AquaticBaseRaceRandomizer(ICollectionSelector collectionSelector)
        {
            this.collectionSelector = collectionSelector;
        }

        public string Randomize(Alignment alignment, CharacterClassPrototype characterClass)
        {
            var allowedBaseRaces = GetAllPossible(alignment, characterClass);
            if (!allowedBaseRaces.Any())
                throw new IncompatibleRandomizersException();

            return collectionSelector.SelectRandomFrom(allowedBaseRaces);
        }

        private bool RaceIsAllowed(string baseRace, Alignment alignment)
        {
            return BaseRaceCanBeAlignment(baseRace, alignment);
        }

        private bool BaseRaceCanBeAlignment(string baseRace, Alignment alignment)
        {
            var alignmentRaces = collectionSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, alignment.Full);
            return alignmentRaces.Contains(baseRace);
        }

        public IEnumerable<string> GetAllPossible(Alignment alignment, CharacterClassPrototype characterClass)
        {
            var aquaticBaseRaces = collectionSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Aquatic);
            return aquaticBaseRaces.Where(r => RaceIsAllowed(r, alignment));
        }
    }
}
