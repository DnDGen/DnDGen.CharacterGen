using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class GoodSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Good);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.ProtectionFromEvil)]
        [TestCase("2", SpellConstants.Aid)]
        [TestCase("3", SpellConstants.MagicCircleAgainstEvil)]
        [TestCase("4", SpellConstants.HolySmite)]
        [TestCase("5", SpellConstants.DispelEvil)]
        [TestCase("6", SpellConstants.BladeBarrier)]
        [TestCase("7", SpellConstants.HolyWord)]
        [TestCase("8", SpellConstants.HolyAura)]
        [TestCase("9", SpellConstants.SummonMonsterIX)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
