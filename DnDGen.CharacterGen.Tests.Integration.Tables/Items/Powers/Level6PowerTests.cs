using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using DnDGen.TreasureGen.Items;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Items.Powers
{
    [TestFixture]
    public class Level6PowerTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Formattable.Percentile.LevelXPower(6); }
        }

        [TestCase(1, 59, PowerConstants.Mundane)]
        [TestCase(60, 99, PowerConstants.Minor)]
        [TestCase(100, 100, PowerConstants.Medium)]
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
