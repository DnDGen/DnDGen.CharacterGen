using DnDGen.CharacterGen.Races.Randomizers.BaseRaces;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Unit.Races.Randomizers.BaseRaces
{
    [TestFixture]
    public class NonMonsterBaseRaceRandomizerTests : BaseRaceRandomizerTestBase
    {
        protected override IEnumerable<string> baseRaces
        {
            get
            {
                return new[]
                {
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
            randomizer = new NonMonsterBaseRaceRandomizer(mockPercentileSelector.Object, mockCollectionSelector.Object);

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters))
                .Returns(new[] { "monster base race", "aquatic base race", "other base race" });
        }

        [Test]
        public void OnlyNonMonsterBaseRacesAllowed()
        {
            var baseRaces = randomizer.GetAllPossible(alignment, characterClass);
            Assert.That(baseRaces, Is.EquivalentTo(new[] { "non-standard base race", "base race", "standard base race" }));
        }
    }
}