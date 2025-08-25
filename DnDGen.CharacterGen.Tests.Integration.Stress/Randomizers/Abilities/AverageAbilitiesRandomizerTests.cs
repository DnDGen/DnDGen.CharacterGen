using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Abilities.Randomizers;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Randomizers.Abilities
{
    [TestFixture]
    public class AverageAbilitiesRandomizerTests : StressTests
    {
        private IAbilitiesRandomizer averageAbilitiesRandomizer;

        [SetUp]
        public void Setup()
        {
            averageAbilitiesRandomizer = GetNewInstanceOf<IAbilitiesRandomizer>(AbilitiesRandomizerTypeConstants.Average);
        }

        [Test]
        public void StressAverageAbilities()
        {
            stressor.Stress(GenerateAndAssertAbilities);
        }

        protected void GenerateAndAssertAbilities()
        {
            var stats = averageAbilitiesRandomizer.Randomize();

            Assert.That(stats.Count, Is.EqualTo(6));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Charisma));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Constitution));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Dexterity));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Intelligence));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Strength));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Wisdom));
            Assert.That(stats.Values.Select(s => s.Value), Is.All.InRange(10, 13));
        }
    }
}