using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class SunSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Sun);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.EndureElements)]
        [TestCase("2", SpellConstants.HeatMetal)]
        [TestCase("3", SpellConstants.SearingLight)]
        [TestCase("4", SpellConstants.FireShield)]
        [TestCase("5", SpellConstants.FlameStrike)]
        [TestCase("6", SpellConstants.FireSeeds)]
        [TestCase("7", SpellConstants.Sunbeam)]
        [TestCase("8", SpellConstants.Sunburst)]
        [TestCase("9", SpellConstants.PrismaticSphere)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
