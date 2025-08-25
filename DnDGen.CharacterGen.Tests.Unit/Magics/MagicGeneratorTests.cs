using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Selectors;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Items;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Magics
{
    [TestFixture]
    public class MagicGeneratorTests
    {
        private Mock<ISpellsGenerator> mockSpellsGenerator;
        private Mock<IAnimalGenerator> mockAnimalGenerator;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<IAdjustmentsSelector> mockAdjustmentsSelector;
        private IMagicGenerator magicGenerator;
        private CharacterClass characterClass;
        private List<Feat> feats;
        private Alignment alignment;
        private Race race;
        private Dictionary<string, Ability> stats;
        private Equipment equipment;
        private Dictionary<string, int> arcaneSpellFailures;
        private List<string> classesThatPrepareSpells;

        [SetUp]
        public void Setup()
        {
            mockSpellsGenerator = new Mock<ISpellsGenerator>();
            mockAnimalGenerator = new Mock<IAnimalGenerator>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockAdjustmentsSelector = new Mock<IAdjustmentsSelector>();
            magicGenerator = new MagicGenerator(mockSpellsGenerator.Object, mockAnimalGenerator.Object, mockCollectionsSelector.Object, mockAdjustmentsSelector.Object);
            characterClass = new CharacterClass();
            feats = new List<Feat>();
            alignment = new Alignment();
            race = new Race();
            stats = new Dictionary<string, Ability>();
            equipment = new Equipment();
            arcaneSpellFailures = new Dictionary<string, int>();
            classesThatPrepareSpells = new List<string>();

            characterClass.Name = "class name";
            classesThatPrepareSpells.Add(characterClass.Name);
            classesThatPrepareSpells.Add("other class");

            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.ArcaneSpellFailures, It.IsAny<string>()))
                .Returns((string table, string name) => arcaneSpellFailures[name]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.PreparesSpells))
                .Returns(classesThatPrepareSpells);
        }

        [Test]
        public void GenerateMagic()
        {
            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic, Is.Not.Null);
        }

        [Test]
        public void GenerateSpells()
        {
            var spellsPerDay = new List<SpellQuantity>();
            var knownSpells = new List<Spell>();
            var preparedSpells = new List<Spell>();

            mockSpellsGenerator.Setup(g => g.GeneratePerDay(characterClass, stats)).Returns(spellsPerDay);
            mockSpellsGenerator.Setup(g => g.GenerateKnown(characterClass, stats)).Returns(knownSpells);
            mockSpellsGenerator.Setup(g => g.GeneratePrepared(characterClass, knownSpells, spellsPerDay)).Returns(preparedSpells);

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.SpellsPerDay, Is.EqualTo(spellsPerDay));
            Assert.That(magic.KnownSpells, Is.EqualTo(knownSpells));
            Assert.That(magic.PreparedSpells, Is.EqualTo(preparedSpells));
        }

        [Test]
        public void DoNotGeneratePreparedSpellsIfClassDoesNotPrepareSpells()
        {
            classesThatPrepareSpells.Remove(characterClass.Name);

            var spellsPerDay = new List<SpellQuantity> { new SpellQuantity() };
            var knownSpells = new List<Spell> { new Spell() };
            var preparedSpells = new List<Spell> { new Spell() };

            mockSpellsGenerator.Setup(g => g.GeneratePerDay(characterClass, stats)).Returns(spellsPerDay);
            mockSpellsGenerator.Setup(g => g.GenerateKnown(characterClass, stats)).Returns(knownSpells);
            mockSpellsGenerator.Setup(g => g.GeneratePrepared(characterClass, knownSpells, spellsPerDay)).Returns(preparedSpells);

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.SpellsPerDay, Is.EqualTo(spellsPerDay));
            Assert.That(magic.KnownSpells, Is.EqualTo(knownSpells));
            Assert.That(magic.PreparedSpells, Is.Not.EqualTo(preparedSpells));
            Assert.That(magic.PreparedSpells, Is.Empty);
        }

        [Test]
        public void NonSpellcasterRakshasaStillGetsSorcererSpells()
        {
            race.BaseRace = RaceConstants.BaseRaces.Rakshasa;

            var spellsPerDay = new List<SpellQuantity>();
            var knownSpells = new List<Spell>();
            var preparedSpells = new List<Spell>();

            mockSpellsGenerator.Setup(g => g.GeneratePerDay(characterClass, stats)).Returns(spellsPerDay);
            mockSpellsGenerator.Setup(g => g.GenerateKnown(characterClass, stats)).Returns(knownSpells);
            mockSpellsGenerator.Setup(g => g.GeneratePrepared(characterClass, knownSpells, spellsPerDay)).Returns(preparedSpells);

            var rakshasaSpellsPerDay = new List<SpellQuantity> { new() { Level = 1, Quantity = 1, Source = CharacterClassConstants.Sorcerer } };
            var rakshasaKnownSpells = new List<Spell> { new() { Name = "rakshasa spell", Sources = { [CharacterClassConstants.Sorcerer] = 1 } } };
            var rakshasaPreparedSpells = new List<Spell> { new() { Name = "rakshasa prepared spell", Sources = { [CharacterClassConstants.Sorcerer] = 1 } } };

            mockSpellsGenerator
                .Setup(g => g.GeneratePerDay(It.Is<CharacterClass>(c => c.Name == CharacterClassConstants.Sorcerer && c.Level == 7), stats))
                .Returns(rakshasaSpellsPerDay);
            mockSpellsGenerator
                .Setup(g => g.GenerateKnown(It.Is<CharacterClass>(c => c.Name == CharacterClassConstants.Sorcerer && c.Level == 7), stats))
                .Returns(rakshasaKnownSpells);
            mockSpellsGenerator
                .Setup(g => g.GeneratePrepared(
                    It.Is<CharacterClass>(c => c.Name == CharacterClassConstants.Sorcerer && c.Level == 7),
                    rakshasaKnownSpells,
                    rakshasaSpellsPerDay))
                .Returns(rakshasaPreparedSpells);

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.SpellsPerDay, Is.EquivalentTo(rakshasaSpellsPerDay));
            Assert.That(magic.KnownSpells, Is.EquivalentTo(rakshasaKnownSpells));
            Assert.That(magic.PreparedSpells, Is.Empty);
        }

        [Test]
        public void SorcererRakshasaStacksRakshasaLevelWithSorcererLevel()
        {
            race.BaseRace = RaceConstants.BaseRaces.Rakshasa;
            characterClass.Name = CharacterClassConstants.Sorcerer;
            characterClass.Level = 1;

            var spellsPerDay = new List<SpellQuantity> { new() { Level = 1, Quantity = 1, Source = "wrong" } };
            var knownSpells = new List<Spell> { new() { Name = "wrong spell", Sources = { [CharacterClassConstants.Sorcerer] = 1 } } };
            var preparedSpells = new List<Spell> { new() { Name = "wrong prepared spell", Sources = { [CharacterClassConstants.Sorcerer] = 1 } } };

            mockSpellsGenerator.Setup(g => g.GeneratePerDay(characterClass, stats)).Returns(spellsPerDay);
            mockSpellsGenerator.Setup(g => g.GenerateKnown(characterClass, stats)).Returns(knownSpells);
            mockSpellsGenerator.Setup(g => g.GeneratePrepared(characterClass, knownSpells, spellsPerDay)).Returns(preparedSpells);

            var rakshasaSpellsPerDay = new List<SpellQuantity> { new() { Level = 1, Quantity = 1, Source = CharacterClassConstants.Sorcerer } };
            var rakshasaKnownSpells = new List<Spell> { new() { Name = "rakshasa spell", Sources = { [CharacterClassConstants.Sorcerer] = 1 } } };
            var rakshasaPreparedSpells = new List<Spell> { new() { Name = "rakshasa prepared spell", Sources = { [CharacterClassConstants.Sorcerer] = 1 } } };

            mockSpellsGenerator
                .Setup(g => g.GeneratePerDay(It.Is<CharacterClass>(c => c.Name == CharacterClassConstants.Sorcerer && c.Level == 8), stats))
                .Returns(rakshasaSpellsPerDay);
            mockSpellsGenerator
                .Setup(g => g.GenerateKnown(It.Is<CharacterClass>(c => c.Name == CharacterClassConstants.Sorcerer && c.Level == 8), stats))
                .Returns(rakshasaKnownSpells);
            mockSpellsGenerator
                .Setup(g => g.GeneratePrepared(
                    It.Is<CharacterClass>(c => c.Name == CharacterClassConstants.Sorcerer && c.Level == 8),
                    rakshasaKnownSpells,
                    rakshasaSpellsPerDay))
                .Returns(rakshasaPreparedSpells);

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.SpellsPerDay, Is.EquivalentTo(rakshasaSpellsPerDay));
            Assert.That(magic.KnownSpells, Is.EquivalentTo(rakshasaKnownSpells));
            Assert.That(magic.PreparedSpells, Is.Empty);
        }

        [Test]
        public void NonSorcererSpellcasterRakshasaStillGetsSorcererSpells()
        {
            race.BaseRace = RaceConstants.BaseRaces.Rakshasa;
            characterClass.Level = 2;

            var spellsPerDay = new List<SpellQuantity> { new() { Level = 1, Quantity = 1, Source = characterClass.Name } };
            var knownSpells = new List<Spell> { new() { Name = "spell", Sources = { [characterClass.Name] = 1 } } };
            var preparedSpells = new List<Spell> { new() { Name = "prepared spell", Sources = { [characterClass.Name] = 1 } } };

            mockSpellsGenerator.Setup(g => g.GeneratePerDay(characterClass, stats)).Returns(spellsPerDay);
            mockSpellsGenerator.Setup(g => g.GenerateKnown(characterClass, stats)).Returns(knownSpells);
            mockSpellsGenerator.Setup(g => g.GeneratePrepared(characterClass, knownSpells, spellsPerDay)).Returns(preparedSpells);

            var rakshasaSpellsPerDay = new List<SpellQuantity> { new() { Level = 1, Quantity = 1, Source = CharacterClassConstants.Sorcerer } };
            var rakshasaKnownSpells = new List<Spell> { new() { Name = "rakshasa spell", Sources = { [CharacterClassConstants.Sorcerer] = 1 } } };
            var rakshasaPreparedSpells = new List<Spell> { new() { Name = "rakshasa prepared spell", Sources = { [CharacterClassConstants.Sorcerer] = 1 } } };

            mockSpellsGenerator
                .Setup(g => g.GeneratePerDay(It.Is<CharacterClass>(c => c.Name == CharacterClassConstants.Sorcerer && c.Level == 7), stats))
                .Returns(rakshasaSpellsPerDay);
            mockSpellsGenerator
                .Setup(g => g.GenerateKnown(It.Is<CharacterClass>(c => c.Name == CharacterClassConstants.Sorcerer && c.Level == 7), stats))
                .Returns(rakshasaKnownSpells);
            mockSpellsGenerator
                .Setup(g => g.GeneratePrepared(
                    It.Is<CharacterClass>(c => c.Name == CharacterClassConstants.Sorcerer && c.Level == 7),
                    rakshasaKnownSpells,
                    rakshasaSpellsPerDay))
                .Returns(rakshasaPreparedSpells);

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.SpellsPerDay, Is.EquivalentTo(rakshasaSpellsPerDay.Union(spellsPerDay)));
            Assert.That(magic.KnownSpells, Is.EquivalentTo(rakshasaKnownSpells.Union(knownSpells)));
            Assert.That(magic.PreparedSpells, Is.EquivalentTo(preparedSpells));
        }

        [Test]
        public void DoNotGenerateAnimal()
        {
            mockAnimalGenerator.Setup(g => g.GenerateFrom(alignment, characterClass, race, feats)).Returns(string.Empty);

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.Animal, Is.Empty);
        }

        [Test]
        public void GenerateAnimal()
        {
            mockAnimalGenerator.Setup(g => g.GenerateFrom(alignment, characterClass, race, feats)).Returns("animal");

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.Animal, Is.EqualTo("animal"));
        }

        [Test]
        public void GenerateNoArcaneSpellFailureIfNotArcaneSpellcaster()
        {
            equipment.Armor = new Armor();
            equipment.Armor.Name = "armor";
            arcaneSpellFailures[equipment.Armor.Name] = 9266;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane))
                .Returns(new[] { "other class", CharacterClassConstants.Bard });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(0));
        }

        [Test]
        public void GenerateNoArcaneSpellFailureIfBardWithLightArmor()
        {
            characterClass.Name = CharacterClassConstants.Bard;
            equipment.Armor = new Armor();
            equipment.Armor.Name = "armor";
            arcaneSpellFailures[equipment.Armor.Name] = 9266;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.LightArmorProficiency))
                .Returns(new[] { equipment.Armor.Name });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(0));
        }

        [Test]
        public void GenerateArcaneSpellFailureIfArcaneSpellcasterWithLightArmor()
        {
            equipment.Armor = new Armor();
            equipment.Armor.Name = "armor";
            arcaneSpellFailures[equipment.Armor.Name] = 9266;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane))
                .Returns(new[] { characterClass.Name, CharacterClassConstants.Bard });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.LightArmorProficiency))
                .Returns(new[] { equipment.Armor.Name });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(9266));
        }

        [Test]
        public void GenerateNoArcaneSpellFailureIfNotBardOrArcaneSpellcasterWithLightArmor()
        {
            equipment.Armor = new Armor();
            equipment.Armor.Name = "armor";
            arcaneSpellFailures[equipment.Armor.Name] = 9266;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.LightArmorProficiency))
                .Returns(new[] { equipment.Armor.Name });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(0));
        }

        [Test]
        public void GenerateNoArcaneSpellFailureIfNoArmorOrOffHand()
        {
            arcaneSpellFailures[string.Empty] = 9266;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane))
                .Returns(new[] { characterClass.Name });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(0));
        }

        [Test]
        public void GenerateArcaneSpellFailureIfArcaneSpellcasterAndHasArmor()
        {
            equipment.Armor = new Armor();
            equipment.Armor.Name = "armor";
            arcaneSpellFailures[equipment.Armor.Name] = 9266;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane))
                .Returns(new[] { characterClass.Name });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(9266));
        }

        [Test]
        public void GenerateArcaneSpellFailureIfArcaneSpellcasterAndHasShield()
        {
            equipment.OffHand = new Armor();
            equipment.OffHand.Name = "shield";
            equipment.OffHand.ItemType = ItemTypeConstants.Armor;
            equipment.OffHand.Attributes = new[] { AttributeConstants.Shield };
            arcaneSpellFailures[equipment.OffHand.Name] = 9266;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane))
                .Returns(new[] { characterClass.Name });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(9266));
        }

        [Test]
        public void GenerateNoArcaneSpellFailureIfArcaneSpellcasterAndNotShieldInOffHand()
        {
            equipment.OffHand = new Weapon();
            equipment.OffHand.Name = "weapon";
            arcaneSpellFailures[equipment.OffHand.Name] = 9266;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane))
                .Returns(new[] { characterClass.Name });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(0));
        }

        [Test]
        public void GenerateArcaneSpellFailureIfArcaneSpellcasterAndHasArmorAndShield()
        {
            equipment.Armor = new Armor();
            equipment.Armor.Name = "armor";

            equipment.OffHand = new Armor();
            equipment.OffHand.Name = "shield";
            equipment.OffHand.ItemType = ItemTypeConstants.Armor;
            equipment.OffHand.Attributes = new[] { AttributeConstants.Shield };

            arcaneSpellFailures[equipment.Armor.Name] = 9266;
            arcaneSpellFailures[equipment.OffHand.Name] = 90210;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane))
                .Returns(new[] { characterClass.Name });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(9266 + 90210));
        }

        [Test]
        public void GenerateArcaneSpellFailureIfBardWithNonLightArmor()
        {
            characterClass.Name = CharacterClassConstants.Bard;
            equipment.Armor = new Armor();
            equipment.Armor.Name = "armor";
            arcaneSpellFailures[equipment.Armor.Name] = 9266;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane))
                .Returns(new[] { characterClass.Name });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.LightArmorProficiency))
                .Returns(new[] { "other armor" });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(9266));
        }

        [Test]
        public void GenerateArcaneSpellFailureIfBardWithShield()
        {
            characterClass.Name = CharacterClassConstants.Bard;
            equipment.OffHand = new Item();
            equipment.OffHand.Name = "shield";
            equipment.OffHand.ItemType = ItemTypeConstants.Armor;
            equipment.OffHand.Attributes = new[] { AttributeConstants.Shield };

            arcaneSpellFailures[equipment.OffHand.Name] = 9266;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane))
                .Returns(new[] { characterClass.Name });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.LightArmorProficiency))
                .Returns(new[] { "other armor" });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(9266));
        }

        [Test]
        public void GenerateArcaneSpellFailureIfBardWithShieldAndLightArmor()
        {
            characterClass.Name = CharacterClassConstants.Bard;
            equipment.OffHand = new Armor();
            equipment.OffHand.Name = "shield";
            equipment.OffHand.ItemType = ItemTypeConstants.Armor;
            equipment.OffHand.Attributes = new[] { AttributeConstants.Shield };
            equipment.Armor = new Armor();
            equipment.Armor.Name = "armor";

            arcaneSpellFailures[equipment.Armor.Name] = 9266;
            arcaneSpellFailures[equipment.OffHand.Name] = 90210;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane))
                .Returns(new[] { characterClass.Name });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.LightArmorProficiency))
                .Returns(new[] { equipment.Armor.Name });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(90210));
        }

        [Test]
        public void GenerateArcaneSpellFailureIfBardWithNonLightArmorAndShield()
        {
            characterClass.Name = CharacterClassConstants.Bard;
            equipment.Armor = new Armor();
            equipment.Armor.Name = "armor";

            equipment.OffHand = new Armor();
            equipment.OffHand.Name = "shield";
            equipment.OffHand.ItemType = ItemTypeConstants.Armor;
            equipment.OffHand.Attributes = new[] { AttributeConstants.Shield };

            arcaneSpellFailures[equipment.Armor.Name] = 9266;
            arcaneSpellFailures[equipment.OffHand.Name] = 90210;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane))
                .Returns(new[] { characterClass.Name });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.LightArmorProficiency))
                .Returns(new[] { "other armor" });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(9266 + 90210));
        }

        [Test]
        public void MithralArmorDecreasesArcaneSpellFailureBy10()
        {
            equipment.Armor = new Armor();
            equipment.Armor.Name = "armor";
            equipment.Armor.Traits.Add(TraitConstants.SpecialMaterials.Mithral);
            arcaneSpellFailures[equipment.Armor.Name] = 9266;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane))
                .Returns(new[] { characterClass.Name });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(9256));
        }

        [Test]
        public void MithralArmorCannotDecreasesArcaneSpellFailureBelow0()
        {
            equipment.Armor = new Armor();
            equipment.Armor.Name = "armor";
            equipment.Armor.Traits.Add(TraitConstants.SpecialMaterials.Mithral);
            arcaneSpellFailures[equipment.Armor.Name] = 5;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane))
                .Returns(new[] { characterClass.Name });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(0));
        }

        [Test]
        public void MithralArmorCannotDecreasesShieldArcaneSpellFailure()
        {
            equipment.Armor = new Armor();
            equipment.Armor.Name = "armor";
            equipment.Armor.Traits.Add(TraitConstants.SpecialMaterials.Mithral);
            arcaneSpellFailures[equipment.Armor.Name] = 5;

            equipment.OffHand = new Item();
            equipment.OffHand.Name = "shield";
            equipment.OffHand.ItemType = ItemTypeConstants.Armor;
            equipment.OffHand.Attributes = new[] { AttributeConstants.Shield };
            arcaneSpellFailures[equipment.OffHand.Name] = 5;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane))
                .Returns(new[] { characterClass.Name });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(5));
        }

        [Test]
        public void MithralShieldDecreasesArcaneSpellFailureBy10()
        {
            equipment.OffHand = new Item();
            equipment.OffHand.Name = "shield";
            equipment.OffHand.ItemType = ItemTypeConstants.Armor;
            equipment.OffHand.Attributes = new[] { AttributeConstants.Shield };
            equipment.OffHand.Traits.Add(TraitConstants.SpecialMaterials.Mithral);
            arcaneSpellFailures[equipment.OffHand.Name] = 9266;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane))
                .Returns(new[] { characterClass.Name });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(9256));
        }

        [Test]
        public void MithralShieldCannotDecreasesArcaneSpellFailureBelow0()
        {
            equipment.OffHand = new Item();
            equipment.OffHand.Name = "shield";
            equipment.OffHand.ItemType = ItemTypeConstants.Armor;
            equipment.OffHand.Attributes = new[] { AttributeConstants.Shield };
            equipment.OffHand.Traits.Add(TraitConstants.SpecialMaterials.Mithral);
            arcaneSpellFailures[equipment.OffHand.Name] = 5;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane))
                .Returns(new[] { characterClass.Name });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(0));
        }

        [Test]
        public void MithralShieldCannotDecreasesArmorArcaneSpellFailure()
        {
            equipment.Armor = new Armor();
            equipment.Armor.Name = "armor";
            arcaneSpellFailures[equipment.Armor.Name] = 5;

            equipment.OffHand = new Item();
            equipment.OffHand.Name = "shield";
            equipment.OffHand.ItemType = ItemTypeConstants.Armor;
            equipment.OffHand.Attributes = new[] { AttributeConstants.Shield };
            equipment.OffHand.Traits.Add(TraitConstants.SpecialMaterials.Mithral);
            arcaneSpellFailures[equipment.OffHand.Name] = 5;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Arcane))
                .Returns(new[] { characterClass.Name });

            var magic = magicGenerator.GenerateWith(alignment, characterClass, race, stats, feats, equipment);
            Assert.That(magic.ArcaneSpellFailure, Is.EqualTo(5));
        }
    }
}
