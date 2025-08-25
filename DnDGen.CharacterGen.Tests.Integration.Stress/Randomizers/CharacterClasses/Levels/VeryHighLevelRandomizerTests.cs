using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Randomizers.CharacterClasses.Levels
{
    [TestFixture]
    public class VeryHighLevelRandomizerTests : StressTests
    {
        private ILevelRandomizer veryHighLevelRandomizer;

        [SetUp]
        public void Setup()
        {
            veryHighLevelRandomizer = GetNewInstanceOf<ILevelRandomizer>(LevelRandomizerTypeConstants.VeryHigh);
        }

        [Test]
        public void StressVeryHighLevel()
        {
            stressor.Stress(AssertLevel);
        }

        protected void AssertLevel()
        {
            var level = veryHighLevelRandomizer.Randomize();
            Assert.That(level, Is.InRange(16, 20));
        }
    }
}