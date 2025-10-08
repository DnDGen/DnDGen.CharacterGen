using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Items.Selectors;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Percentiles;
using Moq;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Items.Selectors
{
    [TestFixture]
    public class TreasureLevelSelectorTests
    {
        private ITreasureLevelSelector selector;
        private Mock<IPercentileSelector> mockPercentileSelector;

        private CharacterClass characterClass;
        private Race race;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<IPercentileSelector>();
            selector = new TreasureLevelSelector(mockPercentileSelector.Object);

            characterClass = new();
            race = new();
        }

        [Test]
        public void SelectLevelFrom_PCCharacterClass_ReturnsCharacterClassLevel()
        {
            characterClass.Level = 9266;
            characterClass.IsNPC = false;

            var level = selector.SelectLevelFrom(characterClass, race);
            Assert.That(level, Is.EqualTo(9266));
        }

        [Test]
        public void SelectLevelFrom_NPCCharacterClass_ReturnsCharacterClassLevel()
        {
            characterClass.Level = 9266;
            characterClass.IsNPC = true;

            race.NPCChallengeRatingAdjustment = -9;

            var level = selector.SelectLevelFrom(characterClass, race);
            Assert.That(level, Is.EqualTo(9266 - 9));
        }

        [Test]
        public void SelectLevelFrom_NPCCharacterClass_ReturnsCharacterClassLevel_AtLeast1()
        {
            characterClass.Level = 9266;
            characterClass.IsNPC = true;

            race.NPCChallengeRatingAdjustment = -90210;

            var level = selector.SelectLevelFrom(characterClass, race);
            Assert.That(level, Is.EqualTo(1));
        }

        [Test]
        public void SelectPowerFrom_PCCharacterClass_ReturnsPowerFromPercentileSelector()
        {
            characterClass.Level = 9266;
            characterClass.IsNPC = false;

            var tableName = TableNameConstants.Formattable.Percentile.LevelXPower(9266);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("my power");

            var power = selector.SelectPowerFrom(characterClass, race);
            Assert.That(power, Is.EqualTo("my power"));
        }

        [Test]
        public void SelectPowerFrom_NPCCharacterClass_ReturnsPowerFromPercentileSelector()
        {
            characterClass.Level = 9266;
            characterClass.IsNPC = true;

            race.NPCChallengeRatingAdjustment = -9;

            var tableName = TableNameConstants.Formattable.Percentile.LevelXPower(9266 - 9);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("my power");

            var power = selector.SelectPowerFrom(characterClass, race);
            Assert.That(power, Is.EqualTo("my power"));
        }

        [Test]
        public void SelectPowerFrom_NPCCharacterClass_ReturnsPowerFromPercentileSelector_AtLeast1()
        {
            characterClass.Level = 9266;
            characterClass.IsNPC = true;

            race.NPCChallengeRatingAdjustment = -90210;

            var tableName = TableNameConstants.Formattable.Percentile.LevelXPower(1);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("my power");

            var power = selector.SelectPowerFrom(characterClass, race);
            Assert.That(power, Is.EqualTo("my power"));
        }
    }
}
