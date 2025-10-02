using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.CharacterClasses
{
    [TestFixture]
    public class ProhibitedFieldsTests : CollectionTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Set.Collection.ProhibitedFields; }
        }

        [Test]
        public override void CollectionNames()
        {
            var names = new[]
            {
                CharacterClassConstants.Barbarian,
                CharacterClassConstants.Bard,
                CharacterClassConstants.Cleric,
                CharacterClassConstants.Druid,
                CharacterClassConstants.Fighter,
                CharacterClassConstants.Monk,
                CharacterClassConstants.Paladin,
                CharacterClassConstants.Ranger,
                CharacterClassConstants.Rogue,
                CharacterClassConstants.Sorcerer,
                CharacterClassConstants.Wizard,
                AlignmentConstants.LawfulGood,
                AlignmentConstants.NeutralGood,
                AlignmentConstants.ChaoticGood,
                AlignmentConstants.LawfulNeutral,
                AlignmentConstants.TrueNeutral,
                AlignmentConstants.ChaoticNeutral,
                AlignmentConstants.LawfulEvil,
                AlignmentConstants.NeutralEvil,
                AlignmentConstants.ChaoticEvil
            };

            AssertCollectionNames(names);
        }

        [TestCase(CharacterClassConstants.Barbarian)]
        [TestCase(CharacterClassConstants.Bard)]
        [TestCase(CharacterClassConstants.Cleric)]
        [TestCase(CharacterClassConstants.Druid)]
        [TestCase(CharacterClassConstants.Fighter)]
        [TestCase(CharacterClassConstants.Monk)]
        [TestCase(CharacterClassConstants.Paladin)]
        [TestCase(CharacterClassConstants.Ranger)]
        [TestCase(CharacterClassConstants.Rogue)]
        [TestCase(CharacterClassConstants.Sorcerer)]
        [TestCase(CharacterClassConstants.Wizard,
            CharacterClassConstants.Schools.Abjuration,
            CharacterClassConstants.Schools.Conjuration,
            CharacterClassConstants.Schools.Enchantment,
            CharacterClassConstants.Schools.Evocation,
            CharacterClassConstants.Schools.Illusion,
            CharacterClassConstants.Schools.Necromancy,
            CharacterClassConstants.Schools.Transmutation)]
        [TestCase(AlignmentConstants.LawfulGood,
            CharacterClassConstants.Domains.Chaos,
            CharacterClassConstants.Domains.Evil)]
        [TestCase(AlignmentConstants.NeutralGood,
            CharacterClassConstants.Domains.Evil)]
        [TestCase(AlignmentConstants.ChaoticGood,
            CharacterClassConstants.Domains.Law,
            CharacterClassConstants.Domains.Evil)]
        [TestCase(AlignmentConstants.LawfulNeutral,
            CharacterClassConstants.Domains.Chaos)]
        [TestCase(AlignmentConstants.TrueNeutral)]
        [TestCase(AlignmentConstants.ChaoticNeutral,
            CharacterClassConstants.Domains.Law)]
        [TestCase(AlignmentConstants.LawfulEvil,
            CharacterClassConstants.Domains.Chaos,
            CharacterClassConstants.Domains.Good)]
        [TestCase(AlignmentConstants.NeutralEvil,
            CharacterClassConstants.Domains.Good)]
        [TestCase(AlignmentConstants.ChaoticEvil,
            CharacterClassConstants.Domains.Law,
            CharacterClassConstants.Domains.Good)]
        public override void AssertDistinctCollection(string name, params string[] collection)
        {
            base.AssertDistinctCollection(name, collection);
        }
    }
}