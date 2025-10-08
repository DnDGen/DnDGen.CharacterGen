using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Races.Randomizers;
using DnDGen.CharacterGen.Races.Randomizers.Metaraces;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Races.Randomizers.Metaraces
{
    [TestFixture]
    public class AnyMetaraceRandomizerTests : ForcableMetaraceRandomizerTests
    {
        protected override IEnumerable<string> allowedMetaraces
        {
            get
            {
                return [
                    RaceConstants.Metaraces.Ghost,
                    RaceConstants.Metaraces.HalfCelestial,
                    RaceConstants.Metaraces.HalfDragon,
                    RaceConstants.Metaraces.HalfFiend,
                    RaceConstants.Metaraces.Lich,
                    RaceConstants.Metaraces.None,
                    RaceConstants.Metaraces.Vampire,
                    RaceConstants.Metaraces.Werebear,
                    RaceConstants.Metaraces.Wereboar,
                    RaceConstants.Metaraces.Wereboar_Dire,
                    RaceConstants.Metaraces.Wererat,
                    RaceConstants.Metaraces.Weretiger,
                    RaceConstants.Metaraces.Werewolf,
                    RaceConstants.Metaraces.Werewolf_Dire,
                ];
            }
        }

        [SetUp]
        public void Setup()
        {
            forcableMetaraceRandomizer = GetNewInstanceOf<IForcableMetaraceRandomizer>(RaceRandomizerTypeConstants.Metarace.AnyMeta);
        }

        [Test]
        public void StressAnyMetarace()
        {
            stressor.Stress(GenerateAndAssertMetarace);
        }

        [Test]
        public void StressForcedAnyMetarace()
        {
            stressor.Stress(GenerateAndAssertForcedMetarace);
        }
    }
}