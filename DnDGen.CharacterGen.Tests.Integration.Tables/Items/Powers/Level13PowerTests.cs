using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using DnDGen.TreasureGen.Items;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Items.Powers
{
    [TestFixture]
    public class Level13PowerTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Formattable.Percentile.LevelXPower(13); }
        }

        [TestCase(1, 19, PowerConstants.Mundane)]
        [TestCase(20, 73, PowerConstants.Minor)]
        [TestCase(74, 95, PowerConstants.Medium)]
        [TestCase(96, 100, PowerConstants.Major)]
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
