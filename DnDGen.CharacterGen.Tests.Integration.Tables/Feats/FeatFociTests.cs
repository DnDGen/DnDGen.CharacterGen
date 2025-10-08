using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Skills;
using DnDGen.CharacterGen.Tables;
using DnDGen.TreasureGen.Items;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Feats
{
    [TestFixture]
    public class FeatFociTests : CollectionTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Set.Collection.FeatFoci; }
        }

        [Test]
        public override void CollectionNames()
        {
            var names = new[]
            {
                FeatConstants.CombatStyle,
                FeatConstants.ExoticWeaponProficiency,
                FeatConstants.GhostSpecialAttack,
                FeatConstants.MartialWeaponProficiency,
                FeatConstants.MonkBonusFeat + "1",
                FeatConstants.MonkBonusFeat + "2",
                FeatConstants.MonkBonusFeat + "6",
                FeatConstants.SimpleWeaponProficiency,
                FeatConstants.Foci.Weapons,
                FeatConstants.Foci.WeaponsWithUnarmed,
                FeatConstants.Foci.WeaponsWithUnarmedAndGrapple,
                FeatConstants.Foci.WeaponsWithUnarmedAndGrappleAndRay,
                GroupConstants.FavoredEnemies,
                GroupConstants.SchoolsOfMagic,
                GroupConstants.Skills,
            };

            AssertCollectionNames(names);
        }

        [TestCase(GroupConstants.SchoolsOfMagic,
            CharacterClassConstants.Schools.Abjuration,
            CharacterClassConstants.Schools.Conjuration,
            CharacterClassConstants.Schools.Divination,
            CharacterClassConstants.Schools.Enchantment,
            CharacterClassConstants.Schools.Evocation,
            CharacterClassConstants.Schools.Illusion,
            CharacterClassConstants.Schools.Necromancy,
            CharacterClassConstants.Schools.Transmutation)]
        [TestCase(FeatConstants.SimpleWeaponProficiency,
            WeaponConstants.Gauntlet,
            FeatConstants.Foci.UnarmedStrike,
            WeaponConstants.Dagger,
            WeaponConstants.PunchingDagger,
            WeaponConstants.SpikedGauntlet,
            WeaponConstants.LightMace,
            WeaponConstants.Sickle,
            WeaponConstants.Club,
            WeaponConstants.HeavyMace,
            WeaponConstants.Morningstar,
            WeaponConstants.Shortspear,
            WeaponConstants.Longspear,
            WeaponConstants.Spear,
            WeaponConstants.Quarterstaff,
            WeaponConstants.HeavyCrossbow,
            WeaponConstants.LightCrossbow,
            WeaponConstants.Dart,
            WeaponConstants.Javelin,
            WeaponConstants.Sling)]
        [TestCase(FeatConstants.MartialWeaponProficiency,
            WeaponConstants.ThrowingAxe,
            WeaponConstants.LightHammer,
            WeaponConstants.Handaxe,
            WeaponConstants.Kukri,
            WeaponConstants.LightPick,
            WeaponConstants.Sap,
            WeaponConstants.ShortSword,
            WeaponConstants.Battleaxe,
            WeaponConstants.Flail,
            WeaponConstants.Longsword,
            WeaponConstants.HeavyPick,
            WeaponConstants.Rapier,
            WeaponConstants.Scimitar,
            WeaponConstants.Trident,
            WeaponConstants.Warhammer,
            WeaponConstants.Falchion,
            WeaponConstants.Glaive,
            WeaponConstants.Greataxe,
            WeaponConstants.Greatclub,
            WeaponConstants.HeavyFlail,
            WeaponConstants.Greatsword,
            WeaponConstants.Guisarme,
            WeaponConstants.Halberd,
            WeaponConstants.Lance,
            WeaponConstants.Ranseur,
            WeaponConstants.Scythe,
            WeaponConstants.Longbow,
            WeaponConstants.CompositeLongbow,
            WeaponConstants.CompositeShortbow,
            WeaponConstants.Shortbow)]
        [TestCase(FeatConstants.ExoticWeaponProficiency,
            WeaponConstants.Kama,
            WeaponConstants.Nunchaku,
            WeaponConstants.Sai,
            WeaponConstants.Bolas,
            WeaponConstants.Siangham,
            WeaponConstants.BastardSword,
            WeaponConstants.DwarvenWaraxe,
            WeaponConstants.Whip,
            WeaponConstants.OrcDoubleAxe,
            WeaponConstants.SpikedChain,
            WeaponConstants.DireFlail,
            WeaponConstants.GnomeHookedHammer,
            WeaponConstants.TwoBladedSword,
            WeaponConstants.DwarvenUrgrosh,
            WeaponConstants.HandCrossbow,
            WeaponConstants.HeavyRepeatingCrossbow,
            WeaponConstants.LightRepeatingCrossbow,
            WeaponConstants.Shuriken,
            WeaponConstants.Net,
            WeaponConstants.PincerStaff)]
        [TestCase(FeatConstants.CombatStyle,
            FeatConstants.TwoWeaponFighting,
            FeatConstants.Foci.Archery)]
        [TestCase(FeatConstants.MonkBonusFeat + "1",
            FeatConstants.StunningFist,
            FeatConstants.ImprovedGrapple)]
        [TestCase(FeatConstants.MonkBonusFeat + "2",
            FeatConstants.CombatReflexes,
            FeatConstants.DeflectArrows)]
        [TestCase(FeatConstants.MonkBonusFeat + "6",
            FeatConstants.ImprovedDisarm,
            FeatConstants.ImprovedTrip)]
        [TestCase(FeatConstants.GhostSpecialAttack,
            FeatConstants.CorruptingGaze,
            FeatConstants.CorruptingTouch,
            FeatConstants.DrainingTouch,
            FeatConstants.FrightfulMoan,
            FeatConstants.HorrificAppearance,
            FeatConstants.Malevolence,
            FeatConstants.Telekinesis)]
        public void FeatFoci(string feat, params string[] foci)
        {
            base.AssertDistinctCollection(feat, foci);
        }

        [Test]
        public void FavoredEnemyFoci()
        {
            var foci = new[]
            {
                "Aberration",
                "Animal",
                "Construct",
                "Dragon",
                "Elemental",
                "Fey",
                "Giant",
                "Humanoid (aquatic)",
                "Humanoid (dwarf)",
                "Humanoid (elf)",
                "Humanoid (goblinoid)",
                "Humanoid (gnoll)",
                "Humanoid (gnome)",
                "Humanoid (halfling)",
                "Humanoid (human)",
                "Humanoid (orc)",
                "Humanoid (reptilian)",
                "Magical beast",
                "Monstrous humanoid",
                "Ooze",
                "Outsider (air)",
                "Outsider (chaotic)",
                "Outsider (earth)",
                "Outsider (evil)",
                "Outsider (fire)",
                "Outsider (good)",
                "Outsider (lawful)",
                "Outsider (native)",
                "Outsider (water)",
                "Plant",
                "Undead",
                "Vermin"
            };

            base.AssertDistinctCollection(GroupConstants.FavoredEnemies, foci);
        }

        [Test]
        public void WeaponFoci()
        {
            var allWeapons = WeaponConstants.GetAllWeapons(false, false).Except(new[]
            {
                WeaponConstants.Arrow,
                WeaponConstants.CrossbowBolt,
                WeaponConstants.SlingBullet,
            }).ToArray();

            base.AssertDistinctCollection(FeatConstants.Foci.Weapons, allWeapons);
        }

        [Test]
        public void WeaponAndUnarmedFoci()
        {
            var weapons = GetCollection(FeatConstants.Foci.Weapons);

            var foci = new[]
            {
                FeatConstants.Foci.UnarmedStrike
            }.Union(weapons).ToArray();

            base.AssertDistinctCollection(FeatConstants.Foci.WeaponsWithUnarmed, foci);
        }

        [Test]
        public void WeaponAndUnarmedAndGrappleFoci()
        {
            var weapons = GetCollection(FeatConstants.Foci.Weapons);

            var foci = new[]
            {
                FeatConstants.Foci.UnarmedStrike,
                FeatConstants.Foci.Grapple,
            }.Union(weapons).ToArray();

            base.AssertDistinctCollection(FeatConstants.Foci.WeaponsWithUnarmedAndGrapple, foci);
        }

        [Test]
        public void WeaponGrappleAndRayFoci()
        {
            var weapons = GetCollection(FeatConstants.Foci.Weapons);

            var foci = new[]
            {
                FeatConstants.Foci.UnarmedStrike,
                FeatConstants.Foci.Grapple,
                FeatConstants.Foci.Ray,
            }.Union(weapons).ToArray();

            base.AssertDistinctCollection(FeatConstants.Foci.WeaponsWithUnarmedAndGrappleAndRay, foci);
        }

        [Test]
        public void SkillsFoci()
        {
            var foci = new[]
            {
                SkillConstants.Appraise,
                SkillConstants.Balance,
                SkillConstants.Bluff,
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Alchemy}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Armorsmithing}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Blacksmithing}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Bookbinding}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Bowmaking}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Brassmaking}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Brewing}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Candlemaking}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Cloth}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Coppersmithing}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Dyemaking}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Gemcutting}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Glass}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Goldsmithing}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Hatmaking}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Hornworking}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Jewelmaking}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Leather}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Locksmithing}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Mapmaking}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Milling}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Painting}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Parchmentmaking}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Pewtermaking}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Potterymaking}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Sculpting}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Shipmaking}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Shoemaking}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Silversmithing}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Skinning}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Soapmaking}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Stonemasonry}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Tanning}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Trapmaking}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Weaponsmithing}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Weaving}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Wheelmaking}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Winemaking}",
                $"{SkillConstants.Craft}/{SkillConstants.Foci.Craft.Woodworking}",
                SkillConstants.Climb,
                SkillConstants.Concentration,
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
                $"{SkillConstants.Knowledge}/{SkillConstants.Foci.Knowledge.Arcana}",
                $"{SkillConstants.Knowledge}/{SkillConstants.Foci.Knowledge.ArchitectureAndEngineering}",
                $"{SkillConstants.Knowledge}/{SkillConstants.Foci.Knowledge.Dungeoneering}",
                $"{SkillConstants.Knowledge}/{SkillConstants.Foci.Knowledge.Geography}",
                $"{SkillConstants.Knowledge}/{SkillConstants.Foci.Knowledge.History}",
                $"{SkillConstants.Knowledge}/{SkillConstants.Foci.Knowledge.Local}",
                $"{SkillConstants.Knowledge}/{SkillConstants.Foci.Knowledge.Nature}",
                $"{SkillConstants.Knowledge}/{SkillConstants.Foci.Knowledge.NobilityAndRoyalty}",
                $"{SkillConstants.Knowledge}/{SkillConstants.Foci.Knowledge.Religion}",
                $"{SkillConstants.Knowledge}/{SkillConstants.Foci.Knowledge.ThePlanes}",
                SkillConstants.Listen,
                SkillConstants.MoveSilently,
                SkillConstants.OpenLock,
                $"{SkillConstants.Perform}/{SkillConstants.Foci.Perform.Act}",
                $"{SkillConstants.Perform}/{SkillConstants.Foci.Perform.Comedy}",
                $"{SkillConstants.Perform}/{SkillConstants.Foci.Perform.Dance}",
                $"{SkillConstants.Perform}/{SkillConstants.Foci.Perform.KeyboardInstruments}",
                $"{SkillConstants.Perform}/{SkillConstants.Foci.Perform.Oratory}",
                $"{SkillConstants.Perform}/{SkillConstants.Foci.Perform.PercussionInstruments}",
                $"{SkillConstants.Perform}/{SkillConstants.Foci.Perform.Sing}",
                $"{SkillConstants.Perform}/{SkillConstants.Foci.Perform.StringInstruments}",
                $"{SkillConstants.Perform}/{SkillConstants.Foci.Perform.WindInstruments}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Adviser}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Alchemist}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.AnimalGroomer}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.AnimalTrainer}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Apothecary}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Appraiser}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Architect}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Armorer}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Barrister}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Blacksmith}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Bookbinder}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Bowyer}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Brazier}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Brewer}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Butler}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Carpenter}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Cartographer}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Cartwright}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Chandler}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.CityGuide}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Clerk}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Cobbler}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Coffinmaker}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Coiffeur}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Cook}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Coppersmith}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Craftsman}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Dowser}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Dyer}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Embalmer}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Engineer}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Entertainer}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.ExoticAnimalTrainer}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Farmer}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Fletcher}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Footman}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Gemcutter}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Goldsmith}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Governess}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Haberdasher}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Healer}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Horner}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Hunter}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Interpreter}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Jeweler}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Laborer}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Launderer}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Limner}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.LocalCourier}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Locksmith}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Maid}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Masseuse}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Matchmaker}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Midwife}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Miller}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Navigator}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Nursemaid}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.OutOfTownCourier}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Painter}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Parchmentmaker}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Pewterer}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Polisher}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Porter}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Potter}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Sage}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.SailorCrewmember}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.SailorMate}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Scribe}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Sculptor}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Shepherd}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Shipwright}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Silversmith}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Skinner}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Soapmaker}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Soothsayer}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Tanner}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Teacher}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Teamster}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Trader}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Valet}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Vintner}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Weaponsmith}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Weaver}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.Wheelwright}",
                $"{SkillConstants.Profession}/{SkillConstants.Foci.Profession.WildernessGuide}",
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
                SkillConstants.UseRope
            };

            base.AssertDistinctCollection(GroupConstants.Skills, foci);
        }
    }
}
