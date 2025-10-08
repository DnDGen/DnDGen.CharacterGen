using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class PlantSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Plant);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.Entangle)]
        [TestCase("2", SpellConstants.Barkskin)]
        [TestCase("3", SpellConstants.PlantGrowth)]
        [TestCase("4", SpellConstants.CommandPlants)]
        [TestCase("5", SpellConstants.WallOfThorns)]
        [TestCase("6", SpellConstants.RepelWood)]
        [TestCase("7", SpellConstants.AnimatePlants)]
        [TestCase("8", SpellConstants.ControlPlants)]
        [TestCase("9", SpellConstants.Shambler)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
