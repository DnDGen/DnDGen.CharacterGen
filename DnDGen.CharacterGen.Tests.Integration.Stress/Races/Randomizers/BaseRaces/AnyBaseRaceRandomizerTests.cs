using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Races.Randomizers;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Races.Randomizers.BaseRaces
{
    [TestFixture]
    public class AnyBaseRaceRandomizerTests : BaseRaceRandomizerTests
    {
        protected override IEnumerable<string> allowedBaseRaces
        {
            get
            {
                return new[] {
                    RaceConstants.BaseRaces.Aasimar,
                    RaceConstants.BaseRaces.Azer,
                    RaceConstants.BaseRaces.BlueSlaad,
                    RaceConstants.BaseRaces.Bugbear,
                    RaceConstants.BaseRaces.Centaur,
                    RaceConstants.BaseRaces.CloudGiant,
                    RaceConstants.BaseRaces.DeathSlaad,
                    RaceConstants.BaseRaces.DeepDwarf,
                    RaceConstants.BaseRaces.DeepHalfling,
                    RaceConstants.BaseRaces.Derro,
                    RaceConstants.BaseRaces.Doppelganger,
                    RaceConstants.BaseRaces.Drow,
                    RaceConstants.BaseRaces.DuergarDwarf,
                    RaceConstants.BaseRaces.FireGiant,
                    RaceConstants.BaseRaces.ForestGnome,
                    RaceConstants.BaseRaces.FrostGiant,
                    RaceConstants.BaseRaces.Gargoyle,
                    RaceConstants.BaseRaces.Githyanki,
                    RaceConstants.BaseRaces.Githzerai,
                    RaceConstants.BaseRaces.Gnoll,
                    RaceConstants.BaseRaces.Goblin,
                    RaceConstants.BaseRaces.GrayElf,
                    RaceConstants.BaseRaces.GraySlaad,
                    RaceConstants.BaseRaces.GreenSlaad,
                    RaceConstants.BaseRaces.Grimlock,
                    RaceConstants.BaseRaces.HalfElf,
                    RaceConstants.BaseRaces.HalfOrc,
                    RaceConstants.BaseRaces.Harpy,
                    RaceConstants.BaseRaces.HighElf,
                    RaceConstants.BaseRaces.HillDwarf,
                    RaceConstants.BaseRaces.HillGiant,
                    RaceConstants.BaseRaces.Hobgoblin,
                    RaceConstants.BaseRaces.HoundArchon,
                    RaceConstants.BaseRaces.Human,
                    RaceConstants.BaseRaces.Janni,
                    RaceConstants.BaseRaces.Kobold,
                    RaceConstants.BaseRaces.LightfootHalfling,
                    RaceConstants.BaseRaces.Lizardfolk,
                    RaceConstants.BaseRaces.MindFlayer,
                    RaceConstants.BaseRaces.Minotaur,
                    RaceConstants.BaseRaces.MountainDwarf,
                    RaceConstants.BaseRaces.Mummy,
                    RaceConstants.BaseRaces.Ogre,
                    RaceConstants.BaseRaces.OgreMage,
                    RaceConstants.BaseRaces.Orc,
                    RaceConstants.BaseRaces.Pixie,
                    RaceConstants.BaseRaces.Rakshasa,
                    RaceConstants.BaseRaces.RedSlaad,
                    RaceConstants.BaseRaces.RockGnome,
                    RaceConstants.BaseRaces.Satyr,
                    RaceConstants.BaseRaces.Scorpionfolk,
                    RaceConstants.BaseRaces.StoneGiant,
                    RaceConstants.BaseRaces.StormGiant,
                    RaceConstants.BaseRaces.Svirfneblin,
                    RaceConstants.BaseRaces.TallfellowHalfling,
                    RaceConstants.BaseRaces.Tiefling,
                    RaceConstants.BaseRaces.Troglodyte,
                    RaceConstants.BaseRaces.Troll,
                    RaceConstants.BaseRaces.WildElf,
                    RaceConstants.BaseRaces.WoodElf,
                    RaceConstants.BaseRaces.YuanTiAbomination,
                    RaceConstants.BaseRaces.YuanTiHalfblood,
                    RaceConstants.BaseRaces.YuanTiPureblood,
                };
            }
        }

        [SetUp]
        public void Setup()
        {
            baseRaceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.BaseRace.AnyBase);
        }

        [Test]
        public void StressAnyBaseRace()
        {
            stressor.Stress(AssertBaseRace);
        }
    }
}