using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Tables;
using DnDGen.TreasureGen.Items;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Items
{
    [TestFixture]
    public class ItemGroupsTests : CollectionTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Set.Collection.ItemGroups; }
        }

        [Test]
        public override void CollectionNames()
        {
            var names = new[]
            {
                FeatConstants.LightArmorProficiency,
                FeatConstants.MediumArmorProficiency,
                FeatConstants.HeavyArmorProficiency,
                FeatConstants.ShieldProficiency,
                FeatConstants.TowerShieldProficiency,
                AttributeConstants.Metal,
                AttributeConstants.Specific,
                ItemTypeConstants.Weapon,
                AttributeConstants.Ammunition,
                AttributeConstants.Melee,
                AttributeConstants.Ranged,
                AttributeConstants.TwoHanded,
            };

            AssertCollectionNames(names);
        }

        //[TestCase(FeatConstants.HeavyArmorProficiency,
        //    ArmorConstants.SplintMail,
        //    ArmorConstants.BandedMail,
        //    ArmorConstants.HalfPlate,
        //    ArmorConstants.FullPlate)]
        //[TestCase(FeatConstants.LightArmorProficiency,
        //    ArmorConstants.PaddedArmor,
        //    ArmorConstants.LeatherArmor,
        //    ArmorConstants.StuddedLeatherArmor,
        //    ArmorConstants.ChainShirt,
        //    ArmorConstants.ElvenChain,
        //    ArmorConstants.CelestialArmor)]
        //[TestCase(FeatConstants.MediumArmorProficiency,
        //    ArmorConstants.HideArmor,
        //    ArmorConstants.ScaleMail,
        //    ArmorConstants.Chainmail,
        //    ArmorConstants.Breastplate,
        //    ArmorConstants.FullPlateOfSpeed)]
        [TestCase(FeatConstants.TowerShieldProficiency, ArmorConstants.TowerShield)]
        [TestCase(AttributeConstants.Metal,
            ArmorConstants.SplintMail,
            ArmorConstants.BandedMail,
            ArmorConstants.HalfPlate,
            ArmorConstants.FullPlate,
            ArmorConstants.StuddedLeatherArmor,
            ArmorConstants.ChainShirt,
            ArmorConstants.ElvenChain,
            ArmorConstants.CelestialArmor,
            ArmorConstants.ScaleMail,
            ArmorConstants.Chainmail,
            ArmorConstants.Breastplate,
            ArmorConstants.FullPlateOfSpeed,
            ArmorConstants.HeavySteelShield,
            ArmorConstants.LightSteelShield)]
        public void ItemGroup(string name, params string[] collection)
        {
            base.AssertDistinctCollection(name, collection);
        }

        [Test]
        public void ItemGroup_LightArmorProficiency()
        {
            var lightArmors = ArmorConstants.GetAllLightArmors(true);
            AssertDistinctCollection(FeatConstants.LightArmorProficiency, [.. lightArmors]);
        }

        [Test]
        public void ItemGroup_MediumArmorProficiency()
        {
            var mediumArmors = ArmorConstants.GetAllMediumArmors(true);
            AssertDistinctCollection(FeatConstants.MediumArmorProficiency, [.. mediumArmors]);
        }

        [Test]
        public void ItemGroup_HeavyArmorProficiency()
        {
            var heavyArmors = ArmorConstants.GetAllHeavyArmors(true);
            AssertDistinctCollection(FeatConstants.HeavyArmorProficiency, [.. heavyArmors]);
        }

        [Test]
        public void ItemGroup_ShieldProficiency()
        {
            var shields = ArmorConstants.GetAllShields(true).Except([ArmorConstants.TowerShield]);
            AssertDistinctCollection(FeatConstants.ShieldProficiency, [.. shields]);
        }

        [Test]
        public void ItemGroup_Specific_ContainsAllSpecificArmors()
        {
            var armors = ArmorConstants.GetAllSpecificArmorsAndShields();
            AssertDistinctCollection(AttributeConstants.Specific, [.. armors]);
        }

        [Test]
        public void ItemGroup_Weapons()
        {
            var weapons = WeaponConstants.GetAllWeapons(false, false).ToArray();
            base.AssertDistinctCollection(ItemTypeConstants.Weapon, weapons);
        }

        [Test]
        public void ItemGroup_Ammunition()
        {
            var weapons = WeaponConstants
                .GetAllAmmunition(false, false)
                .Except([WeaponConstants.Shuriken])
                .ToArray();
            base.AssertDistinctCollection(AttributeConstants.Ammunition, weapons);
        }

        [Test]
        public void ItemGroup_Melee()
        {
            var weapons = WeaponConstants
                .GetAllMelee(false, false)
                .Except([WeaponConstants.ThrowingAxe])
                .ToArray();
            base.AssertDistinctCollection(AttributeConstants.Melee, weapons);
        }

        [Test]
        public void ItemGroup_Ranged()
        {
            var weapons = WeaponConstants
                .GetAllRanged(false, false, false)
                .Union([WeaponConstants.Shuriken, WeaponConstants.ThrowingAxe])
                .ToArray();
            base.AssertDistinctCollection(AttributeConstants.Ranged, weapons);
        }

        [Test]
        public void ItemGroup_TwoHanded()
        {
            var weapons = WeaponConstants.GetAllTwoHandedMelee(false, false).ToArray();
            base.AssertDistinctCollection(AttributeConstants.TwoHanded, weapons);
        }
    }
}
