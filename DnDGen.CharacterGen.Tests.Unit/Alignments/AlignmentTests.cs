using DnDGen.CharacterGen.Alignments;
using NUnit.Framework;
using System;

namespace DnDGen.CharacterGen.Tests.Unit.Alignments
{
    [TestFixture]
    public class AlignmentTests
    {
        private Alignment alignment;

        [SetUp]
        public void Setup()
        {
            alignment = new Alignment();
        }

        [Test]
        public void CanBuildAlignmentByString()
        {
            alignment = new Alignment("lawfulness goodness");
            Assert.That(alignment.Goodness, Is.EqualTo("goodness"));
            Assert.That(alignment.Lawfulness, Is.EqualTo("lawfulness"));
        }

        [TestCase(AlignmentConstants.ChaoticEvil, AlignmentConstants.Chaotic, AlignmentConstants.Evil)]
        [TestCase(AlignmentConstants.ChaoticGood, AlignmentConstants.Chaotic, AlignmentConstants.Good)]
        [TestCase(AlignmentConstants.ChaoticNeutral, AlignmentConstants.Chaotic, AlignmentConstants.Neutral)]
        [TestCase(AlignmentConstants.LawfulEvil, AlignmentConstants.Lawful, AlignmentConstants.Evil)]
        [TestCase(AlignmentConstants.LawfulGood, AlignmentConstants.Lawful, AlignmentConstants.Good)]
        [TestCase(AlignmentConstants.LawfulNeutral, AlignmentConstants.Lawful, AlignmentConstants.Neutral)]
        [TestCase(AlignmentConstants.NeutralEvil, AlignmentConstants.Neutral, AlignmentConstants.Evil)]
        [TestCase(AlignmentConstants.NeutralGood, AlignmentConstants.Neutral, AlignmentConstants.Good)]
        [TestCase(AlignmentConstants.TrueNeutral, AlignmentConstants.Neutral, AlignmentConstants.Neutral)]
        public void ParseAlignment(string fullAlignment, string lawfulness, string goodness)
        {
            alignment = new Alignment(fullAlignment);
            Assert.That(alignment.Full, Is.EqualTo(fullAlignment));
            Assert.That(alignment.Lawfulness, Is.EqualTo(lawfulness));
            Assert.That(alignment.Goodness, Is.EqualTo(goodness));
        }

        [TestCase(AlignmentConstants.ChaoticEvil, AlignmentConstants.Chaotic, AlignmentConstants.Evil)]
        [TestCase(AlignmentConstants.ChaoticGood, AlignmentConstants.Chaotic, AlignmentConstants.Good)]
        [TestCase(AlignmentConstants.ChaoticNeutral, AlignmentConstants.Chaotic, AlignmentConstants.Neutral)]
        [TestCase(AlignmentConstants.LawfulEvil, AlignmentConstants.Lawful, AlignmentConstants.Evil)]
        [TestCase(AlignmentConstants.LawfulGood, AlignmentConstants.Lawful, AlignmentConstants.Good)]
        [TestCase(AlignmentConstants.LawfulNeutral, AlignmentConstants.Lawful, AlignmentConstants.Neutral)]
        [TestCase(AlignmentConstants.NeutralEvil, AlignmentConstants.Neutral, AlignmentConstants.Evil)]
        [TestCase(AlignmentConstants.NeutralGood, AlignmentConstants.Neutral, AlignmentConstants.Good)]
        [TestCase(AlignmentConstants.TrueNeutral, AlignmentConstants.Neutral, AlignmentConstants.Neutral)]
        public void FullAlignment(string fullAlignment, string lawfulness, string goodness)
        {
            alignment.Goodness = goodness;
            alignment.Lawfulness = lawfulness;

            Assert.That(alignment.Full, Is.EqualTo(fullAlignment));
        }

        [Test]
        public void TooShortAlignmentDefaultsToEmpty()
        {
            alignment = new Alignment("good");
            Assert.That(alignment.Goodness, Is.Empty);
            Assert.That(alignment.Lawfulness, Is.Empty);
        }

        [Test]
        public void TooLongAlignmentDefaultsToEmpty()
        {
            alignment = new Alignment("much too good");
            Assert.That(alignment.Goodness, Is.Empty);
            Assert.That(alignment.Lawfulness, Is.Empty);
        }

        [Test]
        public void AlignmentInitialized()
        {
            Assert.That(alignment.Goodness, Is.Empty);
            Assert.That(alignment.Lawfulness, Is.Empty);
        }

        [Test]
        public void FullIsLawfulnessPlusGoodness()
        {
            alignment.Lawfulness = "lawfulness";
            alignment.Goodness = "goodness";
            Assert.That(alignment.Full, Is.EqualTo("lawfulness goodness"));
        }

        [Test]
        public void ToStringIsFull()
        {
            alignment.Lawfulness = Guid.NewGuid().ToString();
            alignment.Goodness = Guid.NewGuid().ToString();

            Assert.That(alignment.ToString(), Is.EqualTo(alignment.Full));
        }

        [Test]
        public void ConvertingToStringUsesFull()
        {
            alignment.Lawfulness = Guid.NewGuid().ToString();
            alignment.Goodness = Guid.NewGuid().ToString();

            var alignmentString = Convert.ToString(alignment);
            Assert.That(alignmentString, Is.EqualTo(alignment.Full));
        }

        [Test]
        public void AlignmentIsNotEqualIfOtherItemNotAlignment()
        {
            alignment.Lawfulness = "lawfulness";
            var otherAlignment = new object();

            Assert.That(alignment, Is.Not.EqualTo(otherAlignment));
        }

        [Test]
        public void AlignmentIsNotEqualIfLawfulnessDiffers()
        {
            alignment.Lawfulness = "lawfulness";
            alignment.Goodness = "goodness";

            var otherAlignment = new Alignment();
            otherAlignment.Lawfulness = "other lawfulness";
            otherAlignment.Goodness = "goodness";

            Assert.That(alignment, Is.Not.EqualTo(otherAlignment));
        }

        [Test]
        public void AlignmentIsNotEqualIfGoodnessDiffers()
        {
            alignment.Lawfulness = "lawfulness";
            alignment.Goodness = "goodness";

            var otherAlignment = new Alignment();
            otherAlignment.Lawfulness = "lawfulness";
            otherAlignment.Goodness = "other goodness";

            Assert.That(alignment, Is.Not.EqualTo(otherAlignment));
        }

        [Test]
        public void AlignmentIsEqualIfGoodnessesAndLawfulnessesMatch()
        {
            alignment.Lawfulness = "lawfulness";
            alignment.Goodness = "goodness";

            var otherAlignment = new Alignment();
            otherAlignment.Lawfulness = "lawfulness";
            otherAlignment.Goodness = "goodness";

            Assert.That(alignment, Is.EqualTo(otherAlignment));
        }

        [Test]
        public void HashCodeIsHashOfFull()
        {
            alignment.Lawfulness = "lawfulness";
            alignment.Goodness = "goodness";

            var alignmentHash = alignment.GetHashCode();
            var alignmentToStringHash = alignment.ToString().GetHashCode();

            Assert.That(alignmentHash, Is.EqualTo(alignmentToStringHash));
        }
    }
}