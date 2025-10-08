using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Abilities.Randomizers;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Abilities.Randomizers
{
    [TestFixture]
    public class PoorAbilitiesRandomizerTests : StressTests
    {
        private IAbilitiesRandomizer poorAbilitiesRandomizer;

        [SetUp]
        public void Setup()
        {
            poorAbilitiesRandomizer = GetNewInstanceOf<IAbilitiesRandomizer>(AbilitiesRandomizerTypeConstants.Poor);
        }

        [Test]
        public void StressPoorAbilities()
        {
            stressor.Stress(AssertAbilities);
        }

        protected void AssertAbilities()
        {
            var stats = poorAbilitiesRandomizer.Randomize();

            Assert.That(stats.Count, Is.EqualTo(6));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Charisma));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Constitution));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Dexterity));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Intelligence));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Strength));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Wisdom));
            Assert.That(stats.Values.Select(s => s.Value), Is.All.InRange(3, 9));
        }
    }
}