using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class AirSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Air);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.ObscuringMist)]
        [TestCase("2", SpellConstants.WindWall)]
        [TestCase("3", SpellConstants.GaseousForm)]
        [TestCase("4", SpellConstants.AirWalk)]
        [TestCase("5", SpellConstants.ControlWinds)]
        [TestCase("6", SpellConstants.ChainLightning)]
        [TestCase("7", SpellConstants.ControlWeather)]
        [TestCase("8", SpellConstants.Whirlwind)]
        [TestCase("9", SpellConstants.ElementalSwarm)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
