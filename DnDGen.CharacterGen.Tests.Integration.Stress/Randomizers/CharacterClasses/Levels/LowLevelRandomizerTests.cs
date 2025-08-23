using DnDGen.CharacterGen.Randomizers.CharacterClasses.Levels;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Randomizers.CharacterClasses.Levels
{
    [TestFixture]
    public class LowLevelRandomizerTests : StressTests
    {
        private ILevelRandomizer lowLevelRandomizer;

        [SetUp]
        public void Setup()
        {
            lowLevelRandomizer = GetNewInstanceOf<ILevelRandomizer>(LevelRandomizerTypeConstants.Low);
        }

        [Test]
        public void StressLowLevel()
        {
            stressor.Stress(AssertLevel);
        }

        protected void AssertLevel()
        {
            var level = lowLevelRandomizer.Randomize();
            Assert.That(level, Is.InRange(1, 5));
        }
    }
}