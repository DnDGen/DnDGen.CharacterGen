using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Races.BaseRaces.Ages.Rolls
{
    [TestFixture]
    public class MaximumAgeRollsTests : CollectionTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Set.Collection.MaximumAgeRolls; }
        }

        [Test]
        public override void CollectionNames()
        {
            var baseRaceGroups = GetTable(TableNameConstants.Set.Collection.BaseRaceGroups);
            var allBaseRaces = baseRaceGroups[GroupConstants.All];

            AssertCollectionNames(allBaseRaces);
        }

        [TestCase(RaceConstants.BaseRaces.Aasimar, "2d20")]
        [TestCase(RaceConstants.BaseRaces.AquaticElf, "4d100")]
        [TestCase(RaceConstants.BaseRaces.Azer, "2d100")]
        [TestCase(RaceConstants.BaseRaces.Bugbear, "2d10")]
        [TestCase(RaceConstants.BaseRaces.Centaur, "2d12")]
        [TestCase(RaceConstants.BaseRaces.CloudGiant, "10d10")]
        [TestCase(RaceConstants.BaseRaces.DeepDwarf, "2d100")]
        [TestCase(RaceConstants.BaseRaces.DeepHalfling, "5d20")]
        [TestCase(RaceConstants.BaseRaces.Derro, "2d100")]
        [TestCase(RaceConstants.BaseRaces.Doppelganger, "2d20")]
        [TestCase(RaceConstants.BaseRaces.Drow, "4d100")]
        [TestCase(RaceConstants.BaseRaces.DuergarDwarf, "2d100")]
        [TestCase(RaceConstants.BaseRaces.FireGiant, "9d10")]
        [TestCase(RaceConstants.BaseRaces.ForestGnome, "3d100")]
        [TestCase(RaceConstants.BaseRaces.FrostGiant, "6d10")]
        [TestCase(RaceConstants.BaseRaces.Githyanki, "50")]
        [TestCase(RaceConstants.BaseRaces.Githzerai, "50")]
        [TestCase(RaceConstants.BaseRaces.Gnoll, "2d10")]
        [TestCase(RaceConstants.BaseRaces.Goblin, "1d20")]
        [TestCase(RaceConstants.BaseRaces.GrayElf, "4d100")]
        [TestCase(RaceConstants.BaseRaces.Grimlock, "2d20")]
        [TestCase(RaceConstants.BaseRaces.HalfElf, "3d20")]
        [TestCase(RaceConstants.BaseRaces.HalfOrc, "2d10")]
        [TestCase(RaceConstants.BaseRaces.Harpy, "3d6")]
        [TestCase(RaceConstants.BaseRaces.HighElf, "4d100")]
        [TestCase(RaceConstants.BaseRaces.HillDwarf, "2d100")]
        [TestCase(RaceConstants.BaseRaces.HillGiant, "5d10")]
        [TestCase(RaceConstants.BaseRaces.Hobgoblin, "2d10")]
        [TestCase(RaceConstants.BaseRaces.HoundArchon, "7d6")]
        [TestCase(RaceConstants.BaseRaces.Human, "2d20")]
        [TestCase(RaceConstants.BaseRaces.Janni, "2d20")]
        [TestCase(RaceConstants.BaseRaces.Kobold, "1d20")]
        [TestCase(RaceConstants.BaseRaces.KuoToa, "2d10")]
        [TestCase(RaceConstants.BaseRaces.LightfootHalfling, "5d20")]
        [TestCase(RaceConstants.BaseRaces.Lizardfolk, "2d20")]
        [TestCase(RaceConstants.BaseRaces.Locathah, "2d20")]
        [TestCase(RaceConstants.BaseRaces.Merfolk, "3d10")]
        [TestCase(RaceConstants.BaseRaces.Merrow, "3d20")]
        [TestCase(RaceConstants.BaseRaces.MindFlayer, "3d10")]
        [TestCase(RaceConstants.BaseRaces.Minotaur, "2d10")]
        [TestCase(RaceConstants.BaseRaces.MountainDwarf, "2d100")]
        [TestCase(RaceConstants.BaseRaces.Ogre, "3d20")]
        [TestCase(RaceConstants.BaseRaces.OgreMage, "3d20")]
        [TestCase(RaceConstants.BaseRaces.Orc, "1d20")]
        [TestCase(RaceConstants.BaseRaces.Pixie, "10d100")]
        [TestCase(RaceConstants.BaseRaces.Rakshasa, "2d20")]
        [TestCase(RaceConstants.BaseRaces.RockGnome, "3d100")]
        [TestCase(RaceConstants.BaseRaces.Sahuagin, "3d20")]
        [TestCase(RaceConstants.BaseRaces.Satyr, "4d12")]
        [TestCase(RaceConstants.BaseRaces.Scorpionfolk, "2d10")]
        [TestCase(RaceConstants.BaseRaces.Scrag, "2d20")]
        [TestCase(RaceConstants.BaseRaces.StoneGiant, "2d100")]
        [TestCase(RaceConstants.BaseRaces.StormGiant, "15d10")]
        [TestCase(RaceConstants.BaseRaces.Svirfneblin, "3d100")]
        [TestCase(RaceConstants.BaseRaces.TallfellowHalfling, "5d20")]
        [TestCase(RaceConstants.BaseRaces.Tiefling, "2d20")]
        [TestCase(RaceConstants.BaseRaces.Troglodyte, "1d20")]
        [TestCase(RaceConstants.BaseRaces.Troll, "2d20")]
        [TestCase(RaceConstants.BaseRaces.WildElf, "4d100")]
        [TestCase(RaceConstants.BaseRaces.WoodElf, "4d100")]
        [TestCase(RaceConstants.BaseRaces.YuanTiAbomination, "3d20")]
        [TestCase(RaceConstants.BaseRaces.YuanTiHalfblood, "3d20")]
        [TestCase(RaceConstants.BaseRaces.YuanTiPureblood, "3d20")]
        public void MaximumAgeRoll(string name, string ageRoll)
        {
            base.AssertDistinctCollection(name, ageRoll);
        }

        [TestCase(RaceConstants.BaseRaces.BlueSlaad)]
        [TestCase(RaceConstants.BaseRaces.DeathSlaad)]
        [TestCase(RaceConstants.BaseRaces.Gargoyle)]
        [TestCase(RaceConstants.BaseRaces.GraySlaad)]
        [TestCase(RaceConstants.BaseRaces.GreenSlaad)]
        [TestCase(RaceConstants.BaseRaces.Kapoacinth)]
        [TestCase(RaceConstants.BaseRaces.Mummy)]
        [TestCase(RaceConstants.BaseRaces.RedSlaad)]
        public void ImmortalCreatures(string creature)
        {
            base.AssertDistinctCollection(creature, RaceConstants.Ages.Ageless.ToString());
        }
    }
}
