using DnDGen.CharacterGen.Randomizers.CharacterClasses.ClassNames;
using DnDGen.CharacterGen.Randomizers.Races.BaseRaces;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Randomizers.Races.BaseRaces
{
    [TestFixture]
    public class SetBaseRaceRandomizerTests : StressTests
    {
        private ISetBaseRaceRandomizer setBaseRaceRandomizer;

        [SetUp]
        public void Setup()
        {
            setBaseRaceRandomizer = GetNewInstanceOf<ISetBaseRaceRandomizer>();
        }

        [Test]
        public void StressSetBaseRace()
        {
            stressor.Stress(AssertBaseRace);
        }

        protected void AssertBaseRace()
        {
            var prototype = GetCharacterPrototype();
            setBaseRaceRandomizer.SetBaseRace = baseRaceRandomizer.Randomize(prototype.Alignment, prototype.CharacterClass);

            var baseRace = setBaseRaceRandomizer.Randomize(prototype.Alignment, prototype.CharacterClass);
            Assert.That(baseRace, Is.EqualTo(setBaseRaceRandomizer.SetBaseRace));
        }

        [Test]
        public void StressNPCSetBaseRace()
        {
            classNameRandomizer = GetNewInstanceOf<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.AnyNPC);
            stressor.Stress(AssertBaseRace);
        }
    }
}