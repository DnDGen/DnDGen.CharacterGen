using DnDGen.CharacterGen.Combats;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Selectors;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Combats
{
    [TestFixture]
    public class ArmorClassGeneratorTests
    {
        private IArmorClassGenerator armorClassGenerator;
        private Equipment equipment;
        private List<Feat> feats;
        private int adjustedDexterityBonus;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<IAdjustmentsSelector> mockAdjustmentsSelector;
        private Race race;
        private Dictionary<string, int> sizeModifiers;

        [SetUp]
        public void Setup()
        {
            mockAdjustmentsSelector = new Mock<IAdjustmentsSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            armorClassGenerator = new ArmorClassGenerator(mockCollectionsSelector.Object, mockAdjustmentsSelector.Object);
            equipment = new Equipment();
            feats = new List<Feat>();
            sizeModifiers = new Dictionary<string, int>();
            adjustedDexterityBonus = 0;
            race = new Race();

            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(TableNameConstants.Set.Adjustments.SizeModifiers)).Returns(sizeModifiers);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.Size))
                .Returns(Enumerable.Empty<string>());
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.NaturalArmor))
                .Returns(Enumerable.Empty<string>());
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.Deflection))
                .Returns(Enumerable.Empty<string>());

            race.Size = "size";
            sizeModifiers["size"] = 0;
            sizeModifiers["other size"] = 0;
        }

        [Test]
        public void ArmorClassesStartsAtBase()
        {
            GenerateAndAssertArmorClass();
        }

        private ArmorClass GenerateAndAssertArmorClass(int full = ArmorClass.BaseArmorClass, int flatFooted = ArmorClass.BaseArmorClass, int touch = ArmorClass.BaseArmorClass, bool circumstantial = false)
        {
            var armorClass = armorClassGenerator.GenerateWith(equipment, adjustedDexterityBonus, feats, race);
            Assert.That(armorClass.Full, Is.EqualTo(full), "full");
            Assert.That(armorClass.FlatFooted, Is.EqualTo(flatFooted), "flat-footed");
            Assert.That(armorClass.Touch, Is.EqualTo(touch), "touch");
            Assert.That(armorClass.CircumstantialBonus, Is.EqualTo(circumstantial));
            Assert.That(armorClass.AdjustedDexterityBonus, Is.EqualTo(adjustedDexterityBonus));

            return armorClass;
        }

        [Test]
        public void ArmorBonusesApplied()
        {
            equipment.Armor = new Armor { Name = "armor", ArmorBonus = 1 };

            var armorClass = GenerateAndAssertArmorClass(11, 11);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(1));
        }

        [Test]
        public void ShieldBonusesApplied()
        {
            equipment.OffHand = new Armor { Name = "shield", ArmorBonus = 1 };

            var armorClass = GenerateAndAssertArmorClass(11, 11);
            Assert.That(armorClass.ShieldBonus, Is.EqualTo(1));
        }

        [Test]
        public void ArmorFromItemApplied()
        {
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.ArmorBonus))
                .Returns(new[] { "bracers", "other item" });

            var bracers = new Item();
            bracers.Name = "bracers";
            bracers.Magic.Bonus = 1;

            var thing = new Item();
            thing.Name = "thing";
            thing.Magic.Bonus = -1;

            equipment.Treasure.Items = new[] { bracers, thing };

            var armorClass = GenerateAndAssertArmorClass(11, 11);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(1));
        }

        [Test]
        public void BestArmorBonusAppliedIfArmor()
        {
            equipment.Armor = new Armor { Name = "armor", ArmorBonus = 2 };

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.ArmorBonus))
                .Returns(new[] { "bracers", "other item" });

            var bracers = new Item();
            bracers.Name = "bracers";
            bracers.Magic.Bonus = 1;

            var thing = new Item();
            thing.Name = "thing";
            thing.Magic.Bonus = -1;

            equipment.Treasure.Items = new[] { bracers, thing };

            var armorClass = GenerateAndAssertArmorClass(12, 12);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(2));
        }

        [Test]
        public void BestArmorBonusAppliedIfArmorWithBonus()
        {
            equipment.Armor = new Armor { Name = "armor", ArmorBonus = 1 };
            equipment.Armor.Magic.Bonus = 1;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.ArmorBonus))
                .Returns(new[] { "bracers", "other item" });

            var bracers = new Item();
            bracers.Name = "bracers";
            bracers.Magic.Bonus = 1;

            var thing = new Item();
            thing.Name = "thing";
            thing.Magic.Bonus = -1;

            equipment.Treasure.Items = new[] { bracers, thing };

            var armorClass = GenerateAndAssertArmorClass(12, 12);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(2));
        }

        [Test]
        public void BestArmorBonusAppliedIfItem()
        {
            equipment.Armor = new Armor { Name = "armor", ArmorBonus = 1 };

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.ArmorBonus))
                .Returns(new[] { "bracers", "other item" });

            var bracers = new Item();
            bracers.Name = "bracers";
            bracers.Magic.Bonus = 2;

            var thing = new Item();
            thing.Name = "thing";
            thing.Magic.Bonus = -1;

            equipment.Treasure.Items = new[] { bracers, thing };

            var armorClass = GenerateAndAssertArmorClass(12, 12);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(2));
        }

        [Test]
        public void BestArmorBonusAppliedIfItemAndShield()
        {
            equipment.Armor = new Armor { Name = "armor", ArmorBonus = 1 };
            equipment.OffHand = new Armor { Name = "shield", ArmorBonus = 1 };

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.ArmorBonus))
                .Returns(new[] { "bracers", "other item" });

            var bracers = new Item();
            bracers.Name = "bracers";
            bracers.Magic.Bonus = 2;

            var thing = new Item();
            thing.Name = "thing";
            thing.Magic.Bonus = -1;

            equipment.Treasure.Items = new[] { bracers, thing };

            var armorClass = GenerateAndAssertArmorClass(13, 13);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(2));
            Assert.That(armorClass.ShieldBonus, Is.EqualTo(1));
        }

        //INFO: Example here is Githzerai's Inertial Armor
        [Test]
        public void AddArmorBonusFromFeats()
        {
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.ArmorBonus))
                .Returns(new[] { "bracers", "other item", "feat", "wrong feat" });

            feats.Add(new Feat());
            feats.Add(new Feat());
            feats[0].Name = "feat";
            feats[0].Power = 1;
            feats[1].Name = "other feat";
            feats[1].Power = -1;

            var armorClass = GenerateAndAssertArmorClass(11, 11);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(1));
        }

        //INFO: Example here is Githzerai's Inertial Armor
        [Test]
        public void AddArmorBonusFromFeatsInAdditionToArmor()
        {
            equipment.Armor = new Armor { Name = "armor", ArmorBonus = 1 };
            equipment.OffHand = new Armor { Name = "shield", ArmorBonus = 1 };

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.ArmorBonus))
                .Returns(new[] { "bracers", "other item", "feat", "wrong feat" });

            feats.Add(new Feat());
            feats.Add(new Feat());
            feats[0].Name = "feat";
            feats[0].Power = 1;
            feats[1].Name = "other feat";
            feats[1].Power = -1;

            var armorClass = GenerateAndAssertArmorClass(13, 13);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(2));
            Assert.That(armorClass.ShieldBonus, Is.EqualTo(1));
        }

        //INFO: Example here is Githzerai's Inertial Armor
        [Test]
        public void AddArmorBonusFromFeatsInAdditionToItem()
        {
            equipment.Armor = new Armor { Name = "armor", ArmorBonus = 1 };
            equipment.OffHand = new Armor { Name = "shield", ArmorBonus = 1 };

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.ArmorBonus))
                .Returns(new[] { "bracers", "other item", "feat", "wrong feat" });

            var bracers = new Item();
            bracers.Name = "bracers";
            bracers.Magic.Bonus = 2;

            var thing = new Item();
            thing.Name = "thing";
            thing.Magic.Bonus = -1;

            equipment.Treasure.Items = new[] { bracers, thing };

            feats.Add(new Feat());
            feats.Add(new Feat());
            feats[0].Name = "feat";
            feats[0].Power = 1;
            feats[1].Name = "other feat";
            feats[1].Power = -1;

            var armorClass = GenerateAndAssertArmorClass(14, 14);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(3));
            Assert.That(armorClass.ShieldBonus, Is.EqualTo(1));
        }

        [Test]
        public void DexterityBonusApplied()
        {
            adjustedDexterityBonus = 1;
            var armorClass = GenerateAndAssertArmorClass(11, touch: 11);
            Assert.That(armorClass.AdjustedDexterityBonus, Is.EqualTo(1));
        }

        [Test]
        public void NegativeDexterityBonusApplied()
        {
            adjustedDexterityBonus = -1;
            var armorClass = GenerateAndAssertArmorClass(9, touch: 9);
            Assert.That(armorClass.AdjustedDexterityBonus, Is.EqualTo(-1));
        }

        [Test]
        public void ArmorEnhancementBonusApplied()
        {
            equipment.Armor = new Armor { Name = "armor", ArmorBonus = 0 };
            equipment.Armor.Magic.Bonus = 1;

            var armorClass = GenerateAndAssertArmorClass(11, 11);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(1));
        }

        [Test]
        public void ShieldEnhancementBonusApplied()
        {
            equipment.OffHand = new Armor { Name = "shield", ArmorBonus = 0 };
            equipment.OffHand.Magic.Bonus = 1;

            var armorClass = GenerateAndAssertArmorClass(11, 11);
            Assert.That(armorClass.ShieldBonus, Is.EqualTo(1));
        }

        [Test]
        public void OffhandNonArmorEnhancementBonusNotApplied()
        {
            equipment.OffHand = new Item { Name = "item" };
            equipment.OffHand.ItemType = ItemTypeConstants.Armor;
            equipment.OffHand.Magic.Bonus = 1;

            GenerateAndAssertArmorClass();
        }

        [Test]
        public void DeflectionBonusApplied()
        {
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.Deflection))
                .Returns(new[] { "ring", "other ring" });

            var ring = new Item();
            ring.Name = "ring";
            ring.Magic.Bonus = 1;

            var thing = new Item();
            thing.Name = "thing";
            thing.Magic.Bonus = -1;

            equipment.Treasure.Items = new[] { ring, thing };

            var armorClass = GenerateAndAssertArmorClass(11, 11, 11);
            Assert.That(armorClass.DeflectionBonus, Is.EqualTo(1));
        }

        [Test]
        public void OnlyHighestDeflectionBonusApplies()
        {
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.Deflection))
                .Returns(new[] { "ring", "other ring" });

            var ring = new Item();
            ring.Name = "ring";
            ring.Magic.Bonus = 1;

            var otherRing = new Item();
            otherRing.Name = "other ring";
            otherRing.Magic.Bonus = 2;

            equipment.Treasure.Items = new[] { ring, otherRing };

            var armorClass = GenerateAndAssertArmorClass(12, 12, 12);
            Assert.That(armorClass.DeflectionBonus, Is.EqualTo(2));
        }

        [Test]
        public void DeflectionBonusesDoNotStack()
        {
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.Deflection))
                .Returns(new[] { "ring", "other ring" });

            var ring = new Item();
            ring.Name = "ring";
            ring.Magic.Bonus = 1;

            var otherRing = new Item();
            otherRing.Name = "other ring";
            otherRing.Magic.Bonus = 1;

            equipment.Treasure.Items = new[] { ring, otherRing };

            var armorClass = GenerateAndAssertArmorClass(11, 11, 11);
            Assert.That(armorClass.DeflectionBonus, Is.EqualTo(1));
        }

        [Test]
        public void TotalNaturalArmorBonusFromFeatApplied()
        {
            feats.Add(new Feat());
            feats.Add(new Feat());
            feats[0].Name = "feat 1";
            feats[0].Power = 1;
            feats[1].Name = "feat 2";
            feats[1].Power = 1;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.NaturalArmor))
                .Returns(new[] { "feat 1", "feat 2", "other feat" });

            var armorClass = GenerateAndAssertArmorClass(12, 12);
            Assert.That(armorClass.NaturalArmorBonus, Is.EqualTo(2));
        }

        [Test]
        public void IfNaturalArmorHasCircumstantialArmorBonus_MarkCircumstantialBonus()
        {
            feats.Add(new Feat());
            feats.Add(new Feat());
            feats[0].Name = "feat 1";
            feats[0].Power = 1;
            feats[1].Name = "feat 2";
            feats[1].Foci = new[] { "focus" };
            feats[1].Power = 1;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.NaturalArmor))
                .Returns(new[] { "feat 1", "feat 2", "other feat" });

            var armorClass = GenerateAndAssertArmorClass(11, 11, circumstantial: true);
            Assert.That(armorClass.NaturalArmorBonus, Is.EqualTo(1));
        }

        [Test]
        public void DoNotOverwriteCircumstantialBonus()
        {
            feats.Add(new Feat());
            feats.Add(new Feat());
            feats[0].Name = "feat 1";
            feats[0].Foci = new[] { "focus" };
            feats[0].Power = 1;
            feats[1].Name = "feat 2";
            feats[1].Power = 1;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.NaturalArmor))
                .Returns(new[] { "feat 1", "feat 2", "other feat" });

            var armorClass = GenerateAndAssertArmorClass(11, 11, circumstantial: true);
            Assert.That(armorClass.NaturalArmorBonus, Is.EqualTo(1));
        }

        [Test]
        public void TotalDodgeBonusFromFeatApplied()
        {
            feats.Add(new Feat());
            feats.Add(new Feat());
            feats[0].Name = "feat 1";
            feats[0].Power = 1;
            feats[1].Name = "feat 2";
            feats[1].Power = 1;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.DodgeBonus))
                .Returns(new[] { "feat 1", "feat 2", "other feat" });

            var armorClass = GenerateAndAssertArmorClass(12, touch: 12);
            Assert.That(armorClass.DodgeBonus, Is.EqualTo(2));
        }

        [Test]
        public void IfDodgeBonusHasCircumstantialArmorBonus_MarkCircumstantialBonus()
        {
            feats.Add(new Feat());
            feats.Add(new Feat());
            feats[0].Name = "feat 1";
            feats[0].Power = 1;
            feats[1].Name = "feat 2";
            feats[1].Foci = new[] { "focus" };
            feats[1].Power = 1;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.DodgeBonus))
                .Returns(new[] { "feat 1", "feat 2", "other feat" });

            var armorClass = GenerateAndAssertArmorClass(11, touch: 11, circumstantial: true);
            Assert.That(armorClass.DodgeBonus, Is.EqualTo(1));
        }

        [Test]
        public void DoNotOverwriteCircumstantialBonusWithDodgeBonus()
        {
            feats.Add(new Feat());
            feats.Add(new Feat());
            feats[0].Name = "feat 1";
            feats[0].Foci = new[] { "focus" };
            feats[0].Power = 1;
            feats[1].Name = "feat 2";
            feats[1].Power = 1;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.NaturalArmor))
                .Returns(new[] { "feat 1", "other feat" });

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.DodgeBonus))
                .Returns(new[] { "feat 2", "other feat" });

            var armorClass = GenerateAndAssertArmorClass(11, touch: 11, circumstantial: true);
            Assert.That(armorClass.DodgeBonus, Is.EqualTo(1));
        }

        [Test]
        public void NaturalArmorBonusFromItemsApplied()
        {
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.NaturalArmor))
                .Returns(new[] { "ring", "other ring" });

            var ring = new Item();
            ring.Name = "ring";
            ring.Magic.Bonus = 1;

            var thing = new Item();
            thing.Name = "thing";
            thing.Magic.Bonus = -1;

            equipment.Treasure.Items = new[] { ring, thing };

            var armorClass = GenerateAndAssertArmorClass(11, 11);
            Assert.That(armorClass.NaturalArmorBonus, Is.EqualTo(1));
        }

        [Test]
        public void OnlyHighestNaturalArmorBonusFromItemsApplies()
        {
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.NaturalArmor))
                .Returns(new[] { "ring", "other ring" });

            var ring = new Item();
            ring.Name = "ring";
            ring.Magic.Bonus = 1;

            var otherRing = new Item();
            otherRing.Name = "other ring";
            otherRing.Magic.Bonus = 2;

            equipment.Treasure.Items = new[] { ring, otherRing };

            var armorClass = GenerateAndAssertArmorClass(12, 12, 10);
            Assert.That(armorClass.NaturalArmorBonus, Is.EqualTo(2));
        }

        [Test]
        public void TotalNaturalArmorBonusFromFeatAndItemsApplied()
        {
            feats.Add(new Feat());
            feats.Add(new Feat());
            feats[0].Name = "feat 1";
            feats[0].Power = 1;
            feats[1].Name = "feat 2";
            feats[1].Power = 1;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.NaturalArmor))
                .Returns(new[] { "feat 1", "feat 2", "ring", "other ring" });

            var ring = new Item();
            ring.Name = "ring";
            ring.Magic.Bonus = 2;

            var otherRing = new Item();
            otherRing.Name = "other ring";
            otherRing.Magic.Bonus = 1;

            equipment.Treasure.Items = new[] { ring, otherRing };

            var armorClass = GenerateAndAssertArmorClass(14, 14, 10);
            Assert.That(armorClass.NaturalArmorBonus, Is.EqualTo(4));
        }

        [Test]
        public void SizeModifiesArmorClass()
        {
            sizeModifiers["size"] = 9266;
            sizeModifiers["other size"] = 90210;

            var armorClass = GenerateAndAssertArmorClass(9276, 9276, 9276);
            Assert.That(armorClass.SizeModifier, Is.EqualTo(9266));
        }

        [Test]
        public void SizeModifiesArmorClassNegatively()
        {
            sizeModifiers["size"] = -4;
            sizeModifiers["other size"] = -2;

            var armorClass = GenerateAndAssertArmorClass(6, 6, 6);
            Assert.That(armorClass.SizeModifier, Is.EqualTo(-4));
        }

        [Test]
        public void ArmorClassesAreSummed()
        {
            adjustedDexterityBonus = 1;

            var feat = new Feat();
            feat.Name = "feat 1";
            feat.Power = 1;
            feats.Add(feat);

            var otherFeat = new Feat();
            otherFeat.Name = "feat 2";
            otherFeat.Power = 1;
            feats.Add(otherFeat);

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.Deflection))
                .Returns(new[] { "ring" });
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.NaturalArmor))
                .Returns(new[] { "feat 1" });
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.DodgeBonus))
                .Returns(new[] { "feat 2" });

            equipment.Armor = new Armor { Name = "armor", ArmorBonus = 1 };
            equipment.Armor.Magic.Bonus = 1;
            equipment.OffHand = new Armor { Name = "shield", ArmorBonus = 1 };
            equipment.OffHand.Magic.Bonus = 1;

            var ring = new Item();
            ring.Name = "ring";
            ring.Magic.Bonus = 1;

            equipment.Treasure.Items = new[] { ring };

            sizeModifiers[race.Size] = 1;

            var armorClass = GenerateAndAssertArmorClass(19, 17, 14);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(2));
            Assert.That(armorClass.DeflectionBonus, Is.EqualTo(1));
            Assert.That(armorClass.DodgeBonus, Is.EqualTo(1));
            Assert.That(armorClass.NaturalArmorBonus, Is.EqualTo(1));
            Assert.That(armorClass.ShieldBonus, Is.EqualTo(2));
            Assert.That(armorClass.SizeModifier, Is.EqualTo(1));
        }

        [Test]
        public void CursedArmorWithOppositeEffectHasNegativeBonus()
        {
            equipment.Armor = new Armor { Name = "armor", ArmorBonus = 3 };
            equipment.Armor.Magic.Bonus = 1;
            equipment.Armor.Magic.Curse = CurseConstants.OppositeEffect;

            var armorClass = GenerateAndAssertArmorClass(12, 12);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(2));
        }

        [Test]
        public void CursedArmorWithOppositeEffectHasNegativeEffect()
        {
            equipment.Armor = new Armor { Name = "armor", ArmorBonus = 1 };
            equipment.Armor.Magic.Bonus = 2;
            equipment.Armor.Magic.Curse = CurseConstants.OppositeEffect;

            var armorClass = GenerateAndAssertArmorClass(9, 9);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(-1));
        }

        [Test]
        public void CursedArmorWithOppositeEffectLosesToItem()
        {
            equipment.Armor = new Armor { Name = "armor", ArmorBonus = 1 };
            equipment.Armor.Magic.Bonus = 2;
            equipment.Armor.Magic.Curse = CurseConstants.OppositeEffect;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.ArmorBonus))
                .Returns(new[] { "bracers", "other item" });

            var bracers = new Item();
            bracers.Name = "bracers";
            bracers.Magic.Bonus = 1;

            equipment.Treasure.Items = new[] { bracers };

            var armorClass = GenerateAndAssertArmorClass(11, 11);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(1));
        }

        [Test]
        public void CursedArmorWithOppositeEffectWinsAgainstItem()
        {
            equipment.Armor = new Armor { Name = "armor", ArmorBonus = 3 };
            equipment.Armor.Magic.Bonus = 1;
            equipment.Armor.Magic.Curse = CurseConstants.OppositeEffect;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.ArmorBonus))
                .Returns(new[] { "bracers", "other item" });

            var bracers = new Item();
            bracers.Name = "bracers";
            bracers.Magic.Bonus = 1;

            equipment.Treasure.Items = new[] { bracers };

            var armorClass = GenerateAndAssertArmorClass(12, 12);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(2));
        }

        [Test]
        public void CursedShieldWithOppositeEffectHasNegativeBonus()
        {
            equipment.OffHand = new Armor { Name = "shield", ArmorBonus = 3 };
            equipment.OffHand.Magic.Bonus = 1;
            equipment.OffHand.Magic.Curse = CurseConstants.OppositeEffect;

            var armorClass = GenerateAndAssertArmorClass(12, 12);
            Assert.That(armorClass.ShieldBonus, Is.EqualTo(2));
        }

        [Test]
        public void CursedShieldWithOppositeEffectHasNegativeEffect()
        {
            equipment.OffHand = new Armor { Name = "shield", ArmorBonus = 1 };
            equipment.OffHand.Magic.Bonus = 2;
            equipment.OffHand.Magic.Curse = CurseConstants.OppositeEffect;

            var armorClass = GenerateAndAssertArmorClass(9, 9);
            Assert.That(armorClass.ShieldBonus, Is.EqualTo(-1));
        }

        [Test]
        public void CursedArmorWithDelusionHasNoBonus()
        {
            equipment.Armor = new Armor { Name = "armor", ArmorBonus = 3 };
            equipment.Armor.Magic.Bonus = 1;
            equipment.Armor.Magic.Curse = CurseConstants.Delusion;

            var armorClass = GenerateAndAssertArmorClass(13, 13);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(3));
        }

        [Test]
        public void CursedArmorWithDelusionLosesToItem()
        {
            equipment.Armor = new Armor { Name = "armor", ArmorBonus = 1 };
            equipment.Armor.Magic.Bonus = 2;
            equipment.Armor.Magic.Curse = CurseConstants.Delusion;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.ArmorBonus))
                .Returns(new[] { "bracers", "other item" });

            var bracers = new Item();
            bracers.Name = "bracers";
            bracers.Magic.Bonus = 2;

            equipment.Treasure.Items = new[] { bracers };

            var armorClass = GenerateAndAssertArmorClass(12, 12);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(2));
        }

        [Test]
        public void CursedArmorWithDelusionWinsAgainstItem()
        {
            equipment.Armor = new Armor { Name = "armor", ArmorBonus = 3 };
            equipment.Armor.Magic.Bonus = 1;
            equipment.Armor.Magic.Curse = CurseConstants.Delusion;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ArmorClassModifiers, GroupConstants.ArmorBonus))
                .Returns(new[] { "bracers", "other item" });

            var bracers = new Item();
            bracers.Name = "bracers";
            bracers.Magic.Bonus = 2;

            equipment.Treasure.Items = new[] { bracers };

            var armorClass = GenerateAndAssertArmorClass(13, 13);
            Assert.That(armorClass.ArmorBonus, Is.EqualTo(3));
        }

        [Test]
        public void CursedShieldWithDelusionHasNoBonus()
        {
            equipment.OffHand = new Armor { Name = "shield", ArmorBonus = 3 };
            equipment.OffHand.Magic.Bonus = 1;
            equipment.OffHand.Magic.Curse = CurseConstants.Delusion;

            var armorClass = GenerateAndAssertArmorClass(13, 13);
            Assert.That(armorClass.ShieldBonus, Is.EqualTo(3));
        }
    }
}