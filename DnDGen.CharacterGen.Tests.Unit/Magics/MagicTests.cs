using DnDGen.CharacterGen.Magics;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Magics
{
    [TestFixture]
    public class MagicTests
    {
        private Magic magic;

        [SetUp]
        public void Setup()
        {
            magic = new Magic();
        }

        [Test]
        public void MagicInitialized()
        {
            Assert.That(magic.Animal, Is.Empty);
            Assert.That(magic.SpellsPerDay, Is.Empty);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(0));
            Assert.That(magic.KnownSpells, Is.Empty);
            Assert.That(magic.PreparedSpells, Is.Empty);
        }
    }
}