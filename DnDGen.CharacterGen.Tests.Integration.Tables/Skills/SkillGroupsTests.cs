using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Skills;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Skills
{
    [TestFixture]
    public class SkillGroupsTests : CollectionTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Set.Collection.SkillGroups; }
        }

        [Test]
        public override void CollectionNames()
        {
            var names = new[]
            {
                FeatConstants.Acrobatic,
                FeatConstants.Agile,
                FeatConstants.Alertness,
                FeatConstants.AnimalAffinity,
                FeatConstants.Athletic,
                FeatConstants.Deceitful,
                FeatConstants.DeftHands,
                FeatConstants.Diligent,
                FeatConstants.Investigator,
                FeatConstants.MagicalAptitude,
                FeatConstants.NatureSense,
                FeatConstants.Negotiator,
                FeatConstants.NimbleFingers,
                FeatConstants.Persuasive,
                FeatConstants.SelfSufficient,
                FeatConstants.Stealthy,
                GroupConstants.All,
                GroupConstants.ArmorCheckPenalty,
                GroupConstants.Untrained,
                SkillConstants.Craft,
                SkillConstants.Knowledge,
                SkillConstants.Perform,
                SkillConstants.Profession,
            };

            AssertCollectionNames(names);
        }

        [TestCase(FeatConstants.Acrobatic,
            SkillConstants.Jump,
            SkillConstants.Tumble)]
        [TestCase(FeatConstants.Agile,
            SkillConstants.Balance,
            SkillConstants.EscapeArtist)]
        [TestCase(FeatConstants.Alertness,
            SkillConstants.Listen,
            SkillConstants.Spot)]
        [TestCase(FeatConstants.AnimalAffinity,
            SkillConstants.HandleAnimal,
            SkillConstants.Ride)]
        [TestCase(FeatConstants.Athletic,
            SkillConstants.Climb,
            SkillConstants.Swim)]
        [TestCase(FeatConstants.Deceitful,
            SkillConstants.Disguise,
            SkillConstants.Forgery)]
        [TestCase(FeatConstants.DeftHands,
            SkillConstants.SleightOfHand,
            SkillConstants.UseRope)]
        [TestCase(FeatConstants.Diligent,
            SkillConstants.Appraise,
            SkillConstants.DecipherScript)]
        [TestCase(FeatConstants.Investigator,
            SkillConstants.GatherInformation,
            SkillConstants.Search)]
        [TestCase(FeatConstants.MagicalAptitude,
            SkillConstants.Spellcraft,
            SkillConstants.UseMagicDevice)]
        [TestCase(FeatConstants.NatureSense,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Nature,
            SkillConstants.Survival)]
        [TestCase(FeatConstants.Negotiator,
            SkillConstants.Diplomacy,
            SkillConstants.SenseMotive)]
        [TestCase(FeatConstants.NimbleFingers,
            SkillConstants.DisableDevice,
            SkillConstants.OpenLock)]
        [TestCase(FeatConstants.Persuasive,
            SkillConstants.Bluff,
            SkillConstants.Intimidate)]
        [TestCase(FeatConstants.SelfSufficient,
            SkillConstants.Heal,
            SkillConstants.Survival)]
        [TestCase(FeatConstants.Stealthy,
            SkillConstants.Hide,
            SkillConstants.MoveSilently)]
        [TestCase(GroupConstants.ArmorCheckPenalty,
            SkillConstants.Balance,
            SkillConstants.Climb,
            SkillConstants.EscapeArtist,
            SkillConstants.Hide,
            SkillConstants.Jump,
            SkillConstants.MoveSilently,
            SkillConstants.SleightOfHand,
            SkillConstants.Swim,
            SkillConstants.Tumble)]
        public void SkillGroup(string name, params string[] skills)
        {
            base.AssertDistinctCollection(name, skills);
        }

        [Test]
        public void AllSkills()
        {
            var skills = new[]
            {
                SkillConstants.Appraise,
                SkillConstants.Balance,
                SkillConstants.Bluff,
                SkillConstants.Climb,
                SkillConstants.Concentration,
                SkillConstants.Craft,
                SkillConstants.DecipherScript,
                SkillConstants.Diplomacy,
                SkillConstants.DisableDevice,
                SkillConstants.Disguise,
                SkillConstants.EscapeArtist,
                SkillConstants.Forgery,
                SkillConstants.GatherInformation,
                SkillConstants.HandleAnimal,
                SkillConstants.Heal,
                SkillConstants.Hide,
                SkillConstants.Intimidate,
                SkillConstants.Jump,
                SkillConstants.Knowledge,
                SkillConstants.Listen,
                SkillConstants.MoveSilently,
                SkillConstants.OpenLock,
                SkillConstants.Perform,
                SkillConstants.Ride,
                SkillConstants.Search,
                SkillConstants.SenseMotive,
                SkillConstants.SleightOfHand,
                SkillConstants.Spellcraft,
                SkillConstants.Spot,
                SkillConstants.Survival,
                SkillConstants.Swim,
                SkillConstants.Tumble,
                SkillConstants.UseMagicDevice,
                SkillConstants.UseRope,
            };

            base.AssertDistinctCollection(GroupConstants.All, skills);
        }

        [Test]
        public void UntrainedSkills()
        {
            var skills = new[]
            {
                SkillConstants.Appraise,
                SkillConstants.Balance,
                SkillConstants.Bluff,
                SkillConstants.Climb,
                SkillConstants.Concentration,
                SkillConstants.Craft,
                SkillConstants.Diplomacy,
                SkillConstants.Disguise,
                SkillConstants.EscapeArtist,
                SkillConstants.Forgery,
                SkillConstants.GatherInformation,
                SkillConstants.Heal,
                SkillConstants.Hide,
                SkillConstants.Intimidate,
                SkillConstants.Jump,
                SkillConstants.Listen,
                SkillConstants.MoveSilently,
                SkillConstants.Perform,
                SkillConstants.Ride,
                SkillConstants.Search,
                SkillConstants.SenseMotive,
                SkillConstants.Spot,
                SkillConstants.Survival,
                SkillConstants.Swim,
                SkillConstants.UseRope,
            };

            base.AssertDistinctCollection(GroupConstants.Untrained, skills);
        }

        [Test]
        public void CraftSkillFoci()
        {
            var foci = new[]
            {
                SkillConstants.Foci.Craft.Alchemy,
                SkillConstants.Foci.Craft.Armorsmithing,
                SkillConstants.Foci.Craft.Blacksmithing,
                SkillConstants.Foci.Craft.Bookbinding,
                SkillConstants.Foci.Craft.Bowmaking,
                SkillConstants.Foci.Craft.Brassmaking,
                SkillConstants.Foci.Craft.Brewing,
                SkillConstants.Foci.Craft.Candlemaking,
                SkillConstants.Foci.Craft.Cloth,
                SkillConstants.Foci.Craft.Coppersmithing,
                SkillConstants.Foci.Craft.Dyemaking,
                SkillConstants.Foci.Craft.Gemcutting,
                SkillConstants.Foci.Craft.Glass,
                SkillConstants.Foci.Craft.Goldsmithing,
                SkillConstants.Foci.Craft.Hatmaking,
                SkillConstants.Foci.Craft.Hornworking,
                SkillConstants.Foci.Craft.Jewelmaking,
                SkillConstants.Foci.Craft.Leather,
                SkillConstants.Foci.Craft.Locksmithing,
                SkillConstants.Foci.Craft.Mapmaking,
                SkillConstants.Foci.Craft.Milling,
                SkillConstants.Foci.Craft.Painting,
                SkillConstants.Foci.Craft.Parchmentmaking,
                SkillConstants.Foci.Craft.Pewtermaking,
                SkillConstants.Foci.Craft.Potterymaking,
                SkillConstants.Foci.Craft.Sculpting,
                SkillConstants.Foci.Craft.Shipmaking,
                SkillConstants.Foci.Craft.Shoemaking,
                SkillConstants.Foci.Craft.Silversmithing,
                SkillConstants.Foci.Craft.Skinning,
                SkillConstants.Foci.Craft.Soapmaking,
                SkillConstants.Foci.Craft.Stonemasonry,
                SkillConstants.Foci.Craft.Tanning,
                SkillConstants.Foci.Craft.Trapmaking,
                SkillConstants.Foci.Craft.Weaponsmithing,
                SkillConstants.Foci.Craft.Weaving,
                SkillConstants.Foci.Craft.Wheelmaking,
                SkillConstants.Foci.Craft.Winemaking,
                SkillConstants.Foci.Craft.Woodworking,
            };

            base.AssertDistinctCollection(SkillConstants.Craft, foci);
        }

        [Test]
        public void KnowledgeSkillFoci()
        {
            var foci = new[]
            {
                SkillConstants.Foci.Knowledge.Arcana,
                SkillConstants.Foci.Knowledge.ArchitectureAndEngineering,
                SkillConstants.Foci.Knowledge.Dungeoneering,
                SkillConstants.Foci.Knowledge.Geography,
                SkillConstants.Foci.Knowledge.History,
                SkillConstants.Foci.Knowledge.Local,
                SkillConstants.Foci.Knowledge.Nature,
                SkillConstants.Foci.Knowledge.NobilityAndRoyalty,
                SkillConstants.Foci.Knowledge.Religion,
                SkillConstants.Foci.Knowledge.ThePlanes,
            };

            base.AssertDistinctCollection(SkillConstants.Knowledge, foci);
        }

        [Test]
        public void PerformSkillFoci()
        {
            var foci = new[]
            {
                SkillConstants.Foci.Perform.Act,
                SkillConstants.Foci.Perform.Comedy,
                SkillConstants.Foci.Perform.Dance,
                SkillConstants.Foci.Perform.KeyboardInstruments,
                SkillConstants.Foci.Perform.Oratory,
                SkillConstants.Foci.Perform.PercussionInstruments,
                SkillConstants.Foci.Perform.Sing,
                SkillConstants.Foci.Perform.StringInstruments,
                SkillConstants.Foci.Perform.WindInstruments,
            };

            base.AssertDistinctCollection(SkillConstants.Perform, foci);
        }

        [Test]
        public void ProfessionSkillFoci()
        {
            var foci = new[]
            {
                SkillConstants.Foci.Profession.Adviser,
                SkillConstants.Foci.Profession.Alchemist,
                SkillConstants.Foci.Profession.AnimalGroomer,
                SkillConstants.Foci.Profession.AnimalTrainer,
                SkillConstants.Foci.Profession.Apothecary,
                SkillConstants.Foci.Profession.Appraiser,
                SkillConstants.Foci.Profession.Architect,
                SkillConstants.Foci.Profession.Armorer,
                SkillConstants.Foci.Profession.Barrister,
                SkillConstants.Foci.Profession.Blacksmith,
                SkillConstants.Foci.Profession.Bookbinder,
                SkillConstants.Foci.Profession.Bowyer,
                SkillConstants.Foci.Profession.Brazier,
                SkillConstants.Foci.Profession.Brewer,
                SkillConstants.Foci.Profession.Butler,
                SkillConstants.Foci.Profession.Carpenter,
                SkillConstants.Foci.Profession.Cartographer,
                SkillConstants.Foci.Profession.Cartwright,
                SkillConstants.Foci.Profession.Chandler,
                SkillConstants.Foci.Profession.CityGuide,
                SkillConstants.Foci.Profession.Clerk,
                SkillConstants.Foci.Profession.Cobbler,
                SkillConstants.Foci.Profession.Coffinmaker,
                SkillConstants.Foci.Profession.Coiffeur,
                SkillConstants.Foci.Profession.Cook,
                SkillConstants.Foci.Profession.Coppersmith,
                SkillConstants.Foci.Profession.Craftsman,
                SkillConstants.Foci.Profession.Dowser,
                SkillConstants.Foci.Profession.Dyer,
                SkillConstants.Foci.Profession.Embalmer,
                SkillConstants.Foci.Profession.Engineer,
                SkillConstants.Foci.Profession.Entertainer,
                SkillConstants.Foci.Profession.ExoticAnimalTrainer,
                SkillConstants.Foci.Profession.Farmer,
                SkillConstants.Foci.Profession.Fletcher,
                SkillConstants.Foci.Profession.Footman,
                SkillConstants.Foci.Profession.Gemcutter,
                SkillConstants.Foci.Profession.Goldsmith,
                SkillConstants.Foci.Profession.Governess,
                SkillConstants.Foci.Profession.Haberdasher,
                SkillConstants.Foci.Profession.Healer,
                SkillConstants.Foci.Profession.Horner,
                SkillConstants.Foci.Profession.Hunter,
                SkillConstants.Foci.Profession.Interpreter,
                SkillConstants.Foci.Profession.Jeweler,
                SkillConstants.Foci.Profession.Laborer,
                SkillConstants.Foci.Profession.Launderer,
                SkillConstants.Foci.Profession.Limner,
                SkillConstants.Foci.Profession.LocalCourier,
                SkillConstants.Foci.Profession.Locksmith,
                SkillConstants.Foci.Profession.Maid,
                SkillConstants.Foci.Profession.Masseuse,
                SkillConstants.Foci.Profession.Matchmaker,
                SkillConstants.Foci.Profession.Midwife,
                SkillConstants.Foci.Profession.Miller,
                SkillConstants.Foci.Profession.Navigator,
                SkillConstants.Foci.Profession.Nursemaid,
                SkillConstants.Foci.Profession.OutOfTownCourier,
                SkillConstants.Foci.Profession.Painter,
                SkillConstants.Foci.Profession.Parchmentmaker,
                SkillConstants.Foci.Profession.Pewterer,
                SkillConstants.Foci.Profession.Polisher,
                SkillConstants.Foci.Profession.Porter,
                SkillConstants.Foci.Profession.Potter,
                SkillConstants.Foci.Profession.Sage,
                SkillConstants.Foci.Profession.SailorCrewmember,
                SkillConstants.Foci.Profession.SailorMate,
                SkillConstants.Foci.Profession.Scribe,
                SkillConstants.Foci.Profession.Sculptor,
                SkillConstants.Foci.Profession.Shepherd,
                SkillConstants.Foci.Profession.Shipwright,
                SkillConstants.Foci.Profession.Silversmith,
                SkillConstants.Foci.Profession.Skinner,
                SkillConstants.Foci.Profession.Soapmaker,
                SkillConstants.Foci.Profession.Soothsayer,
                SkillConstants.Foci.Profession.Tanner,
                SkillConstants.Foci.Profession.Teacher,
                SkillConstants.Foci.Profession.Teamster,
                SkillConstants.Foci.Profession.Trader,
                SkillConstants.Foci.Profession.Valet,
                SkillConstants.Foci.Profession.Vintner,
                SkillConstants.Foci.Profession.Weaponsmith,
                SkillConstants.Foci.Profession.Weaver,
                SkillConstants.Foci.Profession.Wheelwright,
                SkillConstants.Foci.Profession.WildernessGuide,
            };

            base.AssertDistinctCollection(SkillConstants.Profession, foci);
        }
    }
}