using DnDGen.CharacterGen.Randomizers.Races;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Tests.Unit.Randomizers.Races;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Unit.Randomizers.Races.BaseRaces
{
    [TestFixture]
    public abstract class BaseRaceRandomizerTestBase : RaceRandomizerTestBase
    {
        protected RaceRandomizer randomizer;
        protected abstract IEnumerable<string> baseRaces { get; }

        [SetUp]
        public void BaseRaceRandomizerTestBaseSetup()
        {
            mockPercentileSelector.Setup(p => p.SelectAllFrom(Config.Name, It.IsAny<string>())).Returns(baseRaces);
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, alignment.Full)).Returns(baseRaces);
        }
    }
}