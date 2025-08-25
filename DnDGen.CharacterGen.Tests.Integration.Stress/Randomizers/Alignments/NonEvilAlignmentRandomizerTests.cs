using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Alignments.Randomizers;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Randomizers.Alignments
{
    [TestFixture]
    public class NonEvilAlignmentRandomizerTests : StressTests
    {
        [SetUp]
        public void Setup()
        {
            alignmentRandomizer = GetNewInstanceOf<IAlignmentRandomizer>(AlignmentRandomizerTypeConstants.NonEvil);
        }

        [Test]
        public void StressNonEvilAlignment()
        {
            stressor.Stress(AssertAlignment);
        }

        protected void AssertAlignment()
        {
            var alignment = alignmentRandomizer.Randomize();
            Assert.That(alignment.Goodness, Is.EqualTo(AlignmentConstants.Good)
                .Or.EqualTo(AlignmentConstants.Neutral));
            Assert.That(alignment.Lawfulness, Is.EqualTo(AlignmentConstants.Lawful)
                .Or.EqualTo(AlignmentConstants.Neutral)
                .Or.EqualTo(AlignmentConstants.Chaotic));
        }
    }
}