using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Alignments.Randomizers;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Alignments.Randomizers
{
    [TestFixture]
    public class LawfulAlignmentRandomizerTests : StressTests
    {
        [SetUp]
        public void Setup()
        {
            alignmentRandomizer = GetNewInstanceOf<IAlignmentRandomizer>(AlignmentRandomizerTypeConstants.Lawful);
        }

        [Test]
        public void StressLawfulAlignment()
        {
            stressor.Stress(AssertAlignment);
        }

        protected void AssertAlignment()
        {
            var alignment = alignmentRandomizer.Randomize();
            Assert.That(alignment.Goodness, Is.EqualTo(AlignmentConstants.Good)
                .Or.EqualTo(AlignmentConstants.Neutral)
                .Or.EqualTo(AlignmentConstants.Evil));
            Assert.That(alignment.Lawfulness, Is.EqualTo(AlignmentConstants.Lawful));
        }
    }
}