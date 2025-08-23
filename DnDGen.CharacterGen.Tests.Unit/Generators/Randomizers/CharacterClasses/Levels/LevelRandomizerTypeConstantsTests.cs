using DnDGen.CharacterGen.Randomizers.CharacterClasses.Levels;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Generators.Randomizers.CharacterClasses.Levels
{
    [TestFixture]
    public class LevelRandomizerTypeConstantsTests
    {
        [TestCase(LevelRandomizerTypeConstants.Any, "Any")]
        [TestCase(LevelRandomizerTypeConstants.High, "High")]
        [TestCase(LevelRandomizerTypeConstants.Low, "Low")]
        [TestCase(LevelRandomizerTypeConstants.Medium, "Medium")]
        [TestCase(LevelRandomizerTypeConstants.VeryHigh, "Very high")]
        public void Constant(string constant, string value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }
    }
}
