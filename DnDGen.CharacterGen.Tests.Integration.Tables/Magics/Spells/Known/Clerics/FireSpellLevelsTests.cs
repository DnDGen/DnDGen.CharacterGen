using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Magics;
using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Magics.Spells.Known.Clerics
{
    [TestFixture]
    public class FireSpellLevelsTests : CollectionTests
    {
        protected override string tableName => string.Format(TableNameConstants.Formattable.Collection.CLASSSpellLevels, CharacterClassConstants.Domains.Fire);

        [Test]
        public override void CollectionNames()
        {
            var names = Enumerable.Range(0, 10).Select(n => n.ToString());
            AssertCollectionNames(names);
        }

        [TestCase("0")]
        [TestCase("1", SpellConstants.BurningHands)]
        [TestCase("2", SpellConstants.ProduceFlame)]
        [TestCase("3", SpellConstants.ResistEnergy)]
        [TestCase("4", SpellConstants.WallOfFire)]
        [TestCase("5", SpellConstants.FireShield)]
        [TestCase("6", SpellConstants.FireSeeds)]
        [TestCase("7", SpellConstants.FireStorm)]
        [TestCase("8", SpellConstants.IncendiaryCloud)]
        [TestCase("9", SpellConstants.ElementalSwarm)]
        public override void AssertCollection(string name, params string[] collection)
        {
            base.AssertCollection(name, collection);
        }
    }
}
