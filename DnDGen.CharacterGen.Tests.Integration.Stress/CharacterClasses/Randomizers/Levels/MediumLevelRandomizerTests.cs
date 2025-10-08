using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.CharacterClasses.Randomizers.Levels
{
    [TestFixture]
    public class MediumLevelRandomizerTests : StressTests
    {
        private ILevelRandomizer mediumLevelRandomizer;

        [SetUp]
        public void Setup()
        {
            mediumLevelRandomizer = GetNewInstanceOf<ILevelRandomizer>(LevelRandomizerTypeConstants.Medium);
        }

        [Test]
        public void StressMediumLevel()
        {
            stressor.Stress(AssertLevel);
        }

        protected void AssertLevel()
        {
            var level = mediumLevelRandomizer.Randomize();
            Assert.That(level, Is.InRange(6, 10));
        }
    }
}