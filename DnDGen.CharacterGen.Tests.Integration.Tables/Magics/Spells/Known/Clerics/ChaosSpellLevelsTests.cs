using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class ChaosSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Chaos);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.ProtectionFromLaw)]
        [TestCase("2", SpellConstants.Shatter)]
        [TestCase("3", SpellConstants.MagicCircleAgainstLaw)]
        [TestCase("4", SpellConstants.ChaosHammer)]
        [TestCase("5", SpellConstants.DispelLaw)]
        [TestCase("6", SpellConstants.AnimateObjects)]
        [TestCase("7", SpellConstants.WordOfChaos)]
        [TestCase("8", SpellConstants.CloakOfChaos)]
        [TestCase("9", SpellConstants.SummonMonsterIX)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
