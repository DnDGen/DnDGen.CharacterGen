using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.CharacterClasses.Randomizers.Levels
{
    [TestFixture]
    public class AnyLevelRandomizerTests : StressTests
    {
        private ILevelRandomizer anyLevelRandomizer;

        [SetUp]
        public void Setup()
        {
            anyLevelRandomizer = GetNewInstanceOf<ILevelRandomizer>(LevelRandomizerTypeConstants.Any);
        }

        [Test]
        public void StressAnyLevel()
        {
            stressor.Stress(AssertLevel);
        }

        protected void AssertLevel()
        {
            var level = anyLevelRandomizer.Randomize();
            Assert.That(level, Is.InRange(1, 20));
        }
    }
}