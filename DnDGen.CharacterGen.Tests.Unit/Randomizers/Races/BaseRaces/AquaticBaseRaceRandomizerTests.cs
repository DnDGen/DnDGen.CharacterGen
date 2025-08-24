using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Generators.Randomizers.Races.BaseRaces;
using DnDGen.CharacterGen.Randomizers.Races;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Verifiers.Exceptions;
using DnDGen.Infrastructure.Selectors.Collections;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Randomizers.Races.BaseRaces
{
    [TestFixture]
    public class AquaticBaseRaceRandomizerTests
    {
        private RaceRandomizer aquaticBaseRaceRandomizer;
        private Mock<ICollectionSelector> mockCollectionSelector;

        private string firstAquaticBaseRace = "first aquatic base race";
        private string secondAquaticBaseRace = "second aquatic base race";
        private CharacterClassPrototype characterClass;
        private Alignment alignment;
        private List<string> alignmentRaces;
        private List<string> aquaticBaseRaces;

        [SetUp]
        public void Setup()
        {
            mockCollectionSelector = new Mock<ICollectionSelector>();
            aquaticBaseRaceRandomizer = new AquaticBaseRaceRandomizer(mockCollectionSelector.Object);

            alignment = new Alignment();
            characterClass = new CharacterClassPrototype();
            alignmentRaces = new List<string>();
            aquaticBaseRaces = new List<string>() { firstAquaticBaseRace, secondAquaticBaseRace };

            alignment.Goodness = Guid.NewGuid().ToString();
            alignment.Lawfulness = Guid.NewGuid().ToString();

            alignmentRaces.Add(firstAquaticBaseRace);
            alignmentRaces.Add(secondAquaticBaseRace);

            characterClass.Name = "class name";
            characterClass.Level = 1;

            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, alignment.Full)).Returns(alignmentRaces);
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Aquatic)).Returns(aquaticBaseRaces);

            var index = 0;
            mockCollectionSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<string>>())).Returns((IEnumerable<string> ss) => ss.ElementAt(index++ % ss.Count()));
        }

        [Test]
        public void RandomizeGetsAllPossibleResultsFromGetAllPossibleResults()
        {
            var baseRace = aquaticBaseRaceRandomizer.Randomize(alignment, characterClass);
            Assert.That(baseRace, Is.EqualTo(firstAquaticBaseRace));
            mockCollectionSelector.Verify(p => p.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Aquatic), Times.Once);
        }

        [Test]
        public void RandomizeThrowsErrorIfNoPossibleResults()
        {
            mockCollectionSelector
                .Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Aquatic))
                .Returns(Enumerable.Empty<string>());
            Assert.That(() => aquaticBaseRaceRandomizer.Randomize(alignment, characterClass), Throws.InstanceOf<IncompatibleRandomizersException>());
        }

        [Test]
        public void GetAllPossibleResultsGetsResultsFromSelector()
        {
            var baseRaces = aquaticBaseRaceRandomizer.GetAllPossible(alignment, characterClass);
            Assert.That(baseRaces, Is.EquivalentTo(aquaticBaseRaces));
            mockCollectionSelector.Verify(p => p.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Aquatic), Times.Once);
        }

        [Test]
        public void GetAllPossibleResultsFiltersOutBaseRacesNotAllowedByAlignment()
        {
            alignmentRaces.Remove(firstAquaticBaseRace);

            var results = aquaticBaseRaceRandomizer.GetAllPossible(alignment, characterClass);
            Assert.That(results, Contains.Item(secondAquaticBaseRace));
            Assert.That(results.Count(), Is.EqualTo(1));
        }
    }
}