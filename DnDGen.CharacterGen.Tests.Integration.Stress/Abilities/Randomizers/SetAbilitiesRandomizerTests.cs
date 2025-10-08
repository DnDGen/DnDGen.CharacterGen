using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Abilities.Randomizers;
using NUnit.Framework;
using System;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Abilities.Randomizers
{
    [TestFixture]
    public class SetAbilitiesRandomizerTests : StressTests
    {
        private ISetAbilitiesRandomizer setAbilitiesRandomizer;
        private Random random;

        [SetUp]
        public void Setup()
        {
            setAbilitiesRandomizer = GetNewInstanceOf<ISetAbilitiesRandomizer>();
            random = GetNewInstanceOf<Random>();
        }

        [Test]
        public void StressSetAbilities()
        {
            stressor.Stress(GenerateAndAssertAbilities);
        }

        protected void GenerateAndAssertAbilities()
        {
            setAbilitiesRandomizer.SetCharisma = random.Next();
            setAbilitiesRandomizer.SetConstitution = random.Next();
            setAbilitiesRandomizer.SetDexterity = random.Next();
            setAbilitiesRandomizer.SetIntelligence = random.Next();
            setAbilitiesRandomizer.SetStrength = random.Next();
            setAbilitiesRandomizer.SetWisdom = random.Next();

            var stats = setAbilitiesRandomizer.Randomize();

            Assert.That(stats.Count, Is.EqualTo(6));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Charisma));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Constitution));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Dexterity));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Intelligence));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Strength));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Wisdom));
            Assert.That(stats[AbilityConstants.Charisma].Value, Is.EqualTo(setAbilitiesRandomizer.SetCharisma));
            Assert.That(stats[AbilityConstants.Constitution].Value, Is.EqualTo(setAbilitiesRandomizer.SetConstitution));
            Assert.That(stats[AbilityConstants.Dexterity].Value, Is.EqualTo(setAbilitiesRandomizer.SetDexterity));
            Assert.That(stats[AbilityConstants.Intelligence].Value, Is.EqualTo(setAbilitiesRandomizer.SetIntelligence));
            Assert.That(stats[AbilityConstants.Strength].Value, Is.EqualTo(setAbilitiesRandomizer.SetStrength));
            Assert.That(stats[AbilityConstants.Wisdom].Value, Is.EqualTo(setAbilitiesRandomizer.SetWisdom));
        }
    }
}