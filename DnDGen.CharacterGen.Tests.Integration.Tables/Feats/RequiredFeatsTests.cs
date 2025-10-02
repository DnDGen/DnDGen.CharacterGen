using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Feats;
using NUnit.Framework;
using DnDGen.TreasureGen.Items;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Feats
{
    [TestFixture]
    public class RequiredFeatsTests : CollectionTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Set.Collection.RequiredFeats; }
        }

        [Test]
        public override void CollectionNames()
        {
            var names = new[]
            {
                FeatConstants.AugmentSummoning,
                FeatConstants.Cleave,
                FeatConstants.CombatStyleMastery,
                FeatConstants.DeflectArrows,
                FeatConstants.Diehard,
                FeatConstants.ExtraTurning,
                FeatConstants.FarShot,
                FeatConstants.HeavyArmorProficiency,
                FeatConstants.ImprovedCombatStyle,
                FeatConstants.MediumArmorProficiency,
                FeatConstants.RapidShot + CharacterClassConstants.Ranger,
                FeatConstants.TwoWeaponFighting + CharacterClassConstants.Ranger,
                FeatConstants.Manyshot + CharacterClassConstants.Ranger,
                FeatConstants.ImprovedTwoWeaponFighting + CharacterClassConstants.Ranger,
                FeatConstants.ImprovedPreciseShot + CharacterClassConstants.Ranger,
                FeatConstants.GreaterTwoWeaponFighting + CharacterClassConstants.Ranger,
                FeatConstants.GreatCleave,
                FeatConstants.GreaterSpellFocus,
                FeatConstants.GreaterSpellPenetration,
                FeatConstants.GreaterTwoWeaponFighting,
                FeatConstants.GreaterWeaponFocus,
                FeatConstants.GreaterWeaponSpecialization,
                FeatConstants.ImprovedBullRush,
                FeatConstants.ImprovedCritical,
                FeatConstants.ImprovedDisarm,
                FeatConstants.ImprovedFeint,
                FeatConstants.ImprovedGrapple,
                FeatConstants.ImprovedOverrun,
                FeatConstants.ImprovedPreciseShot,
                FeatConstants.ImprovedShieldBash,
                FeatConstants.ImprovedSunder,
                FeatConstants.ImprovedTrip,
                FeatConstants.ImprovedTurning,
                FeatConstants.ImprovedTwoWeaponFighting,
                FeatConstants.Manyshot,
                FeatConstants.MountedArchery,
                FeatConstants.Mobility,
                FeatConstants.NaturalSpell,
                FeatConstants.PreciseShot,
                FeatConstants.RapidReload,
                FeatConstants.RapidShot,
                FeatConstants.RideByAttack,
                FeatConstants.ShotOnTheRun,
                FeatConstants.SnatchArrows,
                FeatConstants.SpiritedCharge,
                FeatConstants.SpringAttack,
                FeatConstants.StunningFist,
                FeatConstants.TowerShieldProficiency,
                FeatConstants.Trample,
                FeatConstants.TwoWeaponDefense,
                FeatConstants.WhirlwindAttack,
                FeatConstants.WeaponFocus,
                FeatConstants.WeaponSpecialization,
                FeatConstants.ImprovedGrapple + CharacterClassConstants.Monk,
                FeatConstants.StunningFist + CharacterClassConstants.Monk,
                FeatConstants.CombatReflexes + CharacterClassConstants.Monk,
                FeatConstants.DeflectArrows + CharacterClassConstants.Monk,
                FeatConstants.ImprovedDisarm + CharacterClassConstants.Monk,
                FeatConstants.ImprovedTrip + CharacterClassConstants.Monk,
                FeatConstants.CorruptingGaze,
                FeatConstants.CorruptingTouch,
                FeatConstants.DrainingTouch,
                FeatConstants.FrightfulMoan,
                FeatConstants.HorrificAppearance,
                FeatConstants.Malevolence,
                FeatConstants.Telekinesis,
            };

            AssertCollectionNames(names);
        }

        [TestCase(FeatConstants.CombatStyleMastery, FeatConstants.ImprovedCombatStyle)]
        [TestCase(FeatConstants.ImprovedCombatStyle, FeatConstants.CombatStyle)]
        [TestCase(FeatConstants.HeavyArmorProficiency, FeatConstants.MediumArmorProficiency)]
        [TestCase(FeatConstants.MediumArmorProficiency, FeatConstants.LightArmorProficiency)]
        [TestCase(FeatConstants.Cleave, FeatConstants.PowerAttack)]
        [TestCase(FeatConstants.DeflectArrows, FeatConstants.ImprovedUnarmedStrike)]
        [TestCase(FeatConstants.Diehard, FeatConstants.Endurance)]
        [TestCase(FeatConstants.ExtraTurning, FeatConstants.Turn)]
        [TestCase(FeatConstants.FarShot, FeatConstants.PointBlankShot)]
        [TestCase(FeatConstants.GreatCleave, FeatConstants.Cleave)]
        [TestCase(FeatConstants.GreaterSpellFocus, FeatConstants.SpellFocus)]
        [TestCase(FeatConstants.GreaterSpellPenetration, FeatConstants.SpellPenetration)]
        [TestCase(FeatConstants.GreaterTwoWeaponFighting, FeatConstants.ImprovedTwoWeaponFighting)]
        [TestCase(FeatConstants.GreaterWeaponFocus, FeatConstants.WeaponFocus)]
        [TestCase(FeatConstants.GreaterWeaponSpecialization, FeatConstants.GreaterWeaponFocus, FeatConstants.WeaponSpecialization)]
        [TestCase(FeatConstants.ImprovedBullRush, FeatConstants.PowerAttack)]
        [TestCase(FeatConstants.ImprovedCritical, ItemTypeConstants.Weapon + GroupConstants.Proficiency)]
        [TestCase(FeatConstants.ImprovedDisarm, FeatConstants.CombatExpertise)]
        [TestCase(FeatConstants.ImprovedFeint, FeatConstants.CombatExpertise)]
        [TestCase(FeatConstants.ImprovedGrapple, FeatConstants.ImprovedUnarmedStrike)]
        [TestCase(FeatConstants.ImprovedOverrun, FeatConstants.PowerAttack)]
        [TestCase(FeatConstants.ImprovedPreciseShot, FeatConstants.PreciseShot)]
        [TestCase(FeatConstants.PreciseShot, FeatConstants.PointBlankShot)]
        [TestCase(FeatConstants.ImprovedShieldBash, FeatConstants.ShieldProficiency)]
        [TestCase(FeatConstants.ImprovedSunder, FeatConstants.PowerAttack)]
        [TestCase(FeatConstants.ImprovedTrip, FeatConstants.CombatExpertise)]
        [TestCase(FeatConstants.ImprovedTurning, FeatConstants.Turn)]
        [TestCase(FeatConstants.ImprovedTwoWeaponFighting, FeatConstants.TwoWeaponFighting)]
        [TestCase(FeatConstants.Manyshot, FeatConstants.RapidShot)]
        [TestCase(FeatConstants.RapidShot, FeatConstants.PointBlankShot)]
        [TestCase(FeatConstants.Mobility, FeatConstants.Dodge)]
        [TestCase(FeatConstants.MountedArchery, FeatConstants.MountedCombat)]
        [TestCase(FeatConstants.NaturalSpell, FeatConstants.WildShape)]
        [TestCase(FeatConstants.RapidReload, ItemTypeConstants.Weapon + GroupConstants.Proficiency)]
        [TestCase(FeatConstants.RideByAttack, FeatConstants.MountedCombat)]
        [TestCase(FeatConstants.ShotOnTheRun,
            FeatConstants.Dodge,
            FeatConstants.Mobility,
            FeatConstants.PointBlankShot)]
        [TestCase(FeatConstants.SnatchArrows,
            FeatConstants.DeflectArrows,
            FeatConstants.ImprovedUnarmedStrike)]
        [TestCase(FeatConstants.SpiritedCharge,
            FeatConstants.MountedCombat,
            FeatConstants.RideByAttack)]
        [TestCase(FeatConstants.SpringAttack,
            FeatConstants.Dodge,
            FeatConstants.Mobility)]
        [TestCase(FeatConstants.StunningFist, FeatConstants.ImprovedUnarmedStrike)]
        [TestCase(FeatConstants.TowerShieldProficiency, FeatConstants.ShieldProficiency)]
        [TestCase(FeatConstants.Trample, FeatConstants.MountedCombat)]
        [TestCase(FeatConstants.TwoWeaponDefense, FeatConstants.TwoWeaponFighting)]
        [TestCase(FeatConstants.WeaponFocus, ItemTypeConstants.Weapon + GroupConstants.Proficiency)]
        [TestCase(FeatConstants.WeaponSpecialization, FeatConstants.WeaponFocus)]
        [TestCase(FeatConstants.WhirlwindAttack,
            FeatConstants.CombatExpertise,
            FeatConstants.Dodge,
            FeatConstants.Mobility,
            FeatConstants.SpringAttack)]
        public void RequiredFeats(string name, params string[] requiredFeats)
        {
            AssertDistinctCollection(name, requiredFeats);
        }

        [TestCase(FeatConstants.AugmentSummoning,
            FeatConstants.SpellFocus, CharacterClassConstants.Schools.Conjuration)]
        [TestCase(FeatConstants.RapidShot + CharacterClassConstants.Ranger,
            FeatConstants.CombatStyle, FeatConstants.Foci.Archery)]
        [TestCase(FeatConstants.TwoWeaponFighting + CharacterClassConstants.Ranger,
            FeatConstants.CombatStyle, FeatConstants.TwoWeaponFighting)]
        [TestCase(FeatConstants.Manyshot + CharacterClassConstants.Ranger,
            FeatConstants.ImprovedCombatStyle, FeatConstants.Foci.Archery)]
        [TestCase(FeatConstants.ImprovedTwoWeaponFighting + CharacterClassConstants.Ranger,
            FeatConstants.ImprovedCombatStyle, FeatConstants.TwoWeaponFighting)]
        [TestCase(FeatConstants.ImprovedPreciseShot + CharacterClassConstants.Ranger,
            FeatConstants.CombatStyleMastery, FeatConstants.Foci.Archery)]
        [TestCase(FeatConstants.GreaterTwoWeaponFighting + CharacterClassConstants.Ranger,
            FeatConstants.CombatStyleMastery, FeatConstants.TwoWeaponFighting)]
        [TestCase(FeatConstants.ImprovedGrapple + CharacterClassConstants.Monk,
            FeatConstants.MonkBonusFeat, FeatConstants.ImprovedGrapple)]
        [TestCase(FeatConstants.StunningFist + CharacterClassConstants.Monk,
            FeatConstants.MonkBonusFeat, FeatConstants.StunningFist)]
        [TestCase(FeatConstants.CombatReflexes + CharacterClassConstants.Monk,
            FeatConstants.MonkBonusFeat, FeatConstants.CombatReflexes)]
        [TestCase(FeatConstants.DeflectArrows + CharacterClassConstants.Monk,
            FeatConstants.MonkBonusFeat, FeatConstants.DeflectArrows)]
        [TestCase(FeatConstants.ImprovedDisarm + CharacterClassConstants.Monk,
            FeatConstants.MonkBonusFeat, FeatConstants.ImprovedDisarm)]
        [TestCase(FeatConstants.ImprovedTrip + CharacterClassConstants.Monk,
            FeatConstants.MonkBonusFeat, FeatConstants.ImprovedTrip)]
        [TestCase(FeatConstants.CorruptingGaze,
            FeatConstants.GhostSpecialAttack, FeatConstants.CorruptingGaze)]
        [TestCase(FeatConstants.CorruptingTouch,
            FeatConstants.GhostSpecialAttack, FeatConstants.CorruptingTouch)]
        [TestCase(FeatConstants.DrainingTouch,
            FeatConstants.GhostSpecialAttack, FeatConstants.DrainingTouch)]
        [TestCase(FeatConstants.FrightfulMoan,
            FeatConstants.GhostSpecialAttack, FeatConstants.FrightfulMoan)]
        [TestCase(FeatConstants.HorrificAppearance,
            FeatConstants.GhostSpecialAttack, FeatConstants.HorrificAppearance)]
        [TestCase(FeatConstants.Malevolence,
            FeatConstants.GhostSpecialAttack, FeatConstants.Malevolence)]
        [TestCase(FeatConstants.Telekinesis,
            FeatConstants.GhostSpecialAttack, FeatConstants.Telekinesis)]
        public void RequiredFeat(string name, string requiredFeat, string requiredFocus)
        {
            var collection = new[] { $"{requiredFeat}/{requiredFocus}" };
            AssertDistinctCollection(name, collection);
        }
    }
}
