using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using DnDGen.RollGen;
using Moq;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.CharacterClasses.Randomizers.Levels
{
    [TestFixture]
    public class LowLevelRandomizerTests
    {
        [Test]
        public void Add0ToRoll()
        {
            var mockDice = new Mock<Dice>();
            mockDice.Setup(d => d.Roll(1).d(5).AsSum<int>()).Returns(9266);
            var randomizer = new LowLevelRandomizer(mockDice.Object);

            var level = randomizer.Randomize();
            Assert.That(level, Is.EqualTo(9266));
        }
    }
}