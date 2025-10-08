using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Races.Randomizers;
using DnDGen.CharacterGen.Races.Randomizers.Metaraces;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Races.Randomizers.Metaraces
{
    [TestFixture]
    public class UndeadMetaraceRandomizerTests : ForcableMetaraceRandomizerTests
    {
        protected override IEnumerable<string> allowedMetaraces
        {
            get
            {
                return new[]
                {
                    RaceConstants.Metaraces.Ghost,
                    RaceConstants.Metaraces.Lich,
                    RaceConstants.Metaraces.None,
                    RaceConstants.Metaraces.Vampire,
                };
            }
        }

        [SetUp]
        public void Setup()
        {
            forcableMetaraceRandomizer = GetNewInstanceOf<IForcableMetaraceRandomizer>(RaceRandomizerTypeConstants.Metarace.UndeadMeta);
        }

        [Test]
        public void StressUndeadMetarace()
        {
            stressor.Stress(GenerateAndAssertMetarace);
        }

        [Test]
        public void StressForcedUndeadMetarace()
        {
            stressor.Stress(GenerateAndAssertForcedMetarace);
        }
    }
}
