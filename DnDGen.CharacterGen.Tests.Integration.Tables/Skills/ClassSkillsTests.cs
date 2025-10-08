using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Skills;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Skills
{
    [TestFixture]
    public class ClassSkillsTests : CollectionTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Set.Collection.ClassSkills; }
        }

        [Test]
        public override void CollectionNames()
        {
            var baseRaceGroups = GetTable(TableNameConstants.Set.Collection.BaseRaceGroups);
            var classGroups = GetTable(TableNameConstants.Set.Collection.ClassNameGroups);
            var skillGroups = GetTable(TableNameConstants.Set.Collection.SkillGroups);

            var names = new[]
            {
                CharacterClassConstants.Domains.Air,
                CharacterClassConstants.Domains.Animal,
                CharacterClassConstants.Domains.Chaos,
                CharacterClassConstants.Domains.Death,
                CharacterClassConstants.Domains.Destruction,
                CharacterClassConstants.Domains.Earth,
                CharacterClassConstants.Domains.Evil,
                CharacterClassConstants.Domains.Fire,
                CharacterClassConstants.Domains.Good,
                CharacterClassConstants.Domains.Healing,
                CharacterClassConstants.Domains.Knowledge,
                CharacterClassConstants.Domains.Law,
                CharacterClassConstants.Domains.Luck,
                CharacterClassConstants.Domains.Magic,
                CharacterClassConstants.Domains.Plant,
                CharacterClassConstants.Domains.Protection,
                CharacterClassConstants.Domains.Strength,
                CharacterClassConstants.Domains.Sun,
                CharacterClassConstants.Domains.Travel,
                CharacterClassConstants.Domains.Trickery,
                CharacterClassConstants.Domains.War,
                CharacterClassConstants.Domains.Water,
                CharacterClassConstants.Schools.Abjuration,
                CharacterClassConstants.Schools.Conjuration,
                CharacterClassConstants.Schools.Divination,
                CharacterClassConstants.Schools.Enchantment,
                CharacterClassConstants.Schools.Evocation,
                CharacterClassConstants.Schools.Illusion,
                CharacterClassConstants.Schools.Necromancy,
                CharacterClassConstants.Schools.Transmutation,
            };

            names = names.Union(classGroups[GroupConstants.All])
                .Union(skillGroups[SkillConstants.Profession])
                .Union(baseRaceGroups[GroupConstants.All]).ToArray();

            AssertCollectionNames(names);
        }

        [TestCase(CharacterClassConstants.Adept,
            SkillConstants.Concentration,
            SkillConstants.Craft,
            SkillConstants.HandleAnimal,
            SkillConstants.Heal,
            SkillConstants.Knowledge + GroupConstants.All,
            SkillConstants.Profession,
            SkillConstants.Spellcraft,
            SkillConstants.Survival)]
        [TestCase(CharacterClassConstants.Aristocrat,
            SkillConstants.Appraise,
            SkillConstants.Bluff,
            SkillConstants.Diplomacy,
            SkillConstants.Disguise,
            SkillConstants.Forgery,
            SkillConstants.GatherInformation,
            SkillConstants.HandleAnimal,
            SkillConstants.Intimidate,
            SkillConstants.Knowledge + GroupConstants.All,
            SkillConstants.Listen,
            SkillConstants.Perform,
            SkillConstants.Ride,
            SkillConstants.SenseMotive,
            SkillConstants.Spot,
            SkillConstants.Swim,
            SkillConstants.Survival)]
        [TestCase(CharacterClassConstants.Barbarian,
            SkillConstants.Climb,
            SkillConstants.Craft,
            SkillConstants.HandleAnimal,
            SkillConstants.Intimidate,
            SkillConstants.Jump,
            SkillConstants.Listen,
            SkillConstants.Ride,
            SkillConstants.Survival,
            SkillConstants.Swim)]
        [TestCase(CharacterClassConstants.Bard,
            SkillConstants.Appraise,
            SkillConstants.Balance,
            SkillConstants.Bluff,
            SkillConstants.Climb,
            SkillConstants.Concentration,
            SkillConstants.Craft,
            SkillConstants.DecipherScript,
            SkillConstants.Diplomacy,
            SkillConstants.Disguise,
            SkillConstants.EscapeArtist,
            SkillConstants.GatherInformation,
            SkillConstants.Hide,
            SkillConstants.Jump,
            SkillConstants.Knowledge + GroupConstants.All,
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.Perform,
            SkillConstants.Profession,
            SkillConstants.SenseMotive,
            SkillConstants.SleightOfHand,
            SkillConstants.Spellcraft,
            SkillConstants.Swim,
            SkillConstants.Tumble,
            SkillConstants.UseMagicDevice)]
        [TestCase(CharacterClassConstants.Cleric,
            SkillConstants.Concentration,
            SkillConstants.Craft,
            SkillConstants.Diplomacy,
            SkillConstants.Heal,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Arcana,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.History,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Religion,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.ThePlanes,
            SkillConstants.Profession,
            SkillConstants.Spellcraft)]
        [TestCase(CharacterClassConstants.Commoner,
            SkillConstants.Climb,
            SkillConstants.Craft,
            SkillConstants.HandleAnimal,
            SkillConstants.Jump,
            SkillConstants.Listen,
            SkillConstants.Profession,
            SkillConstants.Ride,
            SkillConstants.Spot,
            SkillConstants.Swim,
            SkillConstants.UseRope)]
        [TestCase(CharacterClassConstants.Druid,
            SkillConstants.Concentration,
            SkillConstants.Craft,
            SkillConstants.Diplomacy,
            SkillConstants.HandleAnimal,
            SkillConstants.Heal,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Nature,
            SkillConstants.Listen,
            SkillConstants.Profession,
            SkillConstants.Ride,
            SkillConstants.Spellcraft,
            SkillConstants.Spot,
            SkillConstants.Survival,
            SkillConstants.Swim)]
        [TestCase(CharacterClassConstants.Expert)]
        [TestCase(CharacterClassConstants.Fighter,
            SkillConstants.Climb,
            SkillConstants.Craft,
            SkillConstants.HandleAnimal,
            SkillConstants.Intimidate,
            SkillConstants.Jump,
            SkillConstants.Ride,
            SkillConstants.Swim)]
        [TestCase(CharacterClassConstants.Monk,
            SkillConstants.Balance,
            SkillConstants.Climb,
            SkillConstants.Concentration,
            SkillConstants.Craft,
            SkillConstants.Diplomacy,
            SkillConstants.EscapeArtist,
            SkillConstants.Hide,
            SkillConstants.Jump,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Arcana,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Religion,
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.Perform,
            SkillConstants.Profession,
            SkillConstants.SenseMotive,
            SkillConstants.Spot,
            SkillConstants.Swim,
            SkillConstants.Tumble)]
        [TestCase(CharacterClassConstants.Paladin,
            SkillConstants.Concentration,
            SkillConstants.Craft,
            SkillConstants.Diplomacy,
            SkillConstants.HandleAnimal,
            SkillConstants.Heal,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.NobilityAndRoyalty,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Religion,
            SkillConstants.Profession,
            SkillConstants.Ride,
            SkillConstants.SenseMotive)]
        [TestCase(CharacterClassConstants.Ranger,
            SkillConstants.Climb,
            SkillConstants.Concentration,
            SkillConstants.Craft,
            SkillConstants.HandleAnimal,
            SkillConstants.Heal,
            SkillConstants.Hide,
            SkillConstants.Jump,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Dungeoneering,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Geography,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Nature,
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.Profession,
            SkillConstants.Ride,
            SkillConstants.Search,
            SkillConstants.Spot,
            SkillConstants.Survival,
            SkillConstants.Swim,
            SkillConstants.UseRope)]
        [TestCase(CharacterClassConstants.Rogue,
            SkillConstants.Appraise,
            SkillConstants.Balance,
            SkillConstants.Bluff,
            SkillConstants.Climb,
            SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Trapmaking,
            SkillConstants.DecipherScript,
            SkillConstants.Diplomacy,
            SkillConstants.DisableDevice,
            SkillConstants.Disguise,
            SkillConstants.EscapeArtist,
            SkillConstants.Forgery,
            SkillConstants.GatherInformation,
            SkillConstants.Hide,
            SkillConstants.Intimidate,
            SkillConstants.Jump,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Local,
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.OpenLock,
            SkillConstants.Perform,
            SkillConstants.Profession,
            SkillConstants.Search,
            SkillConstants.SenseMotive,
            SkillConstants.SleightOfHand,
            SkillConstants.Spot,
            SkillConstants.Swim,
            SkillConstants.Tumble,
            SkillConstants.UseMagicDevice,
            SkillConstants.UseRope)]
        [TestCase(CharacterClassConstants.Sorcerer,
            SkillConstants.Bluff,
            SkillConstants.Concentration,
            SkillConstants.Craft,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Arcana,
            SkillConstants.Profession,
            SkillConstants.Spellcraft)]
        [TestCase(CharacterClassConstants.Warrior,
            SkillConstants.Climb,
            SkillConstants.HandleAnimal,
            SkillConstants.Intimidate,
            SkillConstants.Jump,
            SkillConstants.Ride,
            SkillConstants.Swim)]
        [TestCase(CharacterClassConstants.Wizard,
            SkillConstants.Concentration,
            SkillConstants.Craft,
            SkillConstants.DecipherScript,
            SkillConstants.Knowledge + GroupConstants.All,
            SkillConstants.Profession,
            SkillConstants.Spellcraft)]
        [TestCase(CharacterClassConstants.Domains.Air)]
        [TestCase(CharacterClassConstants.Domains.Animal, SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Nature)]
        [TestCase(CharacterClassConstants.Domains.Chaos)]
        [TestCase(CharacterClassConstants.Domains.Death)]
        [TestCase(CharacterClassConstants.Domains.Destruction)]
        [TestCase(CharacterClassConstants.Domains.Earth)]
        [TestCase(CharacterClassConstants.Domains.Evil)]
        [TestCase(CharacterClassConstants.Domains.Fire)]
        [TestCase(CharacterClassConstants.Domains.Good)]
        [TestCase(CharacterClassConstants.Domains.Healing)]
        [TestCase(CharacterClassConstants.Domains.Knowledge, SkillConstants.Knowledge + GroupConstants.All)]
        [TestCase(CharacterClassConstants.Domains.Law)]
        [TestCase(CharacterClassConstants.Domains.Luck)]
        [TestCase(CharacterClassConstants.Domains.Magic)]
        [TestCase(CharacterClassConstants.Domains.Plant, SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Nature)]
        [TestCase(CharacterClassConstants.Domains.Protection)]
        [TestCase(CharacterClassConstants.Domains.Strength)]
        [TestCase(CharacterClassConstants.Domains.Sun)]
        [TestCase(CharacterClassConstants.Domains.Travel, SkillConstants.Survival)]
        [TestCase(CharacterClassConstants.Domains.Trickery,
            SkillConstants.Bluff,
            SkillConstants.Disguise,
            SkillConstants.Hide)]
        [TestCase(CharacterClassConstants.Domains.War)]
        [TestCase(CharacterClassConstants.Domains.Water)]
        [TestCase(CharacterClassConstants.Schools.Abjuration)]
        [TestCase(CharacterClassConstants.Schools.Conjuration)]
        [TestCase(CharacterClassConstants.Schools.Divination)]
        [TestCase(CharacterClassConstants.Schools.Enchantment)]
        [TestCase(CharacterClassConstants.Schools.Evocation)]
        [TestCase(CharacterClassConstants.Schools.Illusion)]
        [TestCase(CharacterClassConstants.Schools.Necromancy)]
        [TestCase(CharacterClassConstants.Schools.Transmutation)]
        [TestCase(RaceConstants.BaseRaces.Aasimar)]
        [TestCase(RaceConstants.BaseRaces.AquaticElf, SkillConstants.Swim)]
        [TestCase(RaceConstants.BaseRaces.Azer,
            SkillConstants.Appraise,
            SkillConstants.Climb,
            SkillConstants.Craft,
            SkillConstants.Hide,
            SkillConstants.Jump,
            SkillConstants.Listen,
            SkillConstants.Search,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.BlueSlaad,
            SkillConstants.Climb,
            SkillConstants.Hide,
            SkillConstants.Jump,
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.Bugbear,
            SkillConstants.Climb,
            SkillConstants.Hide,
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.Search,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.Centaur,
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.Spot,
            SkillConstants.Survival)]
        [TestCase(RaceConstants.BaseRaces.CloudGiant,
            SkillConstants.Climb,
            SkillConstants.Craft,
            SkillConstants.Diplomacy,
            SkillConstants.Intimidate,
            SkillConstants.Listen,
            SkillConstants.Perform,
            SkillConstants.SenseMotive,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.DeathSlaad,
            SkillConstants.Climb,
            SkillConstants.Concentration,
            SkillConstants.EscapeArtist,
            SkillConstants.Hide,
            SkillConstants.Intimidate,
            SkillConstants.Jump,
            SkillConstants.Knowledge + "2",
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.Search,
            SkillConstants.Spellcraft,
            SkillConstants.Spot,
            SkillConstants.Survival,
            SkillConstants.UseRope)]
        [TestCase(RaceConstants.BaseRaces.DeepDwarf)]
        [TestCase(RaceConstants.BaseRaces.DeepHalfling)]
        [TestCase(RaceConstants.BaseRaces.Derro,
            SkillConstants.Bluff,
            SkillConstants.Hide,
            SkillConstants.Listen,
            SkillConstants.MoveSilently)]
        [TestCase(RaceConstants.BaseRaces.Doppelganger,
            SkillConstants.Bluff,
            SkillConstants.Diplomacy,
            SkillConstants.Disguise,
            SkillConstants.Intimidate,
            SkillConstants.Listen,
            SkillConstants.SenseMotive,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.Drow)]
        [TestCase(RaceConstants.BaseRaces.DuergarDwarf)]
        [TestCase(RaceConstants.BaseRaces.FireGiant,
            SkillConstants.Climb,
            SkillConstants.Craft,
            SkillConstants.Intimidate,
            SkillConstants.Jump,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.ForestGnome)]
        [TestCase(RaceConstants.BaseRaces.FrostGiant,
            SkillConstants.Climb,
            SkillConstants.Craft,
            SkillConstants.Intimidate,
            SkillConstants.Jump,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.Gargoyle,
            SkillConstants.Hide,
            SkillConstants.Listen,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.Githyanki)]
        [TestCase(RaceConstants.BaseRaces.Githzerai)]
        [TestCase(RaceConstants.BaseRaces.Gnoll,
            SkillConstants.Listen,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.Goblin)]
        [TestCase(RaceConstants.BaseRaces.GrayElf)]
        [TestCase(RaceConstants.BaseRaces.GraySlaad,
            SkillConstants.Climb,
            SkillConstants.Concentration,
            SkillConstants.Hide,
            SkillConstants.Jump,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Arcana,
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.Search,
            SkillConstants.Spellcraft,
            SkillConstants.Spot,
            SkillConstants.Survival)]
        [TestCase(RaceConstants.BaseRaces.GreenSlaad,
            SkillConstants.Climb,
            SkillConstants.Concentration,
            SkillConstants.Hide,
            SkillConstants.Jump,
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.Search,
            SkillConstants.Spot,
            SkillConstants.Survival)]
        [TestCase(RaceConstants.BaseRaces.Grimlock,
            SkillConstants.Climb,
            SkillConstants.Hide,
            SkillConstants.Listen,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.HalfElf)]
        [TestCase(RaceConstants.BaseRaces.HalfOrc)]
        [TestCase(RaceConstants.BaseRaces.Harpy,
            SkillConstants.Bluff,
            SkillConstants.Intimidate,
            SkillConstants.Listen,
            SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.Sing,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.HighElf)]
        [TestCase(RaceConstants.BaseRaces.HillDwarf)]
        [TestCase(RaceConstants.BaseRaces.HillGiant,
            SkillConstants.Climb,
            SkillConstants.Jump,
            SkillConstants.Listen,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.Hobgoblin)]
        [TestCase(RaceConstants.BaseRaces.HoundArchon,
            SkillConstants.Concentration,
            SkillConstants.Hide,
            SkillConstants.Jump,
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.SenseMotive,
            SkillConstants.Spot,
            SkillConstants.Survival)]
        [TestCase(RaceConstants.BaseRaces.Human)]
        [TestCase(RaceConstants.BaseRaces.Janni,
            SkillConstants.Appraise,
            SkillConstants.Concentration,
            SkillConstants.Craft,
            SkillConstants.EscapeArtist,
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.Ride,
            SkillConstants.SenseMotive,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.Kapoacinth,
            SkillConstants.Hide,
            SkillConstants.Listen,
            SkillConstants.Spot,
            SkillConstants.Swim)]
        [TestCase(RaceConstants.BaseRaces.Kobold)]
        [TestCase(RaceConstants.BaseRaces.KuoToa,
            SkillConstants.Craft,
            SkillConstants.EscapeArtist,
            SkillConstants.Knowledge,
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.Search,
            SkillConstants.Spot,
            SkillConstants.Swim)]
        [TestCase(RaceConstants.BaseRaces.LightfootHalfling)]
        [TestCase(RaceConstants.BaseRaces.Lizardfolk,
            SkillConstants.Balance,
            SkillConstants.Jump,
            SkillConstants.Swim)]
        [TestCase(RaceConstants.BaseRaces.Locathah,
            SkillConstants.Craft,
            SkillConstants.Listen,
            SkillConstants.Spot,
            SkillConstants.Swim)]
        [TestCase(RaceConstants.BaseRaces.Merfolk,
            SkillConstants.Listen,
            SkillConstants.Spot,
            SkillConstants.Swim)]
        [TestCase(RaceConstants.BaseRaces.Merrow,
            SkillConstants.Listen,
            SkillConstants.Spot,
            SkillConstants.Swim)]
        [TestCase(RaceConstants.BaseRaces.MindFlayer,
            SkillConstants.Bluff,
            SkillConstants.Concentration,
            SkillConstants.Hide,
            SkillConstants.Intimidate,
            SkillConstants.Knowledge,
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.Minotaur,
            SkillConstants.Intimidate,
            SkillConstants.Jump,
            SkillConstants.Listen,
            SkillConstants.Search,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.MountainDwarf)]
        [TestCase(RaceConstants.BaseRaces.Mummy,
            SkillConstants.Hide,
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.Ogre,
            SkillConstants.Climb,
            SkillConstants.Listen,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.OgreMage,
            SkillConstants.Listen,
            SkillConstants.Spot,
            SkillConstants.Spellcraft,
            SkillConstants.Concentration)]
        [TestCase(RaceConstants.BaseRaces.Orc)]
        [TestCase(RaceConstants.BaseRaces.Pixie)]
        [TestCase(RaceConstants.BaseRaces.Rakshasa,
            SkillConstants.Bluff,
            SkillConstants.Disguise,
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.Perform,
            SkillConstants.SenseMotive,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.RedSlaad,
            SkillConstants.Climb,
            SkillConstants.Hide,
            SkillConstants.Jump,
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.RockGnome)]
        [TestCase(RaceConstants.BaseRaces.Sahuagin,
            SkillConstants.HandleAnimal,
            SkillConstants.Hide,
            SkillConstants.Listen,
            SkillConstants.Profession + "/" + SkillConstants.Foci.Profession.Hunter,
            SkillConstants.Ride,
            SkillConstants.Spot,
            SkillConstants.Survival,
            SkillConstants.Swim)]
        [TestCase(RaceConstants.BaseRaces.Satyr,
            SkillConstants.Bluff,
            SkillConstants.Hide,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Nature,
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.Perform + "/" + SkillConstants.Foci.Perform.WindInstruments,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.Scorpionfolk,
            SkillConstants.Diplomacy,
            SkillConstants.Intimidate,
            SkillConstants.Listen,
            SkillConstants.SenseMotive,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.Scrag, SkillConstants.Swim)]
        [TestCase(RaceConstants.BaseRaces.StoneGiant,
            SkillConstants.Climb,
            SkillConstants.Hide,
            SkillConstants.Listen,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.StormGiant,
            SkillConstants.Climb,
            SkillConstants.Concentration,
            SkillConstants.Craft,
            SkillConstants.Diplomacy,
            SkillConstants.Intimidate,
            SkillConstants.Jump,
            SkillConstants.Listen,
            SkillConstants.Perform,
            SkillConstants.SenseMotive,
            SkillConstants.Spot,
            SkillConstants.Swim)]
        [TestCase(RaceConstants.BaseRaces.Svirfneblin)]
        [TestCase(RaceConstants.BaseRaces.TallfellowHalfling)]
        [TestCase(RaceConstants.BaseRaces.Tiefling)]
        [TestCase(RaceConstants.BaseRaces.Troglodyte,
            SkillConstants.Listen,
            SkillConstants.Hide)]
        [TestCase(RaceConstants.BaseRaces.Troll)]
        [TestCase(RaceConstants.BaseRaces.WildElf)]
        [TestCase(RaceConstants.BaseRaces.WoodElf)]
        [TestCase(RaceConstants.BaseRaces.YuanTiAbomination,
            SkillConstants.Concentration,
            SkillConstants.Craft + "2",
            SkillConstants.Hide,
            SkillConstants.Knowledge + "2",
            SkillConstants.Listen,
            SkillConstants.MoveSilently,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.YuanTiHalfblood,
            SkillConstants.Concentration,
            SkillConstants.Craft + "2",
            SkillConstants.Hide,
            SkillConstants.Knowledge + "2",
            SkillConstants.Listen,
            SkillConstants.Spot)]
        [TestCase(RaceConstants.BaseRaces.YuanTiPureblood,
            SkillConstants.Concentration,
            SkillConstants.Disguise,
            SkillConstants.Hide,
            SkillConstants.Knowledge,
            SkillConstants.Listen,
            SkillConstants.Spot)]
        [TestCase(SkillConstants.Foci.Profession.Adviser,
            SkillConstants.Diplomacy,
            SkillConstants.Knowledge)]
        [TestCase(SkillConstants.Foci.Profession.Alchemist, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Alchemy)]
        [TestCase(SkillConstants.Foci.Profession.AnimalGroomer, SkillConstants.HandleAnimal)]
        [TestCase(SkillConstants.Foci.Profession.AnimalTrainer, SkillConstants.HandleAnimal)]
        [TestCase(SkillConstants.Foci.Profession.Apothecary, SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Nature)]
        [TestCase(SkillConstants.Foci.Profession.Appraiser, SkillConstants.Appraise)]
        [TestCase(SkillConstants.Foci.Profession.Architect, SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.ArchitectureAndEngineering)]
        [TestCase(SkillConstants.Foci.Profession.Armorer, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Armorsmithing)]
        [TestCase(SkillConstants.Foci.Profession.Barrister, SkillConstants.Diplomacy)]
        [TestCase(SkillConstants.Foci.Profession.Blacksmith, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Blacksmithing)]
        [TestCase(SkillConstants.Foci.Profession.Bookbinder, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Bookbinding)]
        [TestCase(SkillConstants.Foci.Profession.Bowyer, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Bowmaking)]
        [TestCase(SkillConstants.Foci.Profession.Brazier, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Brassmaking)]
        [TestCase(SkillConstants.Foci.Profession.Brewer, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Brewing)]
        [TestCase(SkillConstants.Foci.Profession.Butler,
            SkillConstants.Diplomacy,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.NobilityAndRoyalty)]
        [TestCase(SkillConstants.Foci.Profession.Carpenter, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Woodworking)]
        [TestCase(SkillConstants.Foci.Profession.Cartographer, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Mapmaking)]
        [TestCase(SkillConstants.Foci.Profession.Cartwright, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Woodworking)]
        [TestCase(SkillConstants.Foci.Profession.Chandler, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Candlemaking)]
        [TestCase(SkillConstants.Foci.Profession.CityGuide,
            SkillConstants.Diplomacy,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Local)]
        [TestCase(SkillConstants.Foci.Profession.Clerk)]
        [TestCase(SkillConstants.Foci.Profession.Cobbler, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Shoemaking)]
        [TestCase(SkillConstants.Foci.Profession.Coffinmaker, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Woodworking)]
        [TestCase(SkillConstants.Foci.Profession.Coiffeur)]
        [TestCase(SkillConstants.Foci.Profession.Cook)]
        [TestCase(SkillConstants.Foci.Profession.Coppersmith, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Coppersmithing)]
        [TestCase(SkillConstants.Foci.Profession.Craftsman, SkillConstants.Craft)]
        [TestCase(SkillConstants.Foci.Profession.Dowser,
            SkillConstants.Bluff,
            SkillConstants.Survival)]
        [TestCase(SkillConstants.Foci.Profession.Dyer, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Dyemaking)]
        [TestCase(SkillConstants.Foci.Profession.Embalmer, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Alchemy)]
        [TestCase(SkillConstants.Foci.Profession.Engineer, SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.ArchitectureAndEngineering)]
        [TestCase(SkillConstants.Foci.Profession.Entertainer, SkillConstants.Perform)]
        [TestCase(SkillConstants.Foci.Profession.ExoticAnimalTrainer, SkillConstants.HandleAnimal)]
        [TestCase(SkillConstants.Foci.Profession.Farmer, SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Nature)]
        [TestCase(SkillConstants.Foci.Profession.Fletcher, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Bowmaking)]
        [TestCase(SkillConstants.Foci.Profession.Footman,
            SkillConstants.Diplomacy,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.NobilityAndRoyalty)]
        [TestCase(SkillConstants.Foci.Profession.Gemcutter, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Gemcutting)]
        [TestCase(SkillConstants.Foci.Profession.Goldsmith, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Goldsmithing)]
        [TestCase(SkillConstants.Foci.Profession.Governess,
            SkillConstants.Diplomacy,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.NobilityAndRoyalty)]
        [TestCase(SkillConstants.Foci.Profession.Haberdasher, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Hatmaking)]
        [TestCase(SkillConstants.Foci.Profession.Healer, SkillConstants.Heal)]
        [TestCase(SkillConstants.Foci.Profession.Horner, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Hornworking)]
        [TestCase(SkillConstants.Foci.Profession.Hunter, SkillConstants.Survival)]
        [TestCase(SkillConstants.Foci.Profession.Interpreter)]
        [TestCase(SkillConstants.Foci.Profession.Jeweler, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Jewelmaking)]
        [TestCase(SkillConstants.Foci.Profession.Laborer)]
        [TestCase(SkillConstants.Foci.Profession.Launderer)]
        [TestCase(SkillConstants.Foci.Profession.Limner, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Painting)]
        [TestCase(SkillConstants.Foci.Profession.LocalCourier, SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Local)]
        [TestCase(SkillConstants.Foci.Profession.Locksmith, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Locksmithing)]
        [TestCase(SkillConstants.Foci.Profession.Maid, SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.NobilityAndRoyalty)]
        [TestCase(SkillConstants.Foci.Profession.Masseuse, SkillConstants.Heal)]
        [TestCase(SkillConstants.Foci.Profession.Matchmaker,
            SkillConstants.Diplomacy,
            SkillConstants.SenseMotive)]
        [TestCase(SkillConstants.Foci.Profession.Midwife, SkillConstants.Heal)]
        [TestCase(SkillConstants.Foci.Profession.Miller, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Milling)]
        [TestCase(SkillConstants.Foci.Profession.Navigator, SkillConstants.Survival)]
        [TestCase(SkillConstants.Foci.Profession.Nursemaid)]
        [TestCase(SkillConstants.Foci.Profession.OutOfTownCourier,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Local,
            SkillConstants.Ride)]
        [TestCase(SkillConstants.Foci.Profession.Painter, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Painting)]
        [TestCase(SkillConstants.Foci.Profession.Parchmentmaker, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Parchmentmaking)]
        [TestCase(SkillConstants.Foci.Profession.Pewterer, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Pewtermaking)]
        [TestCase(SkillConstants.Foci.Profession.Polisher)]
        [TestCase(SkillConstants.Foci.Profession.Porter)]
        [TestCase(SkillConstants.Foci.Profession.Potter, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Potterymaking)]
        [TestCase(SkillConstants.Foci.Profession.Sage, SkillConstants.Knowledge)]
        [TestCase(SkillConstants.Foci.Profession.SailorCrewmember, SkillConstants.Swim)]
        [TestCase(SkillConstants.Foci.Profession.SailorMate,
            SkillConstants.Intimidate,
            SkillConstants.Swim)]
        [TestCase(SkillConstants.Foci.Profession.Scribe)]
        [TestCase(SkillConstants.Foci.Profession.Sculptor, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Sculpting)]
        [TestCase(SkillConstants.Foci.Profession.Shepherd, SkillConstants.HandleAnimal)]
        [TestCase(SkillConstants.Foci.Profession.Shipwright, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Shipmaking)]
        [TestCase(SkillConstants.Foci.Profession.Silversmith, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Silversmithing)]
        [TestCase(SkillConstants.Foci.Profession.Skinner, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Skinning)]
        [TestCase(SkillConstants.Foci.Profession.Soapmaker, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Soapmaking)]
        [TestCase(SkillConstants.Foci.Profession.Soothsayer, SkillConstants.Bluff)]
        [TestCase(SkillConstants.Foci.Profession.Tanner, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Tanning)]
        [TestCase(SkillConstants.Foci.Profession.Teacher, SkillConstants.Knowledge)]
        [TestCase(SkillConstants.Foci.Profession.Teamster,
            SkillConstants.HandleAnimal,
            SkillConstants.Ride)]
        [TestCase(SkillConstants.Foci.Profession.Trader,
            SkillConstants.Appraise,
            SkillConstants.SenseMotive)]
        [TestCase(SkillConstants.Foci.Profession.Valet, SkillConstants.Diplomacy)]
        [TestCase(SkillConstants.Foci.Profession.Vintner, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Winemaking)]
        [TestCase(SkillConstants.Foci.Profession.Weaponsmith, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Weaponsmithing)]
        [TestCase(SkillConstants.Foci.Profession.Weaver, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Weaving)]
        [TestCase(SkillConstants.Foci.Profession.Wheelwright, SkillConstants.Craft + "/" + SkillConstants.Foci.Craft.Wheelmaking)]
        [TestCase(SkillConstants.Foci.Profession.WildernessGuide,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Local,
            SkillConstants.Knowledge + "/" + SkillConstants.Foci.Knowledge.Nature)]
        public void ClassSkills(string name, params string[] skills)
        {
            base.AssertDistinctCollection(name, skills);
        }
    }
}