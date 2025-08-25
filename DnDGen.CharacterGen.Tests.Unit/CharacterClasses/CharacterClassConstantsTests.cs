using DnDGen.CharacterGen.CharacterClasses;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.CharacterClasses
{
    [TestFixture]
    public class CharacterClassConstantsTests
    {
        [TestCase(CharacterClassConstants.Barbarian, "Barbarian")]
        [TestCase(CharacterClassConstants.Bard, "Bard")]
        [TestCase(CharacterClassConstants.Cleric, "Cleric")]
        [TestCase(CharacterClassConstants.Druid, "Druid")]
        [TestCase(CharacterClassConstants.Fighter, "Fighter")]
        [TestCase(CharacterClassConstants.Monk, "Monk")]
        [TestCase(CharacterClassConstants.Paladin, "Paladin")]
        [TestCase(CharacterClassConstants.Ranger, "Ranger")]
        [TestCase(CharacterClassConstants.Rogue, "Rogue")]
        [TestCase(CharacterClassConstants.Sorcerer, "Sorcerer")]
        [TestCase(CharacterClassConstants.Wizard, "Wizard")]
        [TestCase(CharacterClassConstants.Warrior, "Warrior")]
        [TestCase(CharacterClassConstants.Commoner, "Commoner")]
        [TestCase(CharacterClassConstants.Expert, "Expert")]
        [TestCase(CharacterClassConstants.Adept, "Adept")]
        [TestCase(CharacterClassConstants.Aristocrat, "Aristocrat")]
        [TestCase(CharacterClassConstants.Domains.Air, "Air")]
        [TestCase(CharacterClassConstants.Domains.Animal, "Animal")]
        [TestCase(CharacterClassConstants.Domains.Chaos, "Chaos")]
        [TestCase(CharacterClassConstants.Domains.Death, "Death")]
        [TestCase(CharacterClassConstants.Domains.Destruction, "Destruction")]
        [TestCase(CharacterClassConstants.Domains.Earth, "Earth")]
        [TestCase(CharacterClassConstants.Domains.Evil, "Evil")]
        [TestCase(CharacterClassConstants.Domains.Fire, "Fire")]
        [TestCase(CharacterClassConstants.Domains.Good, "Good")]
        [TestCase(CharacterClassConstants.Domains.Healing, "Healing")]
        [TestCase(CharacterClassConstants.Domains.Knowledge, "Knowledge")]
        [TestCase(CharacterClassConstants.Domains.Law, "Law")]
        [TestCase(CharacterClassConstants.Domains.Luck, "Luck")]
        [TestCase(CharacterClassConstants.Domains.Magic, "Magic")]
        [TestCase(CharacterClassConstants.Domains.Plant, "Plant")]
        [TestCase(CharacterClassConstants.Domains.Protection, "Protection")]
        [TestCase(CharacterClassConstants.Domains.Strength, "Strength")]
        [TestCase(CharacterClassConstants.Domains.Sun, "Sun")]
        [TestCase(CharacterClassConstants.Domains.Travel, "Travel")]
        [TestCase(CharacterClassConstants.Domains.Trickery, "Trickery")]
        [TestCase(CharacterClassConstants.Domains.War, "War")]
        [TestCase(CharacterClassConstants.Domains.Water, "Water")]
        [TestCase(CharacterClassConstants.Schools.Abjuration, "Abjuration")]
        [TestCase(CharacterClassConstants.Schools.Conjuration, "Conjuration")]
        [TestCase(CharacterClassConstants.Schools.Divination, "Divination")]
        [TestCase(CharacterClassConstants.Schools.Enchantment, "Enchantment")]
        [TestCase(CharacterClassConstants.Schools.Evocation, "Evocation")]
        [TestCase(CharacterClassConstants.Schools.Illusion, "Illusion")]
        [TestCase(CharacterClassConstants.Schools.Necromancy, "Necromancy")]
        [TestCase(CharacterClassConstants.Schools.Transmutation, "Transmutation")]
        [TestCase(CharacterClassConstants.TrainingTypes.Intuitive, "Intuitive")]
        [TestCase(CharacterClassConstants.TrainingTypes.SelfTaught, "Self-Taught")]
        [TestCase(CharacterClassConstants.TrainingTypes.Trained, "Trained")]
        public void Constant(string constant, string value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }
    }
}