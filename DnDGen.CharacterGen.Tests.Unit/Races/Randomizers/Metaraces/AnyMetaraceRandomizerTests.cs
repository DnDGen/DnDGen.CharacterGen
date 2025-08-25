using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Races.Randomizers.Metaraces;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Unit.Races.Randomizers.Metaraces
{
    [TestFixture]
    public class AnyMetaraceRandomizerTests : MetaraceRandomizerTestBase
    {
        protected override IEnumerable<string> metaraces
        {
            get
            {
                return new[] {
                    "genetic metarace",
                    "lycanthrope metarace",
                    "undead metarace",
                    RaceConstants.Metaraces.None,
                };
            }
        }

        [SetUp]
        public void Setup()
        {
            randomizer = new AnyMetaraceRandomizer(mockPercentileSelector.Object, mockCollectionSelector.Object);
        }

        [Test]
        public void AllMetaracesAllowed()
        {
            (randomizer as IForcableMetaraceRandomizer).ForceMetarace = false;
            var allMetaraces = randomizer.GetAllPossible(alignment, characterClass);
            Assert.That(allMetaraces, Is.EquivalentTo(metaraces));
        }

        [Test]
        public void OnlyMetaracesAllowed()
        {
            (randomizer as IForcableMetaraceRandomizer).ForceMetarace = true;
            var metaraces = randomizer.GetAllPossible(alignment, characterClass);
            Assert.That(metaraces, Is.EquivalentTo(new[] { "genetic metarace", "lycanthrope metarace", "undead metarace" }));
        }
    }
}