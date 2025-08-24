using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Abilities.Randomizers;
using DnDGen.CharacterGen.Generators.Randomizers.Abilities;
using DnDGen.CharacterGen.Randomizers.Abilities;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Abilities.Randomizers
{
    [TestFixture]
    public class SetAbilitiesRandomizerTests
    {
        private ISetAbilitiesRandomizer randomizer;

        [SetUp]
        public void Setup()
        {
            randomizer = new SetAbilitiesRandomizer();
        }

        [Test]
        public void SetAbilitiesRandomizerIsAnAbilitiesRandomizer()
        {
            Assert.That(randomizer, Is.InstanceOf<IAbilitiesRandomizer>());
        }

        [Test]
        public void SetAbilitiesContainAllAbilities()
        {
            var stats = randomizer.Randomize();

            Assert.That(stats.Count, Is.EqualTo(6));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Charisma));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Constitution));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Dexterity));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Intelligence));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Strength));
            Assert.That(stats.Keys, Contains.Item(AbilityConstants.Wisdom));
        }

        [Test]
        public void AbilitiesAreNotNull()
        {
            var stats = randomizer.Randomize();

            foreach (var stat in stats)
                Assert.That(stat, Is.Not.Null);
        }

        [Test]
        public void AbilitiesDefaultTo10()
        {
            var stats = randomizer.Randomize();

            foreach (var stat in stats.Values)
                Assert.That(stat.Value, Is.EqualTo(10));
        }

        [Test]
        public void AllowAdjustmentsByDefault()
        {
            Assert.That(randomizer.AllowAdjustments, Is.True);
        }

        [Test]
        public void ReturnSetAbilities()
        {
            randomizer.SetStrength = 9266;
            randomizer.SetCharisma = 90210;
            randomizer.SetConstitution = 42;
            randomizer.SetDexterity = -3;
            randomizer.SetIntelligence = int.MaxValue;
            randomizer.SetWisdom = int.MinValue;

            var stats = randomizer.Randomize();
            Assert.That(stats[AbilityConstants.Charisma].Value, Is.EqualTo(90210));
            Assert.That(stats[AbilityConstants.Constitution].Value, Is.EqualTo(42));
            Assert.That(stats[AbilityConstants.Dexterity].Value, Is.EqualTo(-3));
            Assert.That(stats[AbilityConstants.Intelligence].Value, Is.EqualTo(int.MaxValue));
            Assert.That(stats[AbilityConstants.Strength].Value, Is.EqualTo(9266));
            Assert.That(stats[AbilityConstants.Wisdom].Value, Is.EqualTo(int.MinValue));
        }
    }
}