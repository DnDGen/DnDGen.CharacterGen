using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.CharacterClasses.Randomizers.Levels
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

        [Test]
        public void DefaultLevelRandomizerIsAny()
        {
            Assert.That(LevelRandomizerTypeConstants.Default, Is.EqualTo(LevelRandomizerTypeConstants.Any));
        }
    }
}
