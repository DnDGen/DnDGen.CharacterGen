using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class WaterSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Water);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.ObscuringMist)]
        [TestCase("2", SpellConstants.FogCloud)]
        [TestCase("3", SpellConstants.WaterBreathing)]
        [TestCase("4", SpellConstants.ControlWater)]
        [TestCase("5", SpellConstants.IceStorm)]
        [TestCase("6", SpellConstants.ConeOfCold)]
        [TestCase("7", SpellConstants.AcidFog)]
        [TestCase("8", SpellConstants.HorridWilting)]
        [TestCase("9", SpellConstants.ElementalSwarm)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
