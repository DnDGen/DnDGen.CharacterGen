using DnDGen.CharacterGen.Abilities.Randomizers;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Abilities.Randomizers
{
    [TestFixture]
    public class AbilitiesRandomizerTypeConstantsTests
    {
        [TestCase(AbilitiesRandomizerTypeConstants.Average, "Average")]
        [TestCase(AbilitiesRandomizerTypeConstants.BestOfFour, "Best of four")]
        [TestCase(AbilitiesRandomizerTypeConstants.Good, "Good")]
        [TestCase(AbilitiesRandomizerTypeConstants.Heroic, "Heroic")]
        [TestCase(AbilitiesRandomizerTypeConstants.OnesAsSixes, "Ones as sixes")]
        [TestCase(AbilitiesRandomizerTypeConstants.Poor, "Poor")]
        [TestCase(AbilitiesRandomizerTypeConstants.TwoTenSidedDice, "2d10")]
        public void Constant(string constant, string value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }

        [Test]
        public void Default_IsBestOfFour()
        {
            Assert.That(AbilitiesRandomizerTypeConstants.Default, Is.EqualTo(AbilitiesRandomizerTypeConstants.BestOfFour));
        }
    }
}
