using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Abilities.Randomizers;
using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Alignments.Randomizers;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using DnDGen.CharacterGen.Characters;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Languages;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Races.Randomizers;
using DnDGen.CharacterGen.Races.Randomizers.BaseRaces;
using DnDGen.CharacterGen.Races.Randomizers.Metaraces;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Items;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Generators.Characters
{
    internal class CharacterGeneratorTests : IntegrationTests
    {
        private ICharacterGenerator characterGenerator;
        private ICollectionSelector collectionSelector;
        private CharacterAsserter characterAsserter;
        private IAbilitiesRandomizer abilitiesRandomizer;
        private RaceRandomizer metaraceRandomizer;
        private RaceRandomizer baseRaceRandomizer;
        private ILevelRandomizer levelRandomizer;
        private IClassNameRandomizer classNameRandomizer;
        private IAlignmentRandomizer alignmentRandomizer;

        [SetUp]
        public void Setup()
        {
            characterGenerator = GetNewInstanceOf<ICharacterGenerator>();
            collectionSelector = GetNewInstanceOf<ICollectionSelector>();
            characterAsserter = new CharacterAsserter();

            abilitiesRandomizer = GetNewInstanceOf<IAbilitiesRandomizer>(AbilitiesRandomizerTypeConstants.Default);
            metaraceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.Metarace.Default);
            baseRaceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.BaseRace.Default);
            levelRandomizer = GetNewInstanceOf<ILevelRandomizer>(LevelRandomizerTypeConstants.Default);
            classNameRandomizer = GetNewInstanceOf<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.Default);
            alignmentRandomizer = GetNewInstanceOf<IAlignmentRandomizer>(AlignmentRandomizerTypeConstants.Default);
        }

        [Test]
        public void BUG_GenerateWith_ReturnsCharacter_WithoutMetarace()
        {
            //INFO: will try 2 times to see if we get a character without metarace. Should happen at least once, if not more than once
            var hasMeta = true;
            var attempts = 2;

            while (attempts-- > 0 && hasMeta)
            {
                var character = characterGenerator.GenerateWith(
                    alignmentRandomizer,
                    classNameRandomizer,
                    levelRandomizer,
                    baseRaceRandomizer,
                    metaraceRandomizer,
                    abilitiesRandomizer);

                characterAsserter.AssertCharacter(character);
                Assert.That(character, Is.Not.Null);
                Assert.That(character.Summary, Is.Not.Empty);
                Assert.That(character.Alignment.Full, Is.Not.Empty);
                Assert.That(character.Class.Level, Is.AtLeast(1));
                Assert.That(character.Class.Summary, Is.Not.Empty);
                Assert.That(character.Race.Summary, Is.Not.Empty);

                hasMeta = character.Race.Metarace != RaceConstants.Metaraces.None;
            }

            Assert.That(hasMeta, Is.False);
        }

        [Repeat(1000)]
        [Test]
        public void BUG_GenerateWith_ReturnsCharacter_SpecialistWizard()
        {
            metaraceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.Metarace.NoMeta);

            var baseRaceRandomizer = GetNewInstanceOf<ISetBaseRaceRandomizer>();
            baseRaceRandomizer.SetBaseRace = RaceConstants.BaseRaces.Human;

            var levelRandomizer = GetNewInstanceOf<ISetLevelRandomizer>();
            levelRandomizer.SetLevel = 1;

            var classNameRandomizer = GetNewInstanceOf<ISetClassNameRandomizer>();
            classNameRandomizer.SetClassName = CharacterClassConstants.Wizard;

            var character = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(character);
            Assert.That(character, Is.Not.Null);
            Assert.That(character.Summary, Is.Not.Empty);

            if (character.Class.SpecialistFields.Any())
            {
                var message = $"{character.Summary}; S: {string.Join(", ", character.Class.SpecialistFields)}; P: {string.Join(", ", character.Class.ProhibitedFields)}";
                Assert.That(character.Class.SpecialistFields.Count(), Is.EqualTo(1), message);

                var intersect = character.Class.SpecialistFields.Intersect(character.Class.ProhibitedFields);
                Assert.That(intersect, Is.Empty, message);

                if (character.Class.SpecialistFields.First() == CharacterClassConstants.Schools.Divination)
                {
                    Assert.That(character.Class.ProhibitedFields.Count(), Is.EqualTo(1), message);
                }
                else
                {
                    Assert.That(character.Class.ProhibitedFields.Count(), Is.EqualTo(2), message);
                }
            }
        }

        //INFO: Sometimes a first-level commoner only knows unarmed strike. Other times, she only knows one weapon (melee or ranged), and is unable to generate the other
        [Repeat(100)]
        [Test]
        public void BUG_GenerateFirstLevelCommoner()
        {
            var metaraceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.Metarace.NoMeta);

            var levelRandomizer = GetNewInstanceOf<ISetLevelRandomizer>();
            levelRandomizer.SetLevel = 1;

            var classNameRandomizer = GetNewInstanceOf<ISetClassNameRandomizer>();
            classNameRandomizer.SetClassName = CharacterClassConstants.Commoner;

            var commoner = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(commoner);
            Assert.That(commoner.Class.IsNPC, Is.True, commoner.Summary);
            Assert.That(commoner.Class.Level, Is.EqualTo(1), commoner.Summary);
            Assert.That(commoner.Class.Name, Is.EqualTo(CharacterClassConstants.Commoner), commoner.Summary);
            Assert.That(commoner.Class.ProhibitedFields, Is.Empty, commoner.Summary);
            Assert.That(commoner.Class.SpecialistFields, Is.Empty, commoner.Summary);
            Assert.That(commoner.Race.Metarace, Is.EqualTo(RaceConstants.Metaraces.None), commoner.Summary);
            Assert.That(commoner.Race.MetaraceSpecies, Is.Empty, commoner.Summary);
            Assert.That(commoner.Magic.Animal, Is.Empty, commoner.Summary);
            Assert.That(commoner.Magic.ArcaneSpellFailure, Is.Zero, commoner.Summary);
            Assert.That(commoner.Magic.KnownSpells, Is.Empty, commoner.Summary);
            Assert.That(commoner.Magic.PreparedSpells, Is.Empty, commoner.Summary);
            Assert.That(commoner.Magic.SpellsPerDay, Is.Empty, commoner.Summary);
        }

        [Repeat(100)]
        [Test]
        public void BUG_GenerateHighLevelFighter()
        {
            var metaraceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.Metarace.NoMeta);

            var levelRandomizer = GetNewInstanceOf<ISetLevelRandomizer>();
            levelRandomizer.SetLevel = 20;

            var classNameRandomizer = GetNewInstanceOf<ISetClassNameRandomizer>();
            classNameRandomizer.SetClassName = CharacterClassConstants.Fighter;

            var fighter = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(fighter);
            Assert.That(fighter.Class.IsNPC, Is.False, fighter.Summary);
            Assert.That(fighter.Class.Level, Is.EqualTo(20), fighter.Summary);
            Assert.That(fighter.Class.Name, Is.EqualTo(CharacterClassConstants.Fighter), fighter.Summary);
            Assert.That(fighter.Class.ProhibitedFields, Is.Empty, fighter.Summary);
            Assert.That(fighter.Class.SpecialistFields, Is.Empty, fighter.Summary);
            Assert.That(fighter.Magic.Animal, Is.Empty, fighter.Summary);
            Assert.That(fighter.Magic.ArcaneSpellFailure, Is.EqualTo(0), fighter.Summary);
            Assert.That(fighter.Magic.KnownSpells, Is.Empty, fighter.Summary);
            Assert.That(fighter.Magic.PreparedSpells, Is.Empty, fighter.Summary);
            Assert.That(fighter.Magic.SpellsPerDay, Is.Empty, fighter.Summary);
            Assert.That(fighter.Equipment.Armor, Is.Not.Null, fighter.Summary + " armor");
            Assert.That(fighter.Equipment.PrimaryHand, Is.Not.Null, fighter.Summary + " primary hand");
        }

        //INFO: The bug here is that the rare size (Huge) makes the equipment take much longer to generate
        [Repeat(100)]
        [Test]
        public void BUG_GenerateStormGiant()
        {
            var metaraceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.Metarace.NoMeta);

            var baseRaceRandomizer = GetNewInstanceOf<ISetBaseRaceRandomizer>();
            baseRaceRandomizer.SetBaseRace = RaceConstants.BaseRaces.StormGiant;

            var stormGiant = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(stormGiant);
            Assert.That(stormGiant.Class.IsNPC, Is.False, stormGiant.Summary);
            Assert.That(stormGiant.Race.BaseRace, Is.EqualTo(RaceConstants.BaseRaces.StormGiant), stormGiant.Summary);
            Assert.That(stormGiant.Race.Size, Is.EqualTo(RaceConstants.Sizes.Huge), stormGiant.Summary);
            Assert.That(stormGiant.Equipment.Armor, Is.Not.Null, stormGiant.Summary + " armor");
            Assert.That(stormGiant.Equipment.Armor.Size, Is.EqualTo(stormGiant.Race.Size), stormGiant.Summary);
            Assert.That(stormGiant.Equipment.PrimaryHand, Is.Not.Null, stormGiant.Summary + " primary hand");
            Assert.That(stormGiant.Equipment.PrimaryHand.Size, Is.EqualTo(stormGiant.Race.Size), stormGiant.Summary);

            if (stormGiant.Equipment.OffHand != null && stormGiant.Equipment.OffHand is Weapon)
            {
                var weapon = stormGiant.Equipment.OffHand as Weapon;
                Assert.That(weapon.Size, Is.EqualTo(stormGiant.Race.Size), stormGiant.Summary);
            }
            else if (stormGiant.Equipment.OffHand != null && stormGiant.Equipment.OffHand is Armor)
            {
                var armor = stormGiant.Equipment.OffHand as Armor;
                Assert.That(armor.Attributes, Contains.Item(AttributeConstants.Shield), stormGiant.Summary);
                Assert.That(armor.Size, Is.EqualTo(stormGiant.Race.Size), stormGiant.Summary);
            }
        }

        //INFO: Want to verify rakshasas have native sorcerer spells and additional spellcaster class spells
        [Repeat(100)]
        [Test]
        public void BUG_GenerateRakshasaSpellcaster()
        {
            var abilitiesRandomizer = GetNewInstanceOf<IAbilitiesRandomizer>(AbilitiesRandomizerTypeConstants.Heroic);
            var metaraceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.Metarace.NoMeta);
            var classNameRandomizer = GetNewInstanceOf<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.Spellcaster);

            var baseRaceRandomizer = GetNewInstanceOf<ISetBaseRaceRandomizer>();
            baseRaceRandomizer.SetBaseRace = RaceConstants.BaseRaces.Rakshasa;

            var spellcaster = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(spellcaster);
            characterAsserter.AssertSpellcaster(spellcaster);

            Assert.That(spellcaster.Class.IsNPC, Is.False, spellcaster.Summary);
            Assert.That(spellcaster.Race.BaseRace, Is.EqualTo(RaceConstants.BaseRaces.Rakshasa));
            //INFO: Rakshasa are 7th-level Sorcerers and can cast up to 4th-level spells
            Assert.That(spellcaster.Magic.SpellsPerDay.Any(q => q.Source == CharacterClassConstants.Sorcerer && q.Level == 0), Is.True, spellcaster.Summary);
            Assert.That(spellcaster.Magic.SpellsPerDay.Any(q => q.Source == CharacterClassConstants.Sorcerer && q.Level == 1), Is.True, spellcaster.Summary);
            Assert.That(spellcaster.Magic.SpellsPerDay.Any(q => q.Source == CharacterClassConstants.Sorcerer && q.Level == 2), Is.True, spellcaster.Summary);
            Assert.That(spellcaster.Magic.SpellsPerDay.Any(q => q.Source == CharacterClassConstants.Sorcerer && q.Level == 3), Is.True, spellcaster.Summary);
        }

        //INFO: Want to verify rakshasas have native sorcerer spells and additional sorcerer spells at 7 levels higher, even when high level
        [Repeat(100)]
        [Test]
        public void BUG_GenerateHighLevelRakshasaSorcerer()
        {
            var abilitiesRandomizer = GetNewInstanceOf<IAbilitiesRandomizer>(AbilitiesRandomizerTypeConstants.Heroic);
            var metaraceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.Metarace.NoMeta);
            var levelRandomizer = GetNewInstanceOf<ILevelRandomizer>(LevelRandomizerTypeConstants.VeryHigh);

            var classNameRandomizer = GetNewInstanceOf<ISetClassNameRandomizer>();
            classNameRandomizer.SetClassName = CharacterClassConstants.Sorcerer;

            var baseRaceRandomizer = GetNewInstanceOf<ISetBaseRaceRandomizer>();
            baseRaceRandomizer.SetBaseRace = RaceConstants.BaseRaces.Rakshasa;

            var sorcerer = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(sorcerer);
            characterAsserter.AssertSpellcaster(sorcerer);

            Assert.That(sorcerer.Class.IsNPC, Is.False, sorcerer.Summary);
            Assert.That(sorcerer.Race.BaseRace, Is.EqualTo(RaceConstants.BaseRaces.Rakshasa), sorcerer.Summary);
            Assert.That(sorcerer.Class.Name, Is.EqualTo(CharacterClassConstants.Sorcerer), sorcerer.Summary);
            Assert.That(sorcerer.Class.Level, Is.AtLeast(16), sorcerer.Summary);
            Assert.That(sorcerer.Class.EffectiveLevel, Is.AtLeast(18), sorcerer.Summary);
            Assert.That(sorcerer.Magic.SpellsPerDay.Any(q => q.Source == CharacterClassConstants.Sorcerer && q.Level == 0), Is.True, sorcerer.Summary);
            Assert.That(sorcerer.Magic.SpellsPerDay.Any(q => q.Source == CharacterClassConstants.Sorcerer && q.Level == 1), Is.True, sorcerer.Summary);
            Assert.That(sorcerer.Magic.SpellsPerDay.Any(q => q.Source == CharacterClassConstants.Sorcerer && q.Level == 2), Is.True, sorcerer.Summary);
            Assert.That(sorcerer.Magic.SpellsPerDay.Any(q => q.Source == CharacterClassConstants.Sorcerer && q.Level == 3), Is.True, sorcerer.Summary);
            Assert.That(sorcerer.Magic.SpellsPerDay.Any(q => q.Source == CharacterClassConstants.Sorcerer && q.Level == 4), Is.True, sorcerer.Summary);
            Assert.That(sorcerer.Magic.SpellsPerDay.Any(q => q.Source == CharacterClassConstants.Sorcerer && q.Level == 5), Is.True, sorcerer.Summary);
            Assert.That(sorcerer.Magic.SpellsPerDay.Any(q => q.Source == CharacterClassConstants.Sorcerer && q.Level == 6), Is.True, sorcerer.Summary);
            Assert.That(sorcerer.Magic.SpellsPerDay.Any(q => q.Source == CharacterClassConstants.Sorcerer && q.Level == 7), Is.True, sorcerer.Summary);
            Assert.That(sorcerer.Magic.SpellsPerDay.Any(q => q.Source == CharacterClassConstants.Sorcerer && q.Level == 8), Is.True, sorcerer.Summary);
            Assert.That(sorcerer.Magic.SpellsPerDay.Any(q => q.Source == CharacterClassConstants.Sorcerer && q.Level == 9), Is.True, sorcerer.Summary);
        }

        [Repeat(100)]
        [Test]
        public void BUG_GenerateUndeadCharacter()
        {
            var metaraceRandomizer = GetNewInstanceOf<IForcableMetaraceRandomizer>(RaceRandomizerTypeConstants.Metarace.UndeadMeta);
            metaraceRandomizer.ForceMetarace = true;

            var character = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(character);
            Assert.That(character.Class.IsNPC, Is.False, character.Summary);
            characterAsserter.AssertUndead(character);
        }

        [Repeat(100)]
        [Test]
        public void BUG_GeneratePlanetouchedCharacter()
        {
            var metaraceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.Metarace.NoMeta);
            var classNameRandomizer = GetNewInstanceOf<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.Spellcaster);

            var baseRaceRandomizer = GetNewInstanceOf<ISetBaseRaceRandomizer>();
            var planetouched = new[] { RaceConstants.BaseRaces.Aasimar, RaceConstants.BaseRaces.Tiefling };
            baseRaceRandomizer.SetBaseRace = collectionSelector.SelectRandomFrom(planetouched);

            var character = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(character);
            Assert.That(character.Class.IsNPC, Is.False, character.Summary);
            Assert.That(character.Race.BaseRace, Is.EqualTo(RaceConstants.BaseRaces.Aasimar).Or.EqualTo(RaceConstants.BaseRaces.Tiefling));
            Assert.That(character.Class.LevelAdjustment, Is.Positive);
        }

        [Repeat(100)]
        [Test]
        public void BUG_GenerateGhost()
        {
            var character = GetGhost();

            characterAsserter.AssertCharacter(character);
            Assert.That(character.Class.IsNPC, Is.False, character.Summary);
            characterAsserter.AssertGhost(character);
        }

        private Character GetGhost()
        {
            var metaraceRandomizer = GetNewInstanceOf<ISetMetaraceRandomizer>();
            metaraceRandomizer.SetMetarace = RaceConstants.Metaraces.Ghost;

            return characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);
        }

        [TestCase(RaceConstants.BaseRaces.Aasimar, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.Aasimar, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.Aasimar, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.Aasimar, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.Aasimar, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.DeepDwarf, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.DeepDwarf, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.DeepDwarf, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.DeepDwarf, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.DeepDwarf, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.HillDwarf, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.HillDwarf, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.HillDwarf, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.HillDwarf, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.HillDwarf, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.MountainDwarf, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.MountainDwarf, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.MountainDwarf, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.MountainDwarf, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.MountainDwarf, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.DuergarDwarf, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.DuergarDwarf, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.DuergarDwarf, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.DuergarDwarf, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.Drow, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.Drow, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.Drow, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.Drow, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.AquaticElf, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.AquaticElf, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.AquaticElf, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.AquaticElf, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.AquaticElf, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.GrayElf, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.GrayElf, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.GrayElf, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.GrayElf, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.GrayElf, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.HalfElf, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.HalfElf, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.HalfElf, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.HalfElf, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.HalfElf, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.HighElf, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.HighElf, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.HighElf, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.HighElf, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.HighElf, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.WildElf, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.WildElf, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.WildElf, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.WildElf, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.WildElf, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.WoodElf, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.WoodElf, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.WoodElf, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.WoodElf, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.WoodElf, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.Githyanki, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.Githyanki, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.Githyanki, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.Githyanki, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.Githzerai, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.Githzerai, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.Githzerai, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.Githzerai, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.Goblin, 20, 18)]
        [TestCase(RaceConstants.BaseRaces.Goblin, 10, 8)]
        [TestCase(RaceConstants.BaseRaces.Goblin, 4, 2)]
        [TestCase(RaceConstants.BaseRaces.Goblin, 3, 1)]
        [TestCase(RaceConstants.BaseRaces.Goblin, 2, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.Goblin, 1, 1 / 3d)]
        [TestCase(RaceConstants.BaseRaces.LightfootHalfling, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.LightfootHalfling, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.LightfootHalfling, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.LightfootHalfling, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.LightfootHalfling, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.DeepHalfling, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.DeepHalfling, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.DeepHalfling, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.DeepHalfling, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.DeepHalfling, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.TallfellowHalfling, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.TallfellowHalfling, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.TallfellowHalfling, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.TallfellowHalfling, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.TallfellowHalfling, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.Hobgoblin, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.Hobgoblin, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.Hobgoblin, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.Hobgoblin, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.Hobgoblin, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.Human, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.Human, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.Human, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.Human, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.Human, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.Kobold, 20, 17)]
        [TestCase(RaceConstants.BaseRaces.Kobold, 10, 7)]
        [TestCase(RaceConstants.BaseRaces.Kobold, 5, 2)]
        [TestCase(RaceConstants.BaseRaces.Kobold, 4, 1)]
        [TestCase(RaceConstants.BaseRaces.Kobold, 3, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.Kobold, 2, 1 / 3d)]
        [TestCase(RaceConstants.BaseRaces.Kobold, 1, 1 / 4d)]
        [TestCase(RaceConstants.BaseRaces.Orc, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.Orc, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.Orc, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.Orc, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.Orc, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.HalfOrc, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.HalfOrc, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.HalfOrc, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.HalfOrc, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.HalfOrc, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.RockGnome, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.RockGnome, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.RockGnome, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.RockGnome, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.RockGnome, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.ForestGnome, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.ForestGnome, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.ForestGnome, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.ForestGnome, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.ForestGnome, 1, 1 / 2d)]
        [TestCase(RaceConstants.BaseRaces.Svirfneblin, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.Svirfneblin, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.Svirfneblin, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.Svirfneblin, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.Tiefling, 20, 19)]
        [TestCase(RaceConstants.BaseRaces.Tiefling, 10, 9)]
        [TestCase(RaceConstants.BaseRaces.Tiefling, 3, 2)]
        [TestCase(RaceConstants.BaseRaces.Tiefling, 2, 1)]
        [TestCase(RaceConstants.BaseRaces.Tiefling, 1, 1 / 2d)]
        public void BUG_NPCChallengeRating(string race, int level, double cr)
        {
            var metaraceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.Metarace.NoMeta);
            var levelRandomizer = GetNewInstanceOf<ISetLevelRandomizer>();
            var classNameRandomizer = GetNewInstanceOf<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.AnyNPC);
            var baseRaceRandomizer = GetNewInstanceOf<ISetBaseRaceRandomizer>();

            baseRaceRandomizer.SetBaseRace = race;
            levelRandomizer.SetLevel = level;

            var npc = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(npc);
            Assert.That(npc.Class.IsNPC, Is.True, npc.Summary);
            Assert.That(npc.Class.Level, Is.EqualTo(level), npc.Summary);
            Assert.That(npc.Race.BaseRace, Is.EqualTo(race), npc.Summary);
            Assert.That(npc.ChallengeRating, Is.EqualTo(cr), npc.Summary);
        }

        [TestCase(RaceConstants.BaseRaces.Aasimar, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.Aasimar, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.Aasimar, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.Aasimar, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.DeepDwarf, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.DeepDwarf, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.DeepDwarf, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.DeepDwarf, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.DeepDwarf, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.HillDwarf, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.HillDwarf, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.HillDwarf, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.HillDwarf, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.HillDwarf, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.MountainDwarf, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.MountainDwarf, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.MountainDwarf, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.MountainDwarf, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.MountainDwarf, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.DuergarDwarf, 20, 21)]
        [TestCase(RaceConstants.BaseRaces.DuergarDwarf, 10, 11)]
        [TestCase(RaceConstants.BaseRaces.DuergarDwarf, 2, 3)]
        [TestCase(RaceConstants.BaseRaces.DuergarDwarf, 1, 2)]
        [TestCase(RaceConstants.BaseRaces.Drow, 20, 21)]
        [TestCase(RaceConstants.BaseRaces.Drow, 10, 11)]
        [TestCase(RaceConstants.BaseRaces.Drow, 2, 3)]
        [TestCase(RaceConstants.BaseRaces.Drow, 1, 2)]
        [TestCase(RaceConstants.BaseRaces.AquaticElf, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.AquaticElf, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.AquaticElf, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.AquaticElf, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.AquaticElf, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.GrayElf, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.GrayElf, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.GrayElf, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.GrayElf, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.GrayElf, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.HalfElf, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.HalfElf, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.HalfElf, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.HalfElf, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.HalfElf, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.HighElf, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.HighElf, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.HighElf, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.HighElf, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.HighElf, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.WildElf, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.WildElf, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.WildElf, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.WildElf, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.WildElf, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.WoodElf, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.WoodElf, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.WoodElf, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.WoodElf, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.WoodElf, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.Githyanki, 20, 21)]
        [TestCase(RaceConstants.BaseRaces.Githyanki, 10, 11)]
        [TestCase(RaceConstants.BaseRaces.Githyanki, 2, 3)]
        [TestCase(RaceConstants.BaseRaces.Githyanki, 1, 2)]
        [TestCase(RaceConstants.BaseRaces.Githzerai, 20, 21)]
        [TestCase(RaceConstants.BaseRaces.Githzerai, 10, 11)]
        [TestCase(RaceConstants.BaseRaces.Githzerai, 2, 3)]
        [TestCase(RaceConstants.BaseRaces.Githzerai, 1, 2)]
        [TestCase(RaceConstants.BaseRaces.Goblin, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.Goblin, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.Goblin, 4, 4)]
        [TestCase(RaceConstants.BaseRaces.Goblin, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.Goblin, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.Goblin, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.LightfootHalfling, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.LightfootHalfling, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.LightfootHalfling, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.LightfootHalfling, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.LightfootHalfling, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.DeepHalfling, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.DeepHalfling, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.DeepHalfling, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.DeepHalfling, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.DeepHalfling, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.TallfellowHalfling, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.TallfellowHalfling, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.TallfellowHalfling, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.TallfellowHalfling, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.TallfellowHalfling, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.Hobgoblin, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.Hobgoblin, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.Hobgoblin, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.Hobgoblin, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.Hobgoblin, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.Human, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.Human, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.Human, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.Human, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.Human, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.Kobold, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.Kobold, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.Kobold, 5, 5)]
        [TestCase(RaceConstants.BaseRaces.Kobold, 4, 4)]
        [TestCase(RaceConstants.BaseRaces.Kobold, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.Kobold, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.Kobold, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.Orc, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.Orc, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.Orc, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.Orc, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.Orc, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.HalfOrc, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.HalfOrc, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.HalfOrc, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.HalfOrc, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.HalfOrc, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.RockGnome, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.RockGnome, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.RockGnome, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.RockGnome, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.RockGnome, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.ForestGnome, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.ForestGnome, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.ForestGnome, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.ForestGnome, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.ForestGnome, 1, 1)]
        [TestCase(RaceConstants.BaseRaces.Svirfneblin, 20, 21)]
        [TestCase(RaceConstants.BaseRaces.Svirfneblin, 10, 11)]
        [TestCase(RaceConstants.BaseRaces.Svirfneblin, 2, 3)]
        [TestCase(RaceConstants.BaseRaces.Svirfneblin, 1, 2)]
        [TestCase(RaceConstants.BaseRaces.Tiefling, 20, 20)]
        [TestCase(RaceConstants.BaseRaces.Tiefling, 10, 10)]
        [TestCase(RaceConstants.BaseRaces.Tiefling, 3, 3)]
        [TestCase(RaceConstants.BaseRaces.Tiefling, 2, 2)]
        [TestCase(RaceConstants.BaseRaces.Tiefling, 1, 1)]
        public void BUG_PCChallengeRating(string race, int level, double cr)
        {
            var metaraceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.Metarace.NoMeta);
            var levelRandomizer = GetNewInstanceOf<ISetLevelRandomizer>();
            var classNameRandomizer = GetNewInstanceOf<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.AnyPlayer);
            var baseRaceRandomizer = GetNewInstanceOf<ISetBaseRaceRandomizer>();

            baseRaceRandomizer.SetBaseRace = race;
            levelRandomizer.SetLevel = level;

            var pc = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(pc);
            Assert.That(pc.Class.IsNPC, Is.False, pc.Summary);
            Assert.That(pc.Class.Level, Is.EqualTo(level), pc.Summary);
            Assert.That(pc.Race.BaseRace, Is.EqualTo(race), pc.Summary);
            Assert.That(pc.ChallengeRating, Is.EqualTo(cr), pc.Summary);
        }

        [Repeat(100)]
        [Test]
        public void BUG_GenerateCleric()
        {
            //INFO: Doing Good instead of Raw to ensure that the Wisdom is high enough to actually cast spells
            var abilitiesRandomizer = GetNewInstanceOf<IAbilitiesRandomizer>(AbilitiesRandomizerTypeConstants.Good);
            var baseRaceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.BaseRace.AnyBase);
            var metaraceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.Metarace.AnyMeta);
            var alignmentRandomizer = GetNewInstanceOf<IAlignmentRandomizer>(AlignmentRandomizerTypeConstants.Any);
            var levelRandomizer = GetNewInstanceOf<ILevelRandomizer>(LevelRandomizerTypeConstants.Any);

            var classNameRandomizer = GetNewInstanceOf<ISetClassNameRandomizer>();
            classNameRandomizer.SetClassName = CharacterClassConstants.Cleric;

            var cleric = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(cleric);
            Assert.That(cleric.Class.IsNPC, Is.False, cleric.Summary);
            characterAsserter.AssertSpellcaster(cleric);

            if (cleric.Alignment.Lawfulness == AlignmentConstants.Lawful)
                Assert.That(cleric.Class.ProhibitedFields, Contains.Item(CharacterClassConstants.Domains.Chaos), cleric.Summary);
            else
                Assert.That(cleric.Class.ProhibitedFields, Does.Not.Contain(CharacterClassConstants.Domains.Chaos), cleric.Summary);

            if (cleric.Alignment.Lawfulness == AlignmentConstants.Chaotic)
                Assert.That(cleric.Class.ProhibitedFields, Contains.Item(CharacterClassConstants.Domains.Law), cleric.Summary);
            else
                Assert.That(cleric.Class.ProhibitedFields, Does.Not.Contain(CharacterClassConstants.Domains.Law), cleric.Summary);

            if (cleric.Alignment.Goodness == AlignmentConstants.Good)
                Assert.That(cleric.Class.ProhibitedFields, Contains.Item(CharacterClassConstants.Domains.Evil), cleric.Summary);
            else
                Assert.That(cleric.Class.ProhibitedFields, Does.Not.Contain(CharacterClassConstants.Domains.Evil), cleric.Summary);

            if (cleric.Alignment.Goodness == AlignmentConstants.Evil)
                Assert.That(cleric.Class.ProhibitedFields, Contains.Item(CharacterClassConstants.Domains.Good), cleric.Summary);
            else
                Assert.That(cleric.Class.ProhibitedFields, Does.Not.Contain(CharacterClassConstants.Domains.Good), cleric.Summary);

            if (cleric.Class.SpecialistFields.Contains(CharacterClassConstants.Domains.Law))
                Assert.That(cleric.Alignment.Lawfulness, Is.EqualTo(AlignmentConstants.Lawful), cleric.Summary);

            if (cleric.Class.SpecialistFields.Contains(CharacterClassConstants.Domains.Chaos))
                Assert.That(cleric.Alignment.Lawfulness, Is.EqualTo(AlignmentConstants.Chaotic), cleric.Summary);

            if (cleric.Class.SpecialistFields.Contains(CharacterClassConstants.Domains.Good))
                Assert.That(cleric.Alignment.Goodness, Is.EqualTo(AlignmentConstants.Good), cleric.Summary);

            if (cleric.Class.SpecialistFields.Contains(CharacterClassConstants.Domains.Evil))
                Assert.That(cleric.Alignment.Goodness, Is.EqualTo(AlignmentConstants.Evil), cleric.Summary);
        }

        [Repeat(100)]
        [Test]
        public void BUG_GenerateCharacterWithAnimal_LowLevelDoesNotHaveMount()
        {
            var minimums = new Dictionary<string, int>
            {
                { CharacterClassConstants.Adept, 2 },
                { CharacterClassConstants.Aristocrat, 100 },
                { CharacterClassConstants.Commoner, 100 },
                { CharacterClassConstants.Expert, 100 },
                { CharacterClassConstants.Warrior, 100 },
                { CharacterClassConstants.Barbarian, 100 },
                { CharacterClassConstants.Bard, 100 },
                { CharacterClassConstants.Cleric, 100 },
                { CharacterClassConstants.Druid, 1 },
                { CharacterClassConstants.Fighter, 100 },
                { CharacterClassConstants.Monk, 100 },
                { CharacterClassConstants.Paladin, 5 },
                { CharacterClassConstants.Ranger, 4 },
                { CharacterClassConstants.Rogue, 100 },
                { CharacterClassConstants.Sorcerer, 1 },
                { CharacterClassConstants.Wizard, 1 },
            };

            var classNameRandomizer = GetNewInstanceOf<ISetClassNameRandomizer>();
            var className = collectionSelector.SelectRandomFrom(minimums.Keys);
            classNameRandomizer.SetClassName = className;

            var character = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(character);

            if (character.Class.Level < minimums[className])
                Assert.That(character.Magic.Animal, Is.Empty, character.Summary);
            else
                Assert.That(character.Magic.Animal, Is.Not.Empty, character.Summary);
        }

        [Test]
        public void BUG_GenerateWerewolfLord_ChallengeRatingisCorrect()
        {
            var classNameRandomizer = GetNewInstanceOf<ISetClassNameRandomizer>();
            var levelRandomizer = GetNewInstanceOf<ISetLevelRandomizer>();
            var baseRaceRandomizer = GetNewInstanceOf<ISetBaseRaceRandomizer>();
            var metaraceRandomizer = GetNewInstanceOf<ISetMetaraceRandomizer>();

            classNameRandomizer.SetClassName = CharacterClassConstants.Fighter;
            levelRandomizer.SetLevel = 10;
            baseRaceRandomizer.SetBaseRace = RaceConstants.BaseRaces.Human;
            metaraceRandomizer.SetMetarace = RaceConstants.Metaraces.Werewolf_Dire;

            var character = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(character);
            Assert.That(character.ChallengeRating, Is.EqualTo(14), character.Summary);
        }

        [Test]
        public void BUG_GenerateHillGiantDireWereboar_ChallengeRatingisCorrect()
        {
            var classNameRandomizer = GetNewInstanceOf<ISetClassNameRandomizer>();
            var levelRandomizer = GetNewInstanceOf<ISetLevelRandomizer>();
            var baseRaceRandomizer = GetNewInstanceOf<ISetBaseRaceRandomizer>();
            var metaraceRandomizer = GetNewInstanceOf<ISetMetaraceRandomizer>();

            classNameRandomizer.SetClassName = CharacterClassConstants.Fighter;
            levelRandomizer.SetLevel = 1;
            baseRaceRandomizer.SetBaseRace = RaceConstants.BaseRaces.HillGiant;
            metaraceRandomizer.SetMetarace = RaceConstants.Metaraces.Wereboar_Dire;

            var character = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(character);
            Assert.That(character.ChallengeRating, Is.EqualTo(12), character.Summary);
        }

        [Repeat(100)]
        [Test]
        public void BUG_GenerateMummyLord()
        {
            var classNameRandomizer = GetNewInstanceOf<ISetClassNameRandomizer>();
            var levelRandomizer = GetNewInstanceOf<ISetLevelRandomizer>();
            var baseRaceRandomizer = GetNewInstanceOf<ISetBaseRaceRandomizer>();
            var metaraceRandomizer = GetNewInstanceOf<RaceRandomizer>(RaceRandomizerTypeConstants.Metarace.NoMeta);
            var abilitiesRandomizer = GetNewInstanceOf<IAbilitiesRandomizer>(AbilitiesRandomizerTypeConstants.Average);

            classNameRandomizer.SetClassName = CharacterClassConstants.Cleric;
            levelRandomizer.SetLevel = 10;
            baseRaceRandomizer.SetBaseRace = RaceConstants.BaseRaces.Mummy;

            var character = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(character);
            Assert.That(character.Class.Name, Is.EqualTo(CharacterClassConstants.Cleric), character.Summary);
            Assert.That(character.Class.Level, Is.EqualTo(10), character.Summary);
            Assert.That(character.Race.BaseRace, Is.EqualTo(RaceConstants.BaseRaces.Mummy), character.Summary);
            Assert.That(character.Race.Metarace, Is.EqualTo(RaceConstants.Metaraces.None), character.Summary);
            Assert.That(character.Race.Size, Is.EqualTo(RaceConstants.Sizes.Medium));
            Assert.That(character.Race.LandSpeed.Value, Is.EqualTo(20), character.Summary);
            Assert.That(character.Race.Height.Value, Is.InRange(60, 72), character.Summary);
            Assert.That(character.Race.Weight.Value, Is.EqualTo(120).Within(20), character.Summary);
            Assert.That(character.Languages, Contains.Item(LanguageConstants.Common), character.Summary);
            Assert.That(character.Combat.BaseAttack.BaseBonus, Is.EqualTo(7), character.Summary);
            Assert.That(character.Combat.BaseAttack.RacialModifier, Is.EqualTo(4), character.Summary);
            Assert.That(character.Feats.Racial.Select(f => f.Name), Contains.Item(FeatConstants.Despair)
                .And.Contains(FeatConstants.MummyRot)
                .And.Contains(FeatConstants.DamageReduction)
                .And.Contains(FeatConstants.Darkvision)
                .And.Contains(FeatConstants.NaturalArmor)
                .And.Contains(FeatConstants.VulnerabilityToEffect), character.Summary);
            Assert.That(character.Abilities, Does.Not.ContainKey(AbilityConstants.Constitution), character.Summary);
            Assert.That(character.ChallengeRating, Is.EqualTo(15), character.Summary);
        }

        [Test]
        public void DEBUG_GenerateBetaCharacter()
        {
            var classNameRandomizer = GetNewInstanceOf<ISetClassNameRandomizer>();
            var levelRandomizer = GetNewInstanceOf<ILevelRandomizer>(LevelRandomizerTypeConstants.Low);
            var abilitiesRandomizer = GetNewInstanceOf<IAbilitiesRandomizer>(AbilitiesRandomizerTypeConstants.OnesAsSixes);

            classNameRandomizer.SetClassName = CharacterClassConstants.Barbarian;

            var character = characterGenerator.GenerateWith(
                alignmentRandomizer,
                classNameRandomizer,
                levelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);

            characterAsserter.AssertCharacter(character);
        }
    }
}
