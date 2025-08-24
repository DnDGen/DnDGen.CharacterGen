using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Abilities.Randomizers;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Abilities.Randomizers
{
    [TestFixture]
    public class BaseAbilitiesRandomizerTests
    {
        private TestAbilityRandomizer randomizer;

        [SetUp]
        public void Setup()
        {
            randomizer = new TestAbilityRandomizer();
        }

        [Test]
        public void AbilitiesContainAllAbilities()
        {
            var abilities = randomizer.Randomize();

            Assert.That(abilities.Keys, Contains.Item(AbilityConstants.Strength));
            Assert.That(abilities.Keys, Contains.Item(AbilityConstants.Constitution));
            Assert.That(abilities.Keys, Contains.Item(AbilityConstants.Dexterity));
            Assert.That(abilities.Keys, Contains.Item(AbilityConstants.Intelligence));
            Assert.That(abilities.Keys, Contains.Item(AbilityConstants.Wisdom));
            Assert.That(abilities.Keys, Contains.Item(AbilityConstants.Charisma));
            Assert.That(abilities.Count, Is.EqualTo(6));
        }

        [TestCase(-2)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void AbilitiesAreRolledIndividually(int rollChange)
        {
            randomizer.Roll = 9266;
            randomizer.RollChange = rollChange;

            var abilities = randomizer.Randomize();

            Assert.That(abilities[AbilityConstants.Strength].Value, Is.EqualTo(9266 + rollChange * 4));
            Assert.That(abilities[AbilityConstants.Constitution].Value, Is.EqualTo(9266 + rollChange));
            Assert.That(abilities[AbilityConstants.Dexterity].Value, Is.EqualTo(9266 + rollChange * 2));
            Assert.That(abilities[AbilityConstants.Intelligence].Value, Is.EqualTo(9266 + rollChange * 3));
            Assert.That(abilities[AbilityConstants.Wisdom].Value, Is.EqualTo(9266 + rollChange * 5));
            Assert.That(abilities[AbilityConstants.Charisma].Value, Is.EqualTo(9266 + 0));
        }

        private class TestAbilityRandomizer : BaseAbilitiesRandomizer
        {
            public int Roll { get; set; }
            public int RollChange { get; set; }

            private int randomizeCount;

            public TestAbilityRandomizer()
            {
                randomizeCount = 0;
            }

            protected override int RollAbility() => Roll + RollChange * randomizeCount++;
        }
    }
}