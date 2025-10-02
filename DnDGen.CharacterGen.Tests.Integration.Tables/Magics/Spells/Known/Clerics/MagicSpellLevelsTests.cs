using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class MagicSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Magic);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.NystulsMagicAura)]
        [TestCase("2", SpellConstants.Identify)]
        [TestCase("3", SpellConstants.DispelMagic)]
        [TestCase("4", SpellConstants.ImbueWithSpellAbility)]
        [TestCase("5", SpellConstants.SpellResistance)]
        [TestCase("6", SpellConstants.AntimagicField)]
        [TestCase("7", SpellConstants.SpellTurning)]
        [TestCase("8", SpellConstants.ProtectionFromSpells)]
        [TestCase("9", SpellConstants.MordenkainensDisjunction)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
