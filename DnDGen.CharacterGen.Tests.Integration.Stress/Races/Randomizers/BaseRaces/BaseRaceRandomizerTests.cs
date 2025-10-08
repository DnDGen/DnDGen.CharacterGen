using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Races.Randomizers.BaseRaces
{
    [TestFixture]
    public abstract class BaseRaceRandomizerTests : StressTests
    {
        protected abstract IEnumerable<string> allowedBaseRaces { get; }

        protected void AssertBaseRace()
        {
            var prototype = GetCharacterPrototype();

            var baseRace = baseRaceRandomizer.Randomize(prototype.Alignment, prototype.CharacterClass);
            Assert.That(allowedBaseRaces, Contains.Item(baseRace));
        }
    }
}