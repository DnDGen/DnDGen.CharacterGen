using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Verifiers.Exceptions;
using DnDGen.Infrastructure.Selectors.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Races.Randomizers.BaseRaces
{
    internal class SetBaseRaceRandomizer : ISetBaseRaceRandomizer
    {
        public string SetBaseRace { get; set; }

        private readonly ICollectionSelector collectionsSelector;

        public SetBaseRaceRandomizer(ICollectionSelector collectionsSelector)
        {
            this.collectionsSelector = collectionsSelector;
        }

        public string Randomize(Alignment alignment, CharacterClassPrototype characterClass)
        {
            var baseRaces = GetAllPossible(alignment, characterClass);

            if (!baseRaces.Any())
                throw new IncompatibleRandomizersException();

            return baseRaces.Single();
        }

        public IEnumerable<string> GetAllPossible(Alignment alignment, CharacterClassPrototype characterClass)
        {
            var alignmentBaseRaces = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, alignment.Full);

            return alignmentBaseRaces.Intersect(new[] { SetBaseRace });
        }
    }
}