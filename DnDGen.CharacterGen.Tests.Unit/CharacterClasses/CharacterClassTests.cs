using DnDGen.CharacterGen.CharacterClasses;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.CharacterClasses
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
        public void CharacterClassSummary()
        {
            characterClass.Name = "class name";
            characterClass.Level = 9266;

            Assert.That(characterClass.Summary, Is.EqualTo("Level 9266 class name"));
        }

        [Test]
        public void CharacterClassToString()
        {
            characterClass.Name = "class name";
            characterClass.Level = 9266;

            Assert.That(characterClass.ToString(), Is.EqualTo("Level 9266 class name"));
        }
    }
}