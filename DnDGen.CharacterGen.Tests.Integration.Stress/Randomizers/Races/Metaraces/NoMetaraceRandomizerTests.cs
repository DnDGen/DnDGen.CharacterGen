using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Races.Randomizers;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Randomizers.Races.Metaraces
{
    [TestFixture]
    public class NoMetaraceRandomizerTests : StressTests
    {
        [SetUp]
        public void Setup()
        {
            metaraceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.Metarace.NoMeta);
        }

        [Test]
        public void StressNoMetarace()
        {
            stressor.Stress(AssertMetarace);
        }

        protected void AssertMetarace()
        {
            var prototype = GetCharacterPrototype();

            var metarace = metaraceRandomizer.Randomize(prototype.Alignment, prototype.CharacterClass);
            Assert.That(metarace, Is.EqualTo(RaceConstants.Metaraces.None));
        }
    }
}