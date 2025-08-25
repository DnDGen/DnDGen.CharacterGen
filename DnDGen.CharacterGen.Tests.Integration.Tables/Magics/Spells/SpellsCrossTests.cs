using DnDGen.CharacterGen.Selectors;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells
{
    [TestFixture]
    public class SpellsCrossTests : TableTests
    {
        protected override string tableName
        {
            get
            {
                return TableNameConstants.Set.Collection.ClassNameGroups;
            }
        }

        private ICollectionSelector collectionsSelector;
        private IAdjustmentsSelector adjustmentsSelector;

        [SetUp]
        public void Setup()
        {
            collectionsSelector = GetNewInstanceOf<ICollectionSelector>();
            adjustmentsSelector = GetNewInstanceOf<IAdjustmentsSelector>();
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
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        [TestCase(16)]
        [TestCase(17)]
        [TestCase(18)]
        [TestCase(19)]
        [TestCase(20)]
        public void AllSpellcastersHaveSpellsPerDayAtLevel(int level)
        {
            //INFO: We are testing up to level 30 to account for Rakshasas, who might have sorcerer spells up to level 27
            var spellcasters = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.Spellcasters);

            foreach (var spellcaster in spellcasters)
            {
                var tableName = string.Format(TableNameConstants.Formattable.Adjustments.LevelXCLASSSpellsPerDay, level, spellcaster);
                var spellsPerDay = adjustmentsSelector.SelectAllFrom(tableName);
                Assert.That(spellsPerDay, Is.Not.Null, spellcaster);
            }
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
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        [TestCase(16)]
        [TestCase(17)]
        [TestCase(18)]
        [TestCase(19)]
        [TestCase(20)]
        public void AllSpellcastersHaveKnownSpellsAtLevel(int level)
        {
            //INFO: We are testing up to level 30 to account for Rakshasas, who might have sorcerer spells up to level 27
            var spellcasters = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.Spellcasters);

            foreach (var spellcaster in spellcasters)
            {
                var tableName = string.Format(TableNameConstants.Formattable.Adjustments.LevelXCLASSKnownSpells, level, spellcaster);
                var knownSpells = adjustmentsSelector.SelectAllFrom(tableName);
                Assert.That(knownSpells, Is.Not.Null, spellcaster);
            }
        }
    }
}
