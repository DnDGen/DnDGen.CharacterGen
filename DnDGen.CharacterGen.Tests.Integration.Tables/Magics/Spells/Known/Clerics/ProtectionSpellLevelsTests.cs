using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class ProtectionSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Protection);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.Sanctuary)]
        [TestCase("2", SpellConstants.ShieldOther)]
        [TestCase("3", SpellConstants.ProtectionFromEnergy)]
        [TestCase("4", SpellConstants.SpellImmunity)]
        [TestCase("5", SpellConstants.SpellResistance)]
        [TestCase("6", SpellConstants.AntimagicField)]
        [TestCase("7", SpellConstants.Repulsion)]
        [TestCase("8", SpellConstants.MindBlank)]
        [TestCase("9", SpellConstants.PrismaticSphere)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
