using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class HealingSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Healing);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.CureLightWounds)]
        [TestCase("2", SpellConstants.CureModerateWounds)]
        [TestCase("3", SpellConstants.CureSeriousWounds)]
        [TestCase("4", SpellConstants.CureCriticalWounds)]
        [TestCase("5", SpellConstants.CureLightWounds_Mass)]
        [TestCase("6", SpellConstants.Heal)]
        [TestCase("7", SpellConstants.Regenerate)]
        [TestCase("8", SpellConstants.CureCriticalWounds_Mass)]
        [TestCase("9", SpellConstants.Heal_Mass)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
