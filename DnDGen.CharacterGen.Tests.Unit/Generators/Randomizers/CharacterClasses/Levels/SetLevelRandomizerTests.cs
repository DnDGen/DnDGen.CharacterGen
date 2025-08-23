using DnDGen.CharacterGen.Generators.Randomizers.CharacterClasses.Levels;
using DnDGen.CharacterGen.Randomizers.CharacterClasses.Levels;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Generators.Randomizers.CharacterClasses.Levels
{
    [TestFixture]
    public class SetLevelRandomizerTests
    {
        private ISetLevelRandomizer randomizer;

        [SetUp]
        public void Setup()
        {
            randomizer = new SetLevelRandomizer();
        }

        [Test]
        public void SetLevelRandomizerIsALevelRandomizer()
        {
            Assert.That(randomizer, Is.InstanceOf<ILevelRandomizer>());
        }

        [Test]
        public void ReturnSetLevel()
        {
            randomizer.SetLevel = 9266;
            var level = randomizer.Randomize();
            Assert.That(level, Is.EqualTo(9266));
        }

        [Test]
        public void ReturnJustSetLevel()
        {
            randomizer.SetLevel = 9266;

            var levels = randomizer.GetAllPossibleResults();
            Assert.That(levels, Contains.Item(9266));
            Assert.That(levels.Count(), Is.EqualTo(1));
        }
    }
}