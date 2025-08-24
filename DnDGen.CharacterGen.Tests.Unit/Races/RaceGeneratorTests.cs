using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Generators.Races;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Randomizers.Races;
using DnDGen.CharacterGen.Selectors.Collections;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.RollGen;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Races
{
    [TestFixture]
    public class RaceGeneratorTests
    {
        private const string BaseRace = "base race";
        private const string Metarace = "metarace";

        private Mock<IPercentileSelector> mockPercentileSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<IAdjustmentsSelector> mockAdjustmentsSelector;
        private Mock<Dice> mockDice;
        private IRaceGenerator raceGenerator;
        private Alignment alignment;
        private CharacterClass characterClass;
        private RacePrototype racePrototype;
        private Dictionary<string, int> landSpeeds;
        private Dictionary<string, int> aerialSpeeds;
        private Dictionary<string, int> swimSpeeds;
        private Dictionary<string, int> ages;
        private Dictionary<string, int> challengeRatings;
        private Dictionary<string, int> maleHeights;
        private Dictionary<string, int> femaleHeights;
        private Dictionary<string, int> maleWeights;
        private Dictionary<string, int> femaleWeights;
        private Dictionary<string, List<string>> aerialManeuverability;
        private string classType;
        private string baseRaceSize;
        private List<string> baseRacesWithWings;
        private List<string> metaracesWithWings;
        private Mock<RaceRandomizer> mockBaseRaceRandomizer;
        private Mock<RaceRandomizer> mockMetaraceRandomizer;

        [SetUp]
        public void Setup()
        {
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockAdjustmentsSelector = new Mock<IAdjustmentsSelector>();
            mockPercentileSelector = new Mock<IPercentileSelector>();
            mockDice = new Mock<Dice>();
            raceGenerator = new RaceGenerator(
                mockPercentileSelector.Object,
                mockCollectionsSelector.Object,
                mockAdjustmentsSelector.Object,
                mockDice.Object);

            alignment = new Alignment();
            characterClass = new CharacterClass();
            racePrototype = new RacePrototype();
            landSpeeds = [];
            aerialSpeeds = [];
            swimSpeeds = [];
            classType = CharacterClassConstants.TrainingTypes.Intuitive;
            baseRaceSize = RaceConstants.Sizes.Medium;
            baseRacesWithWings = [];
            metaracesWithWings = [];
            challengeRatings = [];
            maleHeights = [];
            femaleHeights = [];
            maleWeights = [];
            femaleWeights = [];
            aerialManeuverability = [];
            mockBaseRaceRandomizer = new Mock<RaceRandomizer>();
            mockMetaraceRandomizer = new Mock<RaceRandomizer>();

            characterClass.Name = "class name";
            characterClass.Level = 15;
            alignment.Goodness = "goodness";
            racePrototype.BaseRace = BaseRace;
            racePrototype.Metarace = Metarace;

            SetUpTablesForBaseRace(BaseRace);
            SetUpTablesForMetarace(Metarace);

            mockDice.Setup(d => d.Roll(1).d(characterClass.Level).Minus(1).AsSum<int>()).Returns(4);
            mockDice.Setup(d => d.Roll(4).d(It.IsAny<int>()).AsSum<int>()).Returns(8);

            mockCollectionsSelector
                .Setup(s => s.FindCollectionOf(
                    Config.Name,
                    TableNameConstants.Set.Collection.ClassNameGroups,
                    characterClass.Name,
                    CharacterClassConstants.TrainingTypes.Intuitive,
                    CharacterClassConstants.TrainingTypes.SelfTaught,
                    CharacterClassConstants.TrainingTypes.Trained))
                .Returns(() => classType);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.HasWings)).Returns(baseRacesWithWings);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, GroupConstants.HasWings)).Returns(metaracesWithWings);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.AerialManeuverability, It.IsAny<string>()))
                .Returns((string assembly, string table, string name) => aerialManeuverability[name]);

            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.ChallengeRatings, It.IsAny<string>()))
                .Returns((string table, string name) => challengeRatings[name]);
            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.PCChallengeRatingAdjustments, It.IsAny<string>()))
                .Returns(0);
            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.NPCChallengeRatingAdjustments, It.IsAny<string>()))
                .Returns(-1);
            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.AerialSpeeds, It.IsAny<string>()))
                .Returns((string table, string name) => aerialSpeeds[name]);
            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.LandSpeeds, It.IsAny<string>()))
                .Returns((string table, string name) => landSpeeds[name]);
            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.SwimSpeeds, It.IsAny<string>()))
                .Returns((string table, string name) => swimSpeeds[name]);

            var tableName = string.Format(TableNameConstants.Formattable.Adjustments.GENDERHeights, "Male");
            mockAdjustmentsSelector.Setup(s => s.SelectFrom(tableName, It.IsAny<string>())).Returns((string table, string name) => maleHeights[name]);

            tableName = string.Format(TableNameConstants.Formattable.Adjustments.GENDERHeights, "Female");
            mockAdjustmentsSelector.Setup(s => s.SelectFrom(tableName, It.IsAny<string>())).Returns((string table, string name) => femaleHeights[name]);

            tableName = string.Format(TableNameConstants.Formattable.Adjustments.GENDERWeights, "Male");
            mockAdjustmentsSelector.Setup(s => s.SelectFrom(tableName, It.IsAny<string>())).Returns((string table, string name) => maleWeights[name]);

            tableName = string.Format(TableNameConstants.Formattable.Adjustments.GENDERWeights, "Female");
            mockAdjustmentsSelector.Setup(s => s.SelectFrom(tableName, It.IsAny<string>())).Returns((string table, string name) => femaleWeights[name]);
        }

        private void SetUpRoll(string roll, int result, int min = 0, int max = int.MaxValue)
        {
            var mockPartial = new Mock<PartialRoll>();

            var count = 0;
            mockPartial.Setup(r => r.AsSum<int>()).Returns(() => result + count++);
            mockPartial.Setup(r => r.AsPotentialMaximum<int>(true)).Returns(max);
            mockPartial.Setup(r => r.AsPotentialMinimum<int>()).Returns(min);

            mockDice.Setup(d => d.Roll(roll)).Returns(mockPartial.Object);
        }

        private void SetUpTablesForBaseRace(string baseRace)
        {
            var tableName = string.Format(TableNameConstants.Formattable.Collection.CLASSTYPEAgeRolls, CharacterClassConstants.TrainingTypes.Trained);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, baseRace)).Returns(new[] { "4200d60000" });
            SetUpRoll("4200d60000", 34);

            tableName = string.Format(TableNameConstants.Formattable.Collection.CLASSTYPEAgeRolls, CharacterClassConstants.TrainingTypes.SelfTaught);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, baseRace)).Returns(new[] { "420d6000" });
            SetUpRoll("420d6000", 24);

            tableName = string.Format(TableNameConstants.Formattable.Collection.CLASSTYPEAgeRolls, CharacterClassConstants.TrainingTypes.Intuitive);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, baseRace)).Returns(new[] { "42d600" });
            SetUpRoll("42d600", 14);

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MaximumAgeRolls, baseRace)).Returns(new[] { "4d26" });
            SetUpRoll("4d26", 1000);

            ages = new Dictionary<string, int>();
            ages[RaceConstants.Ages.Adulthood] = 90210;
            ages[RaceConstants.Ages.MiddleAge] = 90300;
            ages[RaceConstants.Ages.Old] = 90450;
            ages[RaceConstants.Ages.Venerable] = 90600;

            maleHeights[baseRace] = 209;
            femaleHeights[baseRace] = 902;
            maleWeights[baseRace] = 22;
            femaleWeights[baseRace] = 2;

            tableName = string.Format(TableNameConstants.Formattable.Adjustments.RACEAges, baseRace);
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(ages);
            mockAdjustmentsSelector.Setup(s => s.SelectFrom(tableName, It.IsAny<string>())).Returns((string table, string name) => ages[name]);

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.HeightRolls, baseRace)).Returns(new[] { "10d4" });
            SetUpRoll("10d4", 104);

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.WeightRolls, baseRace)).Returns(new[] { "92d66" });
            SetUpRoll("92d66", 424);

            mockCollectionsSelector
                .Setup(s => s.FindCollectionOf(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, baseRace, It.IsAny<string[]>()))
                .Returns(() => baseRaceSize);

            if (landSpeeds.Any())
                landSpeeds[baseRace] = landSpeeds.Last().Value + 1;
            else
                landSpeeds[baseRace] = 9266;

            aerialSpeeds[baseRace] = 0;
            swimSpeeds[baseRace] = 0;
            aerialManeuverability[baseRace] = new List<string> { string.Empty };
            challengeRatings[baseRace] = 0;
        }

        private void SetUpTablesForMetarace(string metarace)
        {
            aerialSpeeds[metarace] = 0;
            aerialManeuverability[metarace] = new List<string> { string.Empty };
            challengeRatings[metarace] = 0;
        }

        [Test]
        public void GeneratePrototype()
        {
            var classPrototype = new CharacterClassPrototype();
            classPrototype.Level = 1029;
            classPrototype.Name = "class prototype name";

            mockBaseRaceRandomizer.Setup(r => r.Randomize(alignment, classPrototype)).Returns("random base race");
            mockMetaraceRandomizer.Setup(r => r.Randomize(alignment, classPrototype)).Returns("random metarace");

            var prototype = raceGenerator.GeneratePrototype(alignment, classPrototype, mockBaseRaceRandomizer.Object, mockMetaraceRandomizer.Object);
            Assert.That(prototype.BaseRace, Is.EqualTo("random base race"));
            Assert.That(prototype.Metarace, Is.EqualTo("random metarace"));
        }

        [Test]
        public void GenerateRaceFromPrototype()
        {
            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.BaseRace, Is.EqualTo(BaseRace));
            Assert.That(race.Metarace, Is.EqualTo(Metarace));
        }

        [Test]
        public void ReturnMale()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.Male)).Returns(true);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.IsMale, Is.True);
        }

        [Test]
        public void ReturnFemale()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.Male)).Returns(false);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.IsMale, Is.False);
        }

        [Test]
        public void RaceGeneratorReturnsMaleForDrowWizard()
        {
            characterClass.Name = CharacterClassConstants.Wizard;
            racePrototype.BaseRace = RaceConstants.BaseRaces.Drow;

            mockCollectionsSelector
                .Setup(s => s.FindCollectionOf(
                    Config.Name,
                    TableNameConstants.Set.Collection.ClassNameGroups,
                    CharacterClassConstants.Wizard,
                    CharacterClassConstants.TrainingTypes.Intuitive,
                    CharacterClassConstants.TrainingTypes.SelfTaught,
                    CharacterClassConstants.TrainingTypes.Trained))
                .Returns(() => classType);

            SetUpTablesForBaseRace(RaceConstants.BaseRaces.Drow);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.IsMale, Is.True);
        }

        [Test]
        public void RaceGeneratorReturnsFemaleForDrowCleric()
        {
            characterClass.Name = CharacterClassConstants.Cleric;
            racePrototype.BaseRace = RaceConstants.BaseRaces.Drow;

            mockCollectionsSelector
                .Setup(s => s.FindCollectionOf(
                    Config.Name,
                    TableNameConstants.Set.Collection.ClassNameGroups,
                    CharacterClassConstants.Cleric,
                    CharacterClassConstants.TrainingTypes.Intuitive,
                    CharacterClassConstants.TrainingTypes.SelfTaught,
                    CharacterClassConstants.TrainingTypes.Trained))
                .Returns(() => classType);

            SetUpTablesForBaseRace(RaceConstants.BaseRaces.Drow);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.IsMale, Is.False);
        }

        [Test]
        public void GetRaceSize()
        {
            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.Size, Is.EqualTo(RaceConstants.Sizes.Medium));
        }

        [Test]
        public void MetaraceHasWings()
        {
            metaracesWithWings.Add(Metarace);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);

            Assert.That(race.Metarace, Is.EqualTo(Metarace));
            Assert.That(race.HasWings, Is.True);
        }

        [Test]
        public void BaseRaceHasWings()
        {
            baseRacesWithWings.Add(BaseRace);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.BaseRace, Is.EqualTo(BaseRace));
            Assert.That(race.HasWings, Is.True);
        }

        [Test]
        public void BaseRaceAndMetaraceHaveWings()
        {
            baseRacesWithWings.Add(BaseRace);
            metaracesWithWings.Add(Metarace);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.BaseRace, Is.EqualTo(BaseRace));
            Assert.That(race.Metarace, Is.EqualTo(Metarace));
            Assert.That(race.HasWings, Is.True);
        }

        [TestCase(RaceConstants.Sizes.Colossal)]
        [TestCase(RaceConstants.Sizes.Gargantuan)]
        [TestCase(RaceConstants.Sizes.Huge)]
        [TestCase(RaceConstants.Sizes.Large)]
        [TestCase(RaceConstants.Sizes.Medium)]
        [TestCase(RaceConstants.Sizes.Small)]
        [TestCase(RaceConstants.Sizes.Tiny)]
        public void BaseRaceAndHalfDragonHaveWings(string size)
        {
            SetUpTablesForMetarace(RaceConstants.Metaraces.HalfDragon);
            aerialSpeeds[RaceConstants.Metaraces.HalfDragon] = 2;
            racePrototype.Metarace = RaceConstants.Metaraces.HalfDragon;

            baseRacesWithWings.Add(BaseRace);

            baseRaceSize = size;

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.BaseRace, Is.EqualTo(BaseRace));
            Assert.That(race.Metarace, Is.EqualTo(RaceConstants.Metaraces.HalfDragon));
            Assert.That(race.HasWings, Is.True);
        }

        [Test]
        public void NoWings()
        {
            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.HasWings, Is.False);
        }

        [TestCase(RaceConstants.Sizes.Colossal)]
        [TestCase(RaceConstants.Sizes.Gargantuan)]
        [TestCase(RaceConstants.Sizes.Huge)]
        [TestCase(RaceConstants.Sizes.Large)]
        public void HalfDragonsHaveWings(string size)
        {
            SetUpTablesForMetarace(RaceConstants.Metaraces.HalfDragon);
            aerialSpeeds[RaceConstants.Metaraces.HalfDragon] = 2;
            racePrototype.Metarace = RaceConstants.Metaraces.HalfDragon;

            baseRaceSize = size;

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.HasWings, Is.True);
        }

        [TestCase(RaceConstants.Sizes.Medium)]
        [TestCase(RaceConstants.Sizes.Small)]
        [TestCase(RaceConstants.Sizes.Tiny)]
        public void HalfDragonsDoNotHaveWings(string size)
        {
            SetUpTablesForMetarace(RaceConstants.Metaraces.HalfDragon);
            aerialSpeeds[RaceConstants.Metaraces.HalfDragon] = 2;
            racePrototype.Metarace = RaceConstants.Metaraces.HalfDragon;

            baseRaceSize = size;

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.HasWings, Is.False);
        }

        [Test]
        public void DetermineSpeciesOfHalfDragonByRandomWithinAlignment()
        {
            SetUpTablesForMetarace(RaceConstants.Metaraces.HalfDragon);
            alignment.Goodness = "goodness";
            alignment.Lawfulness = "lawfulness";
            aerialSpeeds[RaceConstants.Metaraces.HalfDragon] = 2;
            racePrototype.Metarace = RaceConstants.Metaraces.HalfDragon;

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(Config.Name, TableNameConstants.Set.Collection.DragonSpecies, "lawfulness goodness")).Returns("dragon species");

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.MetaraceSpecies, Is.EqualTo("dragon species"));
        }

        [Test]
        public void NonHalfDragonsDoNotDetermineSpecies()
        {
            alignment.Goodness = "goodness";
            alignment.Lawfulness = "lawfulness";

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            mockCollectionsSelector.Verify(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.DragonSpecies, It.IsAny<string>()), Times.Never);
            Assert.That(race.MetaraceSpecies, Is.Empty);
        }

        [Test]
        public void DetermineLandSpeed()
        {
            landSpeeds["other base race"] = 42;

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.LandSpeed.Value, Is.EqualTo(9266));
            Assert.That(race.LandSpeed.Unit, Is.EqualTo("feet per round"));
            Assert.That(race.LandSpeed.Description, Is.Empty);
        }

        [Test]
        public void MetaraceAerialSpeedIsSet()
        {
            aerialSpeeds[Metarace] = 30;
            aerialSpeeds[BaseRace] = 20;
            aerialManeuverability[Metarace][0] = "awkward maneuverability";

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.AerialSpeed.Value, Is.EqualTo(30));
            Assert.That(race.AerialSpeed.Unit, Is.EqualTo("feet per round"));
            Assert.That(race.AerialSpeed.Description, Is.EqualTo("awkward maneuverability"));
        }

        [Test]
        public void MetaraceAerialSpeedIsSet_ButBaseRaceAerialSpeedIsHigher()
        {
            aerialSpeeds[Metarace] = 30;
            aerialSpeeds[BaseRace] = 40;
            aerialManeuverability[Metarace][0] = "awkward maneuverability";
            aerialManeuverability[BaseRace][0] = "awesome maneuverability";

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.AerialSpeed.Value, Is.EqualTo(40));
            Assert.That(race.AerialSpeed.Unit, Is.EqualTo("feet per round"));
            Assert.That(race.AerialSpeed.Description, Is.EqualTo("awesome maneuverability"));
        }

        [Test]
        public void BaseRaceAerialSpeedIsSet()
        {
            SetUpTablesForMetarace(RaceConstants.Metaraces.HalfDragon);

            aerialSpeeds[RaceConstants.Metaraces.HalfDragon] = 2;
            aerialSpeeds[BaseRace] = 20;
            aerialManeuverability[BaseRace][0] = "awkward maneuverability";

            racePrototype.Metarace = RaceConstants.Metaraces.HalfDragon;
            baseRaceSize = RaceConstants.Sizes.Large;

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.AerialSpeed.Value, Is.EqualTo(20));
            Assert.That(race.AerialSpeed.Unit, Is.EqualTo("feet per round"));
            Assert.That(race.AerialSpeed.Description, Is.EqualTo("awkward maneuverability"));
        }

        [Test]
        public void MetaraceAerialSpeedIsMultiplierWithWings()
        {
            SetUpTablesForMetarace(RaceConstants.Metaraces.HalfDragon);
            aerialSpeeds[RaceConstants.Metaraces.HalfDragon] = 2;
            aerialSpeeds[BaseRace] = 0;
            aerialManeuverability[RaceConstants.Metaraces.HalfDragon][0] = "awkward maneuverability";

            racePrototype.Metarace = RaceConstants.Metaraces.HalfDragon;
            baseRaceSize = RaceConstants.Sizes.Large;

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.AerialSpeed.Value, Is.EqualTo(9266 * 2));
            Assert.That(race.AerialSpeed.Unit, Is.EqualTo("feet per round"));
            Assert.That(race.AerialSpeed.Description, Is.EqualTo("awkward maneuverability"));
        }

        [Test]
        public void MetaraceAerialSpeedIsMultiplierWithoutWings()
        {
            SetUpTablesForMetarace(RaceConstants.Metaraces.HalfDragon);
            aerialSpeeds[RaceConstants.Metaraces.HalfDragon] = 2;
            aerialSpeeds[BaseRace] = 0;
            aerialManeuverability[RaceConstants.Metaraces.HalfDragon][0] = "awkward maneuverability";
            racePrototype.Metarace = RaceConstants.Metaraces.HalfDragon;

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.AerialSpeed.Value, Is.EqualTo(0));
            Assert.That(race.AerialSpeed.Unit, Is.EqualTo("feet per round"));
            Assert.That(race.AerialSpeed.Description, Is.Empty);
        }

        [Test]
        public void NoAerialSpeed()
        {
            aerialSpeeds[Metarace] = 0;
            aerialSpeeds[BaseRace] = 0;

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.AerialSpeed.Value, Is.EqualTo(0));
            Assert.That(race.AerialSpeed.Unit, Is.EqualTo("feet per round"));
            Assert.That(race.AerialSpeed.Description, Is.Empty);
        }

        [Test]
        public void GetSwimSpeed()
        {
            swimSpeeds[BaseRace] = 42;

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.SwimSpeed.Value, Is.EqualTo(42));
            Assert.That(race.SwimSpeed.Unit, Is.EqualTo("feet per round"));
            Assert.That(race.SwimSpeed.Description, Is.Empty);
        }

        [Test]
        public void NoSwimSpeed()
        {
            swimSpeeds[BaseRace] = 0;

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.SwimSpeed.Value, Is.EqualTo(0));
            Assert.That(race.SwimSpeed.Unit, Is.EqualTo("feet per round"));
            Assert.That(race.SwimSpeed.Description, Is.Empty);
        }

        [TestCase(1, "Short")]
        [TestCase(2, "Average")]
        [TestCase(3, "Tall")]
        public void GetMaleHeight(int roll, string description)
        {
            SetUpRoll("10d4", roll, 1, 3);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.Male)).Returns(true);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.Height.Value, Is.EqualTo(209 + roll));
            Assert.That(race.Height.Unit, Is.EqualTo("Inches"));
            Assert.That(race.Height.Description, Is.EqualTo(description));
        }

        [Test]
        public void GetNonRandomMaleHeight()
        {
            SetUpRoll("10d4", 1, 1, 1);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.Male)).Returns(true);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.Height.Value, Is.EqualTo(210));
            Assert.That(race.Height.Unit, Is.EqualTo("Inches"));
            Assert.That(race.Height.Description, Is.EqualTo("Average"));
        }

        [TestCase(1, "Short")]
        [TestCase(2, "Average")]
        [TestCase(3, "Tall")]
        public void GetFemaleHeight(int roll, string description)
        {
            SetUpRoll("10d4", roll, 1, 3);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.Male)).Returns(false);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.Height.Value, Is.EqualTo(902 + roll));
            Assert.That(race.Height.Unit, Is.EqualTo("Inches"));
            Assert.That(race.Height.Description, Is.EqualTo(description));
        }

        [Test]
        public void GetNonRandomFemaleHeight()
        {
            SetUpRoll("10d4", 1, 1, 1);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.Male)).Returns(false);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.Height.Value, Is.EqualTo(903));
            Assert.That(race.Height.Unit, Is.EqualTo("Inches"));
            Assert.That(race.Height.Description, Is.EqualTo("Average"));
        }

        [TestCase(1, "Light")]
        [TestCase(2, "Average")]
        [TestCase(3, "Heavy")]
        public void GetMaleWeight(int roll, string description)
        {
            SetUpRoll("92d66", roll, 1, 3);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.Male)).Returns(true);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.Weight.Value, Is.EqualTo(22 + 104 * roll));
            Assert.That(race.Weight.Unit, Is.EqualTo("Pounds"));
            Assert.That(race.Weight.Description, Is.EqualTo(description));
        }

        [Test]
        public void GetNonRandomMaleWeight()
        {
            SetUpRoll("92d66", 1, 1, 1);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.Male)).Returns(true);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.Weight.Value, Is.EqualTo(126));
            Assert.That(race.Weight.Unit, Is.EqualTo("Pounds"));
            Assert.That(race.Weight.Description, Is.EqualTo("Average"));
        }

        [TestCase(1, "Light")]
        [TestCase(2, "Average")]
        [TestCase(3, "Heavy")]
        public void GetFemaleWeight(int roll, string description)
        {
            SetUpRoll("92d66", roll, 1, 3);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.Male)).Returns(false);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.Weight.Value, Is.EqualTo(2 + 104 * roll));
            Assert.That(race.Weight.Unit, Is.EqualTo("Pounds"));
            Assert.That(race.Weight.Description, Is.EqualTo(description));
        }

        [Test]
        public void GetNonRandomFemaleWeight()
        {
            SetUpRoll("92d66", 1, 1, 1);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.Male)).Returns(false);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.Weight.Value, Is.EqualTo(106));
            Assert.That(race.Weight.Unit, Is.EqualTo("Pounds"));
            Assert.That(race.Weight.Description, Is.EqualTo("Average"));
        }

        [TestCase(CharacterClassConstants.TrainingTypes.Intuitive, 90232)]
        [TestCase(CharacterClassConstants.TrainingTypes.SelfTaught, 90242)]
        [TestCase(CharacterClassConstants.TrainingTypes.Trained, 90252)]
        public void GetAgeByClassType(string classTypeForAge, int age)
        {
            classType = classTypeForAge;

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.Age.Value, Is.EqualTo(age));
            Assert.That(race.Age.Unit, Is.EqualTo("Years"));
            Assert.That(race.Age.Description, Is.Not.Empty);
        }

        [TestCase(1, 0, RaceConstants.Ages.Adulthood)]
        [TestCase(2, 10, RaceConstants.Ages.Adulthood)]
        [TestCase(3, 15, RaceConstants.Ages.Adulthood)]
        [TestCase(4, 20, RaceConstants.Ages.Adulthood)]
        [TestCase(5, 25, RaceConstants.Ages.Adulthood)]
        [TestCase(6, 50, RaceConstants.Ages.MiddleAge)]
        [TestCase(7, 75, RaceConstants.Ages.MiddleAge)]
        [TestCase(8, 100, RaceConstants.Ages.MiddleAge)]
        [TestCase(9, 125, RaceConstants.Ages.MiddleAge)]
        [TestCase(10, 150, RaceConstants.Ages.MiddleAge)]
        [TestCase(11, 175, RaceConstants.Ages.MiddleAge)]
        [TestCase(12, 190, RaceConstants.Ages.MiddleAge)]
        [TestCase(13, 200, RaceConstants.Ages.Old)]
        [TestCase(14, 250, RaceConstants.Ages.Old)]
        [TestCase(15, 275, RaceConstants.Ages.Old)]
        [TestCase(16, 300, RaceConstants.Ages.Old)]
        [TestCase(17, 325, RaceConstants.Ages.Old)]
        [TestCase(18, 350, RaceConstants.Ages.Venerable)]
        [TestCase(19, 500, RaceConstants.Ages.Venerable)]
        [TestCase(20, 600, RaceConstants.Ages.Venerable)]
        public void AgeIncreasesAsCharacterLevelsUp(int level, int additionalAge, string description)
        {
            characterClass.Level = level;

            mockDice.Setup(d => d.Roll("42d600").AsSum<int>()).Returns(42);
            mockDice.Setup(d => d.Roll(1).d(level).Minus(1).AsSum<int>()).Returns(level - 1);
            mockDice.Setup(d => d.Roll(level - 1).d(42).AsSum<int>()).Returns(additionalAge);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.Age.Value, Is.EqualTo(90210 + 42 + additionalAge));
            Assert.That(race.Age.Unit, Is.EqualTo("Years"));
            Assert.That(race.Age.Description, Is.EqualTo(description));
        }

        [Test]
        public void AgeIncreasesAsCharacterLevelsUp_AdditionalAgeOfZero()
        {
            characterClass.Level = 20;

            mockDice.Setup(d => d.Roll("42d600").AsSum<int>()).Returns(0);
            mockDice.Setup(d => d.Roll(1).d(20).Minus(1).AsSum<int>()).Returns(19);
            mockDice.Setup(d => d.Roll(19).d(0).AsSum<int>()).Throws<Exception>();

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.Age.Value, Is.EqualTo(90210));
            Assert.That(race.Age.Unit, Is.EqualTo("Years"));
            Assert.That(race.Age.Description, Is.EqualTo(RaceConstants.Ages.Adulthood));
        }

        [TestCase(RaceConstants.Ages.Adulthood, RaceConstants.Ages.Ageless, RaceConstants.Ages.Ageless, RaceConstants.Ages.Ageless)]
        [TestCase(RaceConstants.Ages.MiddleAge, 90300, RaceConstants.Ages.Ageless, RaceConstants.Ages.Ageless)]
        [TestCase(RaceConstants.Ages.Old, 90300, 90450, RaceConstants.Ages.Ageless)]
        public void GetAgeless(string description, int middleAge, int old, int venerable)
        {
            characterClass.Level = 20;

            ages[RaceConstants.Ages.MiddleAge] = middleAge;
            ages[RaceConstants.Ages.Old] = old;
            ages[RaceConstants.Ages.Venerable] = venerable;

            mockDice.Setup(d => d.Roll("42d600").AsSum<int>()).Returns(42);
            mockDice.Setup(d => d.Roll(1).d(20).Minus(1).AsSum<int>()).Returns(19);
            mockDice.Setup(d => d.Roll(19).d(42).AsSum<int>()).Returns(200);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.Age.Value, Is.EqualTo(90452));
            Assert.That(race.Age.Unit, Is.EqualTo("Years"));
            Assert.That(race.Age.Description, Is.EqualTo(description));
        }

        [Test]
        public void GetMaximumAge()
        {
            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.MaximumAge.Value, Is.EqualTo(91600));
            Assert.That(race.MaximumAge.Unit, Is.EqualTo("Years"));
            Assert.That(race.MaximumAge.Description, Is.EqualTo("Will die of natural causes"));
        }

        [Test]
        public void GetImmortalAge()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MaximumAgeRolls, BaseRace))
                .Returns(new[] { RaceConstants.Ages.Ageless.ToString() });
            SetUpRoll(RaceConstants.Ages.Ageless.ToString(), -1);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.MaximumAge.Value, Is.EqualTo(RaceConstants.Ages.Ageless));
            Assert.That(race.MaximumAge.Unit, Is.EqualTo("Years"));
            Assert.That(race.MaximumAge.Description, Is.EqualTo("Immortal"));
        }

        [Test]
        public void GetPixieMaximumAgeDescription()
        {
            racePrototype.BaseRace = RaceConstants.BaseRaces.Pixie;
            SetUpTablesForBaseRace(RaceConstants.BaseRaces.Pixie);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.MaximumAge.Value, Is.EqualTo(91600));
            Assert.That(race.MaximumAge.Unit, Is.EqualTo("Years"));
            Assert.That(race.MaximumAge.Description, Is.EqualTo("Will return to their plane of origin"));
        }

        [Test]
        public void UndeadAreImmortal()
        {
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, GroupConstants.Undead)).Returns(new[] { Metarace });

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.MaximumAge.Value, Is.EqualTo(RaceConstants.Ages.Ageless));
            Assert.That(race.MaximumAge.Unit, Is.EqualTo("Years"));
            Assert.That(race.MaximumAge.Description, Is.EqualTo("Immortal"));
        }

        [Test]
        public void GetMetaraceMortalAge()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, GroupConstants.Undead))
                .Returns(new[] { "other metarace" });

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.MaximumAge.Value, Is.EqualTo(91600));
            Assert.That(race.MaximumAge.Unit, Is.EqualTo("Years"));
            Assert.That(race.MaximumAge.Description, Is.EqualTo("Will die of natural causes"));
        }

        [Test]
        public void IfTooOld_SetToMaximum()
        {
            var mockPartial = new Mock<PartialRoll>();
            mockPartial.Setup(r => r.AsSum<int>()).Returns(1391);
            mockDice.Setup(d => d.Roll("42d600")).Returns(mockPartial.Object);

            characterClass.Level = 1;

            mockDice.Setup(d => d.Roll(1).d(1).Minus(1).AsSum<int>()).Returns(0);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.Age.Value, Is.LessThanOrEqualTo(race.MaximumAge.Value));
            Assert.That(race.MaximumAge.Value, Is.EqualTo(91600));
            Assert.That(race.Age.Value, Is.EqualTo(91600));
        }

        [Test]
        public void DoNotRerollImmortalAge()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MaximumAgeRolls, BaseRace))
                .Returns(new[] { RaceConstants.Ages.Ageless.ToString() });

            SetUpRoll(RaceConstants.Ages.Ageless.ToString(), -1);

            characterClass.Level = 1;
            SetUpRoll("42d600", 2000);

            mockDice.Setup(d => d.Roll(1).d(1).Minus(1).AsSum<int>()).Returns(0);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.Age.Value, Is.EqualTo(92210));
            Assert.That(race.Age.Unit, Is.EqualTo("Years"));
            Assert.That(race.Age.Description, Is.EqualTo(RaceConstants.Ages.Venerable));
            Assert.That(race.MaximumAge.Value, Is.EqualTo(RaceConstants.Ages.Ageless));
            Assert.That(race.MaximumAge.Unit, Is.EqualTo("Years"));
            Assert.That(race.MaximumAge.Description, Is.EqualTo("Immortal"));
        }

        [Test]
        public void SetBaseRaceChallengeRating()
        {
            challengeRatings[BaseRace] = 9266;

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.ChallengeRating, Is.EqualTo(9266));
        }

        [Test]
        public void SetMetaraceChallengeRating()
        {
            challengeRatings[Metarace] = 90210;

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.ChallengeRating, Is.EqualTo(90210));
        }

        [Test]
        public void SetBaseRaceAndMetaraceChallengeRating()
        {
            challengeRatings[BaseRace] = 9266;
            challengeRatings[Metarace] = 90210;

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.ChallengeRating, Is.EqualTo(9266 + 90210));
        }

        [Test]
        public void SetChallengeRatingAdjustments()
        {
            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.PCChallengeRatingAdjustments, racePrototype.BaseRace))
                .Returns(9266);
            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.NPCChallengeRatingAdjustments, racePrototype.BaseRace))
                .Returns(90210);

            var race = raceGenerator.GenerateWith(alignment, characterClass, racePrototype);
            Assert.That(race.PCChallengeRatingAdjustment, Is.EqualTo(9266));
            Assert.That(race.NPCChallengeRatingAdjustment, Is.EqualTo(90210));
        }
    }
}