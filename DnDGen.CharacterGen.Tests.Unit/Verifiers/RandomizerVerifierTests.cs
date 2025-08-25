using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Alignments.Randomizers;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Races.Randomizers;
using DnDGen.CharacterGen.Selectors;
using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Verifiers;
using DnDGen.Infrastructure.Selectors.Collections;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Verifiers
{
    [TestFixture]
    public class RandomizerVerifierTests
    {
        private IRandomizerVerifier verifier;
        private Mock<IAlignmentRandomizer> mockAlignmentRandomizer;
        private Mock<IClassNameRandomizer> mockClassNameRandomizer;
        private Mock<ILevelRandomizer> mockLevelRandomizer;
        private Mock<RaceRandomizer> mockBaseRaceRandomizer;
        private Mock<RaceRandomizer> mockMetaraceRandomizer;
        private Mock<IAdjustmentsSelector> mockAdjustmentsSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<ISetLevelRandomizer> mockSetLevelRandomizer;
        private Mock<ISetClassNameRandomizer> mockSetClassNameRandomizer;

        private CharacterClassPrototype characterClass;
        private List<Alignment> alignments;
        private List<string> classNames;
        private List<int> levels;
        private List<string> baseRaces;
        private List<string> metaraces;
        private Dictionary<string, int> adjustments;
        private Alignment alignment;
        private RacePrototype race;
        private List<string> npcs;

        [SetUp]
        public void Setup()
        {
            mockAdjustmentsSelector = new Mock<IAdjustmentsSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            verifier = new RandomizerVerifier(mockAdjustmentsSelector.Object, mockCollectionsSelector.Object);
            mockAlignmentRandomizer = new Mock<IAlignmentRandomizer>();
            mockClassNameRandomizer = new Mock<IClassNameRandomizer>();
            mockSetClassNameRandomizer = new Mock<ISetClassNameRandomizer>();
            mockLevelRandomizer = new Mock<ILevelRandomizer>();
            mockSetLevelRandomizer = new Mock<ISetLevelRandomizer>();
            mockBaseRaceRandomizer = new Mock<RaceRandomizer>();
            mockMetaraceRandomizer = new Mock<RaceRandomizer>();

            alignments = new List<Alignment>();
            characterClass = new CharacterClassPrototype();
            classNames = new List<string>();
            levels = new List<int>();
            baseRaces = new List<string>();
            metaraces = new List<string>();
            adjustments = new Dictionary<string, int>();
            alignment = new Alignment();
            race = new RacePrototype();
            npcs = new List<string>();

            alignment.Goodness = Guid.NewGuid().ToString();
            alignment.Lawfulness = Guid.NewGuid().ToString();

            mockAlignmentRandomizer.Setup(r => r.GetAllPossibleResults()).Returns(alignments);
            mockClassNameRandomizer.Setup(r => r.GetAllPossibleResults(It.IsAny<Alignment>())).Returns(classNames);
            mockLevelRandomizer.Setup(r => r.GetAllPossibleResults()).Returns(levels);
            mockBaseRaceRandomizer.Setup(r => r.GetAllPossible(It.IsAny<Alignment>(), It.IsAny<CharacterClassPrototype>())).Returns(baseRaces);
            mockMetaraceRandomizer.Setup(r => r.GetAllPossible(It.IsAny<Alignment>(), It.IsAny<CharacterClassPrototype>())).Returns(metaraces);

            mockSetLevelRandomizer.SetupAllProperties();
            mockSetLevelRandomizer.Setup(r => r.GetAllPossibleResults()).Returns(() => new[] { mockSetLevelRandomizer.Object.SetLevel });

            mockSetClassNameRandomizer.SetupAllProperties();
            mockSetClassNameRandomizer.Setup(r => r.GetAllPossibleResults(It.IsAny<Alignment>())).Returns(() => new[] { mockSetClassNameRandomizer.Object.SetClassName });

            alignments.Add(alignment);

            characterClass.Level = 1;
            characterClass.Name = "class name";
            classNames.Add(characterClass.Name);
            levels.Add(characterClass.Level);
            npcs.Add("npc class name");

            race.BaseRace = "base race";
            race.Metarace = "metarace";
            baseRaces.Add(race.BaseRace);
            metaraces.Add(race.Metarace);

            adjustments[race.BaseRace] = 0;
            adjustments[race.Metarace] = 0;

            mockAdjustmentsSelector
                .Setup(p => p.SelectFrom(TableNameConstants.Set.Adjustments.LevelAdjustments, It.IsAny<string>()))
                .Returns((string table, string name) => adjustments[name]);
            mockAdjustmentsSelector.Setup(p => p.SelectAllFrom(TableNameConstants.Set.Adjustments.LevelAdjustments)).Returns(adjustments);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.NPCs)).Returns(npcs);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, alignment.Full)).Returns(classNames);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, alignment.Full)).Returns(baseRaces);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, characterClass.Name)).Returns(baseRaces);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, alignment.Full)).Returns(metaraces);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, characterClass.Name)).Returns(metaraces);
        }

        [Test]
        public void RandomizersVerifiedIfAllRandomizersAreCompatible()
        {
            var verified = verifier.VerifyCompatibility(mockAlignmentRandomizer.Object, mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.True);
        }

        [Test]
        public void RandomizersNotVerifiedIfNoAlignments()
        {
            alignments.Clear();
            var verified = verifier.VerifyCompatibility(mockAlignmentRandomizer.Object, mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void RandomizersNotVerifiedIfNoClassNamesForAnyAlignment()
        {
            classNames.Clear();
            var verified = verifier.VerifyCompatibility(mockAlignmentRandomizer.Object, mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void RandomizersVerifiedIfAtLeastOneClassNameForAnAlignment()
        {
            alignments.Add(new Alignment { Lawfulness = Guid.NewGuid().ToString(), Goodness = Guid.NewGuid().ToString() });
            alignments.Add(new Alignment { Lawfulness = Guid.NewGuid().ToString(), Goodness = Guid.NewGuid().ToString() });

            mockClassNameRandomizer.Setup(r => r.GetAllPossibleResults(alignments[0])).Returns(classNames);
            mockClassNameRandomizer.Setup(r => r.GetAllPossibleResults(alignments[1])).Returns(Enumerable.Empty<string>());
            mockClassNameRandomizer.Setup(r => r.GetAllPossibleResults(alignments[2])).Returns(Enumerable.Empty<string>());

            var verified = verifier.VerifyCompatibility(mockAlignmentRandomizer.Object, mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.True);
        }

        [Test]
        [Ignore("No levels will never be a valid use case")]
        public void RandomizersNotVerifiedIfNoLevels()
        {
            levels.Clear();
            var verified = verifier.VerifyCompatibility(mockAlignmentRandomizer.Object, mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void RandomizersNotVerifiedIfNoBaseRacesForAnyCharacterClass()
        {
            baseRaces.Clear();

            var verified = verifier.VerifyCompatibility(mockAlignmentRandomizer.Object, mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void RandomizersVerifiedIfAtLeastOneBaseRaceForACharacterClass()
        {
            classNames.Add("second class name");
            classNames.Add("third class name");

            mockBaseRaceRandomizer.Setup(r => r.GetAllPossible(It.IsAny<Alignment>(), It.Is<CharacterClassPrototype>(p => p.Name == classNames[0]))).Returns(baseRaces);
            mockBaseRaceRandomizer.Setup(r => r.GetAllPossible(It.IsAny<Alignment>(), It.Is<CharacterClassPrototype>(p => p.Name == classNames[1]))).Returns(Enumerable.Empty<string>());
            mockBaseRaceRandomizer.Setup(r => r.GetAllPossible(It.IsAny<Alignment>(), It.Is<CharacterClassPrototype>(p => p.Name == classNames[2]))).Returns(Enumerable.Empty<string>());

            var verified = verifier.VerifyCompatibility(mockAlignmentRandomizer.Object, mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.True);
        }

        [Test]
        public void RandomizersNotVerifiedIfNoMetaracesForAnyCharacterClass()
        {
            metaraces.Clear();

            var verified = verifier.VerifyCompatibility(mockAlignmentRandomizer.Object, mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void RandomizersVerifiedIfAtLeastOneMetaraceForACharacterClass()
        {
            classNames.Add("second class name");
            classNames.Add("third class name");

            mockMetaraceRandomizer.Setup(r => r.GetAllPossible(It.IsAny<Alignment>(), It.Is<CharacterClassPrototype>(p => p.Name == classNames[0]))).Returns(metaraces);
            mockMetaraceRandomizer.Setup(r => r.GetAllPossible(It.IsAny<Alignment>(), It.Is<CharacterClassPrototype>(p => p.Name == classNames[1]))).Returns(Enumerable.Empty<string>());
            mockMetaraceRandomizer.Setup(r => r.GetAllPossible(It.IsAny<Alignment>(), It.Is<CharacterClassPrototype>(p => p.Name == classNames[2]))).Returns(Enumerable.Empty<string>());

            var verified = verifier.VerifyCompatibility(mockAlignmentRandomizer.Object, mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.True);
        }

        [Test]
        public void RandomizersNotVerifiedIfLevelAdjustmentsInvalid()
        {
            adjustments[baseRaces[0]] = 8;
            adjustments[metaraces[0]] = 8;
            levels.Clear();
            levels.Add(15);

            var verified = verifier.VerifyCompatibility(mockAlignmentRandomizer.Object, mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void RandomizersVerifiedIfOneLevelAdjustmentIsAllowed()
        {
            baseRaces.Add("other base race");
            metaraces.Add("other metarace");

            adjustments[baseRaces[0]] = 8;
            adjustments[metaraces[0]] = 8;
            adjustments[baseRaces[1]] = 0;
            adjustments[metaraces[1]] = 0;

            levels.Clear();
            levels.Add(15);

            var verified = verifier.VerifyCompatibility(mockAlignmentRandomizer.Object, mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.True);
        }

        [Test]
        public void RandomizersNotVerifiedIfNPCLevelAdjustmentsInvalid()
        {
            adjustments[baseRaces[0]] = 20;
            adjustments[metaraces[0]] = 22;

            npcs.AddRange(classNames);

            levels.Clear();
            levels.Add(20);

            var verified = verifier.VerifyCompatibility(mockAlignmentRandomizer.Object, mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void RandomizersVerifiedIfOneNPCLevelAdjustmentIsAllowed()
        {
            baseRaces.Add("other base race");
            metaraces.Add("other metarace");

            adjustments[baseRaces[0]] = 20;
            adjustments[metaraces[0]] = 22;
            adjustments[baseRaces[1]] = 19;
            adjustments[metaraces[1]] = 21;

            npcs.AddRange(classNames);

            levels.Clear();
            levels.Add(20);

            var verified = verifier.VerifyCompatibility(mockAlignmentRandomizer.Object, mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.True);
        }

        [Test]
        public void AlignmentNotVerifiedIfNoClassNamesForAnyAlignment()
        {
            classNames.Clear();

            var verified = verifier.VerifyAlignmentCompatibility(alignments[0], mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void AlignmentVerifiedIfAtLeastOneClassNameForAnAlignment()
        {
            var verified = verifier.VerifyAlignmentCompatibility(alignments[0], mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.True);
        }

        [Test]
        [Ignore("No levels will never be a valid use case")]
        public void AlignmentNotVerifiedIfNoLevels()
        {
            levels.Clear();

            var verified = verifier.VerifyAlignmentCompatibility(alignments[0], mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void AlignmentNotVerifiedIfNoBaseRacesForAnyCharacterClass()
        {
            baseRaces.Clear();

            var verified = verifier.VerifyAlignmentCompatibility(alignments[0], mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void AlignmentVerifiedIfAtLeastOneBaseRaceForACharacterClass()
        {
            classNames.Add("second class name");
            classNames.Add("third class name");

            mockBaseRaceRandomizer.Setup(r => r.GetAllPossible(It.IsAny<Alignment>(), It.Is<CharacterClassPrototype>(p => p.Name == classNames[0]))).Returns(baseRaces);
            mockBaseRaceRandomizer.Setup(r => r.GetAllPossible(It.IsAny<Alignment>(), It.Is<CharacterClassPrototype>(p => p.Name == classNames[1]))).Returns(Enumerable.Empty<string>());
            mockBaseRaceRandomizer.Setup(r => r.GetAllPossible(It.IsAny<Alignment>(), It.Is<CharacterClassPrototype>(p => p.Name == classNames[2]))).Returns(Enumerable.Empty<string>());

            var verified = verifier.VerifyAlignmentCompatibility(alignments[0], mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.True);
        }

        [Test]
        public void AlignmentNotVerifiedIfNoMetaracesForAnyCharacterClass()
        {
            metaraces.Clear();

            var verified = verifier.VerifyAlignmentCompatibility(alignments[0], mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void AlignmentVerifiedIfAtLeastOneMetaraceForACharacterClass()
        {
            classNames.Add("second class name");
            classNames.Add("third class name");

            mockMetaraceRandomizer.Setup(r => r.GetAllPossible(It.IsAny<Alignment>(), It.Is<CharacterClassPrototype>(p => p.Name == classNames[0]))).Returns(metaraces);
            mockMetaraceRandomizer.Setup(r => r.GetAllPossible(It.IsAny<Alignment>(), It.Is<CharacterClassPrototype>(p => p.Name == classNames[1]))).Returns(Enumerable.Empty<string>());
            mockMetaraceRandomizer.Setup(r => r.GetAllPossible(It.IsAny<Alignment>(), It.Is<CharacterClassPrototype>(p => p.Name == classNames[2]))).Returns(Enumerable.Empty<string>());

            var verified = verifier.VerifyAlignmentCompatibility(alignments[0], mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.True);
        }

        [Test]
        public void AlignmentNotVerifiedIfLevelAdjustmentsInvalid()
        {
            adjustments[baseRaces[0]] = 8;
            adjustments[metaraces[0]] = 8;

            levels.Clear();
            levels.Add(15);

            var verified = verifier.VerifyAlignmentCompatibility(alignments[0], mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void AlignmentVerifiedIfOneLevelAdjustmentIsAllowed()
        {
            baseRaces.Add("other base race");
            metaraces.Add("other metarace");

            adjustments[baseRaces[0]] = 8;
            adjustments[metaraces[0]] = 8;
            adjustments[baseRaces[1]] = 0;
            adjustments[metaraces[1]] = 0;

            levels.Clear();
            levels.Add(15);

            var verified = verifier.VerifyAlignmentCompatibility(alignments[0], mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.True);
        }

        [Test]
        public void AlignmentNotVerifiedIfNPCLevelAdjustmentsInvalid()
        {
            adjustments[baseRaces[0]] = 20;
            adjustments[metaraces[0]] = 22;

            npcs.AddRange(classNames);

            levels.Clear();
            levels.Add(20);

            var verified = verifier.VerifyAlignmentCompatibility(alignments[0], mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void AlignmentVerifiedIfOneNPCLevelAdjustmentIsAllowed()
        {
            baseRaces.Add("other base race");
            metaraces.Add("other metarace");

            adjustments[baseRaces[0]] = 20;
            adjustments[metaraces[0]] = 22;
            adjustments[baseRaces[1]] = 19;
            adjustments[metaraces[1]] = 21;

            npcs.AddRange(classNames);

            levels.Clear();
            levels.Add(20);

            var verified = verifier.VerifyAlignmentCompatibility(alignments[0], mockClassNameRandomizer.Object, mockLevelRandomizer.Object, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.True);
        }

        [Test]
        public void CharacterClassNotVerifiedIfNoBaseRacesForAnyCharacterClass()
        {
            baseRaces.Clear();

            var verified = verifier.VerifyCharacterClassCompatibility(alignment, characterClass, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void CharacterClassVerifiedIfAtLeastOneBaseRaceForACharacterClass()
        {
            var verified = verifier.VerifyCharacterClassCompatibility(alignment, characterClass, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.True);
        }

        [Test]
        public void CharacterClassNotVerifiedIfNoMetaracesForAnyCharacterClass()
        {
            metaraces.Clear();
            var verified = verifier.VerifyCharacterClassCompatibility(alignment, characterClass, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void CharacterClassVerifiedIfAtLeastOneMetaraceForACharacterClass()
        {
            var verified = verifier.VerifyCharacterClassCompatibility(alignment, characterClass, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.True);
        }

        [Test]
        public void CharacterClassNotVerifiedIfLevelAdjustmentsInvalid()
        {
            adjustments[baseRaces[0]] = 8;
            adjustments[metaraces[0]] = 8;

            characterClass.Level = 15;

            var verified = verifier.VerifyCharacterClassCompatibility(alignment, characterClass, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void CharacterClassVerifiedIfOneLevelAdjustmentIsAllowed()
        {
            baseRaces.Add("other base race");
            metaraces.Add("other metarace");

            adjustments[baseRaces[0]] = 8;
            adjustments[metaraces[0]] = 8;
            adjustments[baseRaces[1]] = 0;
            adjustments[metaraces[1]] = 1;

            characterClass.Level = 15;

            var verified = verifier.VerifyCharacterClassCompatibility(alignment, characterClass, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.True);
        }

        [Test]
        public void CharacterClassNotVerifiedIfNPCLevelAdjustmentsInvalid()
        {
            adjustments[baseRaces[0]] = 20;
            adjustments[metaraces[0]] = 22;

            characterClass.Level = 20;
            characterClass.IsNPC = true;

            var verified = verifier.VerifyCharacterClassCompatibility(alignment, characterClass, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void CharacterClassVerifiedIfOneNPCLevelAdjustmentIsAllowed()
        {
            baseRaces.Add("other base race");
            metaraces.Add("other metarace");

            adjustments[baseRaces[0]] = 20;
            adjustments[metaraces[0]] = 22;
            adjustments[baseRaces[1]] = 19;
            adjustments[metaraces[1]] = 21;

            characterClass.Level = 20;
            characterClass.IsNPC = true;

            var verified = verifier.VerifyCharacterClassCompatibility(alignment, characterClass, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(verified, Is.True);
        }

        [Test]
        public void RaceNotVerifiedIfLevelAdjustmentsInvalid()
        {
            adjustments[race.BaseRace] = 8;
            adjustments[race.Metarace] = 8;

            characterClass.Level = 15;

            var verified = verifier.VerifyRaceCompatibility(alignment, characterClass, race);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void RaceVerifiedIfLevelAdjustmentIsAllowed()
        {
            adjustments[race.BaseRace] = 7;
            adjustments[race.Metarace] = 8;

            characterClass.Level = 15;

            var verified = verifier.VerifyRaceCompatibility(alignment, characterClass, race);
            Assert.That(verified, Is.True);
        }

        [Test]
        public void RaceNotVerifiedIfNPCLevelAdjustmentsInvalid()
        {
            adjustments[race.BaseRace] = 20;
            adjustments[race.Metarace] = 22;

            characterClass.Level = 20;
            characterClass.IsNPC = true;

            var verified = verifier.VerifyRaceCompatibility(alignment, characterClass, race);
            Assert.That(verified, Is.False);
        }

        [Test]
        public void RaceVerifiedIfNPCLevelAdjustmentIsAllowed()
        {
            adjustments[race.BaseRace] = 20;
            adjustments[race.Metarace] = 21;

            characterClass.Level = 20;
            characterClass.IsNPC = true;

            var verified = verifier.VerifyRaceCompatibility(alignment, characterClass, race);
            Assert.That(verified, Is.True);
        }
    }
}