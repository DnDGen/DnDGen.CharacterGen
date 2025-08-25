using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Verifiers.Exceptions;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Alignments.Randomizers
{
    internal abstract class BaseAlignmentRandomizer : IAlignmentRandomizer
    {
        private readonly IPercentileSelector percentileResultSelector;
        private readonly ICollectionSelector collectionsSelector;

        public BaseAlignmentRandomizer(IPercentileSelector percentileResultSelector, ICollectionSelector collectionsSelector)
        {
            this.percentileResultSelector = percentileResultSelector;
            this.collectionsSelector = collectionsSelector;
        }

        public Alignment Randomize()
        {
            var possibleAlignments = GetAllPossibleResults();
            if (possibleAlignments.Any() == false)
                throw new IncompatibleRandomizersException();

            var alignment = GenerateAlignment();
            if (possibleAlignments.Contains(alignment))
                return alignment;

            return collectionsSelector.SelectRandomFrom(possibleAlignments);
        }

        private Alignment GenerateAlignment()
        {
            var alignment = new Alignment();

            alignment.Lawfulness = percentileResultSelector.SelectFrom(Config.Name, TableNameConstants.Set.Percentile.AlignmentLawfulness);
            alignment.Goodness = percentileResultSelector.SelectFrom(Config.Name, TableNameConstants.Set.Percentile.AlignmentGoodness);

            return alignment;
        }

        public IEnumerable<Alignment> GetAllPossibleResults()
        {
            var goodnesses = percentileResultSelector.SelectAllFrom(Config.Name, TableNameConstants.Set.Percentile.AlignmentGoodness);
            var lawfulnesses = percentileResultSelector.SelectAllFrom(Config.Name, TableNameConstants.Set.Percentile.AlignmentLawfulness);

            foreach (var goodness in goodnesses)
            {
                foreach (var lawfulness in lawfulnesses)
                {
                    var alignment = new Alignment { Goodness = goodness, Lawfulness = lawfulness };
                    if (AlignmentIsAllowed(alignment))
                        yield return alignment;
                }
            }
        }

        protected abstract bool AlignmentIsAllowed(Alignment alignment);
    }
}