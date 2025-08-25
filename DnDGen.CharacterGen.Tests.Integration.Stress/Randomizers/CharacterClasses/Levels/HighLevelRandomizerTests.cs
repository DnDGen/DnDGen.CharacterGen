using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Randomizers.CharacterClasses.Levels
{
    [TestFixture]
    public class HighLevelRandomizerTests : StressTests
    {
        private ILevelRandomizer highLevelRandomizer;

        [SetUp]
        public void Setup()
        {
            highLevelRandomizer = GetNewInstanceOf<ILevelRandomizer>(LevelRandomizerTypeConstants.High);
        }

        [Test]
        public void StressHighLevel()
        {
            stressor.Stress(AssertLevel);
        }

        protected void AssertLevel()
        {
            var level = highLevelRandomizer.Randomize();
            Assert.That(level, Is.InRange(11, 15));
        }
    }
}