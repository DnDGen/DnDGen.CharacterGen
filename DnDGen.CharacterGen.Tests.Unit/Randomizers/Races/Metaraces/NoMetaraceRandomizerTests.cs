using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Generators.Randomizers.Races.Metaraces;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Randomizers.Races;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Randomizers.Races.Metaraces
{
    [TestFixture]
    public class NoMetaraceRandomizerTests
    {
        private RaceRandomizer randomizer;
        private Alignment alignment;
        private CharacterClassPrototype characterClass;

        [SetUp]
        public void Setup()
        {
            randomizer = new NoMetaraceRandomizer();
            alignment = new Alignment();
            characterClass = new CharacterClassPrototype();
        }

        [Test]
        public void RandomizeAlwaysReturnsEmptyString()
        {
            var metarace = randomizer.Randomize(alignment, characterClass);
            Assert.That(metarace, Is.EqualTo(RaceConstants.Metaraces.None));
        }

        [Test]
        public void GetAllPossibleResultsReturnsEnumerableOfEmptyString()
        {
            var metaraces = randomizer.GetAllPossible(alignment, characterClass);
            Assert.That(metaraces.Single(), Is.EqualTo(RaceConstants.Metaraces.None));
        }
    }
}