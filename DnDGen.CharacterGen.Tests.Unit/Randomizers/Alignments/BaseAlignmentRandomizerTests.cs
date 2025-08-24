using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Randomizers.Alignments;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Verifiers.Exceptions;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Randomizers.Alignments
{
    [TestFixture]
    public class BaseAlignmentRandomizerTests
    {
        private TestAlignmentRandomizer alignmentRandomizer;
        private Mock<IPercentileSelector> mockPercentileSelector;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<IPercentileSelector>();
            var mockCollectionsSelector = new Mock<ICollectionSelector>();
            alignmentRandomizer = new TestAlignmentRandomizer(mockPercentileSelector.Object, mockCollectionsSelector.Object);

            mockPercentileSelector
                .Setup(p => p.SelectAllFrom(Config.Name, TableNameConstants.Set.Percentile.AlignmentLawfulness))
                .Returns(new[] { "other lawfulness", "lawfulness" });
            mockPercentileSelector.Setup(p => p.SelectAllFrom(Config.Name, TableNameConstants.Set.Percentile.AlignmentGoodness)).Returns(new[] { "other goodness", "goodness" });
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Set.Percentile.AlignmentLawfulness)).Returns("lawfulness");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Set.Percentile.AlignmentGoodness)).Returns("goodness");

            var index = 0;
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<Alignment>>())).Returns((IEnumerable<Alignment> ss) => ss.ElementAt(index++ % ss.Count()));
        }

        [Test]
        public void ReturnLawfulnessFromSelector()
        {
            var alignment = alignmentRandomizer.Randomize();
            Assert.That(alignment.Lawfulness, Is.EqualTo("lawfulness"));
        }

        [Test]
        public void ReturnsGoodnessFromSelector()
        {
            var alignment = alignmentRandomizer.Randomize();
            Assert.That(alignment.Goodness, Is.EqualTo("goodness"));
        }

        [Test]
        public void RandomizeThrowsErrorIfNoLawfulnessPossibleResults()
        {
            mockPercentileSelector.Setup(p => p.SelectAllFrom(Config.Name, TableNameConstants.Set.Percentile.AlignmentLawfulness)).Returns(Enumerable.Empty<string>());
            Assert.That(() => alignmentRandomizer.Randomize(), Throws.InstanceOf<IncompatibleRandomizersException>());
        }

        [Test]
        public void RandomizeThrowsErrorIfNoGoodnessPossibleResults()
        {
            mockPercentileSelector.Setup(p => p.SelectAllFrom(Config.Name, TableNameConstants.Set.Percentile.AlignmentGoodness)).Returns(Enumerable.Empty<string>());
            Assert.That(() => alignmentRandomizer.Randomize(), Throws.InstanceOf<IncompatibleRandomizersException>());
        }

        [Test]
        public void RandomizeLoopsUntilAlignmentWithAllowedGoodnessIsRolled()
        {
            alignmentRandomizer.NotAllowedGoodness = "goodness";

            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, TableNameConstants.Set.Percentile.AlignmentGoodness))
                .Returns("goodness").Returns("other goodness");

            var alignment = alignmentRandomizer.Randomize();
            Assert.That(alignment.Goodness, Is.EqualTo("other goodness"));
        }

        [Test]
        public void RandomizeLoopsUntilAlignmentWithAllowedLawfulnessIsRolled()
        {
            alignmentRandomizer.NotAllowedLawfulness = "lawfulness";

            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, TableNameConstants.Set.Percentile.AlignmentLawfulness))
                .Returns("lawfulness").Returns("other lawfulness");

            var alignment = alignmentRandomizer.Randomize();
            Assert.That(alignment.Lawfulness, Is.EqualTo("other lawfulness"));
        }

        [Test]
        public void RandomizeReturnsDefault()
        {
            alignmentRandomizer.NotAllowedGoodness = "goodness";
            alignmentRandomizer.NotAllowedLawfulness = "lawfulness";

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Set.Percentile.AlignmentLawfulness))
                .Returns("lawfulness");

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Set.Percentile.AlignmentGoodness))
                .Returns("goodness");

            var alignment = alignmentRandomizer.Randomize();
            Assert.That(alignment.Lawfulness, Is.EqualTo("other lawfulness"));
            Assert.That(alignment.Goodness, Is.EqualTo("other goodness"));
        }

        [Test]
        public void GetAllPossibleResults()
        {
            var allAlignments = alignmentRandomizer.GetAllPossibleResults();
            var alignmentStrings = allAlignments.Select(a => a.ToString());

            Assert.That(alignmentStrings, Contains.Item("lawfulness goodness"));
            Assert.That(alignmentStrings, Contains.Item("lawfulness other goodness"));
            Assert.That(alignmentStrings, Contains.Item("other lawfulness goodness"));
            Assert.That(alignmentStrings, Contains.Item("other lawfulness other goodness"));
            Assert.That(alignmentStrings.Count(), Is.EqualTo(4));
        }

        [Test]
        public void GetAllPossibleResultsFiltersOutUnallowedCharacterClasses()
        {
            alignmentRandomizer.NotAllowedLawfulness = "lawfulness";
            alignmentRandomizer.NotAllowedGoodness = "goodness";

            var results = alignmentRandomizer.GetAllPossibleResults();
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.First().ToString(), Is.EqualTo("other lawfulness other goodness"));
        }

        private class TestAlignmentRandomizer : BaseAlignmentRandomizer
        {
            public string NotAllowedLawfulness { get; set; }
            public string NotAllowedGoodness { get; set; }

            public TestAlignmentRandomizer(IPercentileSelector innerSelector, ICollectionSelector collectionsSelector)
                : base(innerSelector, collectionsSelector)
            { }

            protected override bool AlignmentIsAllowed(Alignment alignment)
            {
                return alignment.Lawfulness != NotAllowedLawfulness && alignment.Goodness != NotAllowedGoodness;
            }
        }
    }
}