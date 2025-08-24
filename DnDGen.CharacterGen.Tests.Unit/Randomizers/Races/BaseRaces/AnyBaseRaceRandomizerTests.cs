using DnDGen.CharacterGen.Generators.Randomizers.Races.BaseRaces;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Unit.Randomizers.Races.BaseRaces
{
    [TestFixture]
    public class AnyBaseRaceRandomizerTests : BaseRaceRandomizerTestBase
    {
        protected override IEnumerable<string> baseRaces
        {
            get
            {
                return new[] {
                    "base race",
                    "standard base race",
                    "non-standard base race",
                    "monster base race",
                    "aquatic base race",
                };
            }
        }

        [SetUp]
        public void Setup()
        {
            randomizer = new AnyBaseRaceRandomizer(mockPercentileSelector.Object, mockCollectionSelector.Object);
        }

        [Test]
        public void AllBaseRacesAllowed()
        {
            var allBaseRaces = randomizer.GetAllPossible(alignment, characterClass);
            Assert.That(allBaseRaces, Is.EquivalentTo(baseRaces));
        }
    }
}