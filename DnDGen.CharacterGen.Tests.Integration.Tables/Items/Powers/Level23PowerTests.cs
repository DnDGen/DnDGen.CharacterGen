using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using DnDGen.TreasureGen.Items;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Items.Powers
{
    [TestFixture]
    public class Level23PowerTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Formattable.Percentile.LevelXPower(23); }
        }

        [TestCase(1, 28, PowerConstants.Medium)]
        [TestCase(29, 100, PowerConstants.Major)]
        public override void Percentile(int lower, int upper, string content)
        {
            base.Percentile(lower, upper, content);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}
