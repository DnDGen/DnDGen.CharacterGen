using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Randomizers.Races.BaseRaces;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Verifiers.Exceptions;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Generators.Randomizers.Races.BaseRaces
{
    [TestFixture]
    public class BaseRaceRandomizerTests
    {
        private TestBaseRaceRandomizer randomizer;
        private Mock<IPercentileSelector> mockPercentileResultSelector;
        private Mock<ICollectionSelector> mockCollectionSelector;

        private string firstBaseRace = "first base race";
        private string secondBaseRace = "second base race";
        private CharacterClassPrototype characterClass;
        private Alignment alignment;
        private List<string> alignmentRaces;

        [SetUp]
        public void Setup()
        {
            mockPercentileResultSelector = new Mock<IPercentileSelector>();
            mockCollectionSelector = new Mock<ICollectionSelector>();
            randomizer = new TestBaseRaceRandomizer(mockPercentileResultSelector.Object, mockCollectionSelector.Object);

            alignment = new Alignment();
            characterClass = new CharacterClassPrototype();
            alignmentRaces = new List<string>();

            characterClass.Name = "class name";
            characterClass.Level = 1;

            alignment.Goodness = Guid.NewGuid().ToString();
            alignment.Lawfulness = Guid.NewGuid().ToString();

            alignmentRaces.Add(firstBaseRace);
            alignmentRaces.Add(secondBaseRace);

            var baseRaces = new[] { firstBaseRace, secondBaseRace, string.Empty };

            mockPercentileResultSelector.Setup(s => s.SelectAllFrom(Config.Name, It.IsAny<string>())).Returns(baseRaces);
            mockPercentileResultSelector.Setup(s => s.SelectFrom(Config.Name, It.IsAny<string>())).Returns(firstBaseRace);
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, alignment.Full)).Returns(alignmentRaces);

            var index = 0;
            mockCollectionSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<string>>())).Returns((IEnumerable<string> ss) => ss.ElementAt(index++ % ss.Count()));
        }

        [Test]
        public void RandomizeGetsAllPossibleResultsFromGetAllPossibleResults()
        {
            randomizer.Randomize(alignment, characterClass);
            mockPercentileResultSelector.Verify(p => p.SelectAllFrom(Config.Name, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void RandomizeThrowsErrorIfNoPossibleResults()
        {
            mockPercentileResultSelector.Setup(p => p.SelectAllFrom(Config.Name, It.IsAny<string>())).Returns(Enumerable.Empty<string>());
            Assert.That(() => randomizer.Randomize(alignment, characterClass), Throws.InstanceOf<IncompatibleRandomizersException>());
        }

        [Test]
        public void RandomizeReturnsBaseRaceFromPercentileResultSelector()
        {
            var result = randomizer.Randomize(alignment, characterClass);
            Assert.That(result, Is.EqualTo(firstBaseRace));
        }

        [Test]
        public void RandomizeAccessesTableAlignmentGoodnessClassNameBaseRaces()
        {
            randomizer.Randomize(alignment, characterClass);
            var tableName = string.Format(TableNameConstants.Formattable.Percentile.GOODNESSCLASSBaseRaces, alignment.Goodness, characterClass.Name);
            mockPercentileResultSelector.Verify(p => p.SelectFrom(Config.Name, tableName), Times.Once);
        }

        [Test]
        public void RandomizeDefaultsOnEmptyBaseRace()
        {
            mockPercentileResultSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(string.Empty);

            var baseRace = randomizer.Randomize(alignment, characterClass);
            Assert.That(baseRace, Is.EqualTo(firstBaseRace));
        }

        [Test]
        public void RandomizeRandomlyDefaultsOnEmptyBaseRace()
        {
            mockPercentileResultSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(string.Empty);
            mockCollectionSelector.Setup(s => s.SelectRandomFrom(new[] { firstBaseRace, secondBaseRace })).Returns(secondBaseRace);

            var baseRace = randomizer.Randomize(alignment, characterClass);
            Assert.That(baseRace, Is.EqualTo(secondBaseRace));
        }

        [Test]
        public void RandomizeDefaultsBaseRaceBecauseNotAllowed()
        {
            randomizer.ForbiddenBaseRace = firstBaseRace;

            mockPercentileResultSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(firstBaseRace);

            var baseRace = randomizer.Randomize(alignment, characterClass);
            Assert.That(baseRace, Is.EqualTo(secondBaseRace));
        }

        [Test]
        public void RandomizeDefaultsInvalidBaseRaceBecauseAlignmentDoesNotAllow()
        {
            alignmentRaces.Remove(firstBaseRace);

            mockPercentileResultSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(firstBaseRace);

            var baseRace = randomizer.Randomize(alignment, characterClass);
            Assert.That(baseRace, Is.EqualTo(secondBaseRace));
        }

        [Test]
        public void RandomizeReturnsDefaultBaseRace()
        {
            alignmentRaces.Remove(firstBaseRace);

            mockPercentileResultSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(firstBaseRace);

            var baseRace = randomizer.Randomize(alignment, characterClass);
            Assert.That(baseRace, Is.EqualTo(secondBaseRace));
        }

        [Test]
        public void GetAllPossibleResultsGetsResultsFromSelector()
        {
            randomizer.GetAllPossible(alignment, characterClass);
            mockPercentileResultSelector.Verify(p => p.SelectAllFrom(Config.Name, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void GetAllPossibleResultsFiltersOutEmptyStrings()
        {
            var classNames = randomizer.GetAllPossible(alignment, characterClass);
            Assert.That(classNames, Contains.Item(firstBaseRace));
            Assert.That(classNames, Contains.Item(secondBaseRace));
            Assert.That(classNames.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetAllPossibleResultsAccessesTableAlignmentGoodnessClassNameBaseRaces()
        {
            characterClass.Name = "className";
            alignment.Goodness = "goodness";

            randomizer.GetAllPossible(alignment, characterClass);
            mockPercentileResultSelector.Verify(p => p.SelectAllFrom(Config.Name, "goodnessclassNameBaseRaces"), Times.Once);
        }

        [Test]
        public void GetAllPossibleResultsFiltersOutUnallowedBaseRaces()
        {
            randomizer.ForbiddenBaseRace = firstBaseRace;

            var results = randomizer.GetAllPossible(alignment, characterClass);
            Assert.That(results, Contains.Item(secondBaseRace));
            Assert.That(results.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetAllPossibleResultsFiltersOutBaseRacesNotAllowedByAlignment()
        {
            alignmentRaces.Remove(firstBaseRace);

            var results = randomizer.GetAllPossible(alignment, characterClass);
            Assert.That(results, Contains.Item(secondBaseRace));
            Assert.That(results.Count(), Is.EqualTo(1));
        }

        private class TestBaseRaceRandomizer : BaseRaceRandomizerBase
        {
            public string ForbiddenBaseRace { get; set; }

            public TestBaseRaceRandomizer(IPercentileSelector percentileResultSelector, ICollectionSelector collectionSelector)
                : base(percentileResultSelector, collectionSelector)
            { }

            protected override bool BaseRaceIsAllowedByRandomizer(string baseRace)
            {
                return baseRace != ForbiddenBaseRace;
            }
        }
    }
}