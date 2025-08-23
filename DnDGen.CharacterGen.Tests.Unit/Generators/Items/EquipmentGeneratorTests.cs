using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items;
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

namespace DnDGen.CharacterGen.Tests.Unit.Generators.Items
{
    [TestFixture]
    public class EquipmentGeneratorTests
    {
        private IEquipmentGenerator equipmentGenerator;
        private Mock<IWeaponGenerator> mockWeaponGenerator;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<IArmorGenerator> mockArmorGenerator;
        private Mock<ITreasureGenerator> mockTreasureGenerator;
        private List<Feat> feats;
        private CharacterClass characterClass;
        private Weapon meleeWeapon;
        private Weapon rangedWeapon;
        private Armor armor;
        private Treasure treasure;
        private Race race;
        private List<string> shieldProficiencyFeats;
        private List<string> weaponProficiencyFeats;
        private Item treasureItem;

        [SetUp]
        public void Setup()
        {
            mockWeaponGenerator = new Mock<IWeaponGenerator>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockArmorGenerator = new Mock<IArmorGenerator>();
            mockTreasureGenerator = new Mock<ITreasureGenerator>();
            equipmentGenerator = new EquipmentGenerator(mockCollectionsSelector.Object, mockWeaponGenerator.Object, mockTreasureGenerator.Object, mockArmorGenerator.Object);
            feats = new List<Feat>();
            characterClass = new CharacterClass();
            meleeWeapon = new Weapon();
            rangedWeapon = new Weapon();
            armor = new Armor();
            treasure = new Treasure();
            race = new Race();
            shieldProficiencyFeats = new List<string>();
            weaponProficiencyFeats = new List<string>();

            characterClass.Level = 9266;
            meleeWeapon.Name = "melee weapon";
            meleeWeapon.ItemType = ItemTypeConstants.Weapon;
            meleeWeapon.Attributes = new[] { AttributeConstants.Melee };
            rangedWeapon.Name = "ranged weapon";
            rangedWeapon.ItemType = ItemTypeConstants.Weapon;
            rangedWeapon.Attributes = new[] { "not melee" };
            armor.Name = "armor";
            armor.ItemType = ItemTypeConstants.Armor;
            treasureItem = new Item { Name = "treasure item" };
            treasure.Items = new[] { treasureItem };

            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(meleeWeapon);
            mockWeaponGenerator.Setup(g => g.GenerateRangedFrom(feats, characterClass, race)).Returns(rangedWeapon);
            mockWeaponGenerator.Setup(g => g.GenerateMeleeFrom(feats, characterClass, race)).Returns(meleeWeapon);
            mockArmorGenerator.Setup(g => g.GenerateArmorFrom(feats, characterClass, race)).Returns(armor);
            mockTreasureGenerator.Setup(g => g.GenerateAtLevel(9266)).Returns(treasure);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, AttributeConstants.Shield + GroupConstants.Proficiency))
                .Returns(shieldProficiencyFeats);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, ItemTypeConstants.Weapon + GroupConstants.Proficiency))
                .Returns(weaponProficiencyFeats);
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
            meleeWeapon.Attributes = meleeWeapon.Attributes.Union(new[] { AttributeConstants.TwoHanded });

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
            var offHandWeapon = new Weapon();
            offHandWeapon.Attributes = new[] { AttributeConstants.Melee };

            mockWeaponGenerator.SetupSequence(g => g.GenerateOneHandedMeleeFrom(feats, characterClass, race))
                .Returns(meleeWeapon).Returns(offHandWeapon);

            feats.Add(new Feat { Name = "other feat" });
            feats.Add(new Feat { Name = "two-weapon feat" });
            var twoWeaponFeats = new[] { "two-weapon feat", "two-handed feat" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TwoHanded))
                .Returns(twoWeaponFeats);

            feats.Add(new Feat { Name = "proficiency feat", Foci = new[] { meleeWeapon.Name } });

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

            feats.Add(new Feat { Name = "other feat" });
            feats.Add(new Feat { Name = "different feat" });
            var twoWeaponFeats = new[] { "two-weapon feat", "two-handed feat" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TwoHanded))
                .Returns(twoWeaponFeats);

            feats.Add(new Feat { Name = "proficiency feat", Foci = new[] { meleeWeapon.Name } });

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
            meleeWeapon.Attributes = meleeWeapon.Attributes.Union(new[] { AttributeConstants.TwoHanded });

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
            var ammo = new Weapon();
            ammo.Name = "ammo";
            ammo.Attributes = new[] { AttributeConstants.Ammunition };

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

            var ammo = new Weapon();
            ammo.Attributes = new[] { AttributeConstants.Ammunition };

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

            var twoHandedWeapon = new Weapon();
            twoHandedWeapon.Attributes = new[] { AttributeConstants.Melee, AttributeConstants.TwoHanded };

            mockWeaponGenerator.Setup(g => g.GenerateMeleeFrom(feats, characterClass, race)).Returns(twoHandedWeapon);

            feats.Add(new Feat { Name = "other feat" });
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
        public void IfMeleeWeaponIsOneHandedAndProficientInShields_GenerateShield()
        {
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(rangedWeapon);

            feats.Add(new Feat { Name = "feat" });
            feats.Add(new Feat { Name = "other feat" });
            shieldProficiencyFeats.Add(feats[0].Name);

            var shield = new Armor();
            shield.Attributes = new[] { AttributeConstants.Shield };
            mockArmorGenerator.Setup(g => g.GenerateShieldFrom(feats, characterClass, race)).Returns(shield);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(rangedWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Contains.Item(meleeWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(shield));
        }

        [Test]
        public void IfMeleeWeaponIsTwoHandedAndProficientInShields_DoNotGenerateShield()
        {
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(rangedWeapon);

            var twoHandedWeapon = new Weapon();
            twoHandedWeapon.Attributes = new[] { AttributeConstants.Melee, AttributeConstants.TwoHanded };

            mockWeaponGenerator.Setup(g => g.GenerateMeleeFrom(feats, characterClass, race)).Returns(twoHandedWeapon);

            feats.Add(new Feat { Name = "feat" });
            feats.Add(new Feat { Name = "other feat" });
            shieldProficiencyFeats.Add(feats[0].Name);

            var shield = new Armor();
            shield.Attributes = new[] { AttributeConstants.Shield };
            mockArmorGenerator.Setup(g => g.GenerateShieldFrom(feats, characterClass, race)).Returns(shield);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(rangedWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Contains.Item(twoHandedWeapon));
            Assert.That(equipment.Treasure.Items, Is.All.Not.EqualTo(shield));
        }

        [Test]
        public void IfMeleeWeaponIsOneHandedAndNotProficientInShields_DoNotGenerateShield()
        {
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(rangedWeapon);

            feats.Add(new Feat { Name = "feat" });
            feats.Add(new Feat { Name = "other feat" });

            Armor noShield = null;
            mockArmorGenerator.Setup(g => g.GenerateShieldFrom(feats, characterClass, race)).Returns(noShield);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(rangedWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Contains.Item(meleeWeapon));
            Assert.That(equipment.Treasure.Items, Is.All.Not.Null);
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

            var ammo = new Weapon();
            ammo.Name = "ammunition";
            ammo.Attributes = new[] { AttributeConstants.Ammunition };
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
            var ammo = new Weapon();
            ammo.Name = "ammunition";
            ammo.Attributes = new[] { AttributeConstants.Ammunition };
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
            var ammo = new Weapon();
            ammo.Name = "ammunition";
            ammo.Attributes = new[] { AttributeConstants.Ammunition };

            mockWeaponGenerator.Setup(g => g.GenerateAmmunition(characterClass, race, It.IsAny<string>())).Returns(ammo);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(meleeWeapon));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Contains.Item(rangedWeapon));
            Assert.That(equipment.Treasure.Items, Is.All.Not.EqualTo(ammo));
            Assert.That(equipment.Treasure.Items, Is.All.Not.Null);
        }

        [Test]
        public void IfPrimaryHandIsOneHandedRangedWeaponAndProficientInShields_DoNotEquipShield()
        {
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(rangedWeapon);

            feats.Add(new Feat { Name = "feat" });
            feats.Add(new Feat { Name = "other feat" });
            shieldProficiencyFeats.Add(feats[0].Name);

            var shield = new Armor();
            shield.Attributes = new[] { AttributeConstants.Shield };
            mockArmorGenerator.Setup(g => g.GenerateShieldFrom(feats, characterClass, race)).Returns(shield);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.OffHand, Is.Null);
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Contains.Item(shield));
        }

        [Test]
        public void IfNoPrimaryHandAndProficientInShields_EquipShield()
        {
            Weapon noWeapon = null;
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(noWeapon);

            feats.Add(new Feat { Name = "feat" });
            feats.Add(new Feat { Name = "other feat", });
            shieldProficiencyFeats.Add(feats[0].Name);

            var shield = new Armor();
            shield.Attributes = new[] { AttributeConstants.Shield };
            mockArmorGenerator.Setup(g => g.GenerateShieldFrom(feats, characterClass, race)).Returns(shield);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.Null);
            Assert.That(equipment.OffHand, Is.EqualTo(shield));
            Assert.That(equipment.Treasure.Items, Contains.Item(treasureItem));
            Assert.That(equipment.Treasure.Items, Is.All.Not.EqualTo(shield));
        }

        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(4, 2)]
        [TestCase(5, 2)]
        [TestCase(6, 3)]
        [TestCase(7, 3)]
        [TestCase(8, 4)]
        [TestCase(9, 4)]
        [TestCase(10, 5)]
        [TestCase(11, 5)]
        [TestCase(12, 6)]
        [TestCase(13, 6)]
        [TestCase(14, 7)]
        [TestCase(15, 7)]
        [TestCase(16, 8)]
        [TestCase(17, 8)]
        [TestCase(18, 9)]
        [TestCase(19, 9)]
        [TestCase(20, 10)]
        public void NPCIsHalfLevel(int npcLevel, int effectiveLevel)
        {
            characterClass.Level = npcLevel;
            characterClass.Name = "class name";
            characterClass.IsNPC = true;

            var npcTreasure = new Treasure();
            mockTreasureGenerator.Setup(g => g.GenerateAtLevel(effectiveLevel)).Returns(npcTreasure);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.Treasure, Is.EqualTo(npcTreasure));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        [TestCase(6, 6)]
        [TestCase(7, 7)]
        [TestCase(8, 8)]
        [TestCase(9, 9)]
        [TestCase(10, 10)]
        [TestCase(11, 11)]
        [TestCase(12, 12)]
        [TestCase(13, 13)]
        [TestCase(14, 14)]
        [TestCase(15, 15)]
        [TestCase(16, 16)]
        [TestCase(17, 17)]
        [TestCase(18, 18)]
        [TestCase(19, 19)]
        [TestCase(20, 20)]
        public void PlayerCharacterIsFullLevel(int level, int effectiveLevel)
        {
            characterClass.Level = level;
            characterClass.Name = "class name";
            characterClass.IsNPC = false;

            var playerTreasure = new Treasure();
            mockTreasureGenerator.Setup(g => g.GenerateAtLevel(effectiveLevel)).Returns(playerTreasure);

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.Treasure, Is.EqualTo(playerTreasure));
        }

        [Test]
        public void IfCharacterOnlyKnowsRangedWeapons_DoNotTryToGenerateMeleeWeaponsForTwoWeaponFeats()
        {
            Weapon noWeapon = null;
            mockWeaponGenerator.Setup(g => g.GenerateOneHandedMeleeFrom(feats, characterClass, race)).Returns(noWeapon);
            mockWeaponGenerator.Setup(g => g.GenerateFrom(feats, characterClass, race)).Returns(rangedWeapon);

            feats.Add(new Feat { Name = "other feat" });
            feats.Add(new Feat { Name = "two-weapon feat" });
            var twoWeaponFeats = new[] { "two-weapon feat", "two-handed feat" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TwoHanded)).Returns(twoWeaponFeats);

            feats.Add(new Feat { Name = "proficiency feat", Foci = new[] { rangedWeapon.Name, "other ranged weapon" } });

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

            meleeWeapon.Attributes = meleeWeapon.Attributes.Union(new[] { AttributeConstants.TwoHanded });

            feats.Add(new Feat { Name = "other feat" });
            feats.Add(new Feat { Name = "two-weapon feat" });
            var twoWeaponFeats = new[] { "two-weapon feat", "two-handed feat" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TwoHanded))
                .Returns(twoWeaponFeats);

            feats.Add(new Feat { Name = "proficiency feat", Foci = new[] { meleeWeapon.Name } });

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

            feats.Add(new Feat { Name = "other feat" });
            feats.Add(new Feat { Name = "two-weapon feat" });
            var twoWeaponFeats = new[] { "two-weapon feat", "two-handed feat" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TwoHanded))
                .Returns(twoWeaponFeats);

            feats.Add(new Feat { Name = "proficiency feat", Foci = new[] { rangedWeapon.Name, "other ranged weapon" } });
            feats.Add(new Feat { Name = FeatConstants.WeaponFamiliarity, Foci = new[] { meleeWeapon.Name } });

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

            meleeWeapon.Attributes = meleeWeapon.Attributes.Union(new[] { AttributeConstants.TwoHanded });

            feats.Add(new Feat { Name = "other feat" });
            feats.Add(new Feat { Name = "two-weapon feat" });
            var twoWeaponFeats = new[] { "two-weapon feat", "two-handed feat" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TwoHanded))
                .Returns(twoWeaponFeats);

            feats.Add(new Feat { Name = "proficiency feat", Foci = new[] { meleeWeapon.Name } });
            feats.Add(new Feat { Name = FeatConstants.WeaponFamiliarity, Foci = new[] { "one-handed melee weapon" } });

            var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
            Assert.That(equipment.PrimaryHand, Is.EqualTo(meleeWeapon));
            Assert.That(equipment.OffHand, Is.EqualTo(meleeWeapon));
            Assert.That(equipment.OffHand, Is.EqualTo(equipment.PrimaryHand));
        }
    }
}
