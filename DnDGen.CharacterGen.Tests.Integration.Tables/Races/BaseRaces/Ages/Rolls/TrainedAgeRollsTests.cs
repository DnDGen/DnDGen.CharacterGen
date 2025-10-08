using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Races.BaseRaces.Ages.Rolls
{
    [TestFixture]
    public class TrainedAgeRollsTests : CollectionTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Formattable.Collection.CLASSTYPEAgeRolls, CharacterClassConstants.TrainingTypes.Trained); }
        }

        [Test]
        public override void CollectionNames()
        {
            var baseRaceGroups = GetTable(TableNameConstants.Set.Collection.BaseRaceGroups);
            AssertCollectionNames(baseRaceGroups[GroupConstants.All]);
        }

        [TestCase(RaceConstants.BaseRaces.Aasimar, "2d8")]
        [TestCase(RaceConstants.BaseRaces.AquaticElf, "10d6")]
        [TestCase(RaceConstants.BaseRaces.Azer, "7d6")]
        [TestCase(RaceConstants.BaseRaces.BlueSlaad, "5d12")]
        [TestCase(RaceConstants.BaseRaces.Bugbear, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Centaur, "3d6")]
        [TestCase(RaceConstants.BaseRaces.CloudGiant, "5d12")]
        [TestCase(RaceConstants.BaseRaces.DeathSlaad, "5d12")]
        [TestCase(RaceConstants.BaseRaces.DeepDwarf, "7d6")]
        [TestCase(RaceConstants.BaseRaces.DeepHalfling, "4d6")]
        [TestCase(RaceConstants.BaseRaces.Derro, "7d6")]
        [TestCase(RaceConstants.BaseRaces.Doppelganger, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Drow, "10d6")]
        [TestCase(RaceConstants.BaseRaces.DuergarDwarf, "7d6")]
        [TestCase(RaceConstants.BaseRaces.FireGiant, "5d12")]
        [TestCase(RaceConstants.BaseRaces.ForestGnome, "9d6")]
        [TestCase(RaceConstants.BaseRaces.FrostGiant, "5d12")]
        [TestCase(RaceConstants.BaseRaces.Gargoyle, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Githyanki, "4d6")]
        [TestCase(RaceConstants.BaseRaces.Githzerai, "4d6")]
        [TestCase(RaceConstants.BaseRaces.Gnoll, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Goblin, "2d6")]
        [TestCase(RaceConstants.BaseRaces.GrayElf, "10d6")]
        [TestCase(RaceConstants.BaseRaces.GraySlaad, "5d12")]
        [TestCase(RaceConstants.BaseRaces.GreenSlaad, "5d12")]
        [TestCase(RaceConstants.BaseRaces.Grimlock, "2d6")]
        [TestCase(RaceConstants.BaseRaces.HalfElf, "3d6")]
        [TestCase(RaceConstants.BaseRaces.HalfOrc, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Harpy, "2d4")]
        [TestCase(RaceConstants.BaseRaces.HighElf, "10d6")]
        [TestCase(RaceConstants.BaseRaces.HillDwarf, "7d6")]
        [TestCase(RaceConstants.BaseRaces.HillGiant, "5d12")]
        [TestCase(RaceConstants.BaseRaces.Hobgoblin, "2d6")]
        [TestCase(RaceConstants.BaseRaces.HoundArchon, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Human, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Janni, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Kapoacinth, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Kobold, "2d6")]
        [TestCase(RaceConstants.BaseRaces.KuoToa, "3d6")]
        [TestCase(RaceConstants.BaseRaces.LightfootHalfling, "3d6")]
        [TestCase(RaceConstants.BaseRaces.Lizardfolk, "1d10")]
        [TestCase(RaceConstants.BaseRaces.Locathah, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Merfolk, "3d6")]
        [TestCase(RaceConstants.BaseRaces.Merrow, "4d6")]
        [TestCase(RaceConstants.BaseRaces.MindFlayer, "5d8")]
        [TestCase(RaceConstants.BaseRaces.Minotaur, "2d6")]
        [TestCase(RaceConstants.BaseRaces.MountainDwarf, "7d6")]
        [TestCase(RaceConstants.BaseRaces.Mummy, "10d20")]
        [TestCase(RaceConstants.BaseRaces.Ogre, "4d6")]
        [TestCase(RaceConstants.BaseRaces.OgreMage, "4d6")]
        [TestCase(RaceConstants.BaseRaces.Orc, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Pixie, "2d4")]
        [TestCase(RaceConstants.BaseRaces.Rakshasa, "2d6")]
        [TestCase(RaceConstants.BaseRaces.RedSlaad, "5d12")]
        [TestCase(RaceConstants.BaseRaces.RockGnome, "9d6")]
        [TestCase(RaceConstants.BaseRaces.Sahuagin, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Satyr, "2d12")]
        [TestCase(RaceConstants.BaseRaces.Scorpionfolk, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Scrag, "2d6")]
        [TestCase(RaceConstants.BaseRaces.StoneGiant, "5d12")]
        [TestCase(RaceConstants.BaseRaces.StormGiant, "5d12")]
        [TestCase(RaceConstants.BaseRaces.Svirfneblin, "9d6")]
        [TestCase(RaceConstants.BaseRaces.TallfellowHalfling, "3d6")]
        [TestCase(RaceConstants.BaseRaces.Tiefling, "2d8")]
        [TestCase(RaceConstants.BaseRaces.Troglodyte, "2d6")]
        [TestCase(RaceConstants.BaseRaces.Troll, "2d6")]
        [TestCase(RaceConstants.BaseRaces.WildElf, "10d6")]
        [TestCase(RaceConstants.BaseRaces.WoodElf, "10d6")]
        [TestCase(RaceConstants.BaseRaces.YuanTiAbomination, "2d6")]
        [TestCase(RaceConstants.BaseRaces.YuanTiHalfblood, "2d6")]
        [TestCase(RaceConstants.BaseRaces.YuanTiPureblood, "2d6")]
        public void TrainedAgeRoll(string name, string ageRoll)
        {
            AssertDistinctCollection(name, ageRoll);
        }
    }
}
