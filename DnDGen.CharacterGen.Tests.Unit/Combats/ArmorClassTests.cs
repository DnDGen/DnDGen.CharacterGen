using DnDGen.CharacterGen.Combats;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Combats
{
    [TestFixture]
    public class ArmorClassTests
    {
        private ArmorClass armorClass;

        [SetUp]
        public void Setup()
        {
            armorClass = new ArmorClass();
        }

        [Test]
        public void ArmorClassInitialized()
        {
            Assert.That(armorClass.FlatFooted, Is.EqualTo(10));
            Assert.That(armorClass.Full, Is.EqualTo(10));
            Assert.That(armorClass.Touch, Is.EqualTo(10));
            Assert.That(armorClass.CircumstantialBonus, Is.False);
            Assert.That(armorClass.AdjustedDexterityBonus, Is.EqualTo(0));
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(0));
            Assert.That(armorClass.DeflectionBonus, Is.EqualTo(0));
            Assert.That(armorClass.DodgeBonus, Is.EqualTo(0));
            Assert.That(armorClass.NaturalArmorBonus, Is.EqualTo(0));
            Assert.That(armorClass.ShieldBonus, Is.EqualTo(0));
            Assert.That(armorClass.SizeModifier, Is.EqualTo(0));
        }

        [Test]
        public void BaseArmorClassIs10()
        {
            Assert.That(ArmorClass.BaseArmorClass, Is.EqualTo(10));
        }

        [Test]
        public void FullArmorClassIsEverything()
        {
            armorClass.AdjustedDexterityBonus = 9266;
            armorClass.ArmorBonus = 90210;
            armorClass.DeflectionBonus = 42;
            armorClass.DodgeBonus = 600;
            armorClass.NaturalArmorBonus = 1337;
            armorClass.ShieldBonus = 1234;
            armorClass.SizeModifier = 2345;

            Assert.That(armorClass.Full, Is.EqualTo(105044));
        }

        [Test]
        public void FullArmorClassMustBePositive()
        {
            armorClass.AdjustedDexterityBonus = -9266;
            armorClass.ArmorBonus = -90210;
            armorClass.DeflectionBonus = -42;
            armorClass.DodgeBonus = -600;
            armorClass.NaturalArmorBonus = -1337;
            armorClass.ShieldBonus = -1234;
            armorClass.SizeModifier = -2345;

            Assert.That(armorClass.Full, Is.EqualTo(1));
        }

        [Test]
        public void FlatFootedArmorClassDoesNotIncludeDodgeOrDexterity()
        {
            armorClass.AdjustedDexterityBonus = 9266;
            armorClass.ArmorBonus = 90210;
            armorClass.DeflectionBonus = 42;
            armorClass.DodgeBonus = 600;
            armorClass.NaturalArmorBonus = 1337;
            armorClass.ShieldBonus = 1234;
            armorClass.SizeModifier = 2345;

            Assert.That(armorClass.FlatFooted, Is.EqualTo(95178));
        }

        [Test]
        public void FlatFootedArmorClassMustBePositive()
        {
            armorClass.AdjustedDexterityBonus = -9266;
            armorClass.ArmorBonus = -90210;
            armorClass.DeflectionBonus = -42;
            armorClass.DodgeBonus = -600;
            armorClass.NaturalArmorBonus = -1337;
            armorClass.ShieldBonus = -1234;
            armorClass.SizeModifier = -2345;

            Assert.That(armorClass.FlatFooted, Is.EqualTo(1));
        }

        [Test]
        public void TouchArmorClassDoesNotIncludeArmorOrShieldOrNatural()
        {
            armorClass.AdjustedDexterityBonus = 9266;
            armorClass.ArmorBonus = 90210;
            armorClass.DeflectionBonus = 42;
            armorClass.DodgeBonus = 600;
            armorClass.NaturalArmorBonus = 1337;
            armorClass.ShieldBonus = 1234;
            armorClass.SizeModifier = 2345;

            Assert.That(armorClass.Touch, Is.EqualTo(12263));
        }

        [Test]
        public void TouchArmorClassMustBePositive()
        {
            armorClass.AdjustedDexterityBonus = -9266;
            armorClass.ArmorBonus = -90210;
            armorClass.DeflectionBonus = -42;
            armorClass.DodgeBonus = -600;
            armorClass.NaturalArmorBonus = -1337;
            armorClass.ShieldBonus = -1234;
            armorClass.SizeModifier = -2345;

            Assert.That(armorClass.Touch, Is.EqualTo(1));
        }
    }
}