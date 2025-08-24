using DnDGen.CharacterGen.Abilities;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Abilities
{
    [TestFixture]
    public class StatConstantsTests
    {
        [TestCase(AbilityConstants.Strength, "Strength")]
        [TestCase(AbilityConstants.Charisma, "Charisma")]
        [TestCase(AbilityConstants.Constitution, "Constitution")]
        [TestCase(AbilityConstants.Dexterity, "Dexterity")]
        [TestCase(AbilityConstants.Intelligence, "Intelligence")]
        [TestCase(AbilityConstants.Wisdom, "Wisdom")]
        public void AbilityConstant(string constant, string value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }
    }
}