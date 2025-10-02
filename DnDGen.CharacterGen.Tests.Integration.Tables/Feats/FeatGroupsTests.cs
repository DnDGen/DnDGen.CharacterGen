using DnDGen.CharacterGen.Combats;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Feats;
using NUnit.Framework;
using DnDGen.TreasureGen.Items;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Feats
{
    [TestFixture]
    public class FeatGroupsTests : CollectionTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Set.Collection.FeatGroups; }
        }

        [Test]
        public override void CollectionNames()
        {
            var names = new[]
            {
                AttributeConstants.Shield + GroupConstants.Proficiency,
                FeatConstants.AttackBonus,
                FeatConstants.SkillBonus,
                GroupConstants.AddMonsterHitDiceToPower,
                GroupConstants.FighterBonusFeats,
                GroupConstants.HasAbilityRequirements,
                GroupConstants.HasClassRequirements,
                GroupConstants.HasSkillRequirements,
                GroupConstants.Initiative,
                GroupConstants.SavingThrows,
                GroupConstants.TakenMultipleTimes,
                GroupConstants.TwoHanded,
                GroupConstants.WizardBonusFeats,
                ItemTypeConstants.Weapon + GroupConstants.Proficiency,
                ItemTypeConstants.Armor + GroupConstants.Proficiency,
                SavingThrowConstants.Fortitude,
                SavingThrowConstants.Reflex,
                SavingThrowConstants.Will,
            };

            AssertCollectionNames(names);
        }

        [TestCase(FeatConstants.AttackBonus,
            FeatConstants.AttackBonus)]
        [TestCase(GroupConstants.AddMonsterHitDiceToPower,
            FeatConstants.SpellResistance)]
        [TestCase(GroupConstants.HasSkillRequirements,
            FeatConstants.MountedArchery,
            FeatConstants.MountedCombat,
            FeatConstants.RideByAttack,
            FeatConstants.SpiritedCharge,
            FeatConstants.Trample,
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
            FeatConstants.Stealthy)]
        [TestCase(GroupConstants.HasAbilityRequirements,
            FeatConstants.PowerAttack,
            FeatConstants.CombatExpertise,
            FeatConstants.DeflectArrows,
            FeatConstants.Dodge,
            FeatConstants.GreaterTwoWeaponFighting,
            FeatConstants.ImprovedGrapple,
            FeatConstants.ImprovedPreciseShot,
            FeatConstants.ImprovedTwoWeaponFighting,
            FeatConstants.Manyshot,
            FeatConstants.Mobility,
            FeatConstants.NaturalSpell,
            FeatConstants.RapidShot,
            FeatConstants.SnatchArrows,
            FeatConstants.SpellMastery,
            FeatConstants.SpringAttack,
            FeatConstants.StunningFist,
            FeatConstants.ShotOnTheRun,
            FeatConstants.TwoWeaponDefense,
            FeatConstants.TwoWeaponFighting,
            FeatConstants.WhirlwindAttack)]
        [TestCase(GroupConstants.TakenMultipleTimes,
            FeatConstants.AttackBonus,
            FeatConstants.SpellMastery,
            FeatConstants.Toughness,
            FeatConstants.SkillMastery,
            FeatConstants.SkillBonus,
            FeatConstants.DodgeBonus,
            FeatConstants.SaveBonus,
            FeatConstants.ExtraTurning)]
        [TestCase(FeatConstants.SkillBonus,
            FeatConstants.SkillBonus,
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
            FeatConstants.Negotiator,
            FeatConstants.NimbleFingers,
            FeatConstants.Persuasive,
            FeatConstants.SelfSufficient,
            FeatConstants.SkillFocus,
            FeatConstants.Stealthy,
            FeatConstants.NatureSense)]
        [TestCase(GroupConstants.WizardBonusFeats,
            FeatConstants.SpellMastery,
            FeatConstants.ScribeScroll,
            FeatConstants.EmpowerSpell,
            FeatConstants.EnlargeSpell,
            FeatConstants.ExtendSpell,
            FeatConstants.HeightenSpell,
            FeatConstants.MaximizeSpell,
            FeatConstants.QuickenSpell,
            FeatConstants.SilentSpell,
            FeatConstants.StillSpell,
            FeatConstants.WidenSpell)]
        [TestCase(ItemTypeConstants.Weapon + GroupConstants.Proficiency,
            FeatConstants.ExoticWeaponProficiency,
            FeatConstants.MartialWeaponProficiency,
            FeatConstants.SimpleWeaponProficiency)]
        [TestCase(ItemTypeConstants.Armor + GroupConstants.Proficiency,
            FeatConstants.HeavyArmorProficiency,
            FeatConstants.LightArmorProficiency,
            FeatConstants.MediumArmorProficiency)]
        [TestCase(GroupConstants.TwoHanded,
            FeatConstants.TwoWeaponDefense,
            FeatConstants.TwoWeaponFighting,
            FeatConstants.GreaterTwoWeaponFighting,
            FeatConstants.ImprovedTwoWeaponFighting)]
        [TestCase(SavingThrowConstants.Fortitude,
            FeatConstants.GreatFortitude)]
        [TestCase(SavingThrowConstants.Reflex,
            FeatConstants.LightningReflexes)]
        [TestCase(SavingThrowConstants.Will,
            FeatConstants.IronWill)]
        [TestCase(GroupConstants.SavingThrows,
            FeatConstants.SaveBonus)]
        [TestCase(GroupConstants.Initiative,
            FeatConstants.ImprovedInitiative)]
        [TestCase(AttributeConstants.Shield + GroupConstants.Proficiency,
            FeatConstants.ShieldProficiency,
            FeatConstants.TowerShieldProficiency)]
        public override void AssertDistinctCollection(string name, params string[] collection)
        {
            base.AssertDistinctCollection(name, collection);
        }

        [Test]
        public void FeatsWithClassRequirements()
        {
            var featNames = new[]
            {
                FeatConstants.CombatCasting,
                FeatConstants.CripplingStrike,
                FeatConstants.DefensiveRoll,
                FeatConstants.GreaterWeaponFocus,
                FeatConstants.GreaterWeaponSpecialization,
                FeatConstants.ImprovedEvasion,
                FeatConstants.ImprovedFamiliar,
                FeatConstants.Leadership,
                FeatConstants.Opportunist,
                FeatConstants.SkillMastery,
                FeatConstants.SlipperyMind,
                FeatConstants.SpellMastery,
                FeatConstants.WeaponSpecialization,
                FeatConstants.EmpowerSpell,
                FeatConstants.EnlargeSpell,
                FeatConstants.EschewMaterials,
                FeatConstants.ExtendSpell,
                FeatConstants.HeightenSpell,
                FeatConstants.ImprovedCounterspell,
                FeatConstants.MaximizeSpell,
                FeatConstants.QuickenSpell,
                FeatConstants.ScribeScroll,
                FeatConstants.SilentSpell,
                FeatConstants.SpellFocus,
                FeatConstants.StillSpell,
                FeatConstants.WidenSpell,
                FeatConstants.SpellPenetration
            };

            base.AssertDistinctCollection(GroupConstants.HasClassRequirements, featNames);
        }

        [Test]
        public void FighterBonusFeats()
        {
            var featNames = new[]
            {
                FeatConstants.BlindFight,
                FeatConstants.CombatExpertise,
                FeatConstants.ImprovedDisarm,
                FeatConstants.ImprovedFeint,
                FeatConstants.ImprovedTrip,
                FeatConstants.WhirlwindAttack,
                FeatConstants.CombatReflexes,
                FeatConstants.Dodge,
                FeatConstants.Mobility,
                FeatConstants.SpringAttack,
                FeatConstants.ExoticWeaponProficiency,
                FeatConstants.ImprovedCritical,
                FeatConstants.ImprovedInitiative,
                FeatConstants.ImprovedShieldBash,
                FeatConstants.ImprovedUnarmedStrike,
                FeatConstants.DeflectArrows,
                FeatConstants.ImprovedGrapple,
                FeatConstants.SnatchArrows,
                FeatConstants.StunningFist,
                FeatConstants.MountedCombat,
                FeatConstants.MountedArchery,
                FeatConstants.RideByAttack,
                FeatConstants.SpiritedCharge,
                FeatConstants.Trample,
                FeatConstants.PointBlankShot,
                FeatConstants.FarShot,
                FeatConstants.PreciseShot,
                FeatConstants.RapidShot,
                FeatConstants.Manyshot,
                FeatConstants.ShotOnTheRun,
                FeatConstants.ImprovedPreciseShot,
                FeatConstants.PowerAttack,
                FeatConstants.Cleave,
                FeatConstants.GreatCleave,
                FeatConstants.ImprovedBullRush,
                FeatConstants.ImprovedOverrun,
                FeatConstants.ImprovedSunder,
                FeatConstants.QuickDraw,
                FeatConstants.RapidReload,
                FeatConstants.TwoWeaponFighting,
                FeatConstants.TwoWeaponDefense,
                FeatConstants.ImprovedTwoWeaponFighting,
                FeatConstants.GreaterTwoWeaponFighting,
                FeatConstants.WeaponFinesse,
                FeatConstants.WeaponFocus,
                FeatConstants.WeaponSpecialization,
                FeatConstants.GreaterWeaponFocus,
                FeatConstants.GreaterWeaponSpecialization
            };

            base.AssertDistinctCollection(GroupConstants.FighterBonusFeats, featNames);
        }
    }
}