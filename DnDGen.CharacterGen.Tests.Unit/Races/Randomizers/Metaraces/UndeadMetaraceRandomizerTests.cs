using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Races.Randomizers.Metaraces;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Unit.Races.Randomizers.Metaraces
{
    [TestFixture]
    public class UndeadMetaraceRandomizerTests : MetaraceRandomizerTestBase
    {
        protected override IEnumerable<string> metaraces
        {
            get
            {
                return new[]
                {
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
            randomizer = new UndeadMetaraceRandomizer(mockPercentileSelector.Object, mockCollectionSelector.Object);

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, GroupConstants.Undead))
                .Returns(new[] { "undead metarace" });
        }

        [Test]
        public void UndeadMetaracesAllowed()
        {
            (randomizer as IForcableMetaraceRandomizer).ForceMetarace = false;
            var metaraces = randomizer.GetAllPossible(alignment, characterClass);
            Assert.That(metaraces, Is.EquivalentTo(new[] { "undead metarace", RaceConstants.Metaraces.None }));
        }

        [Test]
        public void OnlyUndeadMetaracesAllowed()
        {
            (randomizer as IForcableMetaraceRandomizer).ForceMetarace = true;
            var metaraces = randomizer.GetAllPossible(alignment, characterClass);
            Assert.That(metaraces, Is.EquivalentTo(new[] { "undead metarace" }));
        }
    }
}
