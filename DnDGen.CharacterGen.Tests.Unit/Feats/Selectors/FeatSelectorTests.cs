using DnDGen.CharacterGen.Feats.Selectors;
using DnDGen.CharacterGen.Selectors;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Feats.Selectors
{
    [TestFixture]
    public class FeatSelectorTests
    {
        private IFeatsSelector featsSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<IAdjustmentsSelector> mockAdjustmentsSelector;
        private Dictionary<string, IEnumerable<string>> additionalFeatsData;

        [SetUp]
        public void Setup()
        {
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockAdjustmentsSelector = new Mock<IAdjustmentsSelector>();
            featsSelector = new FeatsSelector(mockCollectionsSelector.Object, mockAdjustmentsSelector.Object);

            additionalFeatsData = new Dictionary<string, IEnumerable<string>>();

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, It.IsAny<string>(), It.IsAny<string>())).Returns(Enumerable.Empty<string>());
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(It.IsAny<string>())).Returns(new Dictionary<string, int>());
            mockCollectionsSelector.Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Set.Collection.AdditionalFeatData)).Returns(additionalFeatsData);
        }

        [Test]
        public void GetRacialFeats()
        {
            var racialFeatData = new Dictionary<string, IEnumerable<string>>();
            racialFeatData["racial feat 1"] = BuildRacialFeatData("racial Feat 1", string.Empty, 0, string.Empty, 600, 9266, 0, string.Empty, 0, "ginormous");
            racialFeatData["racial feat 2"] = BuildRacialFeatData("racial Feat 2", "focusness", 42, "fortnight", 0, 0, 90210, "12d34", 14, string.Empty, "stat", "other stat");

            var racialFeatTableName = string.Format(TableNameConstants.Formattable.Collection.RACEFeatData, "race");
            mockCollectionsSelector.Setup(s => s.SelectAllFrom(Config.Name, racialFeatTableName)).Returns(racialFeatData);

            var featRequirements = new Dictionary<string, IEnumerable<string>>();
            featRequirements["racial feat 1"] = new[] { "feat 1", "feat 2/focus" };
            mockCollectionsSelector.Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Set.Collection.RequiredFeats)).Returns(featRequirements);

            var racialFeats = featsSelector.SelectRacial("race");
            Assert.That(racialFeats.Count(), Is.EqualTo(2));

            var first = racialFeats.First();
            var last = racialFeats.Last();

            Assert.That(first.Feat, Is.EqualTo("racial Feat 1"));
            Assert.That(first.SizeRequirement, Is.EqualTo("ginormous"));
            Assert.That(first.MinimumHitDieRequirement, Is.EqualTo(9266));
            Assert.That(first.Power, Is.EqualTo(0));
            Assert.That(first.FocusType, Is.Empty);
            Assert.That(first.Frequency.Quantity, Is.EqualTo(0));
            Assert.That(first.Frequency.TimePeriod, Is.Empty);
            Assert.That(first.MaximumHitDieRequirement, Is.EqualTo(600));
            Assert.That(first.MinimumAbilities, Is.Empty);
            Assert.That(first.RandomFociQuantity, Is.Empty);

            var firstRequirement = first.RequiredFeats.First();
            var lastRequirement = first.RequiredFeats.Last();
            Assert.That(firstRequirement.Feat, Is.EqualTo("feat 1"));
            Assert.That(firstRequirement.Focus, Is.Empty);
            Assert.That(lastRequirement.Feat, Is.EqualTo("feat 2"));
            Assert.That(lastRequirement.Focus, Is.EqualTo("focus"));
            Assert.That(first.RequiredFeats.Count(), Is.EqualTo(2));

            Assert.That(last.Feat, Is.EqualTo("racial Feat 2"));
            Assert.That(last.SizeRequirement, Is.Empty);
            Assert.That(last.MinimumHitDieRequirement, Is.EqualTo(0));
            Assert.That(last.Power, Is.EqualTo(90210));
            Assert.That(last.FocusType, Is.EqualTo("focusness"));
            Assert.That(last.Frequency.Quantity, Is.EqualTo(42));
            Assert.That(last.Frequency.TimePeriod, Is.EqualTo("fortnight"));
            Assert.That(last.MaximumHitDieRequirement, Is.EqualTo(0));
            Assert.That(last.MinimumAbilities["stat"], Is.EqualTo(14));
            Assert.That(last.MinimumAbilities["other stat"], Is.EqualTo(14));
            Assert.That(last.MinimumAbilities.Count, Is.EqualTo(2));
            Assert.That(last.RandomFociQuantity, Is.EqualTo("12d34"));
            Assert.That(last.RequiredFeats, Is.Empty);
        }

        private IEnumerable<string> BuildRacialFeatData(string featName, string focus, int frequencyQuantity, string frequencyTimePeriod, int maxHitDice, int minHitDice, int power, string randomFociQuantity, int requiredStatValue, string size, params string[] requiredAbilities)
        {
            var data = new List<string>(11);
            while (data.Count < data.Capacity)
                data.Add(string.Empty);

            data[DataIndexConstants.RacialFeatData.FeatNameIndex] = featName;
            data[DataIndexConstants.RacialFeatData.FocusIndex] = focus;
            data[DataIndexConstants.RacialFeatData.FrequencyQuantityIndex] = frequencyQuantity.ToString();
            data[DataIndexConstants.RacialFeatData.FrequencyTimePeriodIndex] = frequencyTimePeriod;
            data[DataIndexConstants.RacialFeatData.MaximumHitDiceRequirementIndex] = maxHitDice.ToString();
            data[DataIndexConstants.RacialFeatData.MinimumHitDiceRequirementIndex] = minHitDice.ToString();
            data[DataIndexConstants.RacialFeatData.PowerIndex] = power.ToString();
            data[DataIndexConstants.RacialFeatData.RandomFociQuantity] = randomFociQuantity;
            data[DataIndexConstants.RacialFeatData.RequiredStatIndex] = string.Join(",", requiredAbilities);
            data[DataIndexConstants.RacialFeatData.RequiredStatMinimumValueIndex] = requiredStatValue.ToString();
            data[DataIndexConstants.RacialFeatData.SizeRequirementIndex] = size;

            return data;
        }

        [Test]
        public void GetDifferentRacialFeatsWithSameName()
        {
            var racialFeatData = new Dictionary<string, IEnumerable<string>>();
            racialFeatData["racial feat 1"] = BuildRacialFeatData("racial Feat 1", string.Empty, 0, string.Empty, 600, 9266, 0, string.Empty, 0, "ginormous");
            racialFeatData["racial feat 2"] = BuildRacialFeatData("racial Feat 1", "focusness", 42, "fortnight", 0, 0, 90210, "12d34", 14, string.Empty, "stat", "other stat");

            var racialFeatTableName = string.Format(TableNameConstants.Formattable.Collection.RACEFeatData, "race");
            mockCollectionsSelector.Setup(s => s.SelectAllFrom(Config.Name, racialFeatTableName)).Returns(racialFeatData);

            var featRequirements = new Dictionary<string, IEnumerable<string>>();
            featRequirements["racial feat 1"] = new[] { "feat 1", "feat 2/focus" };
            mockCollectionsSelector.Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Set.Collection.RequiredFeats)).Returns(featRequirements);

            var racialFeats = featsSelector.SelectRacial("race");
            Assert.That(racialFeats.Count(), Is.EqualTo(2));

            var first = racialFeats.First();
            var last = racialFeats.Last();

            Assert.That(first.Feat, Is.EqualTo("racial Feat 1"));
            Assert.That(first.SizeRequirement, Is.EqualTo("ginormous"));
            Assert.That(first.MinimumHitDieRequirement, Is.EqualTo(9266));
            Assert.That(first.Power, Is.EqualTo(0));
            Assert.That(first.FocusType, Is.Empty);
            Assert.That(first.Frequency.Quantity, Is.EqualTo(0));
            Assert.That(first.Frequency.TimePeriod, Is.Empty);
            Assert.That(first.MaximumHitDieRequirement, Is.EqualTo(600));
            Assert.That(first.MinimumAbilities, Is.Empty);
            Assert.That(first.RandomFociQuantity, Is.Empty);

            var firstRequirement = first.RequiredFeats.First();
            var lastRequirement = first.RequiredFeats.Last();
            Assert.That(firstRequirement.Feat, Is.EqualTo("feat 1"));
            Assert.That(firstRequirement.Focus, Is.Empty);
            Assert.That(lastRequirement.Feat, Is.EqualTo("feat 2"));
            Assert.That(lastRequirement.Focus, Is.EqualTo("focus"));
            Assert.That(first.RequiredFeats.Count(), Is.EqualTo(2));

            Assert.That(last.Feat, Is.EqualTo("racial Feat 1"));
            Assert.That(last.SizeRequirement, Is.Empty);
            Assert.That(last.MinimumHitDieRequirement, Is.EqualTo(0));
            Assert.That(last.Power, Is.EqualTo(90210));
            Assert.That(last.FocusType, Is.EqualTo("focusness"));
            Assert.That(last.Frequency.Quantity, Is.EqualTo(42));
            Assert.That(last.Frequency.TimePeriod, Is.EqualTo("fortnight"));
            Assert.That(last.MaximumHitDieRequirement, Is.EqualTo(0));
            Assert.That(last.MinimumAbilities["stat"], Is.EqualTo(14));
            Assert.That(last.MinimumAbilities["other stat"], Is.EqualTo(14));
            Assert.That(last.MinimumAbilities.Count, Is.EqualTo(2));
            Assert.That(last.RandomFociQuantity, Is.EqualTo("12d34"));
            Assert.That(last.RequiredFeats, Is.Empty);
        }

        [Test]
        public void GetClassFeats()
        {
            var classFeatData = new Dictionary<string, IEnumerable<string>>();
            classFeatData["class feat 1"] = BuildClassFeatData("class Feat 1", "focus type A", 3, "Daily", 4, 1, 0, true, string.Empty, "stat");
            classFeatData["class feat 2"] = BuildClassFeatData("class Feat 2", string.Empty, 0, string.Empty, 0, 5, 9266, false, "large", string.Empty);

            var classFeatTableName = string.Format(TableNameConstants.Formattable.Collection.CLASSFeatData, "class name");
            mockCollectionsSelector.Setup(s => s.SelectAllFrom(Config.Name, classFeatTableName)).Returns(classFeatData);

            var featRequirements = new Dictionary<string, IEnumerable<string>>();
            featRequirements["class feat 1"] = new[] { "feat 1", "feat 2/focus" };
            mockCollectionsSelector.Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Set.Collection.RequiredFeats)).Returns(featRequirements);

            var classFeats = featsSelector.SelectClass("class name");
            Assert.That(classFeats.Count(), Is.EqualTo(2));

            var first = classFeats.First();
            var last = classFeats.Last();

            Assert.That(first.Feat, Is.EqualTo("class Feat 1"));
            Assert.That(first.FocusType, Is.EqualTo("focus type A"));
            Assert.That(first.MinimumLevel, Is.EqualTo(1));
            Assert.That(first.Power, Is.EqualTo(0));
            Assert.That(first.Frequency.Quantity, Is.EqualTo(3));
            Assert.That(first.Frequency.TimePeriod, Is.EqualTo("Daily"));
            Assert.That(first.FrequencyQuantityAbility, Is.EqualTo("stat"));
            Assert.That(first.MaximumLevel, Is.EqualTo(4));
            Assert.That(first.SizeRequirement, Is.Empty);
            Assert.That(first.AllowFocusOfAll, Is.True);

            var firstRequirement = first.RequiredFeats.First();
            var lastRequirement = first.RequiredFeats.Last();
            Assert.That(firstRequirement.Feat, Is.EqualTo("feat 1"));
            Assert.That(firstRequirement.Focus, Is.Empty);
            Assert.That(lastRequirement.Feat, Is.EqualTo("feat 2"));
            Assert.That(lastRequirement.Focus, Is.EqualTo("focus"));
            Assert.That(first.RequiredFeats.Count(), Is.EqualTo(2));

            Assert.That(last.Feat, Is.EqualTo("class Feat 2"));
            Assert.That(last.FocusType, Is.Empty);
            Assert.That(last.MinimumLevel, Is.EqualTo(5));
            Assert.That(last.Power, Is.EqualTo(9266));
            Assert.That(last.Frequency.Quantity, Is.EqualTo(0));
            Assert.That(last.Frequency.TimePeriod, Is.Empty);
            Assert.That(last.FrequencyQuantityAbility, Is.Empty);
            Assert.That(last.MaximumLevel, Is.EqualTo(0));
            Assert.That(last.RequiredFeats, Is.Empty);
            Assert.That(last.SizeRequirement, Is.EqualTo("large"));
            Assert.That(last.AllowFocusOfAll, Is.False);
        }

        private IEnumerable<string> BuildClassFeatData(string featName, string focus, int frequencyQuantity, string frequencyTimePeriod, int maxLevel, int minLevel, int power, bool allowFocusOfAll, string size, string frequencyQuantityStat)
        {
            var data = new List<string>(10);
            while (data.Count < data.Capacity)
                data.Add(string.Empty);

            data[DataIndexConstants.CharacterClassFeatData.AllowFocusOfAllIndex] = allowFocusOfAll.ToString();
            data[DataIndexConstants.CharacterClassFeatData.FeatNameIndex] = featName;
            data[DataIndexConstants.CharacterClassFeatData.FocusTypeIndex] = focus;
            data[DataIndexConstants.CharacterClassFeatData.FrequencyQuantityIndex] = frequencyQuantity.ToString();
            data[DataIndexConstants.CharacterClassFeatData.FrequencyQuantityStatIndex] = frequencyQuantityStat;
            data[DataIndexConstants.CharacterClassFeatData.FrequencyTimePeriodIndex] = frequencyTimePeriod;
            data[DataIndexConstants.CharacterClassFeatData.MaximumLevelRequirementIndex] = maxLevel.ToString();
            data[DataIndexConstants.CharacterClassFeatData.MinimumLevelRequirementIndex] = minLevel.ToString();
            data[DataIndexConstants.CharacterClassFeatData.PowerIndex] = power.ToString();
            data[DataIndexConstants.CharacterClassFeatData.SizeRequirementIndex] = size;

            return data;
        }

        [Test]
        public void GetDifferentClassFeatsWithSameName()
        {
            var classFeatData = new Dictionary<string, IEnumerable<string>>();
            classFeatData["class feat 1"] = BuildClassFeatData("class Feat 1", "focus type A", 3, "Daily", 4, 1, 0, true, string.Empty, "stat");
            classFeatData["class feat 2"] = BuildClassFeatData("class Feat 1", string.Empty, 0, string.Empty, 0, 5, 9266, false, "large", string.Empty);

            var classFeatTableName = string.Format(TableNameConstants.Formattable.Collection.CLASSFeatData, "class name");
            mockCollectionsSelector.Setup(s => s.SelectAllFrom(Config.Name, classFeatTableName)).Returns(classFeatData);

            var featRequirements = new Dictionary<string, IEnumerable<string>>();
            featRequirements["class feat 1"] = new[] { "feat 1", "feat 2/focus" };
            mockCollectionsSelector.Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Set.Collection.RequiredFeats)).Returns(featRequirements);

            var classFeats = featsSelector.SelectClass("class name");
            Assert.That(classFeats.Count(), Is.EqualTo(2));

            var first = classFeats.First();
            var last = classFeats.Last();

            Assert.That(first.Feat, Is.EqualTo("class Feat 1"));
            Assert.That(first.FocusType, Is.EqualTo("focus type A"));
            Assert.That(first.MinimumLevel, Is.EqualTo(1));
            Assert.That(first.Power, Is.EqualTo(0));
            Assert.That(first.Frequency.Quantity, Is.EqualTo(3));
            Assert.That(first.Frequency.TimePeriod, Is.EqualTo("Daily"));
            Assert.That(first.FrequencyQuantityAbility, Is.EqualTo("stat"));
            Assert.That(first.MaximumLevel, Is.EqualTo(4));
            Assert.That(first.SizeRequirement, Is.Empty);
            Assert.That(first.AllowFocusOfAll, Is.True);

            var firstRequirement = first.RequiredFeats.First();
            var lastRequirement = first.RequiredFeats.Last();
            Assert.That(firstRequirement.Feat, Is.EqualTo("feat 1"));
            Assert.That(firstRequirement.Focus, Is.Empty);
            Assert.That(lastRequirement.Feat, Is.EqualTo("feat 2"));
            Assert.That(lastRequirement.Focus, Is.EqualTo("focus"));
            Assert.That(first.RequiredFeats.Count(), Is.EqualTo(2));

            Assert.That(last.Feat, Is.EqualTo("class Feat 1"));
            Assert.That(last.FocusType, Is.Empty);
            Assert.That(last.MinimumLevel, Is.EqualTo(5));
            Assert.That(last.Power, Is.EqualTo(9266));
            Assert.That(last.Frequency.Quantity, Is.EqualTo(0));
            Assert.That(last.Frequency.TimePeriod, Is.Empty);
            Assert.That(last.FrequencyQuantityAbility, Is.Empty);
            Assert.That(last.MaximumLevel, Is.EqualTo(0));
            Assert.That(last.RequiredFeats, Is.Empty);
            Assert.That(last.SizeRequirement, Is.EqualTo("large"));
            Assert.That(last.AllowFocusOfAll, Is.False);
        }

        [Test]
        public void GetAdditionalFeats()
        {
            additionalFeatsData["additional feat 1"] = BuildAdditionalFeatData(string.Empty, 0, string.Empty, 9266, 42);
            additionalFeatsData["additional feat 2"] = BuildAdditionalFeatData("focus", 9266, "occasionally", 0, 0);

            var featRequirements = new Dictionary<string, IEnumerable<string>>();
            featRequirements["additional feat 1"] = new[] { "feat 1", "feat 2/focus" };
            mockCollectionsSelector.Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Set.Collection.RequiredFeats)).Returns(featRequirements);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.HasClassRequirements))
                .Returns(new[] { "additional feat 1" });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.HasSkillRequirements))
                .Returns(new[] { "additional feat 2" });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.HasAbilityRequirements))
                .Returns(new[] { "additional feat 2" });

            var feat1ClassRequirements = new Dictionary<string, int>();
            feat1ClassRequirements["class 1"] = 3;
            feat1ClassRequirements["class 3"] = 5;

            var tableName = string.Format(TableNameConstants.Formattable.Adjustments.FEATClassRequirements, "additional feat 1");
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(feat1ClassRequirements);

            var feat2SkillRankRequirements = new Dictionary<string, int>();
            feat2SkillRankRequirements["skill 1"] = 0;
            feat2SkillRankRequirements["skill 2"] = 4;

            tableName = string.Format(TableNameConstants.Formattable.Adjustments.FEATSkillRankRequirements, "additional feat 2");
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(feat2SkillRankRequirements);

            var feat2StatRequirements = new Dictionary<string, int>();
            feat2StatRequirements["stat 1"] = 13;
            feat2StatRequirements["stat 2"] = 16;

            tableName = string.Format(TableNameConstants.Formattable.Adjustments.FEATAbilityRequirements, "additional feat 2");
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(feat2StatRequirements);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TakenMultipleTimes))
                .Returns(new[] { "additional feat 2", "additional feat 3" });

            var additionalFeats = featsSelector.SelectAdditional();
            Assert.That(additionalFeats.Count(), Is.EqualTo(2));

            var first = additionalFeats.First();
            var last = additionalFeats.Last();

            Assert.That(first.Feat, Is.EqualTo("additional feat 1"));
            Assert.That(first.Power, Is.EqualTo(42));
            Assert.That(first.Frequency.Quantity, Is.EqualTo(0));
            Assert.That(first.Frequency.TimePeriod, Is.Empty);
            Assert.That(first.RequiredBaseAttack, Is.EqualTo(9266));
            Assert.That(first.RequiredCharacterClasses["class 1"], Is.EqualTo(3));
            Assert.That(first.RequiredCharacterClasses["class 3"], Is.EqualTo(5));
            Assert.That(first.RequiredCharacterClasses.Count, Is.EqualTo(2));
            Assert.That(first.CanBeTakenMultipleTimes, Is.False);

            var firstRequirement = first.RequiredFeats.First();
            var lastRequirement = first.RequiredFeats.Last();
            Assert.That(firstRequirement.Feat, Is.EqualTo("feat 1"));
            Assert.That(firstRequirement.Focus, Is.Empty);
            Assert.That(lastRequirement.Feat, Is.EqualTo("feat 2"));
            Assert.That(lastRequirement.Focus, Is.EqualTo("focus"));
            Assert.That(first.RequiredFeats.Count(), Is.EqualTo(2));

            Assert.That(first.RequiredSkills, Is.Empty);
            Assert.That(first.RequiredAbilities, Is.Empty);
            Assert.That(first.FocusType, Is.Empty);

            Assert.That(last.Feat, Is.EqualTo("additional feat 2"));
            Assert.That(last.Power, Is.EqualTo(0));
            Assert.That(last.Frequency.Quantity, Is.EqualTo(9266));
            Assert.That(last.Frequency.TimePeriod, Is.EqualTo("occasionally"));
            Assert.That(last.RequiredBaseAttack, Is.EqualTo(0));
            Assert.That(last.RequiredCharacterClasses, Is.Empty);
            Assert.That(last.RequiredFeats, Is.Empty);
            Assert.That(last.CanBeTakenMultipleTimes, Is.True);

            var requiredSkills = last.RequiredSkills.ToArray();
            Assert.That(requiredSkills[0].Skill, Is.EqualTo("skill 1"));
            Assert.That(requiredSkills[0].Ranks, Is.EqualTo(0));
            Assert.That(requiredSkills[0].Focus, Is.Empty);
            Assert.That(requiredSkills[1].Skill, Is.EqualTo("skill 2"));
            Assert.That(requiredSkills[1].Ranks, Is.EqualTo(4));
            Assert.That(requiredSkills[1].Focus, Is.Empty);
            Assert.That(requiredSkills.Length, Is.EqualTo(2));

            Assert.That(last.RequiredSkills.Count, Is.EqualTo(2));
            Assert.That(last.RequiredAbilities["stat 1"], Is.EqualTo(13));
            Assert.That(last.RequiredAbilities["stat 2"], Is.EqualTo(16));
            Assert.That(last.RequiredAbilities.Count, Is.EqualTo(2));
            Assert.That(last.FocusType, Is.EqualTo("focus"));
        }

        private IEnumerable<string> BuildAdditionalFeatData(string focus, int frequencyQuantity, string frequencyTimePeriod, int baseAttack, int power)
        {
            var data = new List<string>(5);
            while (data.Count < data.Capacity)
                data.Add(string.Empty);

            data[DataIndexConstants.AdditionalFeatData.BaseAttackRequirementIndex] = baseAttack.ToString();
            data[DataIndexConstants.AdditionalFeatData.FocusTypeIndex] = focus;
            data[DataIndexConstants.AdditionalFeatData.FrequencyQuantityIndex] = frequencyQuantity.ToString();
            data[DataIndexConstants.AdditionalFeatData.FrequencyTimePeriodIndex] = frequencyTimePeriod;
            data[DataIndexConstants.AdditionalFeatData.PowerIndex] = power.ToString();

            return data;
        }

        [Test]
        public void GetAdditionalFeatsWithRequiredSkillWithFocus()
        {
            additionalFeatsData["additional feat"] = BuildAdditionalFeatData(string.Empty, 0, string.Empty, 0, 0);

            var featRequirements = new Dictionary<string, IEnumerable<string>>();
            featRequirements["additional feat"] = Enumerable.Empty<string>();
            mockCollectionsSelector.Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Set.Collection.RequiredFeats)).Returns(featRequirements);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.HasSkillRequirements))
                .Returns(new[] { "additional feat" });

            var skillRankRequirements = new Dictionary<string, int>();
            skillRankRequirements["skill 1"] = 0;
            skillRankRequirements["skill 2/focus"] = 4;

            var tableName = string.Format(TableNameConstants.Formattable.Adjustments.FEATSkillRankRequirements, "additional feat");
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(skillRankRequirements);

            var additionalFeats = featsSelector.SelectAdditional();
            var featSelection = additionalFeats.Single();
            Assert.That(featSelection.Feat, Is.EqualTo("additional feat"));

            var requiredSkills = featSelection.RequiredSkills.ToArray();
            Assert.That(requiredSkills[0].Skill, Is.EqualTo("skill 1"));
            Assert.That(requiredSkills[0].Ranks, Is.EqualTo(0));
            Assert.That(requiredSkills[0].Focus, Is.Empty);
            Assert.That(requiredSkills[1].Skill, Is.EqualTo("skill 2"));
            Assert.That(requiredSkills[1].Ranks, Is.EqualTo(4));
            Assert.That(requiredSkills[1].Focus, Is.EqualTo("focus"));
            Assert.That(requiredSkills.Length, Is.EqualTo(2));
        }
    }
}