using DnDGen.CharacterGen.Randomizers.Races;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Tests.Unit.Randomizers.Races;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Unit.Randomizers.Races.Metaraces
{
    [TestFixture]
    public abstract class MetaraceRandomizerTestBase : RaceRandomizerTestBase
    {
        protected RaceRandomizer randomizer;
        protected abstract IEnumerable<string> metaraces { get; }

        [SetUp]
        public void MetaraceRandomizerTestBaseSetup()
        {
            mockPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, It.IsAny<string>())).Returns(metaraces);
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, alignment.Full)).Returns(metaraces);
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, characterClass.Name)).Returns(metaraces);
        }
    }
}