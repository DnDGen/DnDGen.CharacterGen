using DnDGen.CharacterGen.Randomizers.Races;
using DnDGen.CharacterGen.Randomizers.Races.Metaraces;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Randomizers.Races.Metaraces
{
    [TestFixture]
    public class SetMetaraceRandomizerTests : StressTests
    {
        private ISetMetaraceRandomizer setMetaraceRandomizer;

        [SetUp]
        public void Setup()
        {
            setMetaraceRandomizer = GetNewInstanceOf<ISetMetaraceRandomizer>();

            var forcableMetaraceRandomizer = metaraceRandomizer as IForcableMetaraceRandomizer;
            forcableMetaraceRandomizer.ForceMetarace = true;
        }

        [TearDown]
        public void TearDown()
        {
            metaraceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.Metarace.AnyMeta);
        }

        [Test]
        public void StressSetMetarace()
        {
            stressor.Stress(AssertMetarace);
        }

        protected void AssertMetarace()
        {
            var prototype = GetCharacterPrototype();
            setMetaraceRandomizer.SetMetarace = metaraceRandomizer.Randomize(prototype.Alignment, prototype.CharacterClass);

            var metarace = setMetaraceRandomizer.Randomize(prototype.Alignment, prototype.CharacterClass);
            Assert.That(metarace, Is.EqualTo(setMetaraceRandomizer.SetMetarace));
        }
    }
}