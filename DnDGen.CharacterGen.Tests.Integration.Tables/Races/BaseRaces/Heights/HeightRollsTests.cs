using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Races.BaseRaces.Heights
{
    [TestFixture]
    public class HeightRollsTests : CollectionTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Set.Collection.HeightRolls; }
        }

        [Test]
        public override void CollectionNames()
        {
            var baseRaceGroups = GetTable(TableNameConstants.Set.Collection.BaseRaceGroups);
            var allBaseRaces = baseRaceGroups[GroupConstants.All];

            AssertCollectionNames(allBaseRaces);
        }

        [TestCase(RaceConstants.BaseRaces.Aasimar, "2d8")]
        [TestCase(RaceConstants.BaseRaces.AquaticElf, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Azer, "2d4")]
        [TestCase(RaceConstants.BaseRaces.BlueSlaad, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Bugbear, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Centaur, "3d4")]
        [TestCase(RaceConstants.BaseRaces.CloudGiant, "2d12")]
        [TestCase(RaceConstants.BaseRaces.DeathSlaad, "2d10")]
        [TestCase(RaceConstants.BaseRaces.DeepDwarf, "2d4")]
        [TestCase(RaceConstants.BaseRaces.DeepHalfling, "2d4")]
        [TestCase(RaceConstants.BaseRaces.Derro, "2d4")]
        [TestCase(RaceConstants.BaseRaces.Doppelganger, "1d6")]
        [TestCase(RaceConstants.BaseRaces.Drow, "2d6")]
        [TestCase(RaceConstants.BaseRaces.DuergarDwarf, "2d4")]
        [TestCase(RaceConstants.BaseRaces.FireGiant, "2d12")]
        [TestCase(RaceConstants.BaseRaces.ForestGnome, "2d4")]
        [TestCase(RaceConstants.BaseRaces.FrostGiant, "2d12")]
        [TestCase(RaceConstants.BaseRaces.Gargoyle, "2d12")]
        [TestCase(RaceConstants.BaseRaces.Githyanki, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Githzerai, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Gnoll, "2d10")]
        [TestCase(RaceConstants.BaseRaces.Goblin, "2d4")]
        [TestCase(RaceConstants.BaseRaces.GrayElf, "2d6")]
        [TestCase(RaceConstants.BaseRaces.GraySlaad, "2d10")]
        [TestCase(RaceConstants.BaseRaces.GreenSlaad, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Grimlock, "2d4")]
        [TestCase(RaceConstants.BaseRaces.HalfElf, "2d8")]
        [TestCase(RaceConstants.BaseRaces.HalfOrc, "2d12")]
        [TestCase(RaceConstants.BaseRaces.Harpy, "2d6")]
        [TestCase(RaceConstants.BaseRaces.HighElf, "2d6")]
        [TestCase(RaceConstants.BaseRaces.HillDwarf, "2d4")]
        [TestCase(RaceConstants.BaseRaces.HillGiant, "2d12")]
        [TestCase(RaceConstants.BaseRaces.Hobgoblin, "2d8")]
        [TestCase(RaceConstants.BaseRaces.HoundArchon, "1d12")]
        [TestCase(RaceConstants.BaseRaces.Human, "2d10")]
        [TestCase(RaceConstants.BaseRaces.Janni, "2d10")]
        [TestCase(RaceConstants.BaseRaces.Kapoacinth, "2d12")]
        [TestCase(RaceConstants.BaseRaces.Kobold, "2d4")]
        [TestCase(RaceConstants.BaseRaces.KuoToa, "2d6")]
        [TestCase(RaceConstants.BaseRaces.LightfootHalfling, "2d4")]
        [TestCase(RaceConstants.BaseRaces.Lizardfolk, "2d10")]
        [TestCase(RaceConstants.BaseRaces.Locathah, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Merfolk, "2d8")]
        [TestCase(RaceConstants.BaseRaces.Merrow, "2d6")]
        [TestCase(RaceConstants.BaseRaces.MindFlayer, "2d8")]
        [TestCase(RaceConstants.BaseRaces.Minotaur, "2d6")]
        [TestCase(RaceConstants.BaseRaces.MountainDwarf, "2d4")]
        [TestCase(RaceConstants.BaseRaces.Mummy, "4d4")]
        [TestCase(RaceConstants.BaseRaces.Ogre, "2d6")]
        [TestCase(RaceConstants.BaseRaces.OgreMage, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Orc, "2d12")]
        [TestCase(RaceConstants.BaseRaces.Pixie, "3d8")]
        [TestCase(RaceConstants.BaseRaces.Rakshasa, "2d10")]
        [TestCase(RaceConstants.BaseRaces.RedSlaad, "2d6")]
        [TestCase(RaceConstants.BaseRaces.RockGnome, "2d4")]
        [TestCase(RaceConstants.BaseRaces.Sahuagin, "2d10")]
        [TestCase(RaceConstants.BaseRaces.Satyr, "1d10")]
        [TestCase(RaceConstants.BaseRaces.Scorpionfolk, "3d10")]
        [TestCase(RaceConstants.BaseRaces.Scrag, "2d10")]
        [TestCase(RaceConstants.BaseRaces.StoneGiant, "2d12")]
        [TestCase(RaceConstants.BaseRaces.StormGiant, "2d12")]
        [TestCase(RaceConstants.BaseRaces.Svirfneblin, "2d4")]
        [TestCase(RaceConstants.BaseRaces.TallfellowHalfling, "2d4")]
        [TestCase(RaceConstants.BaseRaces.Tiefling, "2d8")]
        [TestCase(RaceConstants.BaseRaces.Troglodyte, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Troll, "2d10")]
        [TestCase(RaceConstants.BaseRaces.WoodElf, "2d6")]
        [TestCase(RaceConstants.BaseRaces.WildElf, "2d6")]
        [TestCase(RaceConstants.BaseRaces.YuanTiAbomination, "4d12")]
        [TestCase(RaceConstants.BaseRaces.YuanTiHalfblood, "2d10")]
        [TestCase(RaceConstants.BaseRaces.YuanTiPureblood, "2d10")]
        public void HeightRoll(string name, string heightRoll)
        {
            base.AssertDistinctCollection(name, [heightRoll]);
        }
    }
}
