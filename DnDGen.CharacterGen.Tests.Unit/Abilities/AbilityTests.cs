using DnDGen.CharacterGen.Abilities;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Abilities
{
    [TestFixture]
    public class AbilityTests
    {
        private Ability ability;

        [SetUp]
        public void Setup()
        {
            ability = new Ability("ability name");
        }

        [Test]
        public void AbilityInitialized()
        {
            Assert.That(ability.Name, Is.EqualTo("ability name"));
            Assert.That(ability.Value, Is.EqualTo(10));
        }

        [TestCase(1, -5)]
        [TestCase(2, -4)]
        [TestCase(3, -4)]
        [TestCase(4, -3)]
        [TestCase(5, -3)]
        [TestCase(6, -2)]
        [TestCase(7, -2)]
        [TestCase(8, -1)]
        [TestCase(9, -1)]
        [TestCase(10, 0)]
        [TestCase(11, 0)]
        [TestCase(12, 1)]
        [TestCase(13, 1)]
        [TestCase(14, 2)]
        [TestCase(15, 2)]
        [TestCase(16, 3)]
        [TestCase(17, 3)]
        [TestCase(18, 4)]
        [TestCase(19, 4)]
        [TestCase(20, 5)]
        [TestCase(21, 5)]
        [TestCase(22, 6)]
        [TestCase(23, 6)]
        [TestCase(24, 7)]
        [TestCase(25, 7)]
        [TestCase(26, 8)]
        [TestCase(27, 8)]
        [TestCase(28, 9)]
        [TestCase(29, 9)]
        [TestCase(30, 10)]
        [TestCase(31, 10)]
        [TestCase(32, 11)]
        [TestCase(33, 11)]
        [TestCase(34, 12)]
        [TestCase(35, 12)]
        [TestCase(36, 13)]
        [TestCase(37, 13)]
        [TestCase(38, 14)]
        [TestCase(39, 14)]
        [TestCase(40, 15)]
        public void AbilityBonus(int statValue, int bonus)
        {
            ability.Value = statValue;
            Assert.That(ability.Bonus, Is.EqualTo(bonus));
        }
    }
}