using DnDGen.CharacterGen.Magics;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Unit.Magics
{
    [TestFixture]
    public class SpellTests
    {
        private Spell spell;

        [SetUp]
        public void Setup()
        {
            spell = new Spell();
        }

        [Test]
        public void SpellIsInitialized()
        {
            Assert.That(spell.Sources, Is.Empty);
            Assert.That(spell.Metamagic, Is.Empty);
            Assert.That(spell.Name, Is.Empty);
        }

        [Test]
        public void SummaryFromSingleSource()
        {
            spell.Name = "my spell";
            spell.Sources["my source"] = 9266;

            Assert.That(spell.Summary, Is.EqualTo("my spell (my source/9266)"));
        }

        [Test]
        public void SummaryFromMultipleSources_SameLevel()
        {
            spell.Name = "my spell";
            spell.Sources["my source"] = 9266;
            spell.Sources["my other source"] = 9266;

            Assert.That(spell.Summary, Is.EqualTo("my spell (my source/9266, my other source/9266)"));
        }

        [Test]
        public void SummaryFromMultipleSources_DifferentLevel()
        {
            spell.Name = "my spell";
            spell.Sources["my source"] = 9266;
            spell.Sources["my other source"] = 90210;

            Assert.That(spell.Summary, Is.EqualTo("my spell (my source/9266, my other source/90210)"));
        }
    }
}
