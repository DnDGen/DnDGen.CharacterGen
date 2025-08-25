using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Abilities.Randomizers;
using DnDGen.RollGen;
using Moq;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Abilities.Randomizers
{
    [TestFixture]
    public class OnesAsSixesAbilitiesRandomizerTests
    {
        private IAbilitiesRandomizer randomizer;
        private Mock<Dice> mockDice;

        [SetUp]
        public void Setup()
        {
            mockDice = new Mock<Dice>();
            randomizer = new OnesAsSixesAbilitiesRandomizer(mockDice.Object);

            mockDice.SetupSequence(d => d.Roll("3d6t1").AsSum<int>())
                .Returns(6).Returns(18).Returns(10)
                .Returns(17).Returns(12).Returns(8);
        }

        [Test]
        public void Randomize_CallsRollPerStat()
        {
            var stats = randomizer.Randomize();
            mockDice.Verify(d => d.Roll("3d6t1").AsSum<int>(), Times.Exactly(stats.Count));
        }

        [Test]
        public void Randomize_ReturnRandomizedStats()
        {
            var stats = randomizer.Randomize();
            Assert.That(stats, Has.Count.EqualTo(6)
                .And.ContainKey(AbilityConstants.Charisma)
                .And.ContainKey(AbilityConstants.Constitution)
                .And.ContainKey(AbilityConstants.Dexterity)
                .And.ContainKey(AbilityConstants.Intelligence)
                .And.ContainKey(AbilityConstants.Strength)
                .And.ContainKey(AbilityConstants.Wisdom));
            Assert.That(stats[AbilityConstants.Strength].Value, Is.EqualTo(12));
            Assert.That(stats[AbilityConstants.Constitution].Value, Is.EqualTo(18));
            Assert.That(stats[AbilityConstants.Dexterity].Value, Is.EqualTo(10));
            Assert.That(stats[AbilityConstants.Intelligence].Value, Is.EqualTo(17));
            Assert.That(stats[AbilityConstants.Wisdom].Value, Is.EqualTo(8));
            Assert.That(stats[AbilityConstants.Charisma].Value, Is.EqualTo(6));
        }
    }
}