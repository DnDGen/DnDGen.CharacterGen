using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Races.BaseRaces.Ages.Rolls
{
    [TestFixture]
    public class SelfTaughtAgeRollsTests : CollectionTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Formattable.Collection.CLASSTYPEAgeRolls, CharacterClassConstants.TrainingTypes.SelfTaught); }
        }

        [Test]
        public override void CollectionNames()
        {
            var baseRaceGroups = GetTable(TableNameConstants.Set.Collection.BaseRaceGroups);
            var allBaseRaces = baseRaceGroups[GroupConstants.All];

            AssertCollectionNames(allBaseRaces);
        }

        [TestCase(RaceConstants.BaseRaces.Aasimar, "1d8")]
        [TestCase(RaceConstants.BaseRaces.AquaticElf, "6d6")]
        [TestCase(RaceConstants.BaseRaces.Azer, "5d6")]
        [TestCase(RaceConstants.BaseRaces.BlueSlaad, "5d6")]
        [TestCase(RaceConstants.BaseRaces.Bugbear, "1d6")]
        [TestCase(RaceConstants.BaseRaces.Centaur, "2d6")]
        [TestCase(RaceConstants.BaseRaces.CloudGiant, "5d6")]
        [TestCase(RaceConstants.BaseRaces.DeathSlaad, "5d6")]
        [TestCase(RaceConstants.BaseRaces.DeepDwarf, "5d6")]
        [TestCase(RaceConstants.BaseRaces.DeepHalfling, "3d6")]
        [TestCase(RaceConstants.BaseRaces.Derro, "5d6")]
        [TestCase(RaceConstants.BaseRaces.Doppelganger, "1d6")]
        [TestCase(RaceConstants.BaseRaces.Drow, "6d6")]
        [TestCase(RaceConstants.BaseRaces.DuergarDwarf, "5d6")]
        [TestCase(RaceConstants.BaseRaces.FireGiant, "5d6")]
        [TestCase(RaceConstants.BaseRaces.ForestGnome, "6d6")]
        [TestCase(RaceConstants.BaseRaces.FrostGiant, "5d6")]
        [TestCase(RaceConstants.BaseRaces.Gargoyle, "1d6")]
        [TestCase(RaceConstants.BaseRaces.Githyanki, "3d6")]
        [TestCase(RaceConstants.BaseRaces.Githzerai, "3d6")]
        [TestCase(RaceConstants.BaseRaces.Gnoll, "1d6")]
        [TestCase(RaceConstants.BaseRaces.Goblin, "1d6")]
        [TestCase(RaceConstants.BaseRaces.GrayElf, "6d6")]
        [TestCase(RaceConstants.BaseRaces.GraySlaad, "5d6")]
        [TestCase(RaceConstants.BaseRaces.GreenSlaad, "5d6")]
        [TestCase(RaceConstants.BaseRaces.Grimlock, "1d6")]
        [TestCase(RaceConstants.BaseRaces.HalfElf, "2d6")]
        [TestCase(RaceConstants.BaseRaces.HalfOrc, "1d6")]
        [TestCase(RaceConstants.BaseRaces.Harpy, "1d8")]
        [TestCase(RaceConstants.BaseRaces.HighElf, "6d6")]
        [TestCase(RaceConstants.BaseRaces.HillDwarf, "5d6")]
        [TestCase(RaceConstants.BaseRaces.HillGiant, "5d6")]
        [TestCase(RaceConstants.BaseRaces.Hobgoblin, "1d6")]
        [TestCase(RaceConstants.BaseRaces.HoundArchon, "1d6")]
        [TestCase(RaceConstants.BaseRaces.Human, "1d6")]
        [TestCase(RaceConstants.BaseRaces.Janni, "1d6")]
        [TestCase(RaceConstants.BaseRaces.Kapoacinth, "1d6")]
        [TestCase(RaceConstants.BaseRaces.Kobold, "1d6")]
        [TestCase(RaceConstants.BaseRaces.KuoToa, "1d6")]
        [TestCase(RaceConstants.BaseRaces.LightfootHalfling, "3d6")]
        [TestCase(RaceConstants.BaseRaces.Lizardfolk, "1d6")]
        [TestCase(RaceConstants.BaseRaces.Locathah, "1d6")]
        [TestCase(RaceConstants.BaseRaces.Merfolk, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Merrow, "3d6")]
        [TestCase(RaceConstants.BaseRaces.MindFlayer, "4d6")]
        [TestCase(RaceConstants.BaseRaces.Minotaur, "1d6")]
        [TestCase(RaceConstants.BaseRaces.MountainDwarf, "5d6")]
        [TestCase(RaceConstants.BaseRaces.Mummy, "6d20")]
        [TestCase(RaceConstants.BaseRaces.Ogre, "3d6")]
        [TestCase(RaceConstants.BaseRaces.OgreMage, "3d6")]
        [TestCase(RaceConstants.BaseRaces.Orc, "1d6")]
        [TestCase(RaceConstants.BaseRaces.Pixie, "1d4")]
        [TestCase(RaceConstants.BaseRaces.Rakshasa, "1d6")]
        [TestCase(RaceConstants.BaseRaces.RedSlaad, "5d6")]
        [TestCase(RaceConstants.BaseRaces.RockGnome, "6d6")]
        [TestCase(RaceConstants.BaseRaces.Sahuagin, "1d6")]
        [TestCase(RaceConstants.BaseRaces.Satyr, "2d8")]
        [TestCase(RaceConstants.BaseRaces.Scorpionfolk, "1d6")]
        [TestCase(RaceConstants.BaseRaces.Scrag, "1d6")]
        [TestCase(RaceConstants.BaseRaces.StoneGiant, "5d6")]
        [TestCase(RaceConstants.BaseRaces.StormGiant, "5d6")]
        [TestCase(RaceConstants.BaseRaces.Svirfneblin, "6d6")]
        [TestCase(RaceConstants.BaseRaces.TallfellowHalfling, "3d6")]
        [TestCase(RaceConstants.BaseRaces.Tiefling, "1d8")]
        [TestCase(RaceConstants.BaseRaces.Troglodyte, "1d6")]
        [TestCase(RaceConstants.BaseRaces.Troll, "1d6")]
        [TestCase(RaceConstants.BaseRaces.WildElf, "6d6")]
        [TestCase(RaceConstants.BaseRaces.WoodElf, "6d6")]
        [TestCase(RaceConstants.BaseRaces.YuanTiAbomination, "1d6")]
        [TestCase(RaceConstants.BaseRaces.YuanTiHalfblood, "1d6")]
        [TestCase(RaceConstants.BaseRaces.YuanTiPureblood, "1d6")]
        public void SelfTaughtAgeRoll(string name, string ageRoll)
        {
            base.AssertDistinctCollection(name, ageRoll);
        }
    }
}
