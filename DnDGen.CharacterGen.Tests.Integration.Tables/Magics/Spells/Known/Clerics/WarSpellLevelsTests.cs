using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class WarSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.War);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.MagicWeapon)]
        [TestCase("2", SpellConstants.SpiritualWeapon)]
        [TestCase("3", SpellConstants.MagicVestment)]
        [TestCase("4", SpellConstants.DivinePower)]
        [TestCase("5", SpellConstants.FlameStrike)]
        [TestCase("6", SpellConstants.BladeBarrier)]
        [TestCase("7", SpellConstants.PowerWordBlind)]
        [TestCase("8", SpellConstants.PowerWordStun)]
        [TestCase("9", SpellConstants.PowerWordKill)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
