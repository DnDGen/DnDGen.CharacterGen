using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class StrengthSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Strength);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.EnlargePerson)]
        [TestCase("2", SpellConstants.BullsStrength)]
        [TestCase("3", SpellConstants.MagicVestment)]
        [TestCase("4", SpellConstants.SpellImmunity)]
        [TestCase("5", SpellConstants.RighteousMight)]
        [TestCase("6", SpellConstants.Stoneskin)]
        [TestCase("7", SpellConstants.BigbysGraspingHand)]
        [TestCase("8", SpellConstants.BigbysClenchedFist)]
        [TestCase("9", SpellConstants.BigbysCrushingHand)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
