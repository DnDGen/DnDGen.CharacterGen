using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using NUnit.Framework;
using System;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Randomizers.CharacterClasses.Levels
{
    [TestFixture]
    public class SetLevelRandomizerTests : StressTests
    {
        private ISetLevelRandomizer setLevelRandomizer;
        private Random random;

        [SetUp]
        public void Setup()
        {
            setLevelRandomizer = GetNewInstanceOf<ISetLevelRandomizer>();
            random = GetNewInstanceOf<Random>();
        }

        [Test]
        public void StressSetLevel()
        {
            stressor.Stress(AssertLevel);
        }

        protected void AssertLevel()
        {
            setLevelRandomizer.SetLevel = random.Next();
            var level = setLevelRandomizer.Randomize();
            Assert.That(level, Is.EqualTo(setLevelRandomizer.SetLevel));
        }
    }
}