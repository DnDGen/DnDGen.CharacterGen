using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class AnimalSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Animal);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.CalmAnimals)]
        [TestCase("2", SpellConstants.HoldAnimal)]
        [TestCase("3", SpellConstants.DominateAnimal)]
        [TestCase("4", SpellConstants.SummonNaturesAllyIV)]
        [TestCase("5", SpellConstants.CommuneWithNature)]
        [TestCase("6", SpellConstants.AntilifeShell)]
        [TestCase("7", SpellConstants.AnimalShapes)]
        [TestCase("8", SpellConstants.SummonNaturesAllyVIII)]
        [TestCase("9", SpellConstants.Shapechange)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
