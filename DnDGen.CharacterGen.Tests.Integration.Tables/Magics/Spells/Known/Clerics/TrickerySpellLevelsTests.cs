using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class TrickerySpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Trickery);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.DisguiseSelf)]
        [TestCase("2", SpellConstants.Invisibility)]
        [TestCase("3", SpellConstants.Nondetection)]
        [TestCase("4", SpellConstants.Confusion)]
        [TestCase("5", SpellConstants.FalseVision)]
        [TestCase("6", SpellConstants.Mislead)]
        [TestCase("7", SpellConstants.Screen)]
        [TestCase("8", SpellConstants.PolymorphAnyObject)]
        [TestCase("9", SpellConstants.TimeStop)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
