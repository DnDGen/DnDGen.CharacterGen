using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class EarthSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Earth);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.MagicStone)]
        [TestCase("2", SpellConstants.SoftenEarthAndStone)]
        [TestCase("3", SpellConstants.StoneShape)]
        [TestCase("4", SpellConstants.SpikeStones)]
        [TestCase("5", SpellConstants.WallOfStone)]
        [TestCase("6", SpellConstants.Stoneskin)]
        [TestCase("7", SpellConstants.Earthquake)]
        [TestCase("8", SpellConstants.IronBody)]
        [TestCase("9", SpellConstants.ElementalSwarm)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
