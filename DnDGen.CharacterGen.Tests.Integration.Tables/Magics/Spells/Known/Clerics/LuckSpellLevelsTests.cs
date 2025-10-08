using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class LuckSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Luck);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.EntropicShield)]
        [TestCase("2", SpellConstants.Aid)]
        [TestCase("3", SpellConstants.ProtectionFromEnergy)]
        [TestCase("4", SpellConstants.FreedomOfMovement)]
        [TestCase("5", SpellConstants.BreakEnchantment)]
        [TestCase("6", SpellConstants.Mislead)]
        [TestCase("7", SpellConstants.SpellTurning)]
        [TestCase("8", SpellConstants.MomentOfPrescience)]
        [TestCase("9", SpellConstants.Miracle)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
