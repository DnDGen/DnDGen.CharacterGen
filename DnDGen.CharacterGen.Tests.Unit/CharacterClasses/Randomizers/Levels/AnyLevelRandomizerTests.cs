using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using DnDGen.RollGen;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.CharacterClasses.Randomizers.Levels
{
    [TestFixture]
    public class AnyLevelRandomizerTests
    {
        private ILevelRandomizer randomizer;
        private Mock<Dice> mockDice;

        [SetUp]
        public void Setup()
        {
            mockDice = new Mock<Dice>();
            randomizer = new AnyLevelRandomizer(mockDice.Object);
        }

        [Test]
        public void RandomizeReturnD20Result()
        {
            mockDice.Setup(d => d.Roll(1).d(20).AsSum<int>()).Returns(9266);
            var level = randomizer.Randomize();
            Assert.That(level, Is.EqualTo(9266));
        }

        [Test]
        public void GetAllPossibleResultsReturnsLevelsOneThroughTwenty()
        {
            var levels = randomizer.GetAllPossibleResults();

            for (var level = 1; level <= 20; level++)
                Assert.That(levels, Contains.Item(level));

            Assert.That(levels.Count(), Is.EqualTo(20));
        }
    }
}