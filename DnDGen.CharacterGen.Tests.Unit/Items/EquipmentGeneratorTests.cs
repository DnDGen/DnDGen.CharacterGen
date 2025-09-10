using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items;
using DnDGen.CharacterGen.Items.Selectors;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen;
using DnDGen.TreasureGen.Generators;
using DnDGen.TreasureGen.Items;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Items
{
    [TestFixture]
    public class EquipmentGeneratorTests
    {
        private IEquipmentGenerator equipmentGenerator;
        private Mock<IWeaponGenerator> mockWeaponGenerator;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<IArmorGenerator> mockArmorGenerator;
        private Mock<ITreasureGenerator> mockTreasureGenerator;
        private Mock<ITreasureLevelSelector> mockTreasureLevelSelector;
        private FeatCollections feats;
        private List<Feat> additionalFeats;
        private CharacterClass characterClass;
        private Weapon meleeWeapon;
        private Weapon rangedWeapon;
        private Armor armor;
        private Treasure treasure;
        private Race race;
        private Item treasureItem;

        [SetUp]
        public void Setup()
        {
            mockWeaponGenerator = new Mock<IWeaponGenerator>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockArmorGenerator = new Mock<IArmorGenerator>();
            mockTreasureGenerator = new Mock<ITreasureGenerator>();
            mockTreasureLevelSelector = new Mock<ITreasureLevelSelector>();
            equipmentGenerator = new EquipmentGenerator(
                mockCollectionsSelector.Object,
                mockWeaponGenerator.Object,
                mockTreasureGenerator.Object,
                mockArmorGenerator.Object,
                mockTreasureLevelSelector.Object);
            additionalFeats = [];
            feats = new FeatCollections { Additional = additionalFeats };
            characterClass = new CharacterClass();
            meleeWeapon = new Weapon();
            rangedWeapon = new Weapon();
            armor = new Armor();
            treasure = new Treasure();
            race = new Race();

            characterClass.Level = 9266;
            meleeWeapon.Name = "melee weapon";
            meleeWeapon.ItemType = ItemTypeConstants.Weapon;
            meleeWeapon.Attributes = [AttributeConstants.Melee];
            rangedWeapon.Name = "ranged weapon";
            rangedWeapon.ItemType = ItemTypeConstants.Weapon;
            rangedWeapon.Attributes = ["not melee"];
            armor.Name = "armor";
            armor.ItemType = ItemTypeConstants.Armor;
            treasureItem = new Item { Name = "treasure item" };
            treasure.Items = [treasureItem];

            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(meleeWeapon);
            mockWeaponGenerator.Setup(g => g.GenerateRangedFrom(feats, characterClass, race)).Returns(rangedWeapon);
            mockWeaponGenerator.Setup(g => g.GenerateMeleeFrom(feats, characterClass, race)).Returns(meleeWeapon);
            mockArmorGenerator.Setup(g => g.GenerateArmorFrom(feats, characterClass, race)).Returns(armor);
            mockTreasureLevelSelector.Setup(s => s.SelectLevelFrom(characterClass, race)).Returns(90210);
            mockTreasureGenerator.Setup(g => g.GenerateAtLevel(90210)).Returns(treasure);
        }

        [Test]
        public void GeneratesWeaponForPrimaryHand()
        {
            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(meleeWeapon));
        }

        [Test]
        public void GenerateNoWeaponForPrimaryHand()
        {
            Weapon noWeapon = null;
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(noWeapon);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.Null);
        }

        [Test]
        public void IfWeaponIsTwoHanded_PutInOffHandAsWell()
        {
            meleeWeapon.Attributes = meleeWeapon.Attributes.Union([AttributeConstants.TwoHanded]);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(meleeWeapon));
            Assert.That(equipment.OffHand, Is.EqualTo(meleeWeapon));
        }

        [Test]
        public void IfWeaponIsNotTwoHanded_DoNotPutInOffHandAsWell()
        {
            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(meleeWeapon));
            Assert.That(equipment.OffHand, Is.Null);
        }

        [Test]
        public void IfCharacterHasTwoWeaponFeats_GenerateTwoOneHandedWeapons()
        {
            var offHandWeapon = new Weapon
            {
                Attributes = [AttributeConstants.Melee]
            };

            mockWeaponGenerator.SetupSequence(g => g.GenerateOneHandedMeleeFrom(feats, characterClass, race))
                .Returns(meleeWeapon).Returns(offHandWeapon);

            additionalFeats.Add(new Feat { Name = "other feat" });
            additionalFeats.Add(new Feat { Name = "two-weapon feat" });
            var twoWeaponFeats = new[] { "two-weapon feat", "two-handed feat" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TwoHanded))
                .Returns(twoWeaponFeats);

            additionalFeats.Add(new Feat { Name = "proficiency feat", Foci = [meleeWeapon.Name] });

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(meleeWeapon));
            Assert.That(equipment.OffHand, Is.EqualTo(offHandWeapon));
        }

        [Test]
        public void IfOffHandIsEmptyButDoesNotHaveTwoWeaponFeats_DoNotGenerateSecondWeapon()
        {
            var offHandWeapon = new Weapon();
            mockWeaponGenerator.SetupSequence(g => g.GenerateFrom(feats, characterClass, race))
                .Returns(meleeWeapon).Returns(offHandWeapon);

            mockWeaponGenerator.Setup(g => g.GenerateOneHandedMeleeFrom(feats, characterClass, race)).Returns(offHandWeapon);

            additionalFeats.Add(new Feat { Name = "other feat" });
            additionalFeats.Add(new Feat { Name = "different feat" });
            var twoWeaponFeats = new[] { "two-weapon feat", "two-handed feat" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TwoHanded))
                .Returns(twoWeaponFeats);

            additionalFeats.Add(new Feat { Name = "proficiency feat", Foci = [meleeWeapon.Name] });

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(meleeWeapon));
            Assert.That(equipment.OffHand, Is.Null);
        }

        [Test]
        public void GenerateArmor()
        {
            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.Armor, Is.EqualTo(armor));
        }

        [Test]
        public void CanGenerateNoArmor()
        {
            Armor noArmor = null;
            mockArmorGenerator.Setup(g => g.GenerateArmorFrom(feats, characterClass, race)).Returns(noArmor);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.Armor, Is.Null);
        }

        [Test]
        public void IfOffHandIsEmptyAndProficientInShields_GenerateShield()
        {
            var shield = new Armor();
            mockArmorGenerator.SetupSequence(g => g.GenerateShieldFrom(feats, characterClass, race)).Returns(shield);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.OffHand, Is.EqualTo(shield));
        }

        [Test]
        public void IfOffHandIsEmptyAndProficientInShields_CanGenerateNoShield()
        {
            Armor shield = null;
            mockArmorGenerator.SetupSequence(g => g.GenerateShieldFrom(feats, characterClass, race)).Returns(shield);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.OffHand, Is.Null);
        }

        [Test]
        public void IfOffHandIsNotEmptyAndProficientInShields_DoNotGenerateShield()
        {
            meleeWeapon.Attributes = meleeWeapon.Attributes.Union([AttributeConstants.TwoHanded]);

            var shield = new Armor();
            mockArmorGenerator.SetupSequence(g => g.GenerateShieldFrom(feats, characterClass, race)).Returns(shield);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(meleeWeapon), equipment.PrimaryHand.Name);
            Assert.That(equipment.OffHand, Is.EqualTo(meleeWeapon), equipment.OffHand.Name);
        }

        [Test]
        public void GenerateTreasure()
        {
            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.Treasure, Is.EqualTo(treasure));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
        }

        [Test]
        public void IfWeaponRequiresAmmunition_GenerateMatchingAmmunitionAndAddToTreasure()
        {
            var ammo = new Weapon
            {
                Name = "ammo",
                Attributes = [AttributeConstants.Ammunition]
            };

            meleeWeapon.Ammunition = "ammo";
            mockWeaponGenerator.Setup(g => g.GenerateAmmunition(characterClass, race, "ammo")).Returns(ammo);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(meleeWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Contains.Item(ammo));
        }

        [Test]
        public void IfWeaponDoesNotRequireAmmunition_DoNotGenerateMatchingAmmunitionOrAddToTreasure()
        {
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(rangedWeapon);

            var ammo = new Weapon
            {
                Attributes = [AttributeConstants.Ammunition]
            };

            mockWeaponGenerator.Setup(g => g.GenerateAmmunition(characterClass, race, It.IsAny<string>())).Returns(ammo);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(rangedWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Is.All.Not.EqualTo(ammo));
        }

        [Test]
        public void IfPrimaryHandIsNotMelee_GenerateMeleeWeaponForTreasure()
        {
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(rangedWeapon);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(rangedWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Contains.Item(meleeWeapon));
        }

        [Test]
        public void GenerateNoMeleeWeaponForTreasure()
        {
            Weapon noMeleeWeapon = null;
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(rangedWeapon);
            mockWeaponGenerator.Setup(g => g.GenerateMeleeFrom(feats, characterClass, race)).Returns(noMeleeWeapon);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(rangedWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Is.All.Not.Null);
        }

        [Test]
        public void AllowTwoHandedMeleeIfCharacterDoesNotHaveTwoWeaponFeat()
        {
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(rangedWeapon);

            var twoHandedWeapon = new Weapon
            {
                Attributes = [AttributeConstants.Melee, AttributeConstants.TwoHanded]
            };

            mockWeaponGenerator.Setup(g => g.GenerateMeleeFrom(feats, characterClass, race)).Returns(twoHandedWeapon);

            additionalFeats.Add(new Feat { Name = "other feat" });
            var twoWeaponFeats = new[] { "two-weapon feat", "two-handed feat" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TwoHanded))
                .Returns(twoWeaponFeats);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(rangedWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Contains.Item(twoHandedWeapon), string.Join(",", equipment.Treasure.Items.Select(i => i.Name)));
            Assert.That(equipment.Treasure.Items, Is.All.Not.EqualTo(meleeWeapon));
        }

        [Test]
        public void IfMeleeWeaponIsOneHanded_GenerateShield()
        {
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(rangedWeapon);

            var shield = new Armor
            {
                Attributes = [AttributeConstants.Shield]
            };
            mockArmorGenerator.Setup(g => g.GenerateShieldFrom(feats, characterClass, race)).Returns(shield);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(rangedWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Contains.Item(meleeWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(shield));
        }

        [Test]
        public void IfMeleeWeaponIsOneHanded_GenerateNoShield()
        {
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(rangedWeapon);

            mockArmorGenerator.Setup(g => g.GenerateShieldFrom(feats, characterClass, race)).Returns((Armor)null);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(rangedWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Contains.Item(meleeWeapon));
            Assert.That(equipment.Treasure.Items, Is.All.Not.Null);
        }

        [Test]
        public void IfMeleeWeaponIsTwoHanded_DoNotGenerateShield()
        {
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(rangedWeapon);

            var twoHandedWeapon = new Weapon
            {
                Attributes = [AttributeConstants.Melee, AttributeConstants.TwoHanded]
            };

            mockWeaponGenerator.Setup(g => g.GenerateMeleeFrom(feats, characterClass, race)).Returns(twoHandedWeapon);

            var shield = new Armor
            {
                Attributes = [AttributeConstants.Shield]
            };
            mockArmorGenerator.Setup(g => g.GenerateShieldFrom(feats, characterClass, race)).Returns(shield);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(rangedWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Contains.Item(twoHandedWeapon));
            Assert.That(equipment.Treasure.Items, Is.All.Not.EqualTo(shield));

            mockArmorGenerator.Verify(g => g.GenerateShieldFrom(feats, characterClass, race), Times.Never);
        }

        [Test]
        public void IfPrimaryHandIsMelee_GenerateRangedToCarry()
        {
            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(meleeWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Contains.Item(rangedWeapon));
        }

        [Test]
        public void GenerateNoRangedToCarry()
        {
            Weapon noRangedWeapon = null;
            mockWeaponGenerator.Setup(g => g.GenerateRangedFrom(feats, characterClass, race)).Returns(noRangedWeapon);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(meleeWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Is.All.Not.Null);
        }

        [Test]
        public void IfPrimaryHandIsMelee_GenerateNoRangedOrAmmunitionToCarry()
        {
            Weapon noRangedWeapon = null;
            mockWeaponGenerator.Setup(g => g.GenerateRangedFrom(feats, characterClass, race)).Returns(noRangedWeapon);

            var ammo = new Weapon
            {
                Name = "ammunition",
                Attributes = [AttributeConstants.Ammunition]
            };
            mockWeaponGenerator.Setup(g => g.GenerateAmmunition(characterClass, race, "ammunition")).Returns(ammo);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(meleeWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Is.All.Not.Null);
            Assert.That(equipment.Treasure.Items, Is.All.Not.EqualTo(ammo));
        }

        [Test]
        public void IfPrimaryHandIsMelee_GenerateRangedAndAmmunitionToCarry()
        {
            var ammo = new Weapon
            {
                Name = "ammunition",
                Attributes = [AttributeConstants.Ammunition]
            };
            rangedWeapon.Ammunition = "ammunition";

            mockWeaponGenerator.Setup(g => g.GenerateAmmunition(characterClass, race, "ammunition")).Returns(ammo);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(meleeWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Contains.Item(rangedWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(ammo));
        }

        [Test]
        public void IfPrimaryHandIsMelee_GenerateRangedWithoutAmmunitionToCarry()
        {
            var ammo = new Weapon
            {
                Name = "ammunition",
                Attributes = [AttributeConstants.Ammunition]
            };
            mockWeaponGenerator.Setup(g => g.GenerateAmmunition(characterClass, race, It.IsAny<string>())).Returns(ammo);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(meleeWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Contains.Item(rangedWeapon));
            Assert.That(equipment.Treasure.Items, Is.All.Not.EqualTo(ammo));
            Assert.That(equipment.Treasure.Items, Is.All.Not.Null);
        }

        [Test]
        public void IfPrimaryHandIsOneHandedRangedWeapon_DoNotEquipShield()
        {
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(rangedWeapon);

            var shield = new Armor
            {
                Attributes = [AttributeConstants.Shield]
            };
            mockArmorGenerator.Setup(g => g.GenerateShieldFrom(feats, characterClass, race)).Returns(shield);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.OffHand, Is.Null);
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Contains.Item(shield));
        }

        [Test]
        public void IfNoPrimaryHand_EquipShield()
        {
            Weapon noWeapon = null;
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(noWeapon);

            var shield = new Armor
            {
                Attributes = [AttributeConstants.Shield]
            };
            mockArmorGenerator.Setup(g => g.GenerateShieldFrom(feats, characterClass, race)).Returns(shield);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.Null);
            Assert.That(equipment.OffHand, Is.EqualTo(shield));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Is.All.Not.EqualTo(shield));
        }

        [Test]
        public void IfNoPrimaryHand_EquipNoShield()
        {
            Weapon noWeapon = null;
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(noWeapon);

            mockArmorGenerator.Setup(g => g.GenerateShieldFrom(feats, characterClass, race)).Returns((Armor)null);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.Null);
            Assert.That(equipment.OffHand, Is.Null);
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Is.All.Not.Null);
        }

        [Test]
        public void IfCharacterOnlyKnowsRangedWeapons_DoNotTryToGenerateMeleeWeaponsForTwoWeaponFeats()
        {
            Weapon noWeapon = null;
            mockWeaponGenerator.Setup(g => g.GenerateOneHandedMeleeFrom(feats, characterClass, race)).Returns(noWeapon);
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(rangedWeapon);

            additionalFeats.Add(new Feat { Name = "other feat" });
            additionalFeats.Add(new Feat { Name = "two-weapon feat" });
            var twoWeaponFeats = new[] { "two-weapon feat", "two-handed feat" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TwoHanded)).Returns(twoWeaponFeats);

            additionalFeats.Add(new Feat { Name = "proficiency feat", Foci = [rangedWeapon.Name, "other ranged weapon"] });

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(rangedWeapon));
            Assert.That(equipment.OffHand, Is.Null);
        }

        [Test]
        public void IfCharacterOnlyKnowsTwoHandedWeapons_DoNotTryToGenerateOneHandedMeleeWeaponsForTwoWeaponFeats()
        {
            Weapon noWeapon = null;
            mockWeaponGenerator.Setup(g => g.GenerateOneHandedMeleeFrom(feats, characterClass, race)).Returns(noWeapon);
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(meleeWeapon);

            meleeWeapon.Attributes = meleeWeapon.Attributes.Union([AttributeConstants.TwoHanded]);

            additionalFeats.Add(new Feat { Name = "other feat" });
            additionalFeats.Add(new Feat { Name = "two-weapon feat" });
            var twoWeaponFeats = new[] { "two-weapon feat", "two-handed feat" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TwoHanded))
                .Returns(twoWeaponFeats);

            additionalFeats.Add(new Feat { Name = "proficiency feat", Foci = [meleeWeapon.Name] });

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(meleeWeapon));
            Assert.That(equipment.OffHand, Is.EqualTo(meleeWeapon));
            Assert.That(equipment.OffHand, Is.EqualTo(equipment.PrimaryHand));
        }

        [Test]
        public void IfCharacterOnlyKnowsRangedWeaponsAndHasWeaponFamiliarity_DoNotTryToGenerateMeleeWeaponsForTwoWeaponFeats()
        {
            Weapon noWeapon = null;
            mockWeaponGenerator.Setup(g => g.GenerateOneHandedMeleeFrom(feats, characterClass, race)).Returns(noWeapon);
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(rangedWeapon);

            additionalFeats.Add(new Feat { Name = "other feat" });
            additionalFeats.Add(new Feat { Name = "two-weapon feat" });
            var twoWeaponFeats = new[] { "two-weapon feat", "two-handed feat" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TwoHanded))
                .Returns(twoWeaponFeats);

            additionalFeats.Add(new Feat { Name = "proficiency feat", Foci = [rangedWeapon.Name, "other ranged weapon"] });
            additionalFeats.Add(new Feat { Name = FeatConstants.WeaponFamiliarity, Foci = [meleeWeapon.Name] });

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(rangedWeapon));
            Assert.That(equipment.OffHand, Is.Null);
        }

        [Test]
        public void IfCharacterOnlyKnowsTwoHandedWeaponsAndHasWeaponFamiliarity_DoNotTryToGenerateOneHandedMeleeWeaponsForTwoWeaponFeats()
        {
            Weapon noWeapon = null;
            mockWeaponGenerator.Setup(g => g.GenerateOneHandedMeleeFrom(feats, characterClass, race)).Returns(noWeapon);
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(meleeWeapon);

            meleeWeapon.Attributes = meleeWeapon.Attributes.Union([AttributeConstants.TwoHanded]);

            additionalFeats.Add(new Feat { Name = "other feat" });
            additionalFeats.Add(new Feat { Name = "two-weapon feat" });
            var twoWeaponFeats = new[] { "two-weapon feat", "two-handed feat" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TwoHanded))
                .Returns(twoWeaponFeats);

            additionalFeats.Add(new Feat { Name = "proficiency feat", Foci = [meleeWeapon.Name] });
            additionalFeats.Add(new Feat { Name = FeatConstants.WeaponFamiliarity, Foci = ["one-handed melee weapon"] });

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(meleeWeapon));
            Assert.That(equipment.OffHand, Is.EqualTo(meleeWeapon));
            Assert.That(equipment.OffHand, Is.EqualTo(equipment.PrimaryHand));
        }
    }
}
