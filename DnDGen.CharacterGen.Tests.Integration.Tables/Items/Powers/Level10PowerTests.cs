using DnDGen.CharacterGen.Tables;
using NUnit.Framework;
using DnDGen.TreasureGen.Items;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Items.Powers
{
    [TestFixture]
    public class Level10PowerTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Formattable.Percentile.LevelXPower(10); }
        }

        [TestCase(1, 40, PowerConstants.Mundane)]
        [TestCase(41, 88, PowerConstants.Minor)]
        [TestCase(89, 99, PowerConstants.Medium)]
        [TestCase(100, 100, PowerConstants.Major)]
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
