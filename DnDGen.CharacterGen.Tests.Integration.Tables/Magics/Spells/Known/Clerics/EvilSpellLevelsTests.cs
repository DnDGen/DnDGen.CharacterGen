using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class EvilSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Evil);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.ProtectionFromGood)]
        [TestCase("2", SpellConstants.Desecrate)]
        [TestCase("3", SpellConstants.MagicCircleAgainstGood)]
        [TestCase("4", SpellConstants.UnholyBlight)]
        [TestCase("5", SpellConstants.DispelGood)]
        [TestCase("6", SpellConstants.CreateUndead)]
        [TestCase("7", SpellConstants.Blasphemy)]
        [TestCase("8", SpellConstants.UnholyAura)]
        [TestCase("9", SpellConstants.SummonMonsterIX)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
