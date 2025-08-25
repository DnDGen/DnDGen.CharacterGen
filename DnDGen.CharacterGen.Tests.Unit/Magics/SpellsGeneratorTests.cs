using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Selectors;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Magics
{
    [TestFixture]
    public class SpellsGeneratorTests
    {
        private Mock<IAdjustmentsSelector> mockAdjustmentsSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<IPercentileSelector> mockPercentileSelector;
        private ISpellsGenerator spellsGenerator;
        private CharacterClass characterClass;
        private Dictionary<string, Ability> abilities;
        private List<string> spellcasters;
        private Dictionary<string, int> spellsPerDayForClass;
        private Dictionary<string, int> spellsKnownForClass;
        private Dictionary<string, Dictionary<string, List<string>>> spells;
        private List<string> divineCasters;

        [SetUp]
        public void Setup()
        {
            mockAdjustmentsSelector = new Mock<IAdjustmentsSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockPercentileSelector = new Mock<IPercentileSelector>();
            spellsGenerator = new SpellsGenerator(mockCollectionsSelector.Object, mockAdjustmentsSelector.Object, mockPercentileSelector.Object);
            characterClass = new CharacterClass();
            spellcasters = [];
            abilities = [];
            spellsPerDayForClass = [];
            spellsKnownForClass = [];
            spells = [];
            divineCasters = [];

            characterClass.Name = "class name";
            characterClass.Level = 9;
            spellcasters.Add(characterClass.Name);
            spellcasters.Add("other class");
            spellsPerDayForClass["0"] = 90210;
            spellsPerDayForClass["1"] = 42;
            spellsKnownForClass["0"] = 2;
            spellsKnownForClass["1"] = 1;
            abilities["stat"] = new Ability("stat")
            {
                Value = 11
            };
            abilities["other stat"] = new Ability("other stat")
            {
                Value = 11
            };
            divineCasters.Add("other divine class");

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.Spellcasters))
                .Returns(spellcasters);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, SpellConstants.Sources.Divine))
                .Returns(divineCasters);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.AbilityGroups, characterClass.Name + GroupConstants.Spellcasters))
                .Returns(["stat"]);

            var tableName = string.Format(TableNameConstants.Formattable.Adjustments.LevelXCLASSSpellsPerDay, characterClass.Level, characterClass.Name);
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(spellsPerDayForClass);

            tableName = string.Format(TableNameConstants.Formattable.Adjustments.LevelXCLASSKnownSpells, characterClass.Level, characterClass.Name);
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(spellsKnownForClass);

            SetUpDomain(characterClass.Name);
            SetUpDomain("domain 1");
            SetUpDomain("domain 2");

            AddSpell("spell 1", characterClass.Name, 0);
            AddSpell("spell 2", characterClass.Name, 0);
            AddSpell("spell 3", characterClass.Name, 1);
            AddSpell("spell 4", characterClass.Name, 1);
            AddSpell("spell 1", "domain 1", 0);
            AddSpell("spell 3", "domain 1", 1);
            AddSpell("spell 2", "domain 2", 0);
            AddSpell("spell 4", "domain 2", 1);

            var index = 0;
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<string>>())).Returns((IEnumerable<string> c) => c.ElementAt(index++ % c.Count()));
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<Spell>>())).Returns((IEnumerable<Spell> c) => c.ElementAt(index++ % c.Count()));
        }

        private void AddSpell(string spell, string source, int level)
        {
            if (!spells.ContainsKey(source))
                SetUpDomain(source);

            var levelKey = level.ToString();
            if (!spells[source].ContainsKey(levelKey))
                spells[source][levelKey] = [];

            spells[source][levelKey].Add(spell);
        }

        private void SetUpDomain(string domain)
        {
            spells[domain] = new Dictionary<string, List<string>> {
                { "0", [] },
                { "1", [] },
                { "2", [] },
                { "3", [] },
                { "4", [] },
                { "5", [] },
            };

            var tableName = string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, domain);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, tableName, It.IsAny<string>()))
                .Returns((string _, string _, string l) => spells[domain][l]);
            mockCollectionsSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(() => spells[domain].ToDictionary(s => s.Key, s => s.Value.AsEnumerable()));
        }

        [Test]
        public void GeneratePerDay_DoNotGenerateSpellsPerDayIfNotASpellcaster()
        {
            spellcasters.Remove(characterClass.Name);
            var spellsPerDay = spellsGenerator.GeneratePerDay(characterClass, abilities);
            Assert.That(spellsPerDay, Is.Empty);
        }

        [Test]
        public void GeneratePerDay_GenerateSpellsPerDay()
        {
            var spellsPerDay = spellsGenerator.GeneratePerDay(characterClass, abilities);

            var cantrips = spellsPerDay.First(s => s.Level == 0);
            Assert.That(cantrips.Quantity, Is.EqualTo(90210));

            var firstLevelSpells = spellsPerDay.First(s => s.Level == 1);
            Assert.That(firstLevelSpells.Quantity, Is.EqualTo(42));

            Assert.That(spellsPerDay.Select(s => s.Source), Is.All.EqualTo(characterClass.Name));
            Assert.That(spellsPerDay.Count, Is.EqualTo(2));
        }

        [Test]
        public void GeneratePerDay_IfLevelAbove20_UseLevel20SpellsPerDay()
        {
            characterClass.Level = 9266;
            abilities["stat"].Value = 16;

            var level20SpellsPerDay = new Dictionary<string, int>();
            level20SpellsPerDay["0"] = 90210;
            level20SpellsPerDay["1"] = 42;
            level20SpellsPerDay["2"] = 600;
            level20SpellsPerDay["3"] = 1337;

            var tableName = string.Format(TableNameConstants.Formattable.Adjustments.LevelXCLASSSpellsPerDay, 20, characterClass.Name);
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(level20SpellsPerDay);

            var spellsPerDay = spellsGenerator.GeneratePerDay(characterClass, abilities).ToArray();
            Assert.That(spellsPerDay[0].Level, Is.EqualTo(0));
            Assert.That(spellsPerDay[0].Quantity, Is.EqualTo(90210));
            Assert.That(spellsPerDay[1].Level, Is.EqualTo(1));
            Assert.That(spellsPerDay[1].Quantity, Is.EqualTo(42 + 1));
            Assert.That(spellsPerDay[2].Level, Is.EqualTo(2));
            Assert.That(spellsPerDay[2].Quantity, Is.EqualTo(600 + 1));
            Assert.That(spellsPerDay[3].Level, Is.EqualTo(3));
            Assert.That(spellsPerDay[3].Quantity, Is.EqualTo(1337 + 1));
            Assert.That(spellsPerDay.Length, Is.EqualTo(4));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10, 0)]
        [TestCase(11, 0, 0)]
        [TestCase(12, 0, 1, 0)]
        [TestCase(13, 0, 1, 0, 0)]
        [TestCase(14, 0, 1, 1, 0, 0)]
        [TestCase(15, 0, 1, 1, 0, 0, 0)]
        [TestCase(16, 0, 1, 1, 1, 0, 0, 0)]
        [TestCase(17, 0, 1, 1, 1, 0, 0, 0, 0)]
        [TestCase(18, 0, 1, 1, 1, 1, 0, 0, 0, 0)]
        [TestCase(19, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0)]
        [TestCase(20, 0, 2, 1, 1, 1, 1, 0, 0, 0, 0)]
        [TestCase(21, 0, 2, 1, 1, 1, 1, 0, 0, 0, 0)]
        [TestCase(22, 0, 2, 2, 1, 1, 1, 1, 0, 0, 0)]
        [TestCase(23, 0, 2, 2, 1, 1, 1, 1, 0, 0, 0)]
        [TestCase(24, 0, 2, 2, 2, 1, 1, 1, 1, 0, 0)]
        [TestCase(25, 0, 2, 2, 2, 1, 1, 1, 1, 0, 0)]
        [TestCase(26, 0, 2, 2, 2, 2, 1, 1, 1, 1, 0)]
        [TestCase(27, 0, 2, 2, 2, 2, 1, 1, 1, 1, 0)]
        [TestCase(28, 0, 3, 2, 2, 2, 2, 1, 1, 1, 1)]
        [TestCase(29, 0, 3, 2, 2, 2, 2, 1, 1, 1, 1)]
        [TestCase(30, 0, 3, 3, 2, 2, 2, 2, 1, 1, 1)]
        [TestCase(31, 0, 3, 3, 2, 2, 2, 2, 1, 1, 1)]
        [TestCase(32, 0, 3, 3, 3, 2, 2, 2, 2, 1, 1)]
        [TestCase(33, 0, 3, 3, 3, 2, 2, 2, 2, 1, 1)]
        [TestCase(34, 0, 3, 3, 3, 3, 2, 2, 2, 2, 1)]
        [TestCase(35, 0, 3, 3, 3, 3, 2, 2, 2, 2, 1)]
        [TestCase(36, 0, 4, 3, 3, 3, 3, 2, 2, 2, 2)]
        [TestCase(37, 0, 4, 3, 3, 3, 3, 2, 2, 2, 2)]
        [TestCase(38, 0, 4, 4, 3, 3, 3, 3, 2, 2, 2)]
        [TestCase(39, 0, 4, 4, 3, 3, 3, 3, 2, 2, 2)]
        [TestCase(40, 0, 4, 4, 4, 3, 3, 3, 3, 2, 2)]
        [TestCase(41, 0, 4, 4, 4, 3, 3, 3, 3, 2, 2)]
        [TestCase(42, 0, 4, 4, 4, 4, 3, 3, 3, 3, 2)]
        [TestCase(43, 0, 4, 4, 4, 4, 3, 3, 3, 3, 2)]
        [TestCase(44, 0, 5, 4, 4, 4, 4, 3, 3, 3, 3)]
        [TestCase(45, 0, 5, 4, 4, 4, 4, 3, 3, 3, 3)]
        public void GeneratePerDay_AddBonusSpellsPerDayForStat(int statValue, params int[] levelBonuses)
        {
            abilities["stat"].Value = statValue;
            spellsPerDayForClass["0"] = 10;
            spellsPerDayForClass["1"] = 9;
            spellsPerDayForClass["2"] = 8;
            spellsPerDayForClass["3"] = 7;
            spellsPerDayForClass["4"] = 6;
            spellsPerDayForClass["5"] = 5;
            spellsPerDayForClass["6"] = 4;
            spellsPerDayForClass["7"] = 3;
            spellsPerDayForClass["8"] = 2;
            spellsPerDayForClass["9"] = 1;

            var generatedSpellsPerDay = spellsGenerator.GeneratePerDay(characterClass, abilities);

            for (var spellLevel = 0; spellLevel < levelBonuses.Length; spellLevel++)
            {
                var expectedQuantity = 10 - spellLevel + levelBonuses[spellLevel];
                var spells = generatedSpellsPerDay.First(s => s.Level == spellLevel);
                Assert.That(spells.Quantity, Is.EqualTo(expectedQuantity), spellLevel.ToString());
            }

            Assert.That(generatedSpellsPerDay.Count, Is.EqualTo(levelBonuses.Length));
        }

        [Test]
        public void GeneratePerDay_CannotGetSpellsPerDayInLevelThatAbilitiesDoNotAllow()
        {
            abilities["stat"].Value = 10;

            var spellsPerDay = spellsGenerator.GeneratePerDay(characterClass, abilities);
            Assert.That(spellsPerDay.Count, Is.EqualTo(1));

            var cantrips = spellsPerDay.First(s => s.Level == 0);
            Assert.That(cantrips.Quantity, Is.EqualTo(90210));
        }

        [Test]
        public void GeneratePerDay_CannotGetBonusSpellsPerDayInLevelThatCharacterCannotCast()
        {
            abilities["stat"].Value = 45;

            var spellsPerDay = spellsGenerator.GeneratePerDay(characterClass, abilities);
            Assert.That(spellsPerDay.Count, Is.EqualTo(2));

            var cantrips = spellsPerDay.First(s => s.Level == 0);
            Assert.That(cantrips.Quantity, Is.EqualTo(90210));

            var firstLevelSpells = spellsPerDay.First(s => s.Level == 1);
            Assert.That(firstLevelSpells.Quantity, Is.EqualTo(47));
        }

        [Test]
        public void GeneratePerDay_CanGetBonusSpellsPerDayInLevelWithQuantityOf0()
        {
            abilities["stat"].Value = 45;
            spellsPerDayForClass["2"] = 0;

            var spellsPerDay = spellsGenerator.GeneratePerDay(characterClass, abilities);
            Assert.That(spellsPerDay.Count, Is.EqualTo(3));

            var cantrips = spellsPerDay.First(s => s.Level == 0);
            Assert.That(cantrips.Quantity, Is.EqualTo(90210));

            var firstLevelSpells = spellsPerDay.First(s => s.Level == 1);
            Assert.That(firstLevelSpells.Quantity, Is.EqualTo(47));

            var secondLevelSpells = spellsPerDay.First(s => s.Level == 2);
            Assert.That(secondLevelSpells.Quantity, Is.EqualTo(4));
        }

        [Test]
        public void GeneratePerDay_IfTotalSpellsPerDayIs0AndDoesNotHaveDomainSpell_RemoveSpellLevel()
        {
            spellsPerDayForClass["1"] = 0;

            var spellsPerDay = spellsGenerator.GeneratePerDay(characterClass, abilities);
            var cantrips = spellsPerDay.First(s => s.Level == 0);

            Assert.That(cantrips.Quantity, Is.EqualTo(90210));
            Assert.That(spellsPerDay.Count, Is.EqualTo(1));
        }

        [Test]
        public void GeneratePerDay_IfTotalSpellsPerDayIs0AndHasDomainSpell_DoNotRemoveSpellLevel()
        {
            spellsPerDayForClass["1"] = 0;
            characterClass.SpecialistFields = new[] { "specialist" };

            var spellsPerDay = spellsGenerator.GeneratePerDay(characterClass, abilities);
            Assert.That(spellsPerDay.Count, Is.EqualTo(2));

            var cantrips = spellsPerDay.First(s => s.Level == 0);
            Assert.That(cantrips.Quantity, Is.EqualTo(90210));
            Assert.That(cantrips.HasDomainSpell, Is.False);

            var firstLevelSpells = spellsPerDay.First(s => s.Level == 1);
            Assert.That(firstLevelSpells.Quantity, Is.EqualTo(0));
            Assert.That(firstLevelSpells.HasDomainSpell, Is.True);
        }

        [Test]
        public void GeneratePerDay_IfTotalSpellsPerDayIsGreaterThan0AndDoesNotHaveDomainSpell_DoNotRemoveSpellLevel()
        {
            spellsPerDayForClass["1"] = 1;

            var spellsPerDay = spellsGenerator.GeneratePerDay(characterClass, abilities);
            Assert.That(spellsPerDay.Count, Is.EqualTo(2));

            var cantrips = spellsPerDay.First(s => s.Level == 0);
            Assert.That(cantrips.Quantity, Is.EqualTo(90210));
            Assert.That(cantrips.HasDomainSpell, Is.False);

            var firstLevelSpells = spellsPerDay.First(s => s.Level == 1);
            Assert.That(firstLevelSpells.Quantity, Is.EqualTo(1));
            Assert.That(firstLevelSpells.HasDomainSpell, Is.False);
        }

        [Test]
        public void GeneratePerDay_AllSpellLevelsPerDayExcept0GetDomainSpellIfClassHasSpecialistFields()
        {
            characterClass.SpecialistFields = new[] { "specialist" };
            spellsPerDayForClass["0"] = 10;
            spellsPerDayForClass["1"] = 9;
            spellsPerDayForClass["2"] = 8;
            spellsPerDayForClass["3"] = 7;
            spellsPerDayForClass["4"] = 6;
            spellsPerDayForClass["5"] = 5;
            spellsPerDayForClass["6"] = 4;
            spellsPerDayForClass["7"] = 3;
            spellsPerDayForClass["8"] = 2;
            spellsPerDayForClass["9"] = 1;

            var spellsPerDay = spellsGenerator.GeneratePerDay(characterClass, abilities);

            var cantrips = spellsPerDay.First(s => s.Level == 0);
            Assert.That(cantrips.HasDomainSpell, Is.False);

            var nonCantrips = spellsPerDay.Except(new[] { cantrips });
            foreach (var nonCantrip in nonCantrips)
                Assert.That(nonCantrip.HasDomainSpell, Is.True);
        }

        [Test]
        public void GeneratePerDay_AllSpellLevelsPerDayExcept0GetDomainSpellIfClassHasMultipleSpecialistFields()
        {
            characterClass.SpecialistFields = new[] { "specialist", "also specialist" };
            spellsPerDayForClass["0"] = 10;
            spellsPerDayForClass["1"] = 9;
            spellsPerDayForClass["2"] = 8;
            spellsPerDayForClass["3"] = 7;
            spellsPerDayForClass["4"] = 6;
            spellsPerDayForClass["5"] = 5;
            spellsPerDayForClass["6"] = 4;
            spellsPerDayForClass["7"] = 3;
            spellsPerDayForClass["8"] = 2;
            spellsPerDayForClass["9"] = 1;

            var spellsPerDay = spellsGenerator.GeneratePerDay(characterClass, abilities);

            var cantrips = spellsPerDay.First(s => s.Level == 0);
            Assert.That(cantrips.HasDomainSpell, Is.False);

            var nonCantrips = spellsPerDay.Except(new[] { cantrips });
            foreach (var nonCantrip in nonCantrips)
                Assert.That(nonCantrip.HasDomainSpell, Is.True, nonCantrip.Level.ToString());
        }

        [Test]
        public void GeneratePerDay_NoSpellLevelsPerDayGetDomainSpellIfClassDoesNotHaveSpecialistFields()
        {
            spellsPerDayForClass["0"] = 10;
            spellsPerDayForClass["1"] = 9;
            spellsPerDayForClass["2"] = 8;
            spellsPerDayForClass["3"] = 7;
            spellsPerDayForClass["4"] = 6;
            spellsPerDayForClass["5"] = 5;
            spellsPerDayForClass["6"] = 4;
            spellsPerDayForClass["7"] = 3;
            spellsPerDayForClass["8"] = 2;
            spellsPerDayForClass["9"] = 1;

            var spellsPerDay = spellsGenerator.GeneratePerDay(characterClass, abilities);

            foreach (var spells in spellsPerDay)
                Assert.That(spells.HasDomainSpell, Is.False, spells.Level.ToString());
        }

        [Test]
        public void GenerateKnown_DoNotGenerateKnownSpellsIfNotASpellcaster()
        {
            spellcasters.Remove(characterClass.Name);
            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities);
            Assert.That(spellsKnown, Is.Empty);
        }

        [Test]
        public void GenerateKnown_GenerateKnownSpells()
        {
            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(3));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 3 (class name/1)"));
        }

        [Test]
        public void GenerateKnown_IfLevelAbove20_UseLevel20KnownSpells()
        {
            characterClass.Level = 9266;
            abilities["stat"].Value = 16;

            var level20KnownSpells = new Dictionary<string, int>
            {
                ["0"] = 90,
                ["1"] = 210,
                ["2"] = 42,
                ["3"] = 600,
                ["4"] = 13,
                ["5"] = 37
            };

            foreach (var kvp in level20KnownSpells)
            {
                var spellLevel = Convert.ToInt32(kvp.Key);
                var quantity = kvp.Value;

                while (quantity-- > 0)
                {
                    var spell = Guid.NewGuid().ToString();
                    AddSpell(spell, characterClass.Name, spellLevel);
                }
            }

            var tableName = string.Format(TableNameConstants.Formattable.Adjustments.LevelXCLASSKnownSpells, 20, characterClass.Name);
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(level20KnownSpells);

            var knownSpells = spellsGenerator.GenerateKnown(characterClass, abilities);
            Assert.That(knownSpells.Count(s => s.Sources[characterClass.Name] == 0), Is.EqualTo(90));
            Assert.That(knownSpells.Count(s => s.Sources[characterClass.Name] == 1), Is.EqualTo(210));
            Assert.That(knownSpells.Count(s => s.Sources[characterClass.Name] == 2), Is.EqualTo(42));
            Assert.That(knownSpells.Count(s => s.Sources[characterClass.Name] == 3), Is.EqualTo(600));
            Assert.That(knownSpells.Count(s => s.Sources[characterClass.Name] == 4), Is.EqualTo(13));
            Assert.That(knownSpells.Count(s => s.Sources[characterClass.Name] == 5), Is.EqualTo(37));
            Assert.That(knownSpells.Count, Is.EqualTo(90 + 210 + 42 + 600 + 13 + 37));
        }

        [Test]
        public void GenerateKnown_KnowAllSpellsBySheerQuantity()
        {
            spellsKnownForClass["0"] = 1;
            spellsKnownForClass["1"] = 3;

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 3 (class name/1)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 4 (class name/1)"));
        }

        [Test]
        public void GenerateKnown_DivineCastersKnowAllSpells()
        {
            divineCasters.Add(characterClass.Name);

            spellsKnownForClass["0"] = 1;
            spellsKnownForClass["1"] = 1;

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(4));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 3 (class name/1)"));
            Assert.That(spellsKnown[3].Summary, Is.EqualTo("spell 4 (class name/1)"));
        }

        [Test]
        public void GenerateKnown_DivineCastersKnowAllSpecialistSpells()
        {
            divineCasters.Add(characterClass.Name);

            characterClass.SpecialistFields = ["special domain"];
            SetUpDomain("special domain");
            AddSpell("spell 5", "special domain", 1);
            AddSpell("other special domain spell", "special domain", 1);
            AddSpell("too high domain spell", "special domain", 2);

            spellsKnownForClass["0"] = 1;
            spellsKnownForClass["1"] = 1;

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(6));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 3 (class name/1)"));
            Assert.That(spellsKnown[3].Summary, Is.EqualTo("spell 4 (class name/1)"));
            Assert.That(spellsKnown[4].Summary, Is.EqualTo("spell 5 (special domain/1)"));
            Assert.That(spellsKnown[5].Summary, Is.EqualTo("other special domain spell (special domain/1)"));
        }

        [Test]
        public void BUG_GenerateKnown_DivineCastersKnowAllSpecialistSpells_DisparateLevels()
        {
            abilities["stat"].Value = 16;
            divineCasters.Add(characterClass.Name);

            characterClass.SpecialistFields = ["special domain", "other domain"];
            SetUpDomain("special domain");
            SetUpDomain("other domain");
            AddSpell("spell 5", "special domain", 1);
            AddSpell("other special domain spell", "special domain", 1);
            AddSpell("too high domain spell", "special domain", 3);
            AddSpell("spell 6", "other domain", 1);
            AddSpell("other domain spell", "other domain", 1);
            AddSpell("too high other domain spell", "other domain", 3);
            AddSpell("spell 7", "special domain", 1);
            AddSpell("spell 7", "class name", 2);

            spellsKnownForClass["0"] = 1;
            spellsKnownForClass["1"] = 1;
            spellsKnownForClass["2"] = 1;

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(9));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 3 (class name/1)"));
            Assert.That(spellsKnown[3].Summary, Is.EqualTo("spell 4 (class name/1)"));
            Assert.That(spellsKnown[4].Summary, Is.EqualTo("spell 5 (special domain/1)"));
            Assert.That(spellsKnown[5].Summary, Is.EqualTo("other special domain spell (special domain/1)"));
            Assert.That(spellsKnown[6].Summary, Is.EqualTo("spell 7 (special domain/1, class name/2)"));
            Assert.That(spellsKnown[7].Summary, Is.EqualTo("spell 6 (other domain/1)"));
            Assert.That(spellsKnown[8].Summary, Is.EqualTo("other domain spell (other domain/1)"));
        }

        [Test]
        public void GenerateKnown_DivineCastersKnowAllOverlappingSpecialistSpells()
        {
            divineCasters.Add(characterClass.Name);

            characterClass.SpecialistFields = ["special domain"];
            SetUpDomain("special domain");
            AddSpell("spell 5", "special domain", 1);
            AddSpell("spell 3", "special domain", 1);
            AddSpell("other special domain spell", "special domain", 2);

            spellsKnownForClass["0"] = 1;
            spellsKnownForClass["1"] = 1;

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(5));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 3 (class name/1, special domain/1)"));
            Assert.That(spellsKnown[3].Summary, Is.EqualTo("spell 4 (class name/1)"));
            Assert.That(spellsKnown[4].Summary, Is.EqualTo("spell 5 (special domain/1)"));
        }

        [Test]
        public void GenerateKnown_DivineCastersKnowAllSpecialistSpellsFromMultipleDomains()
        {
            divineCasters.Add(characterClass.Name);

            characterClass.SpecialistFields = ["special domain", "other domain"];
            SetUpDomain("special domain");
            SetUpDomain("other domain");
            AddSpell("spell 5", "special domain", 1);
            AddSpell("other special domain spell", "special domain", 1);
            AddSpell("too high domain spell", "special domain", 2);
            AddSpell("spell 6", "other domain", 1);
            AddSpell("other domain spell", "other domain", 1);
            AddSpell("too high other domain spell", "other domain", 2);

            spellsKnownForClass["0"] = 1;
            spellsKnownForClass["1"] = 1;

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(8));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 3 (class name/1)"));
            Assert.That(spellsKnown[3].Summary, Is.EqualTo("spell 4 (class name/1)"));
            Assert.That(spellsKnown[4].Summary, Is.EqualTo("spell 5 (special domain/1)"));
            Assert.That(spellsKnown[5].Summary, Is.EqualTo("other special domain spell (special domain/1)"));
            Assert.That(spellsKnown[6].Summary, Is.EqualTo("spell 6 (other domain/1)"));
            Assert.That(spellsKnown[7].Summary, Is.EqualTo("other domain spell (other domain/1)"));
        }

        [Test]
        public void GenerateKnown_DivineCastersDoNotKnowProhibitedSpells()
        {
            divineCasters.Add(characterClass.Name);

            characterClass.ProhibitedFields = ["prohibited field"];
            SetUpDomain("prohibited field");
            AddSpell("spell 3", "prohibited field", 1);
            AddSpell("other forbidden spell", "prohibited field", 1);

            spellsKnownForClass["0"] = 1;
            spellsKnownForClass["1"] = 1;

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(3));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 4 (class name/1)"));
        }

        [Test]
        public void GenerateKnown_DivineCastersKnowSpellsEvenIfQuantityForLevelIs0()
        {
            divineCasters.Add(characterClass.Name);

            spellsKnownForClass["0"] = 1;
            spellsKnownForClass["1"] = 0;

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(4));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 3 (class name/1)"));
            Assert.That(spellsKnown[3].Summary, Is.EqualTo("spell 4 (class name/1)"));
        }

        [Test]
        public void GenerateKnown_DivineCastersDoNotKnowSpellsBeyondLevel()
        {
            divineCasters.Add(characterClass.Name);

            spellsKnownForClass["0"] = 1;
            spellsKnownForClass["1"] = 1;

            AddSpell("too high spell", characterClass.Name, 2);

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(4));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 3 (class name/1)"));
            Assert.That(spellsKnown[3].Summary, Is.EqualTo("spell 4 (class name/1)"));
        }

        [Test]
        public void GenerateKnown_DivineCastersDoNotKnowSpecialistSpellsBeyondLevel()
        {
            divineCasters.Add(characterClass.Name);

            characterClass.SpecialistFields = ["special domain"];
            SetUpDomain("special domain");
            AddSpell("domain spell", "special domain", 1);
            AddSpell("too high spell", "special domain", 2);

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(5));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 3 (class name/1)"));
            Assert.That(spellsKnown[3].Summary, Is.EqualTo("spell 4 (class name/1)"));
            Assert.That(spellsKnown[4].Summary, Is.EqualTo("domain spell (special domain/1)"));
        }

        [Test]
        public void GenerateKnown_DivineCastersKnowSpecialistSpellsEvenIfQuantityForLevelIs0()
        {
            divineCasters.Add(characterClass.Name);

            characterClass.SpecialistFields = ["special domain"];
            SetUpDomain("special domain");
            AddSpell("spell 5", "special domain", 1);
            AddSpell("other special domain spell", "special domain", 2);

            spellsKnownForClass["0"] = 1;
            spellsKnownForClass["1"] = 0;

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(5));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 3 (class name/1)"));
            Assert.That(spellsKnown[3].Summary, Is.EqualTo("spell 4 (class name/1)"));
            Assert.That(spellsKnown[4].Summary, Is.EqualTo("spell 5 (special domain/1)"));
        }

        [Test]
        public void GenerateKnown_DivineCastersDoNotKnowSpellsBeyondWhatAbilitiesAllow()
        {
            abilities["stat"].Value = 10;
            divineCasters.Add(characterClass.Name);

            spellsKnownForClass["0"] = 1;
            spellsKnownForClass["1"] = 1;

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(2));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
        }

        [Test]
        public void GenerateKnown_DivineCastersDoNotKnowSpecialistSpellsBeyondWhatAbilitiesAllow()
        {
            abilities["stat"].Value = 10;
            divineCasters.Add(characterClass.Name);

            characterClass.SpecialistFields = ["special domain"];
            SetUpDomain("special domain");
            AddSpell("spell 5", "special domain", 1);
            AddSpell("other special domain spell", "special domain", 2);

            spellsKnownForClass["0"] = 1;
            spellsKnownForClass["1"] = 0;

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(2));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
        }

        [Test]
        public void GenerateKnown_CannotHaveMoreThanNormalKnownSpellQuantity()
        {
            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSKnowsAdditionalSpells, characterClass.Name);
            mockPercentileSelector
                .SetupSequence(s => s.SelectFrom<bool>(Config.Name, tableName))
                .Returns(false)
                .Returns(false);

            spellsKnownForClass["0"] = 1;
            spellsKnownForClass["1"] = 1;

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(2));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 4 (class name/1)"));
        }

        [Test]
        public void GenerateKnown_CanHaveMoreThanNormalKnownSpellQuantity()
        {
            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSKnowsAdditionalSpells, characterClass.Name);
            mockPercentileSelector
                .SetupSequence(s => s.SelectFrom<bool>(Config.Name, tableName))
                .Returns(true).Returns(false)
                .Returns(true).Returns(false);

            AddSpell("spell 5", characterClass.Name, 0);
            AddSpell("spell 6", characterClass.Name, 1);

            spellsKnownForClass["0"] = 1;
            spellsKnownForClass["1"] = 1;

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(4));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 5 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 6 (class name/1)"));
            Assert.That(spellsKnown[3].Summary, Is.EqualTo("spell 4 (class name/1)"));
        }

        [Test]
        public void GenerateKnown_CanHaveManyMoreThanNormalKnownSpellQuantity()
        {
            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSKnowsAdditionalSpells, characterClass.Name);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom<bool>(Config.Name, tableName))
                .Returns(true).Returns(true).Returns(true).Returns(false)
                .Returns(true).Returns(true).Returns(false);

            AddSpell("spell 5", characterClass.Name, 0);
            AddSpell("spell 8", characterClass.Name, 0);
            AddSpell("spell 6", characterClass.Name, 0);
            AddSpell("spell 7", characterClass.Name, 1);
            AddSpell("spell 9", characterClass.Name, 1);

            spellsKnownForClass["0"] = 1;
            spellsKnownForClass["1"] = 1;

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(7));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 5 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 6 (class name/0)"));
            Assert.That(spellsKnown[3].Summary, Is.EqualTo("spell 8 (class name/0)"));
            Assert.That(spellsKnown[4].Summary, Is.EqualTo("spell 3 (class name/1)"));
            Assert.That(spellsKnown[5].Summary, Is.EqualTo("spell 9 (class name/1)"));
            Assert.That(spellsKnown[6].Summary, Is.EqualTo("spell 4 (class name/1)"));
        }

        [Test]
        public void GenerateKnown_GetRandomKnownSpecialistSpells()
        {
            characterClass.SpecialistFields = ["special domain"];
            SetUpDomain("special domain");
            AddSpell("spell 5", characterClass.Name, 1);
            AddSpell("spell 5", "special domain", 1);
            AddSpell("other special domain spell", "special domain", 2);

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(4));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 3 (class name/1)"));
            Assert.That(spellsKnown[3].Summary, Is.EqualTo("spell 5 (special domain/1)"));
        }

        [Test]
        public void GenerateKnown_GetRandomKnownNonSpecialistSpellIfAllSpecialistSpellsAreKnown()
        {
            characterClass.SpecialistFields = ["special domain"];
            SetUpDomain("special domain");
            AddSpell("spell 5", characterClass.Name, 1);
            AddSpell("spell 3", "special domain", 1);
            AddSpell("other special domain spell", "special domain", 2);

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(4));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 3 (class name/1, special domain/1)"));
            Assert.That(spellsKnown[3].Summary, Is.EqualTo("spell 5 (class name/1)"));
        }

        [Test]
        public void GenerateKnown_RandomKnownSpellsAreNotProhibitedSpells()
        {
            characterClass.ProhibitedFields = ["bad domain"];
            SetUpDomain("bad domain");
            AddSpell("spell 3", "bad domain", 1);

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(3));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 4 (class name/1)"));
        }

        [Test]
        public void GenerateKnown_RandomKnownSpellsAreNotAnyProhibitedSpells()
        {
            characterClass.ProhibitedFields = ["bad domain", "domain 3"];
            SetUpDomain("bad domain");
            AddSpell("spell 3", "bad domain", 1);
            SetUpDomain("domain 3");
            AddSpell("spell 1", "domain 3", 1);

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(2));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 2 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 4 (class name/1)"));
        }

        [Test]
        public void GenerateKnown_RandomKnownSpellsAreSpecialistSpellsEvenIfQuantityForLevelIs0()
        {
            characterClass.SpecialistFields = ["special domain"];
            SetUpDomain("special domain");
            AddSpell("spell 5", "special domain", 1);
            AddSpell("other special domain spell", "special domain", 2);

            spellsKnownForClass["1"] = 0;

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(3));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 5 (special domain/1)"));
        }

        [Test]
        public void GenerateKnown_RandomKnownSpellsAreNotBeyondWhatAbilitiesAllow()
        {
            abilities["stat"].Value = 10;

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(2));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
        }

        [Test]
        public void GenerateKnown_RandomKnownSpellsAreNotSpecialistSpellsBeyondWhatAbilitiesAllow()
        {
            abilities["stat"].Value = 10;
            characterClass.SpecialistFields = ["domain 2"];

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(2));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
        }

        [Test]
        public void GenerateKnown_CannotHaveDuplicateKnownSpells()
        {
            var repeat = true;
            var index = 0;
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<string>>()))
                .Returns((IEnumerable<string> c) =>
                {
                    var item = c.ElementAt(index % c.Count());
                    if (!repeat)
                        index++;

                    repeat = !repeat;
                    return item;
                });

            var spellsKnown = spellsGenerator.GenerateKnown(characterClass, abilities).ToList();
            Assert.That(spellsKnown.Count, Is.EqualTo(3));
            Assert.That(spellsKnown[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsKnown[1].Summary, Is.EqualTo("spell 2 (class name/0)"));
            Assert.That(spellsKnown[2].Summary, Is.EqualTo("spell 3 (class name/1)"));
        }

        [Test]
        public void GeneratePrepared_DoNotGeneratePreparedSpellsIfNotASpellcaster()
        {
            spellcasters.Remove(characterClass.Name);

            var knownSpells = new List<Spell>
            {
                new() { Name = spells[characterClass.Name]["0"][0], Sources = { ["source"] = 0 } },
                new() { Name = spells[characterClass.Name]["0"][1], Sources = { ["source"] = 0 } },
                new() { Name = "other cantrip", Sources = { ["source"] = 0 } },
                new() { Name = spells[characterClass.Name]["1"][0], Sources = { ["source"] = 1 } },
                new() { Name = "other spell", Sources = { ["source"] = 1 } }
            };

            var spellsPerDay = new List<SpellQuantity>
            {
                new() { Level = 0, Quantity = 2, Source = "source" },
                new() { Level = 1, Quantity = 1, Source = "source" }
            };

            var preparedSpells = spellsGenerator.GeneratePrepared(characterClass, knownSpells, spellsPerDay);
            Assert.That(preparedSpells, Is.Empty);
        }

        [Test]
        public void GeneratePrepared_GetRandomPreparedSpells()
        {
            var knownSpells = new List<Spell>
            {
                new() { Name = spells[characterClass.Name]["0"][0], Sources = { ["source"] = 0 } },
                new() { Name = spells[characterClass.Name]["0"][1], Sources = { ["source"] = 0 } },
                new() { Name = "other cantrip", Sources = { ["source"] = 0 } },
                new() { Name = spells[characterClass.Name]["1"][0], Sources = { ["source"] = 1 } },
                new() { Name = "other spell", Sources = { ["source"] = 1 } }
            };

            var spellsPerDay = new List<SpellQuantity>
            {
                new() { Level = 0, Quantity = 2, Source = "source" },
                new() { Level = 1, Quantity = 1, Source = "source" }
            };

            var spellsPrepared = spellsGenerator.GeneratePrepared(characterClass, knownSpells, spellsPerDay).ToList();
            Assert.That(spellsPrepared.Count, Is.EqualTo(3));
            Assert.That(spellsPrepared[0].Summary, Is.EqualTo("spell 1 (source/0)"));
            Assert.That(spellsPrepared[1].Summary, Is.EqualTo("spell 2 (source/0)"));
            Assert.That(spellsPrepared[2].Summary, Is.EqualTo("spell 3 (source/1)"));
        }

        [Test]
        public void GeneratePrepared_GetRandomPreparedSpecialistSpells()
        {
            characterClass.SpecialistFields = ["domain 2"];
            AddSpell("other spell", "domain 2", 1);

            var knownSpells = new List<Spell>
            {
                new() { Name = spells[characterClass.Name]["0"][0], Sources = { [characterClass.Name] = 0 } },
                new() { Name = "wrong spell", Sources = { ["my other source"] = 0 } },
                new() { Name = spells[characterClass.Name]["0"][1], Sources = { [characterClass.Name] = 0, ["domain 2"] = 0 } },
                new() { Name = "other cantrip", Sources = { [characterClass.Name] = 0 } },
                new() { Name = spells[characterClass.Name]["1"][0], Sources = { [characterClass.Name] = 1 } },
                new() { Name = "other spell", Sources = { ["domain 2"] = 1 } }
            };

            var spellsPerDay = new List<SpellQuantity>
            {
                new() { Level = 0, Quantity = 2, Source = characterClass.Name },
                new() { Level = 1, Quantity = 1, Source = characterClass.Name, HasDomainSpell = true }
            };

            var spellsPrepared = spellsGenerator.GeneratePrepared(characterClass, knownSpells, spellsPerDay).ToList();
            Assert.That(spellsPrepared.Count, Is.EqualTo(4));
            Assert.That(spellsPrepared[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsPrepared[1].Summary, Is.EqualTo("spell 2 (class name/0, domain 2/0)"));
            Assert.That(spellsPrepared[2].Summary, Is.EqualTo("spell 3 (class name/1)"));
            Assert.That(spellsPrepared[3].Summary, Is.EqualTo("other spell (domain 2/1)"));
        }

        [Test]
        public void GeneratePrepared_GetRandomPreparedSpecialistSpells_FromOnly1Domain()
        {
            characterClass.SpecialistFields = ["domain 1", "domain 2"];
            AddSpell("domain 1 spell", "domain 1", 1);
            AddSpell("domain 2 spell", "domain 2", 1);

            var knownSpells = new List<Spell>
            {
                new() { Name = spells[characterClass.Name]["0"][0], Sources = { [characterClass.Name] = 0 } },
                new() { Name = "wrong spell", Sources = { ["my other source"] = 0 } },
                new() { Name = spells[characterClass.Name]["0"][1], Sources = { [characterClass.Name] = 0, ["domain 2"] = 0 }  },
                new() { Name = "other cantrip", Sources = { [characterClass.Name] = 0 } },
                new() { Name = spells[characterClass.Name]["1"][0], Sources = { [characterClass.Name] = 1 } },
                new() { Name = "domain 1 spell", Sources = { ["domain 1"] = 1 } },
                new() { Name = spells[characterClass.Name]["1"][1],Sources = { [characterClass.Name] = 1 } },
                new() { Name = "domain 2 spell", Sources = { ["domain 2"] = 1 }  },
            };

            var spellsPerDay = new List<SpellQuantity>
            {
                new() { Level = 0, Quantity = 2, Source = characterClass.Name },
                new() { Level = 1, Quantity = 1, Source = characterClass.Name, HasDomainSpell = true }
            };

            var spellsPrepared = spellsGenerator.GeneratePrepared(characterClass, knownSpells, spellsPerDay).ToList();
            Assert.That(spellsPrepared.Count, Is.EqualTo(4));
            Assert.That(spellsPrepared[0].Summary, Is.EqualTo("spell 1 (class name/0)"));
            Assert.That(spellsPrepared[1].Summary, Is.EqualTo("spell 2 (class name/0, domain 2/0)"));
            Assert.That(spellsPrepared[2].Summary, Is.EqualTo("spell 3 (class name/1)"));
            Assert.That(spellsPrepared[3].Summary, Is.EqualTo("domain 2 spell (domain 2/1)"));
        }

        [Test]
        public void GeneratePrepared_CanHaveDuplicatePreparedSpells()
        {
            var knownSpells = new List<Spell>
            {
                new() { Name = spells[characterClass.Name]["0"][0], Sources = { ["my source"] = 0 } },
                new() { Name = "wrong spell", Sources = { ["my other source"] = 0 } },
                new() { Name = spells[characterClass.Name]["0"][1], Sources = { ["my source"] = 0, ["domain 2"] = 0 }  },
                new() { Name = "other cantrip", Sources = { ["my source"] = 0 } },
                new() { Name = spells[characterClass.Name]["1"][0], Sources = { ["my source"] = 1 } },
                new() { Name = "other spell", Sources = { ["my source"] = 1 } }
            };

            var spellsPerDay = new List<SpellQuantity>
            {
                new() { Level = 0, Quantity = 2, Source = "my source" },
                new() { Level = 1, Quantity = 1, Source = "my source" }
            };

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<Spell>>())).Returns((IEnumerable<Spell> c) => c.First());

            var spellsPrepared = spellsGenerator.GeneratePrepared(characterClass, knownSpells, spellsPerDay).ToList();
            Assert.That(spellsPrepared.Count, Is.EqualTo(3));
            Assert.That(spellsPrepared[0].Summary, Is.EqualTo("spell 1 (my source/0)"));
            Assert.That(spellsPrepared[1].Summary, Is.EqualTo("spell 1 (my source/0)"));
            Assert.That(spellsPrepared[2].Summary, Is.EqualTo("spell 3 (my source/1)"));
        }
    }
}
