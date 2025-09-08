using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using DnDGen.TreasureGen.Items;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Items.Powers
{
    [TestFixture]
    public class Level15PowerTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Formattable.Percentile.LevelXPower(15); }
        }

        [TestCase(1, 11, PowerConstants.Mundane)]
        [TestCase(12, 46, PowerConstants.Minor)]
        [TestCase(47, 90, PowerConstants.Medium)]
        [TestCase(91, 100, PowerConstants.Major)]
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
