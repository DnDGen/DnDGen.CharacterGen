using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Verifiers.Exceptions;
using DnDGen.Infrastructure.Selectors.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Randomizers.Races.Metaraces
{
    internal class SetMetaraceRandomizer : ISetMetaraceRandomizer
    {
        public string SetMetarace { get; set; }

        private readonly ICollectionSelector collectionsSelector;

        public SetMetaraceRandomizer(ICollectionSelector collectionsSelector)
        {
            this.collectionsSelector = collectionsSelector;
        }

        public string Randomize(Alignment alignment, CharacterClassPrototype characterClass)
        {
            var metaraces = GetAllPossible(alignment, characterClass);

            if (!metaraces.Any())
                throw new IncompatibleRandomizersException();

            return metaraces.Single();
        }

        public IEnumerable<string> GetAllPossible(Alignment alignment, CharacterClassPrototype characterClass)
        {
            var alignmentMetaraces = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, alignment.Full);
            var classMetaraces = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, characterClass.Name);

            return alignmentMetaraces.Intersect(classMetaraces).Intersect(new[] { SetMetarace });
        }
    }
}