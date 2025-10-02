using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class DeathSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Death);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.CauseFear)]
        [TestCase("2", SpellConstants.DeathKnell)]
        [TestCase("3", SpellConstants.AnimateDead)]
        [TestCase("4", SpellConstants.DeathWard)]
        [TestCase("5", SpellConstants.SlayLiving)]
        [TestCase("6", SpellConstants.CreateUndead)]
        [TestCase("7", SpellConstants.Destruction)]
        [TestCase("8", SpellConstants.CreateGreaterUndead)]
        [TestCase("9", SpellConstants.WailOfTheBanshee)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
