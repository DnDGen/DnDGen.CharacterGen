using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Verifiers.Exceptions;
using DnDGen.Infrastructure.Selectors.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Randomizers.CharacterClasses.ClassNames
{
    internal class SetClassNameRandomizer : ISetClassNameRandomizer
    {
        public string SetClassName { get; set; }

        private readonly ICollectionSelector collectionsSelector;

        public SetClassNameRandomizer(ICollectionSelector collectionsSelector)
        {
            this.collectionsSelector = collectionsSelector;
        }

        public string Randomize(Alignment alignment)
        {
            var classes = GetAllPossibleResults(alignment);

            if (classes.Any() == false)
                throw new IncompatibleRandomizersException();

            return classes.Single();
        }

        public IEnumerable<string> GetAllPossibleResults(Alignment alignment)
        {
            var alignmentClasses = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, alignment.Full);
            return alignmentClasses.Intersect(new[] { SetClassName });
        }
    }
}