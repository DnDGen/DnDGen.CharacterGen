using DnDGen.CharacterGen.Abilities.Randomizers;
using DnDGen.RollGen;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Abilities.Randomizers
{
    [TestFixture]
    public class BestOfFourAbilitiesRandomizerTests
    {
        private IAbilitiesRandomizer randomizer;
        private Mock<Dice> mockDice;

        [SetUp]
        public void Setup()
        {
            mockDice = new Mock<Dice>();
            randomizer = new BestOfFourAbilitiesRandomizer(mockDice.Object);

            mockDice.Setup(d => d.Roll("4d6k3").AsSum<int>()).Returns(3);
        }

        [Test]
        public void BestOfFourAbilitiesCalls4d6k3OncePerAbility()
        {
            var stats = randomizer.Randomize();
            mockDice.Verify(d => d.Roll("4d6k3").AsSum<int>(), Times.Exactly(stats.Count));
        }

        [Test]
        public void BestOfFourIgnoresLowestRollPerAbility()
        {
            mockDice.SetupSequence(d => d.Roll("4d6k3").AsSum<int>()).Returns(9);

            var stats = randomizer.Randomize();
            var stat = stats.Values.First();
            Assert.That(stat.Value, Is.EqualTo(9));
        }
    }
}