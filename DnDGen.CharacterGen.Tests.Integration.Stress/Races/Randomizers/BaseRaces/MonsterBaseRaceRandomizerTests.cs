using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Races.Randomizers;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Races.Randomizers.BaseRaces
{
    [TestFixture]
    public class MonsterBaseRaceRandomizerTests : BaseRaceRandomizerTests
    {
        protected override IEnumerable<string> allowedBaseRaces
        {
            get
            {
                //INFO: Not including aquatic, as they cannot randomly appear
                return new[]
                {
                    RaceConstants.BaseRaces.Azer,
                    RaceConstants.BaseRaces.BlueSlaad,
                    RaceConstants.BaseRaces.Bugbear,
                    RaceConstants.BaseRaces.Centaur,
                    RaceConstants.BaseRaces.CloudGiant,
                    RaceConstants.BaseRaces.DeathSlaad,
                    RaceConstants.BaseRaces.Derro,
                    RaceConstants.BaseRaces.Doppelganger,
                    RaceConstants.BaseRaces.FireGiant,
                    RaceConstants.BaseRaces.FrostGiant,
                    RaceConstants.BaseRaces.Gargoyle,
                    RaceConstants.BaseRaces.Githyanki,
                    RaceConstants.BaseRaces.Githzerai,
                    RaceConstants.BaseRaces.Gnoll,
                    RaceConstants.BaseRaces.Goblin,
                    RaceConstants.BaseRaces.GraySlaad,
                    RaceConstants.BaseRaces.GreenSlaad,
                    RaceConstants.BaseRaces.Grimlock,
                    RaceConstants.BaseRaces.Harpy,
                    RaceConstants.BaseRaces.HillGiant,
                    RaceConstants.BaseRaces.Hobgoblin,
                    RaceConstants.BaseRaces.HoundArchon,
                    RaceConstants.BaseRaces.Janni,
                    RaceConstants.BaseRaces.Kobold,
                    RaceConstants.BaseRaces.Lizardfolk,
                    RaceConstants.BaseRaces.MindFlayer,
                    RaceConstants.BaseRaces.Minotaur,
                    RaceConstants.BaseRaces.Mummy,
                    RaceConstants.BaseRaces.Ogre,
                    RaceConstants.BaseRaces.OgreMage,
                    RaceConstants.BaseRaces.Orc,
                    RaceConstants.BaseRaces.Pixie,
                    RaceConstants.BaseRaces.Rakshasa,
                    RaceConstants.BaseRaces.RedSlaad,
                    RaceConstants.BaseRaces.Satyr,
                    RaceConstants.BaseRaces.Scorpionfolk,
                    RaceConstants.BaseRaces.StoneGiant,
                    RaceConstants.BaseRaces.StormGiant,
                    RaceConstants.BaseRaces.Troglodyte,
                    RaceConstants.BaseRaces.Troll,
                    RaceConstants.BaseRaces.YuanTiAbomination,
                    RaceConstants.BaseRaces.YuanTiHalfblood,
                    RaceConstants.BaseRaces.YuanTiPureblood,
                };
            }
        }

        [SetUp]
        public void Setup()
        {
            baseRaceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.BaseRace.MonsterBase);
        }

        [Test]
        public void StressMonsterBaseRace()
        {
            stressor.Stress(AssertBaseRace);
        }
    }
}