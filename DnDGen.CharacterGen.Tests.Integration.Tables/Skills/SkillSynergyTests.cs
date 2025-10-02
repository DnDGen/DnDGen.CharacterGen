using DnDGen.CharacterGen.Skills;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Skills
{
    [TestFixture]
    public class SkillSynergyTests : CollectionTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Set.Collection.SkillSynergy; }
        }

        [Test]
        public override void CollectionNames()
        {
            var featFoci = GetTable(TableNameConstants.Set.Collection.FeatFoci);
            AssertCollectionNames(featFoci[GroupConstants.Skills]);
        }

        [TestCase(SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Appraiser,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Trader)]
        [TestCase(SkillConstants.Balance)]
        [TestCase(SkillConstants.Bluff,
            SkillConstants.Diplomacy,
            SkillConstants.Disguise,
            SkillConstants.Intimidate,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Dowser,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Soothsayer,
            SkillConstants.SleightOfHand)]
        [TestCase(SkillConstants.Climb)]
        [TestCase(SkillConstants.Concentration)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Alchemy,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Alchemist,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Embalmer)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Armorsmithing,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Armorer,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Blacksmithing,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Blacksmith,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Bookbinding,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Bookbinder,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Bowmaking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Bowyer,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Fletcher)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Brassmaking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Brazier,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Brewing,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Brewer,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Candlemaking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Chandler,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Cloth,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Coppersmithing,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Coppersmith,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Dyemaking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Dyer)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Gemcutting,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Gemcutter)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Glass,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Goldsmithing,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Goldsmith)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Hatmaking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Haberdasher)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Hornworking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Horner)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Jewelmaking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Jeweler)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Leather,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Locksmithing,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Locksmith)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Mapmaking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Cartographer,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Milling,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Miller)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Painting,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Limner,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Painter)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Parchmentmaking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Parchmentmaker)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Pewtermaking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Pewterer)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Potterymaking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Potter)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Sculpting,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Sculptor)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Shipmaking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Shipwright)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Shoemaking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Cobbler,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Silversmithing,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Silversmith)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Skinning,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Skinner)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Soapmaking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Soapmaker)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Stonemasonry,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Tanning,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Tanner)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Trapmaking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Weaponsmithing,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Weaponsmith)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Weaving,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Weaver)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Wheelmaking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Wheelwright)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Winemaking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Vintner)]
        [TestCase(SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Woodworking,
            SkillConstants.Appraise,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Carpenter,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Cartwright,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Coffinmaker,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman)]
        [TestCase(SkillConstants.DecipherScript,
            SkillConstants.UseMagicDevice)]
        [TestCase(SkillConstants.Diplomacy,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Barrister,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Butler,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.CityGuide,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Footman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Governess,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Matchmaker,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Valet)]
        [TestCase(SkillConstants.DisableDevice)]
        [TestCase(SkillConstants.Disguise)]
        [TestCase(SkillConstants.EscapeArtist,
            SkillConstants.UseRope)]
        [TestCase(SkillConstants.Forgery)]
        [TestCase(SkillConstants.GatherInformation)]
        [TestCase(SkillConstants.HandleAnimal,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.AnimalGroomer,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.AnimalTrainer,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.ExoticAnimalTrainer,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Shepherd,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Teamster,
            SkillConstants.Ride)]
        [TestCase(SkillConstants.Heal,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Healer,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Masseuse,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Midwife)]
        [TestCase(SkillConstants.Hide)]
        [TestCase(SkillConstants.Intimidate,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.SailorMate)]
        [TestCase(SkillConstants.Jump,
            SkillConstants.Tumble)]
        [TestCase(SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Arcana,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Adviser,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Sage,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Teacher,
            SkillConstants.Spellcraft)]
        [TestCase(SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.ArchitectureAndEngineering,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Adviser,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Architect,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Engineer,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Sage,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Teacher,
            SkillConstants.Search)]
        [TestCase(SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Dungeoneering,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Adviser,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Sage,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Teacher,
            SkillConstants.Survival)]
        [TestCase(SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Geography,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Adviser,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Sage,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Teacher,
            SkillConstants.Survival)]
        [TestCase(SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.History,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Adviser,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Sage,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Teacher)]
        [TestCase(SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Local,
            SkillConstants.GatherInformation,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Adviser,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.CityGuide,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.LocalCourier,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.OutOfTownCourier,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Sage,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Teacher,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.WildernessGuide)]
        [TestCase(SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Nature,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Adviser,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Apothecary,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Farmer,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Sage,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Teacher,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.WildernessGuide,
            SkillConstants.Survival)]
        [TestCase(SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.NobilityAndRoyalty,
            SkillConstants.Diplomacy,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Adviser,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Butler,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Footman,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Governess,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Maid,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Sage,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Teacher)]
        [TestCase(SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Religion,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Adviser,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Sage,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Teacher)]
        [TestCase(SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.ThePlanes,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Adviser,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Sage,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Teacher,
            SkillConstants.Survival)]
        [TestCase(SkillConstants.Listen)]
        [TestCase(SkillConstants.MoveSilently)]
        [TestCase(SkillConstants.OpenLock)]
        [TestCase(SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.Act,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Entertainer)]
        [TestCase(SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.Comedy,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Entertainer)]
        [TestCase(SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.Dance,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Entertainer)]
        [TestCase(SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.KeyboardInstruments,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Entertainer)]
        [TestCase(SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.Oratory,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Entertainer)]
        [TestCase(SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.PercussionInstruments,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Entertainer)]
        [TestCase(SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.Sing,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Entertainer)]
        [TestCase(SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.StringInstruments,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Entertainer)]
        [TestCase(SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.WindInstruments,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Entertainer)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Adviser,
            SkillConstants.Diplomacy,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Arcana,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.ArchitectureAndEngineering,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Dungeoneering,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Geography,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.History,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Local,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Nature,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.NobilityAndRoyalty,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Religion,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.ThePlanes)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Alchemist,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Alchemy)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.AnimalGroomer,
            SkillConstants.HandleAnimal)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.AnimalTrainer,
            SkillConstants.HandleAnimal)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Apothecary,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Nature)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Appraiser,
            SkillConstants.Appraise)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Architect,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.ArchitectureAndEngineering)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Armorer,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Armorsmithing)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Barrister,
            SkillConstants.Diplomacy)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Blacksmith,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Blacksmithing)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Bookbinder,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Bookbinding)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Bowyer,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Bowmaking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Brazier,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Brassmaking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Brewer,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Brewing)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Butler,
            SkillConstants.Diplomacy,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.NobilityAndRoyalty)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Carpenter,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Woodworking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Cartographer,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Mapmaking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Cartwright,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Woodworking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Chandler,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Candlemaking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.CityGuide,
            SkillConstants.Diplomacy,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Local)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Clerk)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Cobbler,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Shoemaking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Coffinmaker,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Woodworking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Coiffeur)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Cook)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Coppersmith,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Coppersmithing)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Dowser,
            SkillConstants.Bluff,
            SkillConstants.Survival)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Dyer,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Dyemaking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Embalmer,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Alchemy)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Engineer,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.ArchitectureAndEngineering)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Entertainer,
            SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.Act,
            SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.Comedy,
            SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.Dance,
            SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.KeyboardInstruments,
            SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.Oratory,
            SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.PercussionInstruments,
            SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.Sing,
            SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.StringInstruments,
            SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.WindInstruments)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.ExoticAnimalTrainer,
            SkillConstants.HandleAnimal)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Farmer,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Nature)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Fletcher,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Bowmaking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Footman,
            SkillConstants.Diplomacy,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.NobilityAndRoyalty)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Gemcutter,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Gemcutting)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Goldsmith,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Goldsmithing)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Governess,
            SkillConstants.Diplomacy,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.NobilityAndRoyalty)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Haberdasher,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Hatmaking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Healer,
            SkillConstants.Heal)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Horner,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Hornworking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Hunter,
            SkillConstants.Survival)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Interpreter)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Jeweler,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Jewelmaking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Laborer)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Launderer)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Limner,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Painting)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.LocalCourier,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Local)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Locksmith,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Locksmithing)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Maid,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.NobilityAndRoyalty)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Masseuse,
            SkillConstants.Heal)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Matchmaker,
            SkillConstants.Diplomacy,
            SkillConstants.SenseMotive)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Midwife,
            SkillConstants.Heal)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Miller,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Milling)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Navigator,
            SkillConstants.Survival)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Nursemaid)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.OutOfTownCourier,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Local,
            SkillConstants.Ride)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Painter,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Painting)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Parchmentmaker,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Parchmentmaking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Pewterer,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Pewtermaking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Polisher)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Porter)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Potter,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Potterymaking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Sage,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Arcana,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.ArchitectureAndEngineering,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Dungeoneering,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Geography,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.History,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Local,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Nature,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.NobilityAndRoyalty,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Religion,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.ThePlanes)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.SailorCrewmember,
            SkillConstants.Swim)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.SailorMate,
            SkillConstants.Intimidate,
            SkillConstants.Swim)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Scribe)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Sculptor,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Sculpting)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Shepherd,
            SkillConstants.HandleAnimal)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Shipwright,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Shipmaking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Silversmith,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Silversmithing)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Skinner,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Skinning)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Soapmaker,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Soapmaking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Soothsayer,
            SkillConstants.Bluff)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Tanner,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Tanning)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Teacher,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Arcana,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.ArchitectureAndEngineering,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Dungeoneering,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Geography,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.History,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Local,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Nature,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.NobilityAndRoyalty,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Religion,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.ThePlanes)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Teamster,
            SkillConstants.HandleAnimal,
            SkillConstants.Ride)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Trader,
            SkillConstants.Appraise,
            SkillConstants.SenseMotive)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Valet,
            SkillConstants.Diplomacy)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Vintner,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Winemaking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Weaponsmith,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Weaponsmithing)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Weaver,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Weaving)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Wheelwright,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Wheelmaking)]
        [TestCase(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.WildernessGuide,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Local,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Nature)]
        [TestCase(SkillConstants.Ride,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.OutOfTownCourier,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Teamster)]
        [TestCase(SkillConstants.Search,
            SkillConstants.Survival)]
        [TestCase(SkillConstants.SenseMotive,
            SkillConstants.Diplomacy,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Matchmaker,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Trader)]
        [TestCase(SkillConstants.SleightOfHand)]
        [TestCase(SkillConstants.Spellcraft,
            SkillConstants.UseMagicDevice)]
        [TestCase(SkillConstants.Spot)]
        [TestCase(SkillConstants.Survival,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Nature,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Dowser,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Hunter,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Navigator)]
        [TestCase(SkillConstants.Swim,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.SailorCrewmember,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.SailorMate)]
        [TestCase(SkillConstants.Tumble,
            SkillConstants.Balance,
            SkillConstants.Jump)]
        [TestCase(SkillConstants.UseMagicDevice,
            SkillConstants.Spellcraft)]
        [TestCase(SkillConstants.UseRope,
            SkillConstants.Climb,
            SkillConstants.EscapeArtist)]
        public void SynergySkills(string skill, params string[] synergies)
        {
            base.AssertDistinctCollection(skill, synergies);
        }

        [Test]
        public void CraftsmanSkillSynergy()
        {
            var skills = new[]
            {
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Alchemy,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Armorsmithing,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Blacksmithing,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Bookbinding,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Bowmaking,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Brassmaking,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Brewing,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Candlemaking,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Cloth,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Coppersmithing,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Dyemaking,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Gemcutting,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Glass,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Goldsmithing,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Hatmaking,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Hornworking,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Jewelmaking,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Leather,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Locksmithing,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Mapmaking,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Milling,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Painting,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Parchmentmaking,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Pewtermaking,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Potterymaking,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Sculpting,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Shipmaking,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Shoemaking,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Silversmithing,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Skinning,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Soapmaking,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Stonemasonry,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Tanning,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Trapmaking,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Weaponsmithing,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Weaving,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Wheelmaking,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Winemaking,
                SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Woodworking,
            };

            AssertDistinctCollection(SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Craftsman, skills);
        }
    }
}