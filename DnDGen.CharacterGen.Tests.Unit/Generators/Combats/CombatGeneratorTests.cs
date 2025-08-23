using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Combats;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Selectors.Collections;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Items;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Unit.Generators.Combats
{
    [TestFixture]
    public class CombatGeneratorTests
    {
        private Mock<ISavingThrowsGenerator> mockSavingThrowsGenerator;
        private Mock<IAdjustmentsSelector> mockAdjustmentsSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<IArmorClassGenerator> mockArmorClassGenerator;
        private Mock<IHitPointsGenerator> mockHitPointsGenerator;
        private ICombatGenerator combatGenerator;
        private CharacterClass characterClass;
        private List<Feat> feats;
        private Dictionary<string, Ability> stats;
        private Equipment equipment;
        private Race race;
        private Dictionary<string, int> racialBaseAttackAdjustments;
        private List<string> initiativeFeats;
        private List<string> attackBonusFeats;
        private Dictionary<string, int> sizeModifiers;
        private string baseAttackType;

        [SetUp]
        public void Setup()
        {
            mockArmorClassGenerator = new Mock<IArmorClassGenerator>();
            mockHitPointsGenerator = new Mock<IHitPointsGenerator>();
            mockSavingThrowsGenerator = new Mock<ISavingThrowsGenerator>();
            mockAdjustmentsSelector = new Mock<IAdjustmentsSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            combatGenerator = new CombatGenerator(
                mockArmorClassGenerator.Object,
                mockHitPointsGenerator.Object,
                mockSavingThrowsGenerator.Object,
                mockAdjustmentsSelector.Object,
                mockCollectionsSelector.Object);

            characterClass = new CharacterClass();
            feats = new List<Feat>();
            stats = new Dictionary<string, Ability>();
            equipment = new Equipment();
            race = new Race();
            sizeModifiers = new Dictionary<string, int>();
            racialBaseAttackAdjustments = new Dictionary<string, int>();
            initiativeFeats = new List<string>();
            attackBonusFeats = new List<string>();
            baseAttackType = GroupConstants.PoorBaseAttack;

            characterClass.Name = "class name";
            characterClass.Level = 20;
            race.Size = "size";
            stats[AbilityConstants.Constitution] = new Ability(AbilityConstants.Constitution);
            stats[AbilityConstants.Constitution].Value = 9266;
            stats[AbilityConstants.Dexterity] = new Ability(AbilityConstants.Dexterity);
            stats[AbilityConstants.Dexterity].Value = 42;
            stats[AbilityConstants.Strength] = new Ability(AbilityConstants.Strength);
            stats[AbilityConstants.Strength].Value = 600;
            racialBaseAttackAdjustments[string.Empty] = 0;
            sizeModifiers[race.Size] = 0;

            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.SizeModifiers, It.IsAny<string>()))
                .Returns((string table, string name) => sizeModifiers[name]);
            mockCollectionsSelector
                .Setup(s => s.FindCollectionOf(
                    Config.Name,
                    TableNameConstants.Set.Collection.ClassNameGroups,
                    characterClass.Name,
                    GroupConstants.GoodBaseAttack,
                    GroupConstants.AverageBaseAttack,
                    GroupConstants.PoorBaseAttack))
                .Returns(() => baseAttackType);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.Initiative)).Returns(initiativeFeats);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, FeatConstants.AttackBonus)).Returns(attackBonusFeats);
            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.RacialBaseAttackAdjustments, It.IsAny<string>()))
                .Returns((string table, string name) => racialBaseAttackAdjustments[name]);
        }

        [Test]
        public void GetGoodBaseAttack()
        {
            baseAttackType = GroupConstants.GoodBaseAttack;
            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            Assert.That(baseAttack.BaseBonus, Is.EqualTo(20));
        }

        [Test]
        public void GetAverageBaseAttack()
        {
            baseAttackType = GroupConstants.AverageBaseAttack;
            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            Assert.That(baseAttack.BaseBonus, Is.EqualTo(15));
        }

        [Test]
        public void GetPoorBaseAttack()
        {
            baseAttackType = GroupConstants.PoorBaseAttack;
            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            Assert.That(baseAttack.BaseBonus, Is.EqualTo(10));
        }

        [Test]
        public void ThrowExceptionIfNoBaseAttack()
        {
            baseAttackType = "not a base attack group";
            Assert.That(() => combatGenerator.GenerateBaseAttackWith(characterClass, race, stats), Throws.ArgumentException.With.Message.EqualTo("class name has no base attack"));
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
        public void GoodBaseAttackBonus(int level, int bonus)
        {
            characterClass.Level = level;
            baseAttackType = GroupConstants.GoodBaseAttack;

            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            Assert.That(baseAttack.BaseBonus, Is.EqualTo(bonus));
        }

        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(4, 3)]
        [TestCase(5, 3)]
        [TestCase(6, 4)]
        [TestCase(7, 5)]
        [TestCase(8, 6)]
        [TestCase(9, 6)]
        [TestCase(10, 7)]
        [TestCase(11, 8)]
        [TestCase(12, 9)]
        [TestCase(13, 9)]
        [TestCase(14, 10)]
        [TestCase(15, 11)]
        [TestCase(16, 12)]
        [TestCase(17, 12)]
        [TestCase(18, 13)]
        [TestCase(19, 14)]
        [TestCase(20, 15)]
        public void AverageBaseAttackBonus(int level, int bonus)
        {
            characterClass.Level = level;
            baseAttackType = GroupConstants.AverageBaseAttack;

            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            Assert.That(baseAttack.BaseBonus, Is.EqualTo(bonus));
        }

        [TestCase(1, 0)]
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
        public void PoorBaseAttackBonus(int level, int bonus)
        {
            characterClass.Level = level;
            baseAttackType = GroupConstants.PoorBaseAttack;

            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            Assert.That(baseAttack.BaseBonus, Is.EqualTo(bonus));
        }

        [Test]
        public void GetAllRacialBaseAttackAdjustments()
        {
            race.BaseRace = "base race";
            race.Metarace = "metarace";

            racialBaseAttackAdjustments["base race"] = 1;
            racialBaseAttackAdjustments["other base race"] = 7;
            racialBaseAttackAdjustments["metarace"] = 3;
            racialBaseAttackAdjustments["other metarace"] = 5;

            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            Assert.That(baseAttack.RacialModifier, Is.EqualTo(4));
        }

        [Test]
        public void GetSizeModifierForBaseAttack()
        {
            sizeModifiers[race.Size] = 1;
            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            Assert.That(baseAttack.SizeModifier, Is.EqualTo(1));
        }

        [Test]
        public void GetNegativeSizeModifierForBaseAttack()
        {
            sizeModifiers[race.Size] = -1;
            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            Assert.That(baseAttack.SizeModifier, Is.EqualTo(-1));
        }

        [Test]
        public void SetStatBonusesOnBaseAttack()
        {
            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            Assert.That(baseAttack.StrengthBonus, Is.EqualTo(stats[AbilityConstants.Strength].Bonus));
            Assert.That(baseAttack.DexterityBonus, Is.EqualTo(stats[AbilityConstants.Dexterity].Bonus));
        }

        [Test]
        public void ReturnCombatWithBaseAttack()
        {
            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            var combat = combatGenerator.GenerateWith(baseAttack, characterClass, race, feats, stats, equipment);
            Assert.That(combat.BaseAttack, Is.EqualTo(baseAttack));
        }

        [Test]
        public void CombatWithoutCircumstantialBonusToBaseAttack()
        {
            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);

            feats.Add(new Feat());
            feats[0].Name = "feat 1";
            feats[0].Power = 90210;

            feats.Add(new Feat());
            feats[1].Name = "feat 2";
            feats[1].Power = 1337;

            attackBonusFeats.Add(feats[1].Name);

            var combat = combatGenerator.GenerateWith(baseAttack, characterClass, race, feats, stats, equipment);
            Assert.That(combat.BaseAttack, Is.EqualTo(baseAttack));
            Assert.That(combat.BaseAttack.BaseBonus, Is.EqualTo(1347));
            Assert.That(combat.BaseAttack.CircumstantialBonus, Is.False);
        }

        [Test]
        public void CombatWithCircumstantialBonusToBaseAttack()
        {
            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);

            feats.Add(new Feat());
            feats[0].Name = "feat 1";
            feats[0].Power = 90210;

            feats.Add(new Feat());
            feats[1].Name = "feat 2";
            feats[1].Power = 1337;

            feats.Add(new Feat());
            feats[2].Name = "feat 3";
            feats[2].Foci = new[] { "focus" };
            feats[2].Power = 42;

            attackBonusFeats.Add(feats[1].Name);
            attackBonusFeats.Add(feats[2].Name);

            var combat = combatGenerator.GenerateWith(baseAttack, characterClass, race, feats, stats, equipment);
            Assert.That(combat.BaseAttack, Is.EqualTo(baseAttack));
            Assert.That(combat.BaseAttack.BaseBonus, Is.EqualTo(1347));
            Assert.That(combat.BaseAttack.CircumstantialBonus, Is.True);
        }

        [Test]
        public void DoNotOverwriteCircumstantialAttackBonus()
        {
            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);

            feats.Add(new Feat());
            feats[0].Name = "feat 1";
            feats[0].Power = 90210;

            feats.Add(new Feat());
            feats[1].Name = "feat 2";
            feats[1].Foci = new[] { "focus" };
            feats[1].Power = 1337;

            feats.Add(new Feat());
            feats[2].Name = "feat 3";
            feats[2].Power = 42;

            attackBonusFeats.Add(feats[1].Name);
            attackBonusFeats.Add(feats[2].Name);

            var combat = combatGenerator.GenerateWith(baseAttack, characterClass, race, feats, stats, equipment);
            Assert.That(combat.BaseAttack, Is.EqualTo(baseAttack));
            Assert.That(combat.BaseAttack.BaseBonus, Is.EqualTo(52));
            Assert.That(combat.BaseAttack.CircumstantialBonus, Is.True);
        }

        [Test]
        public void AdjustedDexterityBonusIsBonus()
        {
            equipment.Armor = new Armor { Name = "armor", MaxDexterityBonus = 17 };

            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            var combat = combatGenerator.GenerateWith(baseAttack, characterClass, race, feats, stats, equipment);

            Assert.That(combat.AdjustedDexterityBonus, Is.EqualTo(16));
        }

        [Test]
        public void AdjustedDexterityBonusIsMaxBonusOfArmor()
        {
            equipment.Armor = new Armor { Name = "armor", MaxDexterityBonus = 5 };

            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            var combat = combatGenerator.GenerateWith(baseAttack, characterClass, race, feats, stats, equipment);

            Assert.That(combat.AdjustedDexterityBonus, Is.EqualTo(5));
        }

        [Test]
        public void AdustedDexterityBonusIsMaxBonusIfShield()
        {
            equipment.OffHand = new Armor { Name = "shield", MaxDexterityBonus = 5 };

            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            var combat = combatGenerator.GenerateWith(baseAttack, characterClass, race, feats, stats, equipment);

            Assert.That(combat.AdjustedDexterityBonus, Is.EqualTo(5));
        }

        [Test]
        public void MithralIncreasesArmorMaxDexterityBonusBy2()
        {
            equipment.Armor = new Armor { Name = "armor", MaxDexterityBonus = 5 };
            equipment.Armor.Traits.Add(TraitConstants.SpecialMaterials.Mithral);

            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            var combat = combatGenerator.GenerateWith(baseAttack, characterClass, race, feats, stats, equipment);

            Assert.That(combat.AdjustedDexterityBonus, Is.EqualTo(7));
        }

        [Test]
        public void GetArmorClassFromGeneratorWithMaxDexterityBonus()
        {
            equipment.Armor = new Armor { Name = "armor", MaxDexterityBonus = 5 };
            var armorClass = new ArmorClass();
            mockArmorClassGenerator.Setup(g => g.GenerateWith(equipment, 5, feats, race)).Returns(armorClass);

            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            var combat = combatGenerator.GenerateWith(baseAttack, characterClass, race, feats, stats, equipment);

            Assert.That(combat.ArmorClass, Is.EqualTo(armorClass));
        }

        [Test]
        public void GetArmorClassFromGeneratorWithDexterityBonus()
        {
            var armorClass = new ArmorClass();
            mockArmorClassGenerator.Setup(g => g.GenerateWith(equipment, 16, feats, race)).Returns(armorClass);

            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            var combat = combatGenerator.GenerateWith(baseAttack, characterClass, race, feats, stats, equipment);

            Assert.That(combat.ArmorClass, Is.EqualTo(armorClass));
        }

        [Test]
        public void GetHitPointsFromGenerator()
        {
            mockHitPointsGenerator.Setup(g => g.GenerateWith(characterClass, stats[AbilityConstants.Constitution].Bonus, race, feats)).Returns(90210);

            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            var combat = combatGenerator.GenerateWith(baseAttack, characterClass, race, feats, stats, equipment);

            Assert.That(combat.HitPoints, Is.EqualTo(90210));
        }

        [Test]
        public void GetHitPointsFromGeneratorWhenCharacterDoesNotHaveConstitution()
        {
            stats.Remove(AbilityConstants.Constitution);

            mockHitPointsGenerator.Setup(g => g.GenerateWith(characterClass, 0, race, feats)).Returns(90210);

            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            var combat = combatGenerator.GenerateWith(baseAttack, characterClass, race, feats, stats, equipment);

            Assert.That(combat.HitPoints, Is.EqualTo(90210));
        }

        [Test]
        public void GetSavingThrowsFromGenerator()
        {
            var savingThrows = new SavingThrows();
            mockSavingThrowsGenerator.Setup(g => g.GenerateWith(characterClass, feats, stats)).Returns(savingThrows);

            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            var combat = combatGenerator.GenerateWith(baseAttack, characterClass, race, feats, stats, equipment);

            Assert.That(combat.SavingThrows, Is.EqualTo(savingThrows));
        }

        [Test]
        public void InitiativeBonusIsSumOfBonuses()
        {
            feats.Add(new Feat());
            feats[0].Name = "feat 1";
            feats[0].Power = 90210;

            feats.Add(new Feat());
            feats[1].Name = "feat 2";
            feats[1].Power = 600;

            initiativeFeats.Add(feats[1].Name);

            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            var combat = combatGenerator.GenerateWith(baseAttack, characterClass, race, feats, stats, equipment);

            Assert.That(combat.InitiativeBonus, Is.EqualTo(616));
        }

        [Test]
        public void InitiativeBonusIncludesNegativeDexterityBonus()
        {
            stats[AbilityConstants.Dexterity].Value = 1;

            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            var combat = combatGenerator.GenerateWith(baseAttack, characterClass, race, feats, stats, equipment);

            Assert.That(combat.InitiativeBonus, Is.EqualTo(-5));
        }

        [Test]
        public void InitiativeBonusUsesAdjustedDexterityBonus()
        {
            equipment.Armor = new Armor { Name = "armor", MaxDexterityBonus = 5 };

            var baseAttack = combatGenerator.GenerateBaseAttackWith(characterClass, race, stats);
            var combat = combatGenerator.GenerateWith(baseAttack, characterClass, race, feats, stats, equipment);

            Assert.That(combat.InitiativeBonus, Is.EqualTo(5));
        }
    }
}