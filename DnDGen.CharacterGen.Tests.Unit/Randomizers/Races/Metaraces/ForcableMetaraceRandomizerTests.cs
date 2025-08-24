using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Randomizers.Races.Metaraces;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Verifiers.Exceptions;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Randomizers.Races.Metaraces
{
    [TestFixture]
    public class ForcableMetaraceRandomizerTests
    {
        private TestForcableMetaraceRandomizer forcableMetaraceRandomizer;
        private Mock<IPercentileSelector> mockPercentileSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;

        private string firstMetarace = "first metarace";
        private string secondMetarace = "second metarace";
        private CharacterClassPrototype characterClass;
        private Alignment alignment;
        private List<string> alignmentRaces;
        private List<string> classRaces;
        private List<string> metaraces;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<IPercentileSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            forcableMetaraceRandomizer = new TestForcableMetaraceRandomizer(mockPercentileSelector.Object, mockCollectionsSelector.Object);

            alignment = new Alignment();
            characterClass = new CharacterClassPrototype();
            alignmentRaces = new List<string>();
            classRaces = new List<string>();

            metaraces = new List<string> { firstMetarace, secondMetarace, RaceConstants.Metaraces.None };

            alignment.Goodness = Guid.NewGuid().ToString();
            alignment.Lawfulness = Guid.NewGuid().ToString();
            characterClass.Level = 1;
            characterClass.Name = Guid.NewGuid().ToString();

            alignmentRaces.Add(firstMetarace);
            alignmentRaces.Add(secondMetarace);
            classRaces.Add(firstMetarace);
            classRaces.Add(secondMetarace);

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.GOODNESSCLASSMetaraces, alignment.Goodness, characterClass.Name);
            mockPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(metaraces);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns(firstMetarace);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, alignment.Full)).Returns(alignmentRaces);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, characterClass.Name)).Returns(classRaces);

            var index = 0;
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<string>>())).Returns((IEnumerable<string> ss) => ss.ElementAt(index++ % ss.Count()));
        }

        [Test]
        public void RandomizeThrowsErrorIfNoPossibleResults()
        {
            metaraces.Clear();
            Assert.That(() => forcableMetaraceRandomizer.Randomize(alignment, characterClass), Throws.InstanceOf<IncompatibleRandomizersException>());
        }

        [Test]
        public void RandomizeReturnsMetaraceFromSelector()
        {
            var result = forcableMetaraceRandomizer.Randomize(alignment, characterClass);
            Assert.That(result, Is.EqualTo(firstMetarace));
        }

        [Test]
        public void RandomizeRerollsNoMetarace()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(RaceConstants.Metaraces.None);

            forcableMetaraceRandomizer.ForceMetarace = true;

            var metarace = forcableMetaraceRandomizer.Randomize(alignment, characterClass);
            Assert.That(metarace, Is.EqualTo(firstMetarace));
        }

        [Test]
        public void RandomizeDoesNotRerollNoMetarace()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(RaceConstants.Metaraces.None);

            forcableMetaraceRandomizer.ForceMetarace = false;

            var metarace = forcableMetaraceRandomizer.Randomize(alignment, characterClass);
            Assert.That(metarace, Is.EqualTo(RaceConstants.Metaraces.None));
        }

        [Test]
        public void RandomizeRerollsInvalidMetaraceBecauseNotAllowed_None()
        {
            forcableMetaraceRandomizer.ForbiddenMetarace = firstMetarace;

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(firstMetarace);

            var metarace = forcableMetaraceRandomizer.Randomize(alignment, characterClass);
            Assert.That(metarace, Is.EqualTo(RaceConstants.Metaraces.None));
        }

        [Test]
        public void RandomizeRerollsInvalidMetaraceBecauseNotAllowed_Forced()
        {
            forcableMetaraceRandomizer.ForbiddenMetarace = firstMetarace;

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(firstMetarace);

            forcableMetaraceRandomizer.ForceMetarace = true;

            var metarace = forcableMetaraceRandomizer.Randomize(alignment, characterClass);
            Assert.That(metarace, Is.EqualTo(secondMetarace));
        }

        [Test]
        public void RandomizeRerollsInvalidMetaraceBecauseAlignmentDoesNotAllow_None()
        {
            alignmentRaces.Remove(firstMetarace);

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(firstMetarace);

            var metarace = forcableMetaraceRandomizer.Randomize(alignment, characterClass);
            Assert.That(metarace, Is.EqualTo(RaceConstants.Metaraces.None));
        }

        [Test]
        public void RandomizeRerollsInvalidMetaraceBecauseAlignmentDoesNotAllow_Forced()
        {
            alignmentRaces.Remove(firstMetarace);

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(firstMetarace);

            forcableMetaraceRandomizer.ForceMetarace = true;

            var metarace = forcableMetaraceRandomizer.Randomize(alignment, characterClass);
            Assert.That(metarace, Is.EqualTo(secondMetarace));
        }

        [Test]
        public void RandomizeRerollsInvalidMetaraceBecauseClassDoesNotAllow_None()
        {
            classRaces.Remove(firstMetarace);

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(firstMetarace);

            var metarace = forcableMetaraceRandomizer.Randomize(alignment, characterClass);
            Assert.That(metarace, Is.EqualTo(RaceConstants.Metaraces.None));
        }

        [Test]
        public void RandomizeRerollsInvalidMetaraceBecauseClassDoesNotAllow_Forced()
        {
            classRaces.Remove(firstMetarace);

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(firstMetarace);

            forcableMetaraceRandomizer.ForceMetarace = true;

            var metarace = forcableMetaraceRandomizer.Randomize(alignment, characterClass);
            Assert.That(metarace, Is.EqualTo(secondMetarace));
        }

        [Test]
        public void RandomizeReturnsNoMetarace()
        {
            classRaces.Remove(firstMetarace);

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(firstMetarace);

            var metarace = forcableMetaraceRandomizer.Randomize(alignment, characterClass);
            Assert.That(metarace, Is.EqualTo(RaceConstants.Metaraces.None));
        }

        [Test]
        public void RandomizeReturnsForcedMetarace()
        {
            classRaces.Remove(firstMetarace);
            forcableMetaraceRandomizer.ForceMetarace = true;

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(firstMetarace);

            var metarace = forcableMetaraceRandomizer.Randomize(alignment, characterClass);
            Assert.That(metarace, Is.EqualTo(secondMetarace));
        }

        [Test]
        public void GetAllPossibleResultsGetsNonEmptyResults()
        {
            var races = forcableMetaraceRandomizer.GetAllPossible(alignment, characterClass);

            Assert.That(races, Contains.Item(firstMetarace));
            Assert.That(races, Contains.Item(secondMetarace));
        }

        [Test]
        public void GetAllPossibleResultsFiltersOutUnallowedBaseRaces()
        {
            forcableMetaraceRandomizer.ForbiddenMetarace = firstMetarace;
            var results = forcableMetaraceRandomizer.GetAllPossible(alignment, characterClass);

            Assert.That(results, Contains.Item(secondMetarace));
            Assert.That(results, Is.All.Not.EqualTo(firstMetarace));
        }

        [Test]
        public void IfForceMetaraceIsTrueThenEmptyMetaraceIsNotAllowed()
        {
            forcableMetaraceRandomizer.ForceMetarace = true;
            var results = forcableMetaraceRandomizer.GetAllPossible(alignment, characterClass);
            Assert.That(results, Is.All.Not.EqualTo(RaceConstants.Metaraces.None));
        }

        [Test]
        public void IfForceMetaraceIsFalseThenEmptyMetaraceIsAllowed()
        {
            forcableMetaraceRandomizer.ForceMetarace = false;
            var results = forcableMetaraceRandomizer.GetAllPossible(alignment, characterClass);
            Assert.That(results, Contains.Item(RaceConstants.Metaraces.None));
        }

        [Test]
        public void GetAllPossibleResultsFiltersOutMetaracesNotAllowedByAlignment()
        {
            alignmentRaces.Remove(firstMetarace);

            var results = forcableMetaraceRandomizer.GetAllPossible(alignment, characterClass);
            Assert.That(results, Contains.Item(secondMetarace));
            Assert.That(results, Is.All.Not.EqualTo(firstMetarace));
        }

        [Test]
        public void GetAllPossibleResultsFiltersOutMetaracesNotAllowedByClass()
        {
            classRaces.Remove(firstMetarace);

            var results = forcableMetaraceRandomizer.GetAllPossible(alignment, characterClass);
            Assert.That(results, Contains.Item(secondMetarace));
            Assert.That(results, Is.All.Not.EqualTo(firstMetarace));
        }

        private class TestForcableMetaraceRandomizer : ForcableMetaraceBase
        {
            public string ForbiddenMetarace { get; set; }

            public TestForcableMetaraceRandomizer(IPercentileSelector percentileResultSelector, ICollectionSelector collectionSelector)
                : base(percentileResultSelector, collectionSelector)
            { }

            protected override bool MetaraceIsAllowed(string metarace)
            {
                return metarace != ForbiddenMetarace;
            }
        }
    }
}