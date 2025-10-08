using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Races.Randomizers;
using DnDGen.CharacterGen.Races.Randomizers.Metaraces;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Races.Randomizers.Metaraces
{
    [TestFixture]
    public class LycanthropeMetaraceRandomizerTests : ForcableMetaraceRandomizerTests
    {
        protected override IEnumerable<string> allowedMetaraces
        {
            get
            {
                return
                [
                    RaceConstants.Metaraces.Werebear,
                    RaceConstants.Metaraces.Wereboar,
                    RaceConstants.Metaraces.Wereboar_Dire,
                    RaceConstants.Metaraces.Wererat,
                    RaceConstants.Metaraces.Weretiger,
                    RaceConstants.Metaraces.Werewolf,
                    RaceConstants.Metaraces.Werewolf_Dire,
                    RaceConstants.Metaraces.None
                ];
            }
        }

        [SetUp]
        public void Setup()
        {
            forcableMetaraceRandomizer = GetNewInstanceOf<IForcableMetaraceRandomizer>(RaceRandomizerTypeConstants.Metarace.LycanthropeMeta);
        }

        [Test]
        public void StressLycanthropeMetarace()
        {
            stressor.Stress(GenerateAndAssertMetarace);
        }

        [Test]
        [Ignore("Because lycanthropes can only have a particular alignment, generating alignments that satisfy the randomizer takes over 200% of stress test time limit")]
        public void StressForcedLycanthropeMetarace()
        {
            stressor.Stress(GenerateAndAssertForcedMetarace);
        }
    }
}