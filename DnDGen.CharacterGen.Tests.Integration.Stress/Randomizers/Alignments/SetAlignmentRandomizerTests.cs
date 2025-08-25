using DnDGen.CharacterGen.Alignments.Randomizers;
using NUnit.Framework;
using System;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Randomizers.Alignments
{
    [TestFixture]
    public class SetAlignmentRandomizerTests : StressTests
    {
        private ISetAlignmentRandomizer setAlignmentRandomizer;

        [SetUp]
        public void Setup()
        {
            setAlignmentRandomizer = GetNewInstanceOf<ISetAlignmentRandomizer>();
        }

        [Test]
        public void StressSetAlignment()
        {
            stressor.Stress(AssertAlignment);
        }

        protected void AssertAlignment()
        {
            setAlignmentRandomizer.SetAlignment.Goodness = Guid.NewGuid().ToString();
            setAlignmentRandomizer.SetAlignment.Lawfulness = Guid.NewGuid().ToString();

            var alignment = setAlignmentRandomizer.Randomize();
            Assert.That(alignment, Is.EqualTo(setAlignmentRandomizer.SetAlignment));
            Assert.That(alignment.Full, Is.EqualTo(setAlignmentRandomizer.SetAlignment.Full));
        }
    }
}