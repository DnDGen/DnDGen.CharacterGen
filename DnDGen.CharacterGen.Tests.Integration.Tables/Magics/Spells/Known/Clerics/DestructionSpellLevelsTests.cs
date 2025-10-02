using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class DestructionSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Destruction);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.InflictLightWounds)]
        [TestCase("2", SpellConstants.Shatter)]
        [TestCase("3", SpellConstants.Contagion)]
        [TestCase("4", SpellConstants.InflictCriticalWounds)]
        [TestCase("5", SpellConstants.InflictLightWounds_Mass)]
        [TestCase("6", SpellConstants.Harm)]
        [TestCase("7", SpellConstants.Disintegrate)]
        [TestCase("8", SpellConstants.Earthquake)]
        [TestCase("9", SpellConstants.Implosion)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
