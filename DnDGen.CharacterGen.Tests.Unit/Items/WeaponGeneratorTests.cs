using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Generators.Items;
using DnDGen.CharacterGen.Items;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Generators;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Items
{
    [TestFixture]
    public class WeaponGeneratorTests
    {
        private IWeaponGenerator weaponGenerator;
        private Mock<IPercentileSelector> mockPercentileSelector;
        private Mock<MundaneItemGenerator> mockMundaneWeaponGenerator;
        private Mock<MagicalItemGenerator> mockMagicalWeaponGenerator;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<JustInTimeFactory> mockJustInTimeFactory;
        private Weapon magicalWeapon;
        private List<Feat> feats;
        private List<string> proficiencyFeats;
        private CharacterClass characterClass;
        private List<string> allWeapons;
        private List<string> allAmmunitions;
        private List<string> allMeleeWeapons;
        private List<string> allRangedWeapons;
        private List<string> allTwoHandedWeapons;
        private List<string> allProficientWeapons;
        private Race race;
        private string powerTableName;
        private string power;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<IPercentileSelector>();
            mockMundaneWeaponGenerator = new Mock<MundaneItemGenerator>();
            mockMagicalWeaponGenerator = new Mock<MagicalItemGenerator>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockJustInTimeFactory = new Mock<JustInTimeFactory>();
            weaponGenerator = new WeaponGenerator(mockCollectionsSelector.Object, mockPercentileSelector.Object, mockJustInTimeFactory.Object);
            magicalWeapon = new Weapon();
            feats = new List<Feat>();
            characterClass = new CharacterClass();
            proficiencyFeats = new List<string>();
            allWeapons = new List<string>();
            allAmmunitions = new List<string>();
            allMeleeWeapons = new List<string>();
            allRangedWeapons = new List<string>();
            allTwoHandedWeapons = new List<string>();
            allProficientWeapons = new List<string>();
            race = new Race();

            race.Size = "size";
            race.BaseRace = "base race";
            magicalWeapon = CreateOneHandedMeleeWeapon("magical weapon");
            magicalWeapon.IsMagical = true;
            characterClass.Name = "class name";
            characterClass.Level = 9266;
            feats.Add(new Feat { Name = "all proficiency", Foci = new[] { FeatConstants.Foci.All } });
            proficiencyFeats.Add(feats[0].Name);

            allWeapons.Add("other weapon");
            allWeapons.Add("other melee");
            allWeapons.Add("other ranged");
            allWeapons.Add("other two-handed");
            allWeapons.Add("other ammo");
            allAmmunitions.Add("other ammo");
            allMeleeWeapons.Add("other weapon");
            allMeleeWeapons.Add("other melee");
            allMeleeWeapons.Add("other two-handed");
            allRangedWeapons.Add("other ranged");
            allRangedWeapons.Add("other ammo");
            allTwoHandedWeapons.Add("other two-handed");
            allProficientWeapons.Add("other weapon");
            allProficientWeapons.Add("other melee");
            allProficientWeapons.Add("other ranged");
            allProficientWeapons.Add("other two-handed");
            allProficientWeapons.Add("other ammo");

            powerTableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, 9266);
            power = "power";
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.ToArray())))
                .Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, ItemTypeConstants.Weapon + GroupConstants.Proficiency))
                .Returns(proficiencyFeats);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatFoci, feats[0].Name)).Returns(allProficientWeapons);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, ItemTypeConstants.Weapon)).Returns(allWeapons);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Ammunition)).Returns(allAmmunitions);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Melee)).Returns(allMeleeWeapons);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Ranged)).Returns(allRangedWeapons);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.TwoHanded)).Returns(allTwoHandedWeapons);

            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon)).Returns(mockMundaneWeaponGenerator.Object);
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>(ItemTypeConstants.Weapon)).Returns(mockMagicalWeaponGenerator.Object);
        }

        private IEnumerable<string> ProficientSet(params string[] expected)
        {
            return It.Is<IEnumerable<string>>(ss => ss.Intersect(expected).Count() == expected.Length && ss.Count() == expected.Length);
        }

        [Test]
        public void GenerateFrom_GenerateNoWeapon()
        {
            feats.Add(new Feat { Name = "feat 1" });
            feats.Add(new Feat { Name = "feat 2", Foci = new[] { FeatConstants.Foci.UnarmedStrike } });

            proficiencyFeats.Clear();
            proficiencyFeats.Add(feats[1].Name);
            proficiencyFeats.Add(feats[2].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.Null);
        }

        [Test]
        public void GenerateFrom_GenerateMundaneWeapon()
        {
            var mundaneWeapon = CreateOneHandedMeleeWeapon("mundane weapon");
            var wrongMundaneWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            allProficientWeapons.Remove(magicalWeapon.Name);

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Except(allAmmunitions).ToArray())))
                .Returns("my random weapon");
            mockMundaneWeaponGenerator.Setup(g => g.Generate("my random weapon", race.Size)).Returns(mundaneWeapon);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(mundaneWeapon));
        }

        private Weapon CreateWeapon(string name)
        {
            var weapon = new Weapon();
            weapon.Name = name;
            weapon.ItemType = ItemTypeConstants.Weapon;
            weapon.Size = race.Size;

            allWeapons.Add(name);
            allProficientWeapons.Add(name);

            return weapon;
        }

        private Weapon CreateOneHandedMeleeWeapon(string name)
        {
            var weapon = CreateWeapon(name);
            weapon.Attributes = new[] { AttributeConstants.Melee };

            allMeleeWeapons.Add(name);

            return weapon;
        }

        private Weapon CreateTwoHandedMeleeWeapon(string name)
        {
            var weapon = CreateWeapon(name);
            weapon.Attributes = new[] { AttributeConstants.Melee, AttributeConstants.TwoHanded };

            allMeleeWeapons.Add(name);
            allTwoHandedWeapons.Add(name);

            return weapon;
        }

        private Weapon CreateRangedWeapon(string name)
        {
            var weapon = CreateWeapon(name);
            weapon.Attributes = new[] { AttributeConstants.Ranged };

            allRangedWeapons.Add(name);

            return weapon;
        }

        private Weapon CreateAmmunition(string name)
        {
            var weapon = CreateWeapon(name);
            weapon.Attributes = new[] { AttributeConstants.Ranged, AttributeConstants.Ammunition };

            allRangedWeapons.Add(name);
            allAmmunitions.Add(name);

            return weapon;
        }

        [Test]
        public void GenerateFrom_CanWieldSpecificMundaneWeaponProficiency()
        {
            var mundaneWeapon = CreateOneHandedMeleeWeapon("mundane weapon");
            var wrongMundaneWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            var specialties = new[] { mundaneWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(specialties))).Returns("my random weapon");
            mockMundaneWeaponGenerator.Setup(g => g.Generate("my random weapon", race.Size)).Returns(mundaneWeapon);

            feats.Add(new Feat { Name = "feat2", Foci = specialties });
            proficiencyFeats.Add(feats[1].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(mundaneWeapon));
        }

        [Test]
        public void GenerateFrom_PreferMundaneWeaponsPickedAsFocusForNonProficiencyFeats()
        {
            var mundaneWeapon = CreateOneHandedMeleeWeapon("mundane weapon");
            var wrongMundaneWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            var specialties = new[] { mundaneWeapon.Name };
            var wrongSpecialties = new[] { wrongMundaneWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(specialties))).Returns("my random weapon");
            mockMundaneWeaponGenerator.Setup(g => g.Generate("my random weapon", race.Size)).Returns(mundaneWeapon);

            feats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            feats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add(feats[1].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(mundaneWeapon));
        }

        [Test]
        public void GenerateFrom_DoNotPreferMundaneWeaponPickedAsFocusForWeaponFamiliarity()
        {
            var mundaneWeapon = CreateOneHandedMeleeWeapon("mundane weapon");
            var wrongMundaneWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            var specialties = new[] { mundaneWeapon.Name };
            var wrongSpecialties = new[] { wrongMundaneWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(specialties))).Returns("my random weapon");
            mockMundaneWeaponGenerator.Setup(g => g.Generate("my random weapon", race.Size)).Returns(mundaneWeapon);

            feats.Add(new Feat { Name = FeatConstants.WeaponFamiliarity, Foci = wrongSpecialties });
            feats.Add(new Feat { Name = "feat3", Foci = specialties });

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(mundaneWeapon), weapon.Name);
        }

        [Test]
        public void GenerateFrom_PreferAnyMundaneWeaponsPickedAsFocusForNonProficiencyFeats()
        {
            var mundaneWeapon = CreateOneHandedMeleeWeapon("mundane weapon");
            var otherMundaneWeapon = CreateOneHandedMeleeWeapon("other mundane weapon");
            var wrongMundaneWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            var specialties = new[] { mundaneWeapon.Name };
            var wrongSpecialties = new[] { wrongMundaneWeapon.Name };
            var multipleSpecialties = new[] { mundaneWeapon.Name, otherMundaneWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(multipleSpecialties))).Returns("my random weapon");
            mockMundaneWeaponGenerator.Setup(g => g.Generate("my random weapon", race.Size)).Returns(otherMundaneWeapon);

            feats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            feats.Add(new Feat { Name = "feat3", Foci = multipleSpecialties });
            feats.Add(new Feat { Name = "feat4", Foci = specialties });
            proficiencyFeats.Add(feats[1].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(otherMundaneWeapon), weapon.Name);
        }

        [Test]
        public void GenerateFrom_PreferMundaneWeaponsPickedAsFocusForProficiencyFeats()
        {
            var mundaneWeapon = CreateOneHandedMeleeWeapon("mundane weapon");
            var wrongMundaneWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            var specialties = new[] { mundaneWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(specialties))).Returns("my random weapon");
            mockMundaneWeaponGenerator.Setup(g => g.Generate("my random weapon", race.Size)).Returns(mundaneWeapon);

            feats.Add(new Feat { Name = "feat2", Foci = specialties });
            proficiencyFeats.Add(feats[1].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(mundaneWeapon));
        }

        [Test]
        public void GenerateFrom_PreferAnyMundaneWeaponsPickedAsFocusForProficiencyFeats()
        {
            var mundaneWeapon = CreateOneHandedMeleeWeapon("mundane weapon");
            var otherMundaneWeapon = CreateOneHandedMeleeWeapon("other mundane weapon");
            var wrongMundaneWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            var specialties = new[] { mundaneWeapon.Name };
            var multipleSpecialties = new[] { mundaneWeapon.Name, otherMundaneWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(multipleSpecialties))).Returns("my random weapon");
            mockMundaneWeaponGenerator.Setup(g => g.Generate("my random weapon", race.Size)).Returns(otherMundaneWeapon);

            feats.Add(new Feat { Name = "feat2", Foci = multipleSpecialties });
            feats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add(feats[1].Name);
            proficiencyFeats.Add(feats[2].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(otherMundaneWeapon));
        }

        [Test]
        public void GenerateFrom_NoPreferenceForMundaneWeapons()
        {
            var mundaneWeapon = CreateOneHandedMeleeWeapon("mundane weapon");
            var wrongMundaneWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Except(allAmmunitions).ToArray())))
                .Returns("my random weapon");
            mockMundaneWeaponGenerator.Setup(g => g.Generate("my random weapon", race.Size)).Returns(mundaneWeapon);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(mundaneWeapon));
        }

        [Test]
        public void GenerateFrom_GenerateMagicalWeapon()
        {
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Except(allAmmunitions).ToArray()))).Returns("my random weapon");

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon));
        }

        [Test]
        public void GenerateFrom_CanWieldSpecificMagicalWeaponProficiency()
        {
            var wrongMagicalWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            var specialties = new[] { magicalWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(specialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.SetupSequence(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon).Returns(wrongMagicalWeapon);

            feats.Add(new Feat { Name = "feat2", Foci = new[] { magicalWeapon.Name } });
            proficiencyFeats.Add(feats[1].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon));
        }

        [Test]
        public void GenerateFrom_PreferMagicalWeaponsPickedAsFocusForNonProficiencyFeats()
        {
            var wrongMagicalWeapon = CreateOneHandedMeleeWeapon("wrong weapon");

            var specialties = new[] { magicalWeapon.Name };
            var wrongSpecialties = new[] { wrongMagicalWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(specialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            feats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            feats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add(feats[1].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon));
        }

        [Test]
        public void GenerateFrom_DoNotPreferMagicalWeaponsPickedAsFocusForWeaponFamiliarity()
        {
            var wrongMagicalWeapon = CreateOneHandedMeleeWeapon("wrong weapon");

            var specialties = new[] { magicalWeapon.Name };
            var wrongSpecialties = new[] { wrongMagicalWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(specialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            feats.Add(new Feat { Name = FeatConstants.WeaponFamiliarity, Foci = wrongSpecialties });
            feats.Add(new Feat { Name = "feat3", Foci = specialties });

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateFrom_PreferAnyMagicalWeaponsPickedAsFocusForNonProficiencyFeats()
        {
            var otherMagicalWeapon = CreateOneHandedMeleeWeapon("other magical weapon");
            var wrongMagicalWeapon = CreateOneHandedMeleeWeapon("wrong weapon");

            var specialties = new[] { magicalWeapon.Name };
            var multipleSpecialties = new[] { magicalWeapon.Name, otherMagicalWeapon.Name };
            var wrongSpecialties = new[] { wrongMagicalWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(multipleSpecialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(otherMagicalWeapon);

            feats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            feats.Add(new Feat { Name = "feat3", Foci = multipleSpecialties });
            feats.Add(new Feat { Name = "feat4", Foci = specialties });
            proficiencyFeats.Add(feats[1].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(otherMagicalWeapon));
        }

        [Test]
        public void GenerateFrom_PreferMagicalWeaponsPickedAsFocusForProficiencyFeats()
        {
            var wrongMagicalWeapon = CreateOneHandedMeleeWeapon("wrong weapon");

            var specialties = new[] { magicalWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(specialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            feats.Add(new Feat { Name = "feat2", Foci = specialties });
            proficiencyFeats.Add(feats[1].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon));
        }

        [Test]
        public void GenerateFrom_PreferAnyMagicalWeaponsPickedAsFocusForProficiencyFeats()
        {
            var otherMagicalWeapon = CreateOneHandedMeleeWeapon("other magical weapon");
            var wrongMagicalWeapon = CreateOneHandedMeleeWeapon("wrong weapon");

            var specialties = new[] { magicalWeapon.Name };
            var multipleSpecialties = new[] { magicalWeapon.Name, otherMagicalWeapon.Name };
            var wrongSpecialties = new[] { wrongMagicalWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(multipleSpecialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(otherMagicalWeapon);

            feats.Add(new Feat { Name = "feat2", Foci = multipleSpecialties });
            feats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add(feats[1].Name);
            proficiencyFeats.Add(feats[2].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(otherMagicalWeapon));
        }

        [Test]
        public void GenerateFrom_NoPreferenceForMagicalWeapons()
        {
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Except(allAmmunitions).ToArray()))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateFrom_SaveBonusesOfAllDoNotCountAsProficiencyFeats()
        {
            feats.Add(new Feat { Name = FeatConstants.SaveBonus, Foci = new[] { FeatConstants.Foci.All } });
            feats[0].Foci = new[] { magicalWeapon.Name };

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(magicalWeapon.Name))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        //INFO: Example here is Shurikens
        [Test]
        public void GenerateFrom_ThrownAmmunitionIsAllowed()
        {
            var shuriken = CreateRangedWeapon("thrown ammo");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Except(allAmmunitions).ToArray()))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(shuriken);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(shuriken), weapon.Name);
        }

        [Test]
        public void GenerateFrom_AmmunitionIsNotAllowed()
        {
            var ammo = CreateAmmunition("my ammo");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Except(allAmmunitions).ToArray()))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateAmmunition_ReturnsAmmunition()
        {
            var ammunition = CreateAmmunition("my ammo");

            var ammunitions = new[] { "ammo" };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(ammunitions))).Returns("my random ammo");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random ammo", race.Size)).Returns(ammunition);

            var generatedAmmunition = weaponGenerator.GenerateAmmunition(characterClass, race, "ammo");
            Assert.That(generatedAmmunition, Is.EqualTo(ammunition), generatedAmmunition.Name);
        }

        [Test]
        public void GenerateMeleeFrom_MeleeWeaponMustBeMelee()
        {
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Intersect(allMeleeWeapons).ToArray()))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            var weapon = weaponGenerator.GenerateMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateMeleeFrom_IfGenerationOfMeleeWeaponFails_TryWithoutNonProficiencyWeaponFoci()
        {
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            var otherMagicalWeapon = CreateTwoHandedMeleeWeapon("two-handed melee");

            var wrongSpecialties = new[] { rangedWeapon.Name };
            var specialties = new[] { otherMagicalWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(specialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(otherMagicalWeapon);

            feats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            feats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add("feat3");

            var weapon = weaponGenerator.GenerateMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(otherMagicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateMeleeFrom_IfGenerationOfMeleeWeaponFailsAgain_TryWithoutSpecificProficiencyWeaponFoci()
        {
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            var otherRangedWeapon = CreateRangedWeapon("other ranged weapon");

            var wrongSpecialties = new[] { rangedWeapon.Name };
            var specialties = new[] { otherRangedWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Intersect(allMeleeWeapons).ToArray()))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            feats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            feats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add("feat3");

            var weapon = weaponGenerator.GenerateMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateMeleeFrom_IfNoMeleeWeaponsPossible_ReturnNothing()
        {
            allProficientWeapons.Clear();
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            var otherRangedWeapon = CreateRangedWeapon("other ranged weapon");

            var weapon = weaponGenerator.GenerateMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.Null);
            mockMagicalWeaponGenerator.Verify(g => g.Generate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Never);
        }

        [Test]
        public void GenerateOneHandedMeleeFrom_OneHandedMeleeWeaponMustBeMelee()
        {
            var rangedWeapon = CreateRangedWeapon("ranged weapon");

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Intersect(allMeleeWeapons).Except(allTwoHandedWeapons).ToArray())))
                .Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            var weapon = weaponGenerator.GenerateOneHandedMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateOneHandedMeleeFrom_OneHandedMeleeWeaponMustBeOneHanded()
        {
            var twoHandedWeapon = CreateTwoHandedMeleeWeapon("two-handed weapon");

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Intersect(allMeleeWeapons).Except(allTwoHandedWeapons).ToArray())))
                .Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            var weapon = weaponGenerator.GenerateOneHandedMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateOneHandedMeleeFrom_IfGenerationOfOneHandedMeleeWeaponFails_TryWithoutNonProficiencyWeaponFoci()
        {
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            var otherMagicalWeapon = CreateOneHandedMeleeWeapon("one-handed weapon");

            var wrongSpecialties = new[] { rangedWeapon.Name };
            var specialties = new[] { otherMagicalWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(specialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(otherMagicalWeapon);

            feats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            feats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add("feat3");

            var weapon = weaponGenerator.GenerateOneHandedMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(otherMagicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateOneHandedMeleeFrom_IfGenerationOfOneHandedMeleeWeaponFailsAgain_TryWithoutSpecificProficiencyWeaponFoci()
        {
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            var twoHandedWeapon = CreateTwoHandedMeleeWeapon("two-handed weapon");

            var wrongSpecialties = new[] { rangedWeapon.Name };
            var specialties = new[] { twoHandedWeapon.Name };
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(allProficientWeapons.Intersect(allMeleeWeapons).Except(allTwoHandedWeapons).ToArray()))
                .Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon")).Returns(magicalWeapon);

            feats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            feats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add("feat3");

            var weapon = weaponGenerator.GenerateOneHandedMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateOneHandedMeleeFrom_IfNoOneHandedMeleeWeaponsPossible_ReturnNothing()
        {
            allProficientWeapons.Clear();
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            var twoHandedWeapon = CreateTwoHandedMeleeWeapon("two-handed weapon");

            var weapon = weaponGenerator.GenerateOneHandedMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.Null);
            mockMagicalWeaponGenerator.Verify(g => g.Generate(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GenerateRangedFrom_RangedWeaponMustNotBeMelee()
        {
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet("ranged weapon", "other ranged"))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(rangedWeapon);

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(rangedWeapon), weapon.Name);
        }

        [Test]
        public void GenerateRangedFrom_IfGenerationOfRangedWeaponFails_TryWithoutNonProficiencyWeaponFoci()
        {
            var meleeWeapon = CreateOneHandedMeleeWeapon("melee weapon");
            var rangedWeapon = CreateRangedWeapon("ranged weapon");

            var wrongSpecialties = new[] { meleeWeapon.Name };
            var specialties = new[] { rangedWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(specialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(rangedWeapon);

            feats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            feats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add("feat3");

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(rangedWeapon), weapon.Name);
        }

        [Test]
        public void GenerateRangedFrom_IfGenerationOfRangedWeaponFailsAgain_TryWithoutSpecificProficiencyWeaponFoci()
        {
            var meleeWeapon = CreateOneHandedMeleeWeapon("melee weapon");
            var otherMeleeWeapon = CreateOneHandedMeleeWeapon("other melee weapon");
            var rangedWeapon = CreateRangedWeapon("ranged weapon");

            var wrongSpecialties = new[] { meleeWeapon.Name };
            var specialties = new[] { otherMeleeWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet("ranged weapon", "other ranged"))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(rangedWeapon);

            feats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            feats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add("feat3");

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.Not.Null);
            Assert.That(weapon, Is.EqualTo(rangedWeapon), weapon.Name);
        }

        [Test]
        public void GenerateRangedFrom_RangedWeaponCannotBeAmmunition()
        {
            var ammunition = CreateAmmunition("my ammo");
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet("ranged weapon", "other ranged"))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(rangedWeapon);

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(rangedWeapon), weapon.Name);
        }

        //INFO: Example here is Shurikens
        [Test]
        public void GenerateRangedFrom_RangedThrownAmmunitionIsAllowed()
        {
            var thrown = CreateRangedWeapon("thrown weapon");
            var rangedWeapon = CreateRangedWeapon("ranged weapon");

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet("thrown weapon", "ranged weapon", "other ranged"))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(thrown);

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(thrown), weapon.Name);
        }

        [Test]
        public void GenerateRangedFrom_GenerateNoRangedWeapon()
        {
            feats.Add(new Feat { Name = "feat2", Foci = new[] { magicalWeapon.Name } });
            proficiencyFeats.Add("feat2");

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.Null);
        }

        [Test]
        public void GenerateRangedFrom_IfNoRangedWeaponsPossible_ReturnNothing()
        {
            allProficientWeapons.Clear();
            var meleeWeapon = CreateOneHandedMeleeWeapon("melee weapon");
            var otherMeleeWeapon = CreateTwoHandedMeleeWeapon("two-handed weapon");
            var ammunition = CreateAmmunition("my ammo");

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.Null);
            mockMagicalWeaponGenerator.Verify(g => g.Generate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Never);
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
        public void GenerateFrom_NPCIsHalfLevel(int npcLevel, int effectiveLevel)
        {
            characterClass.Level = npcLevel;
            characterClass.Name = "class name";
            characterClass.IsNPC = true;

            var npcWeapon = CreateOneHandedMeleeWeapon("melee weapon");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, effectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("npc power");
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Except(allAmmunitions).ToArray())))
                .Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("npc power", "my random weapon", race.Size)).Returns(npcWeapon);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(npcWeapon));
        }

        [Test]
        public void GenerateFrom_LevelAdjustmentAffectsNPCLevelForWeapon()
        {
            characterClass.Level = 9266;
            characterClass.LevelAdjustment = 90210;
            characterClass.Name = "class name";
            characterClass.IsNPC = true;

            var playerWeapon = CreateOneHandedMeleeWeapon("melee weapon");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, characterClass.EffectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("npc power");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Except(allAmmunitions).ToArray()))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("npc power", "my random weapon", race.Size)).Returns(playerWeapon);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(playerWeapon));
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
        public void GenerateFrom_PlayerCharacterIsFullLevel(int level, int effectiveLevel)
        {
            characterClass.Level = level;
            characterClass.Name = "class name";
            characterClass.IsNPC = false;

            var playerWeapon = CreateOneHandedMeleeWeapon("melee weapon");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, effectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("player power");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Except(allAmmunitions).ToArray()))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("player power", "my random weapon", race.Size)).Returns(playerWeapon);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(playerWeapon));
        }

        [Test]
        public void GenerateFrom_LevelAdjustmentAffectsPlayerLevelForWeapon()
        {
            characterClass.Level = 9266;
            characterClass.LevelAdjustment = 90210;
            characterClass.Name = "class name";
            characterClass.IsNPC = false;

            var playerWeapon = CreateOneHandedMeleeWeapon("melee weapon");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, characterClass.EffectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("player power");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Except(allAmmunitions).ToArray()))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("player power", "my random weapon", race.Size)).Returns(playerWeapon);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(playerWeapon));
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
        public void GenerateAmmunition_NPCIsHalfLevel(int npcLevel, int effectiveLevel)
        {
            characterClass.Level = npcLevel;
            characterClass.Name = "class name";
            characterClass.IsNPC = true;

            var npcWeapon = CreateAmmunition("my ammo");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, effectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("npc power");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet("my ammo"))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("npc power", "my random weapon", race.Size)).Returns(npcWeapon);

            var weapon = weaponGenerator.GenerateAmmunition(characterClass, race, "my ammo");
            Assert.That(weapon, Is.EqualTo(npcWeapon));
        }

        [Test]
        public void GenerateAmmunition_LevelAdjustmentAffectsNPCLevelForWeapon()
        {
            characterClass.Level = 9266;
            characterClass.LevelAdjustment = 90210;
            characterClass.Name = "class name";
            characterClass.IsNPC = true;

            var npcWeapon = CreateAmmunition("my ammo");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, characterClass.EffectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("npc power");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet("my ammo"))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("npc power", "my random weapon", race.Size)).Returns(npcWeapon);

            var weapon = weaponGenerator.GenerateAmmunition(characterClass, race, "my ammo");
            Assert.That(weapon, Is.EqualTo(npcWeapon));
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
        public void GenerateAmmunition_PlayerCharacterIsFullLevel(int level, int effectiveLevel)
        {
            characterClass.Level = level;
            characterClass.Name = "class name";
            characterClass.IsNPC = false;

            var playerWeapon = CreateAmmunition("my ammo");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, effectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("player power");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet("my ammo"))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("player power", "my random weapon", race.Size)).Returns(playerWeapon);

            var weapon = weaponGenerator.GenerateAmmunition(characterClass, race, "my ammo");
            Assert.That(weapon, Is.EqualTo(playerWeapon));
        }

        [Test]
        public void GenerateAmmunition_LevelAdjustmentAffectsPlayerLevelForWeapon()
        {
            characterClass.Level = 9266;
            characterClass.LevelAdjustment = 90210;
            characterClass.Name = "class name";
            characterClass.IsNPC = false;

            var playerWeapon = CreateAmmunition("my ammo");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, characterClass.EffectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("player power");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet("my ammo"))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("player power", "my random weapon", race.Size)).Returns(playerWeapon);

            var weapon = weaponGenerator.GenerateAmmunition(characterClass, race, "my ammo");
            Assert.That(weapon, Is.EqualTo(playerWeapon));
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
        public void GenerateMeleeFrom_NPCIsHalfLevel(int npcLevel, int effectiveLevel)
        {
            characterClass.Level = npcLevel;
            characterClass.Name = "class name";
            characterClass.IsNPC = true;

            var npcWeapon = CreateOneHandedMeleeWeapon("melee weapon");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, effectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("npc power");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Intersect(allMeleeWeapons).ToArray()))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("npc power", "my random weapon", race.Size)).Returns(npcWeapon);

            var weapon = weaponGenerator.GenerateMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(npcWeapon));
        }

        [Test]
        public void GenerateMeleeFrom_LevelAdjustmentAffectsNPCLevelForWeapon()
        {
            characterClass.Level = 9266;
            characterClass.LevelAdjustment = 90210;
            characterClass.Name = "class name";
            characterClass.IsNPC = true;

            var playerWeapon = CreateOneHandedMeleeWeapon("melee weapon");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, characterClass.EffectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("npc power");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Intersect(allMeleeWeapons).ToArray()))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("npc power", "my random weapon", race.Size)).Returns(playerWeapon);

            var weapon = weaponGenerator.GenerateMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(playerWeapon));
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
        public void GenerateMeleeFrom_PlayerCharacterIsFullLevel(int level, int effectiveLevel)
        {
            characterClass.Level = level;
            characterClass.Name = "class name";
            characterClass.IsNPC = false;

            var playerWeapon = CreateOneHandedMeleeWeapon("melee weapon");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, effectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("player power");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Intersect(allMeleeWeapons).ToArray()))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("player power", "my random weapon", race.Size)).Returns(playerWeapon);

            var weapon = weaponGenerator.GenerateMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(playerWeapon));
        }

        [Test]
        public void GenerateMeleeFrom_LevelAdjustmentAffectsPlayerLevelForWeapon()
        {
            characterClass.Level = 9266;
            characterClass.LevelAdjustment = 90210;
            characterClass.Name = "class name";
            characterClass.IsNPC = false;

            var playerWeapon = CreateOneHandedMeleeWeapon("melee weapon");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, characterClass.EffectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("player power");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Intersect(allMeleeWeapons).ToArray()))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("player power", "my random weapon", race.Size)).Returns(playerWeapon);

            var weapon = weaponGenerator.GenerateMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(playerWeapon));
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
        public void GenerateRangedFrom_NPCIsHalfLevel(int npcLevel, int effectiveLevel)
        {
            characterClass.Level = npcLevel;
            characterClass.Name = "class name";
            characterClass.IsNPC = true;

            var npcWeapon = CreateRangedWeapon("ranged weapon");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, effectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("npc power");
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Intersect(allRangedWeapons).Except(allAmmunitions).ToArray())))
                .Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("npc power", "my random weapon", race.Size)).Returns(npcWeapon);

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(npcWeapon));
        }

        [Test]
        public void GenerateRangedFrom_LevelAdjustmentAffectsNPCLevelForWeapon()
        {
            characterClass.Level = 9266;
            characterClass.LevelAdjustment = 90210;
            characterClass.Name = "class name";
            characterClass.IsNPC = true;

            var playerWeapon = CreateRangedWeapon("ranged weapon");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, characterClass.EffectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("npc power");
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Intersect(allRangedWeapons).Except(allAmmunitions).ToArray())))
                .Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("npc power", "my random weapon", race.Size)).Returns(playerWeapon);

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(playerWeapon));
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
        public void GenerateRangedFrom_PlayerCharacterIsFullLevel(int level, int effectiveLevel)
        {
            characterClass.Level = level;
            characterClass.Name = "class name";
            characterClass.IsNPC = false;

            var playerWeapon = CreateRangedWeapon("ranged weapon");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, effectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("player power");
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Intersect(allRangedWeapons).Except(allAmmunitions).ToArray())))
                .Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("player power", "my random weapon", race.Size)).Returns(playerWeapon);

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(playerWeapon));
        }

        [Test]
        public void GenerateRangedFrom_LevelAdjustmentAffectsPlayerLevelForWeapon()
        {
            characterClass.Level = 9266;
            characterClass.LevelAdjustment = 90210;
            characterClass.Name = "class name";
            characterClass.IsNPC = false;

            var playerWeapon = CreateRangedWeapon("ranged weapon");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, characterClass.EffectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("player power");
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Intersect(allRangedWeapons).Except(allAmmunitions).ToArray())))
                .Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("player power", "my random weapon", race.Size)).Returns(playerWeapon);

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(playerWeapon));
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
        public void GenerateOneHandedMeleeFrom_NPCIsHalfLevel(int npcLevel, int effectiveLevel)
        {
            characterClass.Level = npcLevel;
            characterClass.Name = "class name";
            characterClass.IsNPC = true;

            var npcWeapon = CreateOneHandedMeleeWeapon("melee weapon");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, effectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("npc power");
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Intersect(allMeleeWeapons).Except(allTwoHandedWeapons).ToArray())))
                .Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("npc power", "my random weapon", race.Size)).Returns(npcWeapon);

            var weapon = weaponGenerator.GenerateOneHandedMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(npcWeapon));
        }

        [Test]
        public void GenerateOneHandedMeleeFrom_LevelAdjustmentAffectsNPCLevelForWeapon()
        {
            characterClass.Level = 9266;
            characterClass.LevelAdjustment = 90210;
            characterClass.Name = "class name";
            characterClass.IsNPC = true;

            var playerWeapon = CreateOneHandedMeleeWeapon("melee weapon");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, characterClass.EffectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("npc power");
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Intersect(allMeleeWeapons).Except(allTwoHandedWeapons).ToArray())))
                .Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("npc power", "my random weapon", race.Size)).Returns(playerWeapon);

            var weapon = weaponGenerator.GenerateOneHandedMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(playerWeapon));
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
        public void GenerateOneHandedMeleeFrom_PlayerCharacterIsFullLevel(int level, int effectiveLevel)
        {
            characterClass.Level = level;
            characterClass.Name = "class name";
            characterClass.IsNPC = false;

            var playerWeapon = CreateOneHandedMeleeWeapon("melee weapon");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, effectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("player power");
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Intersect(allMeleeWeapons).Except(allTwoHandedWeapons).ToArray())))
                .Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("player power", "my random weapon", race.Size)).Returns(playerWeapon);

            var weapon = weaponGenerator.GenerateOneHandedMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(playerWeapon));
        }

        [Test]
        public void GenerateOneHandedMeleeFrom_LevelAdjustmentAffectsPlayerLevelForWeapon()
        {
            characterClass.Level = 9266;
            characterClass.LevelAdjustment = 90210;
            characterClass.Name = "class name";
            characterClass.IsNPC = false;

            var playerWeapon = CreateOneHandedMeleeWeapon("melee weapon");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, characterClass.EffectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("player power");
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(ProficientSet(allProficientWeapons.Intersect(allMeleeWeapons).Except(allTwoHandedWeapons).ToArray())))
                .Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate("player power", "my random weapon", race.Size)).Returns(playerWeapon);

            var weapon = weaponGenerator.GenerateOneHandedMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(playerWeapon));
        }
    }
}
