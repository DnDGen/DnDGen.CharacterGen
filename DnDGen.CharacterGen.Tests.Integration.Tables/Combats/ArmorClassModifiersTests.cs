using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Feats;
using NUnit.Framework;
using DnDGen.TreasureGen.Items.Magical;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Combats
{
    [TestFixture]
    public class ArmorClassModifiersTests : CollectionTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Set.Collection.ArmorClassModifiers; }
        }

        [Test]
        public override void CollectionNames()
        {
            var names = new[]
            {
                GroupConstants.Deflection,
                GroupConstants.NaturalArmor,
                GroupConstants.DodgeBonus,
                GroupConstants.ArmorBonus
            };

            AssertCollectionNames(names);
        }

        [TestCase(GroupConstants.Deflection,
            RingConstants.Protection)]
        [TestCase(GroupConstants.NaturalArmor,
            FeatConstants.NaturalArmor,
            FeatConstants.ArmorBonus,
            WondrousItemConstants.AmuletOfNaturalArmor)]
        [TestCase(GroupConstants.DodgeBonus,
            FeatConstants.DodgeBonus,
            FeatConstants.Dodge)]
        [TestCase(GroupConstants.ArmorBonus,
            FeatConstants.InertialArmor,
            WondrousItemConstants.BracersOfArmor)]
        public void ArmorClassModifier(string name, params string[] collection)
        {
            base.AssertDistinctCollection(name, collection);
        }
    }
}
