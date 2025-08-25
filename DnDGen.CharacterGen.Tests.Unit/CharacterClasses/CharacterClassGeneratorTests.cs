using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Selectors;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.CharacterClasses
{
    [TestFixture]
    public class CharacterClassGeneratorTests
    {
        private const string ClassName = "class name";
        private const string BaseRacePlusOne = "baserace+1";

        private Mock<IPercentileSelector> mockPercentileSelector;
        private Mock<IAdjustmentsSelector> mockAdjustmentsSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private ICharacterClassGenerator characterClassGenerator;
        private Alignment alignment;
        private CharacterClassPrototype classPrototype;
        private RacePrototype racePrototype;
        private Dictionary<string, int> specialistFieldQuantities;
        private Dictionary<string, int> prohibitedFieldQuantities;
        private List<string> specialistFields;
        private List<string> prohibitedFields;
        private Mock<IClassNameRandomizer> mockClassNameRandomizer;
        private Mock<ILevelRandomizer> mockLevelRandomizer;
        private Dictionary<string, int> levelAdjustments;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<IPercentileSelector>();
            mockAdjustmentsSelector = new Mock<IAdjustmentsSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            characterClassGenerator = new CharacterClassGenerator(mockAdjustmentsSelector.Object, mockCollectionsSelector.Object, mockPercentileSelector.Object);

            alignment = new Alignment();
            classPrototype = new CharacterClassPrototype();
            racePrototype = new RacePrototype();
            specialistFieldQuantities = [];
            prohibitedFieldQuantities = [];
            specialistFields = [];
            prohibitedFields = [];
            levelAdjustments = [];
            mockClassNameRandomizer = new Mock<IClassNameRandomizer>();
            mockLevelRandomizer = new Mock<ILevelRandomizer>();

            alignment.Goodness = "goodness";
            alignment.Lawfulness = "lawfulness";
            classPrototype.Level = 9266;
            classPrototype.Name = ClassName;
            racePrototype.BaseRace = "base race";
            racePrototype.Metarace = "metarace";

            levelAdjustments[racePrototype.BaseRace] = 0;
            levelAdjustments[BaseRacePlusOne] = 1;
            levelAdjustments[RaceConstants.Metaraces.None] = 0;
            levelAdjustments[racePrototype.Metarace] = 2;

            SetUpClassName(ClassName);

            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.SpecialistFieldQuantities, It.IsAny<string>()))
                .Returns((string _, string c) => specialistFieldQuantities[c]);
            mockAdjustmentsSelector
                .Setup(p => p.SelectFrom(TableNameConstants.Set.Adjustments.LevelAdjustments, It.IsAny<string>()))
                .Returns((string table, string name) => levelAdjustments[name]);
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(TableNameConstants.Set.Adjustments.ProhibitedFieldQuantities)).Returns(prohibitedFieldQuantities);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, racePrototype.BaseRace))
                .Returns(specialistFields);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, racePrototype.Metarace))
                .Returns(specialistFields);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, alignment.Full))
                .Returns(specialistFields);
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<string>>())).Returns((IEnumerable<string> c) => c.First());
        }

        private void SetUpClassName(string className)
        {
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, className)).Returns(specialistFields);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ProhibitedFields, className)).Returns(prohibitedFields);
        }

        [Test]
        public void GeneratePrototype_GeneratesPrototype()
        {
            var npcs = new[] { "class name", "other class name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.NPCs)).Returns(npcs);

            mockClassNameRandomizer.Setup(r => r.Randomize(alignment)).Returns("random class name");
            mockLevelRandomizer.Setup(r => r.Randomize()).Returns(90210);

            var prototype = characterClassGenerator.GeneratePrototype(alignment, mockClassNameRandomizer.Object, mockLevelRandomizer.Object);
            Assert.That(prototype.Level, Is.EqualTo(90210));
            Assert.That(prototype.Name, Is.EqualTo("random class name"));
            Assert.That(prototype.IsNPC, Is.False);
        }

        [Test]
        public void GeneratePrototype_GeneratesNPCPrototype()
        {
            var npcs = new[] { "class name", "random class name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.NPCs)).Returns(npcs);

            mockClassNameRandomizer.Setup(r => r.Randomize(alignment)).Returns("random class name");
            mockLevelRandomizer.Setup(r => r.Randomize()).Returns(90210);

            var prototype = characterClassGenerator.GeneratePrototype(alignment, mockClassNameRandomizer.Object, mockLevelRandomizer.Object);
            Assert.That(prototype.Level, Is.EqualTo(90210));
            Assert.That(prototype.Name, Is.EqualTo("random class name"));
            Assert.That(prototype.IsNPC, Is.True);
        }

        [Test]
        public void GenerateWith_GeneratorReturnsClassFromPrototype()
        {
            classPrototype.IsNPC = false;

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.Name, Is.EqualTo(ClassName));
            Assert.That(characterClass.Level, Is.EqualTo(9266));
            Assert.That(characterClass.LevelAdjustment, Is.EqualTo(2));
            Assert.That(characterClass.IsNPC, Is.False);
            Assert.That(characterClass.EffectiveLevel, Is.EqualTo(9268));
        }

        [Test]
        public void GenerateWith_GeneratorReturnsNPCClassFromPrototype()
        {
            classPrototype.IsNPC = true;

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.Name, Is.EqualTo(ClassName));
            Assert.That(characterClass.Level, Is.EqualTo(9266));
            Assert.That(characterClass.LevelAdjustment, Is.EqualTo(2));
            Assert.That(characterClass.IsNPC, Is.True);
            Assert.That(characterClass.EffectiveLevel, Is.EqualTo(9268 / 2));
        }

        [Test]
        public void GenerateWith_DoNotGetSpecialistFieldsIfShouldNotHaveAny()
        {
            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSHasSpecialistFields, ClassName);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(false);
            specialistFieldQuantities[ClassName] = 1;
            specialistFields.Add("field 1");

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.SpecialistFields, Is.Empty);
        }

        [Test]
        public void GenerateWith_DoNotGetSpecialistFieldsIfNone()
        {
            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSHasSpecialistFields, ClassName);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);
            specialistFieldQuantities[ClassName] = 1;

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.SpecialistFields, Is.Empty);
        }

        [Test]
        public void GenerateWith_GetSpecialistField()
        {
            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSHasSpecialistFields, ClassName);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);
            specialistFieldQuantities[ClassName] = 1;
            specialistFields.Add("field 1");
            specialistFields.Add("field 2");

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.Is<IEnumerable<string>>(fs => fs.Contains("field 2")))).Returns("field 2");

            prohibitedFieldQuantities["field 1"] = 0;
            prohibitedFieldQuantities["field 2"] = 0;

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.SpecialistFields, Contains.Item("field 2"));
            Assert.That(characterClass.SpecialistFields.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateWith_GetSpecialistFields()
        {
            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSHasSpecialistFields, ClassName);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);
            specialistFieldQuantities[ClassName] = 2;
            specialistFields.Add("field 1");
            specialistFields.Add("field 2");
            specialistFields.Add("field 3");

            mockCollectionsSelector.SetupSequence(s => s.SelectRandomFrom(It.Is<IEnumerable<string>>(fs => fs.Contains("field 3") && fs.Contains("field 1"))))
                .Returns("field 3").Returns("field 1");

            prohibitedFieldQuantities["field 1"] = 0;
            prohibitedFieldQuantities["field 2"] = 0;
            prohibitedFieldQuantities["field 3"] = 0;

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.SpecialistFields, Contains.Item("field 3"));
            Assert.That(characterClass.SpecialistFields, Contains.Item("field 1"));
            Assert.That(characterClass.SpecialistFields.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GenerateWith_CannotSpecializeInAlignmentFieldThatDoesNotMatchAlignment()
        {
            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSHasSpecialistFields, ClassName);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);
            specialistFieldQuantities[ClassName] = 1;
            specialistFields.Add("non-alignment field");
            specialistFields.Add("alignment field");

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, alignment.ToString()))
                .Returns(["alignment field"]);

            prohibitedFieldQuantities["alignment field"] = 0;

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.SpecialistFields, Contains.Item("alignment field"));
            Assert.That(characterClass.SpecialistFields.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateWith_DoNotGetProhibitedFieldsIfThereAreNoSpecialistFields()
        {
            prohibitedFields.Add("field 1");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.Is<IEnumerable<string>>(fs => fs.Contains("field 1")))).Returns("field 1");

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.SpecialistFields, Is.Empty);
            Assert.That(characterClass.ProhibitedFields, Is.Empty);
        }

        [Test]
        public void GenerateWith_DoNotGetProhibitedFieldsIfNone()
        {
            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSHasSpecialistFields, ClassName);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);
            specialistFieldQuantities[ClassName] = 1;
            specialistFields.Add("field 1");

            prohibitedFieldQuantities["field 1"] = 1;

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.ProhibitedFields, Is.Empty);
        }

        [Test]
        public void GenerateWith_GetProhibitedField()
        {
            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSHasSpecialistFields, ClassName);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);
            specialistFieldQuantities[ClassName] = 1;
            specialistFields.Add("field 1");

            prohibitedFieldQuantities["field 1"] = 1;
            prohibitedFields.Add("field 1");
            prohibitedFields.Add("field 2");

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.ProhibitedFields, Contains.Item("field 2"));
            Assert.That(characterClass.ProhibitedFields.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateWith_ProhibitedFieldCannotAlreadyBeASpecialistField()
        {
            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSHasSpecialistFields, ClassName);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);
            specialistFieldQuantities[ClassName] = 1;
            specialistFields.Add("field 1");

            prohibitedFieldQuantities["field 1"] = 1;
            prohibitedFields.Add("field 1");
            prohibitedFields.Add("field 2");

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.Is<IEnumerable<string>>(fs => fs.Contains("field 1")))).Returns("field 1");

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.ProhibitedFields, Contains.Item("field 2"));
            Assert.That(characterClass.ProhibitedFields.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateWith_GetProhibitedFields()
        {
            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSHasSpecialistFields, ClassName);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);
            specialistFieldQuantities[ClassName] = 1;
            specialistFields.Add("field 2");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.Is<IEnumerable<string>>(fs => fs.Contains("field 2")))).Returns("field 2");

            prohibitedFieldQuantities["field 2"] = 2;
            prohibitedFields.Add("field 1");
            prohibitedFields.Add("field 2");
            prohibitedFields.Add("field 3");
            prohibitedFields.Add("field 4");
            mockCollectionsSelector
                .SetupSequence(s => s.SelectRandomFrom(It.Is<IEnumerable<string>>(fs => fs.Contains("field 1") && fs.Contains("field 4"))))
                .Returns("field 4")
                .Returns("field 1");

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.ProhibitedFields, Contains.Item("field 4"));
            Assert.That(characterClass.ProhibitedFields, Contains.Item("field 1"));
            Assert.That(characterClass.ProhibitedFields.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GenerateWith_GetProhibitedFields_ClericsGetProhibitedBasedOnAlignment()
        {
            classPrototype.Name = CharacterClassConstants.Cleric;
            SetUpClassName(CharacterClassConstants.Cleric);

            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSHasSpecialistFields, CharacterClassConstants.Cleric);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);
            specialistFieldQuantities[CharacterClassConstants.Cleric] = 2;
            specialistFields.Add("field 1");
            specialistFields.Add("field 2");
            specialistFields.Add("non-alignment field 1");
            specialistFields.Add("non-alignment field 2");

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ProhibitedFields, alignment.ToString()))
                .Returns(["non-alignment field 1", "non-alignment field 2"]);

            prohibitedFieldQuantities["field 1"] = 0;
            prohibitedFieldQuantities["field 2"] = 0;

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.Name, Is.EqualTo(CharacterClassConstants.Cleric));
            Assert.That(characterClass.SpecialistFields, Is.EquivalentTo(["field 1", "field 2"]));
            Assert.That(characterClass.ProhibitedFields, Is.EquivalentTo(["non-alignment field 1", "non-alignment field 2"]));
        }

        [Test]
        public void GenerateWith_GetProhibitedFieldsFromMultipleSpecialistFields()
        {
            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSHasSpecialistFields, ClassName);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);
            specialistFieldQuantities[ClassName] = 2;
            specialistFields.Add("field 1");
            specialistFields.Add("field 2");
            mockCollectionsSelector.SetupSequence(s => s.SelectRandomFrom(It.Is<IEnumerable<string>>(fs => fs.Contains("field 1") && fs.Contains("field 2"))))
                .Returns("field 1").Returns("field 2");

            prohibitedFieldQuantities["field 1"] = 1;
            prohibitedFieldQuantities["field 2"] = 1;
            prohibitedFields.Add("field 1");
            prohibitedFields.Add("field 2");
            prohibitedFields.Add("field 3");
            prohibitedFields.Add("field 4");
            prohibitedFields.Add("field 5");
            mockCollectionsSelector
                .SetupSequence(s => s.SelectRandomFrom(It.Is<IEnumerable<string>>(fs => fs.Contains("field 3") && fs.Contains("field 5"))))
                .Returns("field 5")
                .Returns("field 3");

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.ProhibitedFields, Contains.Item("field 5"));
            Assert.That(characterClass.ProhibitedFields, Contains.Item("field 3"));
            Assert.That(characterClass.ProhibitedFields.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GenerateWith_ReturnIntersectionOfAllRestrictionsOfSpecialistFields()
        {
            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSHasSpecialistFields, ClassName);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);
            specialistFieldQuantities[ClassName] = 1;
            specialistFields.Add("specialist field");
            specialistFields.Add("alignment specialist field");
            specialistFields.Add("other specialist field");
            prohibitedFieldQuantities["specialist field"] = 0;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, racePrototype.Metarace))
                .Returns(["metarace specialist field", "specialist field"]);

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, racePrototype.BaseRace))
                .Returns(["base race specialist field", "specialist field"]);

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, alignment.ToString()))
                .Returns(["alignment specialist field", "specialist field"]);

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.SpecialistFields.Single(), Is.EqualTo("specialist field"));
        }

        [Test]
        public void GenerateWith_ReturnEqualNumberOfIntersectionOfAllRestrictionsOfSpecialistFields()
        {
            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSHasSpecialistFields, ClassName);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);
            specialistFieldQuantities[ClassName] = 2;
            specialistFields.Add("specialist field");
            specialistFields.Add("other specialist field");
            specialistFields.Add("alignment specialist field");
            prohibitedFieldQuantities["specialist field"] = 0;
            prohibitedFieldQuantities["other specialist field"] = 0;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, racePrototype.Metarace))
                .Returns(["metarace specialist field", "specialist field", "other specialist field"]);

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, racePrototype.BaseRace))
                .Returns(["base race specialist field", "specialist field", "other specialist field"]);

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, alignment.ToString()))
                .Returns(["other alignment specialist field", "specialist field", "other specialist field"]);

            var count = 0;
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<string>>())).Returns((IEnumerable<string> c) => c.ElementAt(count++ % c.Count()));

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.SpecialistFields, Is.EqualTo(["specialist field", "other specialist field"]));
        }

        [Test]
        public void GenerateWith_ReturnFewerIntersectionOfAllRestrictionsOfSpecialistFields()
        {
            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSHasSpecialistFields, ClassName);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);
            specialistFieldQuantities[ClassName] = 2;
            specialistFields.Add("specialist field");
            specialistFields.Add("other specialist field");
            specialistFields.Add("alignment specialist field");
            prohibitedFieldQuantities["specialist field"] = 0;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, racePrototype.Metarace))
                .Returns(["metarace specialist field", "specialist field", "other specialist field"]);

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, racePrototype.BaseRace))
                .Returns(["base race specialist field", "specialist field"]);

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, alignment.ToString()))
                .Returns(["alignment specialist field", "specialist field", "another specialist field"]);

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.SpecialistFields, Is.EqualTo(["specialist field"]));
        }

        [Test]
        public void GenerateWith_ReturnNoSpecialistFieldsWhenTooRestricted()
        {
            var tableName = string.Format(TableNameConstants.Formattable.TrueOrFalse.CLASSHasSpecialistFields, ClassName);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);
            specialistFieldQuantities[ClassName] = 2;
            specialistFields.Add("specialist field");
            specialistFields.Add("other specialist field");
            specialistFields.Add("alignment specialist field");
            prohibitedFieldQuantities["specialist field"] = 0;
            prohibitedFieldQuantities["other specialist field"] = 0;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, racePrototype.Metarace))
                .Returns(["metarace specialist field", "other specialist field"]);

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, racePrototype.BaseRace))
                .Returns(["base race specialist field", "specialist field"]);

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.SpecialistFields, alignment.ToString()))
                .Returns(["alignment specialist field", "specialist field"]);

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.SpecialistFields, Is.Empty);
        }

        [Test]
        public void GenerateWith_AppliesBaseRaceLevelAdjustment()
        {
            classPrototype.Level = 2;
            racePrototype.BaseRace = BaseRacePlusOne;
            racePrototype.Metarace = RaceConstants.Metaraces.None;

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.Level, Is.EqualTo(2));
            Assert.That(characterClass.LevelAdjustment, Is.EqualTo(1));
            Assert.That(characterClass.EffectiveLevel, Is.EqualTo(3));
        }

        [Test]
        public void GenerateWith_AppliesMetaraceLevelAdjustment()
        {
            classPrototype.Level = 2;

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.Level, Is.EqualTo(2));
            Assert.That(characterClass.LevelAdjustment, Is.EqualTo(2));
            Assert.That(characterClass.EffectiveLevel, Is.EqualTo(4));
        }

        [Test]
        public void GenerateWith_ApplyBaseRaceAndMetaraceLevelAdjustments()
        {
            classPrototype.Level = 2;
            racePrototype.BaseRace = BaseRacePlusOne;

            var characterClass = characterClassGenerator.GenerateWith(alignment, classPrototype, racePrototype);
            Assert.That(characterClass.Level, Is.EqualTo(2));
            Assert.That(characterClass.LevelAdjustment, Is.EqualTo(3));
            Assert.That(characterClass.EffectiveLevel, Is.EqualTo(5));
        }
    }
}