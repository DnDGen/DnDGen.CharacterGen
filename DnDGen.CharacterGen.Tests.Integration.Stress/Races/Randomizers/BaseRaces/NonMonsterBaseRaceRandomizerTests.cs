using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Races.Randomizers;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Races.Randomizers.BaseRaces
{
    [TestFixture]
    public class NonMonsterBaseRaceRandomizerTests : BaseRaceRandomizerTests
    {
        protected override IEnumerable<string> allowedBaseRaces
        {
            get
            {
                return new[]
                {
                    RaceConstants.BaseRaces.Aasimar,
                    RaceConstants.BaseRaces.DeepDwarf,
                    RaceConstants.BaseRaces.DeepHalfling,
                    RaceConstants.BaseRaces.Drow,
                    RaceConstants.BaseRaces.DuergarDwarf,
                    RaceConstants.BaseRaces.ForestGnome,
                    RaceConstants.BaseRaces.GrayElf,
                    RaceConstants.BaseRaces.HalfElf,
                    RaceConstants.BaseRaces.HalfOrc,
                    RaceConstants.BaseRaces.HighElf,
                    RaceConstants.BaseRaces.HillDwarf,
                    RaceConstants.BaseRaces.Human,
                    RaceConstants.BaseRaces.LightfootHalfling,
                    RaceConstants.BaseRaces.MountainDwarf,
                    RaceConstants.BaseRaces.RockGnome,
                    RaceConstants.BaseRaces.Svirfneblin,
                    RaceConstants.BaseRaces.TallfellowHalfling,
                    RaceConstants.BaseRaces.Tiefling,
                    RaceConstants.BaseRaces.WildElf,
                    RaceConstants.BaseRaces.WoodElf,
                };
            }
        }

        [SetUp]
        public void Setup()
        {
            baseRaceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.BaseRace.NonMonsterBase);
        }

        [Test]
        public void StressNonMonsterBaseRace()
        {
            stressor.Stress(AssertBaseRace);
        }
    }
}