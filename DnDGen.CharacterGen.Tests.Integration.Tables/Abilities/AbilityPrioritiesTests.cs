using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Abilities
{
    [TestFixture]
    public class AbilityPrioritiesTests : CollectionTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Set.Collection.AbilityPriorities; }
        }

        [Test]
        public override void CollectionNames()
        {
            var classGroups = collectionsMapper.Map(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups);
            var names = classGroups[GroupConstants.All];

            AssertCollectionNames(names);
        }

        [TestCase(CharacterClassConstants.Adept,
            AbilityConstants.Wisdom)]
        [TestCase(CharacterClassConstants.Aristocrat)]
        [TestCase(CharacterClassConstants.Barbarian,
            AbilityConstants.Strength,
            AbilityConstants.Dexterity,
            AbilityConstants.Constitution)]
        [TestCase(CharacterClassConstants.Bard,
            AbilityConstants.Charisma,
            AbilityConstants.Intelligence,
            AbilityConstants.Constitution)]
        [TestCase(CharacterClassConstants.Cleric,
            AbilityConstants.Wisdom,
            AbilityConstants.Charisma,
            AbilityConstants.Constitution)]
        [TestCase(CharacterClassConstants.Commoner)]
        [TestCase(CharacterClassConstants.Druid,
            AbilityConstants.Wisdom,
            AbilityConstants.Constitution)]
        [TestCase(CharacterClassConstants.Expert,
            AbilityConstants.Intelligence)]
        [TestCase(CharacterClassConstants.Fighter,
            AbilityConstants.Strength,
            AbilityConstants.Constitution)]
        [TestCase(CharacterClassConstants.Monk,
            AbilityConstants.Wisdom,
            AbilityConstants.Strength,
            AbilityConstants.Dexterity,
            AbilityConstants.Constitution)]
        [TestCase(CharacterClassConstants.Paladin,
            AbilityConstants.Charisma,
            AbilityConstants.Wisdom,
            AbilityConstants.Strength,
            AbilityConstants.Constitution)]
        [TestCase(CharacterClassConstants.Ranger,
            AbilityConstants.Strength,
            AbilityConstants.Wisdom,
            AbilityConstants.Dexterity,
            AbilityConstants.Constitution)]
        [TestCase(CharacterClassConstants.Rogue,
            AbilityConstants.Dexterity,
            AbilityConstants.Intelligence,
            AbilityConstants.Constitution)]
        [TestCase(CharacterClassConstants.Sorcerer,
            AbilityConstants.Charisma,
            AbilityConstants.Dexterity,
            AbilityConstants.Constitution)]
        [TestCase(CharacterClassConstants.Warrior,
            AbilityConstants.Strength,
            AbilityConstants.Constitution)]
        [TestCase(CharacterClassConstants.Wizard,
            AbilityConstants.Intelligence,
            AbilityConstants.Dexterity,
            AbilityConstants.Constitution)]
        public void AbilityPriorities(string name, params string[] items)
        {
            base.AssertOrderedCollection(name, items);
        }
    }
}