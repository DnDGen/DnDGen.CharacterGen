using DnDGen.CharacterGen.CharacterClasses;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Classes
{
    [TestFixture]
    public class CharacterClassTests
    {
        private CharacterClass characterClass;

        [SetUp]
        public void Setup()
        {
            characterClass = new CharacterClass();
        }

        [Test]
        public void CharacterClassInitialized()
        {
            Assert.That(characterClass.Name, Is.Empty);
            Assert.That(characterClass.Level, Is.EqualTo(0));
            Assert.That(characterClass.EffectiveLevel, Is.EqualTo(0));
            Assert.That(characterClass.IsNPC, Is.False);
            Assert.That(characterClass.SpecialistFields, Is.Empty);
            Assert.That(characterClass.ProhibitedFields, Is.Empty);
        }

        [Test]
        public void EffectiveLevelIsLevel()
        {
            characterClass.Level = 9266;
            Assert.That(characterClass.EffectiveLevel, Is.EqualTo(9266));
        }

        [Test]
        public void EffectiveLevelIsLevelAndAdjustment()
        {
            characterClass.Level = 9266;
            characterClass.LevelAdjustment = 90210;
            Assert.That(characterClass.EffectiveLevel, Is.EqualTo(9266 + 90210));
        }

        [Test]
        public void EffectiveLevelOfNPCIsHalfOfLevel()
        {
            characterClass.Level = 9266;
            characterClass.IsNPC = true;
            Assert.That(characterClass.EffectiveLevel, Is.EqualTo(4633));
        }

        [Test]
        public void EffectiveLevelOfNPCIsHalfOfLevelAndAdjustment()
        {
            characterClass.Level = 9266;
            characterClass.LevelAdjustment = 90210;
            characterClass.IsNPC = true;
            Assert.That(characterClass.EffectiveLevel, Is.EqualTo(9266 / 2 + 90210 / 2));
        }

        [TestCase(-10, 0, -10)]
        [TestCase(-9, 0, -9)]
        [TestCase(-8, 0, -8)]
        [TestCase(-7, 0, -7)]
        [TestCase(-6, 0, -6)]
        [TestCase(-5, 0, -5)]
        [TestCase(-4, 0, -4)]
        [TestCase(-3, 0, -3)]
        [TestCase(-2, 0, -2)]
        [TestCase(-1, 0, -1)]
        [TestCase(0, 0, 0)]
        [TestCase(1, 0, 1)]
        [TestCase(2, 0, 2)]
        [TestCase(3, 0, 3)]
        [TestCase(4, 0, 4)]
        [TestCase(5, 0, 5)]
        [TestCase(6, 0, 6)]
        [TestCase(7, 0, 7)]
        [TestCase(8, 0, 8)]
        [TestCase(9, 0, 9)]
        [TestCase(10, 0, 10)]
        [TestCase(11, 0, 11)]
        [TestCase(12, 0, 12)]
        [TestCase(13, 0, 13)]
        [TestCase(14, 0, 14)]
        [TestCase(15, 0, 15)]
        [TestCase(16, 0, 16)]
        [TestCase(17, 0, 17)]
        [TestCase(18, 0, 18)]
        [TestCase(19, 0, 19)]
        [TestCase(20, 0, 20)]
        [TestCase(1, 1, 2)]
        [TestCase(2, 1, 3)]
        [TestCase(3, 1, 4)]
        [TestCase(4, 1, 5)]
        [TestCase(5, 1, 6)]
        [TestCase(6, 1, 7)]
        [TestCase(7, 1, 8)]
        [TestCase(8, 1, 9)]
        [TestCase(9, 1, 10)]
        [TestCase(10, 1, 11)]
        [TestCase(11, 1, 12)]
        [TestCase(12, 1, 13)]
        [TestCase(13, 1, 14)]
        [TestCase(14, 1, 15)]
        [TestCase(15, 1, 16)]
        [TestCase(16, 1, 17)]
        [TestCase(17, 1, 18)]
        [TestCase(18, 1, 19)]
        [TestCase(19, 1, 20)]
        [TestCase(20, 1, 21)]
        [TestCase(20, 2, 22)]
        [TestCase(20, 3, 23)]
        [TestCase(20, 4, 24)]
        [TestCase(20, 5, 25)]
        [TestCase(20, 6, 26)]
        [TestCase(20, 7, 27)]
        [TestCase(20, 8, 28)]
        [TestCase(20, 9, 29)]
        [TestCase(20, 10, 30)]
        [TestCase(20, 11, 31)]
        public void EffectiveLevelOfCharacterWithAdjustment(int level, int levelAdjustment, int effectiveLevel)
        {
            characterClass.Level = level;
            characterClass.LevelAdjustment = levelAdjustment;
            characterClass.IsNPC = false;
            Assert.That(characterClass.EffectiveLevel, Is.EqualTo(effectiveLevel));
        }

        [TestCase(-10, 0, -5)]
        [TestCase(-9, 0, -4.5)]
        [TestCase(-8, 0, -4)]
        [TestCase(-7, 0, -3.5)]
        [TestCase(-6, 0, -3)]
        [TestCase(-5, 0, -2.5)]
        [TestCase(-4, 0, -2)]
        [TestCase(-3, 0, -1.5)]
        [TestCase(-2, 0, -1)]
        [TestCase(-1, 0, -.5)]
        [TestCase(0, 0, 0)]
        [TestCase(1, 0, .5)]
        [TestCase(2, 0, 1)]
        [TestCase(3, 0, 1)]
        [TestCase(4, 0, 2)]
        [TestCase(5, 0, 2)]
        [TestCase(6, 0, 3)]
        [TestCase(7, 0, 3)]
        [TestCase(8, 0, 4)]
        [TestCase(9, 0, 4)]
        [TestCase(10, 0, 5)]
        [TestCase(11, 0, 5)]
        [TestCase(12, 0, 6)]
        [TestCase(13, 0, 6)]
        [TestCase(14, 0, 7)]
        [TestCase(15, 0, 7)]
        [TestCase(16, 0, 8)]
        [TestCase(17, 0, 8)]
        [TestCase(18, 0, 9)]
        [TestCase(19, 0, 9)]
        [TestCase(20, 0, 10)]
        [TestCase(-10, 1, -4.5)]
        [TestCase(-9, 1, -4)]
        [TestCase(-8, 1, -3.5)]
        [TestCase(-7, 1, -3)]
        [TestCase(-6, 1, -2.5)]
        [TestCase(-5, 1, -2)]
        [TestCase(-4, 1, -1.5)]
        [TestCase(-3, 1, -1)]
        [TestCase(-2, 1, -.5)]
        [TestCase(-1, 1, 0)]
        [TestCase(0, 1, .5)]
        [TestCase(1, 1, 1)]
        [TestCase(2, 1, 1)]
        [TestCase(3, 1, 2)]
        [TestCase(4, 1, 2)]
        [TestCase(5, 1, 3)]
        [TestCase(6, 1, 3)]
        [TestCase(7, 1, 4)]
        [TestCase(8, 1, 4)]
        [TestCase(9, 1, 5)]
        [TestCase(10, 1, 5)]
        [TestCase(11, 1, 6)]
        [TestCase(12, 1, 6)]
        [TestCase(13, 1, 7)]
        [TestCase(14, 1, 7)]
        [TestCase(15, 1, 8)]
        [TestCase(16, 1, 8)]
        [TestCase(17, 1, 9)]
        [TestCase(18, 1, 9)]
        [TestCase(19, 1, 10)]
        [TestCase(20, 1, 10)]
        [TestCase(20, 2, 11)]
        [TestCase(20, 3, 11)]
        [TestCase(20, 4, 12)]
        [TestCase(20, 5, 12)]
        [TestCase(20, 6, 13)]
        [TestCase(20, 7, 13)]
        [TestCase(20, 8, 14)]
        [TestCase(20, 9, 14)]
        [TestCase(20, 10, 15)]
        [TestCase(20, 11, 15)]
        public void EffectiveLevelOfNPCWithAdjustment(int level, int levelAdjustment, double effectiveLevel)
        {
            characterClass.Level = level;
            characterClass.LevelAdjustment = levelAdjustment;
            characterClass.IsNPC = true;
            Assert.That(characterClass.EffectiveLevel, Is.EqualTo(effectiveLevel));
        }

        [Test]
        public void CharacterClassSummary()
        {
            characterClass.Name = "class name";
            characterClass.Level = 9266;

            Assert.That(characterClass.Summary, Is.EqualTo("Level 9266 class name"));
        }
    }
}