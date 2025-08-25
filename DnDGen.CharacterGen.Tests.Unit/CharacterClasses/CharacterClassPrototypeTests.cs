using DnDGen.CharacterGen.CharacterClasses;
using NUnit.Framework;
using System;

namespace DnDGen.CharacterGen.Tests.Unit.CharacterClasses
{
    [TestFixture]
    public class CharacterClassPrototypeTests
    {
        private CharacterClassPrototype characterClass;

        [SetUp]
        public void Setup()
        {
            characterClass = new CharacterClassPrototype();
        }

        [Test]
        public void Summary_ReturnsLevelAndName()
        {
            characterClass.Level = 9266;
            characterClass.Name = "My Class";

            Assert.That(characterClass.Summary, Is.EqualTo("Level 9266 My Class"));
        }

        [Test]
        public void PrototypeInitialized()
        {
            Assert.That(characterClass.Level, Is.Zero);
            Assert.That(characterClass.Name, Is.Empty);
        }

        [Test]
        public void ToStringIsSummary()
        {
            characterClass.Level = 90210;
            characterClass.Name = Guid.NewGuid().ToString();

            Assert.That(characterClass.ToString(), Is.EqualTo(characterClass.Summary));
        }

        [Test]
        public void ConvertingToStringUsesSummary()
        {
            characterClass.Level = 42;
            characterClass.Name = Guid.NewGuid().ToString();

            var alignmentString = Convert.ToString(characterClass);
            Assert.That(alignmentString, Is.EqualTo(characterClass.Summary));
        }

        [Test]
        public void PrototypeIsNotEqualIfOtherItemNotClassPrototype()
        {
            characterClass.Level = 600;
            var otherClass = new object();

            Assert.That(characterClass, Is.Not.EqualTo(otherClass));
        }

        [Test]
        public void PrototypeIsNotEqualIfLevelDiffers()
        {
            characterClass.Level = 1337;
            characterClass.Name = "My Class";

            var otherClass = new CharacterClassPrototype();
            otherClass.Level = 1336;
            otherClass.Name = "My Class";

            Assert.That(characterClass, Is.Not.EqualTo(otherClass));
        }

        [Test]
        public void PrototypeIsNotEqualIfNameDiffers()
        {
            characterClass.Level = 96;
            characterClass.Name = "My Class";

            var otherClass = new CharacterClassPrototype();
            otherClass.Level = 96;
            otherClass.Name = "My Other Class";

            Assert.That(characterClass, Is.Not.EqualTo(otherClass));
        }

        [Test]
        public void PrototypeIsEqualIfLevelsAndNamesMatch()
        {
            characterClass.Level = 783;
            characterClass.Name = "My Class";

            var otherClass = new CharacterClassPrototype();
            otherClass.Level = 783;
            otherClass.Name = "My Class";

            Assert.That(characterClass, Is.EqualTo(otherClass));
        }

        [Test]
        public void HashCodeIsHashOfSummary()
        {
            characterClass.Level = 8245;
            characterClass.Name = "My Class";

            var classHash = characterClass.GetHashCode();
            var classToStringHash = characterClass.ToString().GetHashCode();

            Assert.That(classHash, Is.EqualTo(classToStringHash));
        }
    }
}