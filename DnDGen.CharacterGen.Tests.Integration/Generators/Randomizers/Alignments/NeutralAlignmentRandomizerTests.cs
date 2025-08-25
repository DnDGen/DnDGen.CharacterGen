using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Alignments.Randomizers;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Generators.Randomizers.Alignments
{
    [TestFixture]
    public class NeutralAlignmentRandomizerTests : IntegrationTests
    {
        private IAlignmentRandomizer alignmentRandomizer;
        private int attempts;

        [SetUp]
        public void Setup()
        {
            alignmentRandomizer = GetNewInstanceOf<IAlignmentRandomizer>(AlignmentRandomizerTypeConstants.Neutral);
            attempts = 100;
        }

        [Test]
        public void NeutralGoodnessHappens()
        {
            var alignment = new Alignment();

            while (attempts-- > 0 && !(alignment.Goodness == AlignmentConstants.Neutral && alignment.Lawfulness != AlignmentConstants.Neutral))
            {
                alignment = alignmentRandomizer.Randomize();
            }

            Assert.That(alignment.Lawfulness, Is.Not.EqualTo(AlignmentConstants.Neutral));
            Assert.That(alignment.Goodness, Is.EqualTo(AlignmentConstants.Neutral));
        }

        [Test]
        public void NeutralLawfulnessHappens()
        {
            var alignment = new Alignment();

            while (attempts-- > 0 && !(alignment.Goodness != AlignmentConstants.Neutral && alignment.Lawfulness == AlignmentConstants.Neutral))
            {
                alignment = alignmentRandomizer.Randomize();
            }

            Assert.That(alignment.Lawfulness, Is.EqualTo(AlignmentConstants.Neutral));
            Assert.That(alignment.Goodness, Is.Not.EqualTo(AlignmentConstants.Neutral));
        }

        [Test]
        public void TrueNeutralHappens()
        {
            var alignment = new Alignment();

            while (attempts-- > 0 && !(alignment.Goodness == AlignmentConstants.Neutral && alignment.Lawfulness == AlignmentConstants.Neutral))
            {
                alignment = alignmentRandomizer.Randomize();
            }

            Assert.That(alignment.Lawfulness, Is.EqualTo(AlignmentConstants.Neutral));
            Assert.That(alignment.Goodness, Is.EqualTo(AlignmentConstants.Neutral));
        }
    }
}