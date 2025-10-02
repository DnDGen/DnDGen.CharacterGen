using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class LawSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Law);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.ProtectionFromChaos)]
        [TestCase("2", SpellConstants.CalmEmotions)]
        [TestCase("3", SpellConstants.MagicCircleAgainstChaos)]
        [TestCase("4", SpellConstants.OrdersWrath)]
        [TestCase("5", SpellConstants.DispelChaos)]
        [TestCase("6", SpellConstants.HoldMonster)]
        [TestCase("7", SpellConstants.Dictum)]
        [TestCase("8", SpellConstants.ShieldOfLaw)]
        [TestCase("9", SpellConstants.SummonMonsterIX)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
