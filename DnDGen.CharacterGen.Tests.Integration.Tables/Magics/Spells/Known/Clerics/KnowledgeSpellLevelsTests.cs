using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class KnowledgeSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Knowledge);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.DetectSecretDoors)]
        [TestCase("2", SpellConstants.DetectThoughts)]
        [TestCase("3", SpellConstants.ClairaudienceClairvoyance)]
        [TestCase("4", SpellConstants.Divination)]
        [TestCase("5", SpellConstants.TrueSeeing)]
        [TestCase("6", SpellConstants.FindThePath)]
        [TestCase("7", SpellConstants.LegendLore)]
        [TestCase("8", SpellConstants.DiscernLocation)]
        [TestCase("9", SpellConstants.Foresight)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
