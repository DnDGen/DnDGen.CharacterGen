using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Randomizers.Races.Metaraces;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Randomizers.Races.Metaraces
{
    [TestFixture]
    public abstract class ForcableMetaraceRandomizerTests : StressTests
    {
        protected IForcableMetaraceRandomizer forcableMetaraceRandomizer
        {
            get { return metaraceRandomizer as IForcableMetaraceRandomizer; }
            set { metaraceRandomizer = value; }
        }

        protected abstract IEnumerable<string> allowedMetaraces { get; }

        protected void GenerateAndAssertMetarace()
        {
            var metarace = GenerateMetarace();
            Assert.That(allowedMetaraces, Contains.Item(metarace));
        }

        private string GenerateMetarace()
        {
            var prototype = GetCharacterPrototype();
            return metaraceRandomizer.Randomize(prototype.Alignment, prototype.CharacterClass);
        }

        protected void GenerateAndAssertForcedMetarace()
        {
            forcableMetaraceRandomizer.ForceMetarace = true;

            var metarace = GenerateMetarace();
            Assert.That(allowedMetaraces, Contains.Item(metarace));
            Assert.That(metarace, Is.Not.EqualTo(RaceConstants.Metaraces.None));
        }
    }
}