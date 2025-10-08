using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Alignments.Randomizers;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Alignments.Randomizers
{
    [TestFixture]
    public class NonNeutralAlignmentRandomizerTests : StressTests
    {
        [SetUp]
        public void Setup()
        {
            alignmentRandomizer = GetNewInstanceOf<IAlignmentRandomizer>(AlignmentRandomizerTypeConstants.NonNeutral);
        }

        [Test]
        public void StressNonNeutralAlignment()
        {
            stressor.Stress(AssertAlignment);
        }

        protected void AssertAlignment()
        {
            var alignment = alignmentRandomizer.Randomize();
            Assert.That(alignment.Goodness, Is.EqualTo(AlignmentConstants.Good)
                .Or.EqualTo(AlignmentConstants.Evil));
            Assert.That(alignment.Lawfulness, Is.EqualTo(AlignmentConstants.Lawful)
                .Or.EqualTo(AlignmentConstants.Chaotic));
        }
    }
}