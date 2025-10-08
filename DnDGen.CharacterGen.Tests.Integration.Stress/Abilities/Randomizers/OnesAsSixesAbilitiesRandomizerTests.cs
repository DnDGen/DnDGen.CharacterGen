using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Abilities.Randomizers;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Abilities.Randomizers
{
    [TestFixture]
    public class OnesAsSixesAbilitiesRandomizerTests : StressTests
    {
        private IAbilitiesRandomizer onesAsSixesAbilitiesRandomizer;

        [SetUp]
        public void Setup()
        {
            onesAsSixesAbilitiesRandomizer = GetNewInstanceOf<IAbilitiesRandomizer>(AbilitiesRandomizerTypeConstants.OnesAsSixes);
        }

        [Test]
        public void StressOnesAsSixesAbilities()
        {
            stressor.Stress(AssertAbilities);
        }

        protected void AssertAbilities()
        {
            var stats = onesAsSixesAbilitiesRandomizer.Randomize();

            Assert.That(stats.Count, Is.EqualTo(6));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Charisma));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Constitution));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Dexterity));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Intelligence));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Strength));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Wisdom));
            Assert.That(stats.Values.Select(s => s.Value), Is.All.InRange(6, 18));
        }
    }
}