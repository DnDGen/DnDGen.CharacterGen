using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using Moq;
using NUnit.Framework;
using System;

namespace DnDGen.CharacterGen.Tests.Unit.Races.Randomizers
{
    [TestFixture]
    public abstract class RaceRandomizerTestBase
    {
        internal Mock<IPercentileSelector> mockPercentileSelector;
        internal Mock<ICollectionSelector> mockCollectionSelector;
        protected Alignment alignment;
        protected CharacterClassPrototype characterClass;

        [SetUp]
        public void RaceRandomizerTestBaseSetup()
        {
            mockPercentileSelector = new Mock<IPercentileSelector>();
            mockCollectionSelector = new Mock<ICollectionSelector>();
            characterClass = new CharacterClassPrototype();
            alignment = new Alignment();

            alignment.Goodness = Guid.NewGuid().ToString();
            alignment.Lawfulness = Guid.NewGuid().ToString();
            characterClass.Level = 1;
            characterClass.Name = Guid.NewGuid().ToString();
        }
    }
}