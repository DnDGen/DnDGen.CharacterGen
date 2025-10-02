using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Abilities
{
    [TestFixture]
    public class AbilityGroupsTests : CollectionTests
    {
        protected override string tableName
        {
            get
            {
                return TableNameConstants.Set.Collection.AbilityGroups;
            }
        }

        public override void CollectionNames()
        {
            var names = new[]
            {
                CharacterClassConstants.Bard + GroupConstants.Spellcasters,
                CharacterClassConstants.Cleric + GroupConstants.Spellcasters,
                CharacterClassConstants.Druid + GroupConstants.Spellcasters,
                CharacterClassConstants.Paladin + GroupConstants.Spellcasters,
                CharacterClassConstants.Ranger + GroupConstants.Spellcasters,
                CharacterClassConstants.Sorcerer + GroupConstants.Spellcasters,
                CharacterClassConstants.Wizard + GroupConstants.Spellcasters,
                CharacterClassConstants.Adept + GroupConstants.Spellcasters,
                GroupConstants.All
            };

            AssertCollectionNames(names);
        }

        [TestCase(CharacterClassConstants.Bard + GroupConstants.Spellcasters, AbilityConstants.Charisma)]
        [TestCase(CharacterClassConstants.Cleric + GroupConstants.Spellcasters, AbilityConstants.Wisdom)]
        [TestCase(CharacterClassConstants.Druid + GroupConstants.Spellcasters, AbilityConstants.Wisdom)]
        [TestCase(CharacterClassConstants.Paladin + GroupConstants.Spellcasters, AbilityConstants.Wisdom)]
        [TestCase(CharacterClassConstants.Ranger + GroupConstants.Spellcasters, AbilityConstants.Wisdom)]
        [TestCase(CharacterClassConstants.Sorcerer + GroupConstants.Spellcasters, AbilityConstants.Charisma)]
        [TestCase(CharacterClassConstants.Wizard + GroupConstants.Spellcasters, AbilityConstants.Intelligence)]
        [TestCase(CharacterClassConstants.Adept + GroupConstants.Spellcasters, AbilityConstants.Wisdom)]
        [TestCase(GroupConstants.All,
            AbilityConstants.Charisma,
            AbilityConstants.Constitution,
            AbilityConstants.Dexterity,
            AbilityConstants.Intelligence,
            AbilityConstants.Strength,
            AbilityConstants.Wisdom)]
        public void AbilityGroup(string name, params string[] abilities)
        {
            base.AssertCollection(name, abilities);
        }
    }
}
