using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Mappers.Percentiles;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Items.Powers
{
    [TestFixture]
    public class CrossPowersTests : TableTests
    {
        protected override string tableName
        {
            get
            {
                return TableNameConstants.Formattable.Percentile.LevelXPower(1);
            }
        }

        private IEnumerable<int> levels;
        private IEnumerable<int> indices;
        private PercentileMapper percentileMapper;

        [SetUp]
        public void Setup()
        {
            percentileMapper = GetNewInstanceOf<PercentileMapper>();
            levels = Enumerable.Range(1, 30);
            indices = Enumerable.Range(1, 100);
        }

        [Test]
        public void PowerTableExistsForAllLevels()
        {
            foreach (var level in levels)
            {
                AssertTable(level);
            }
        }

        private void AssertTable(int level)
        {
            var tableName = TableNameConstants.Formattable.Percentile.LevelXPower(level);
            var table = percentileMapper.Map(Config.Name, tableName);

            Assert.That(table, Is.Not.Null);
            Assert.That(table.Keys, Is.EquivalentTo(indices));
            Assert.That(table.Values, Is.All.Not.Null);
            Assert.That(table.Values, Is.All.Not.Empty);
        }
    }
}
