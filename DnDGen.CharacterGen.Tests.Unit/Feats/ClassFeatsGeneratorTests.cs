using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Skills;
using DnDGen.Infrastructure.Selectors.Collections;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using DnDGen.CharacterGen.Feats.Selectors;

namespace DnDGen.CharacterGen.Tests.Unit.Feats
{
    [TestFixture]
    public class ClassFeatsGeneratorTests
    {
        private Mock<IFeatsSelector> mockFeatsSelector;
        private Mock<IFeatFocusGenerator> mockFeatFocusGenerator;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private IClassFeatsGenerator classFeatsGenerator;
        private CharacterClass characterClass;
        private Dictionary<string, Ability> stats;
        private Dictionary<string, List<CharacterClassFeatSelection>> classFeatSelections;
        private List<Feat> racialFeats;
        private List<Skill> skills;
        private Race race;

        [SetUp]
        public void Setup()
        {
            mockFeatsSelector = new Mock<IFeatsSelector>();
            mockFeatFocusGenerator = new Mock<IFeatFocusGenerator>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            classFeatsGenerator = new ClassFeatsGenerator(mockFeatsSelector.Object, mockFeatFocusGenerator.Object, mockCollectionsSelector.Object);
            characterClass = new CharacterClass();
            stats = new Dictionary<string, Ability>();
            stats[AbilityConstants.Intelligence] = new Ability(AbilityConstants.Intelligence);
            classFeatSelections = new Dictionary<string, List<CharacterClassFeatSelection>>();
            racialFeats = new List<Feat>();
            skills = new List<Skill>();
            race = new Race();

            mockFeatsSelector.Setup(s => s.SelectClass(It.IsAny<string>())).Returns(Enumerable.Empty<CharacterClassFeatSelection>());
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<Feat>>())).Returns((IEnumerable<Feat> c) => c.First());

            characterClass.Name = "class name";
            characterClass.Level = 1;
        }

        [Test]
        public void GetClassFeatsForClass()
        {
            AddClassFeat(characterClass.Name, "class feat 1", power: 9266);
            AddClassFeat(characterClass.Name, "class feat 2", frequencyPeriod: "fortnight", frequencyQuantity: 600);

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            var first = feats.First();
            var last = feats.Last();

            Assert.That(first.Name, Is.EqualTo("class feat 1"));
            Assert.That(first.Power, Is.EqualTo(9266));
            Assert.That(first.Frequency.Quantity, Is.EqualTo(0));
            Assert.That(first.Frequency.TimePeriod, Is.Empty);

            Assert.That(last.Name, Is.EqualTo("class feat 2"));
            Assert.That(last.Power, Is.EqualTo(0));
            Assert.That(last.Frequency.Quantity, Is.EqualTo(600));
            Assert.That(last.Frequency.TimePeriod, Is.EqualTo("fortnight"));

            Assert.That(feats.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetClassFeatsForSpecializations()
        {
            characterClass.SpecialistFields = new[] { "specialist 1", "specialist 2" };

            AddClassFeat("specialist 1", "specialist feat 1", power: 9266);
            AddClassFeat("specialist 1", "specialist feat 2", frequencyPeriod: "fortnight", frequencyQuantity: 600);
            AddClassFeat("specialist 2", "specialist feat 3", power: 42);

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            var feat1 = feats.First(f => f.Name == "specialist feat 1");
            var feat2 = feats.First(f => f.Name == "specialist feat 2");
            var feat3 = feats.First(f => f.Name == "specialist feat 3");

            Assert.That(feat1.Name, Is.EqualTo("specialist feat 1"));
            Assert.That(feat1.Power, Is.EqualTo(9266));
            Assert.That(feat1.Frequency.Quantity, Is.EqualTo(0));
            Assert.That(feat1.Frequency.TimePeriod, Is.Empty);

            Assert.That(feat2.Name, Is.EqualTo("specialist feat 2"));
            Assert.That(feat2.Power, Is.EqualTo(0));
            Assert.That(feat2.Frequency.Quantity, Is.EqualTo(600));
            Assert.That(feat2.Frequency.TimePeriod, Is.EqualTo("fortnight"));

            Assert.That(feat3.Name, Is.EqualTo("specialist feat 3"));
            Assert.That(feat3.Power, Is.EqualTo(42));
            Assert.That(feat3.Frequency.Quantity, Is.EqualTo(0));
            Assert.That(feat3.Frequency.TimePeriod, Is.Empty);

            Assert.That(feats.Count(), Is.EqualTo(3));
        }

        [Test]
        public void GetAllClassFeats()
        {
            characterClass.SpecialistFields = new[] { "specialist" };

            AddClassFeat(characterClass.Name, "class feat", power: 9266);
            AddClassFeat("specialist", "specialist feat", power: 42);

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            Assert.That(feats.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetClassFeatsWithMatchingRequirements()
        {
            characterClass.Level = 2;

            AddClassFeat(characterClass.Name, "feat 1");
            AddClassFeat(characterClass.Name, "feat 2", minimumLevel: 2);
            AddClassFeat(characterClass.Name, "feat 3", minimumLevel: 3);
            AddClassFeat(characterClass.Name, "feat 4", maximumLevel: 1);
            AddClassFeat(characterClass.Name, "feat 5", maximumLevel: 2);

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Contains.Item("feat 1"));
            Assert.That(featNames, Contains.Item("feat 2"));
            Assert.That(featNames, Contains.Item("feat 5"));
            Assert.That(featNames.Count(), Is.EqualTo(3));
        }

        private void AddClassFeat(string className, string feat, string focusType = "", int minimumLevel = 1, int maximumLevel = 0, int frequencyQuantity = 0, string frequencyPeriod = "", int power = 0, params string[] requiredFeatNames)
        {
            var selection = new CharacterClassFeatSelection();
            selection.Feat = feat;
            selection.FocusType = focusType;
            selection.MinimumLevel = minimumLevel;
            selection.RequiredFeats = requiredFeatNames.Select(f => new RequiredFeatSelection { Feat = f });
            selection.Frequency.Quantity = frequencyQuantity;
            selection.Frequency.TimePeriod = frequencyPeriod;
            selection.MaximumLevel = maximumLevel;
            selection.Power = power;
            selection.AllowFocusOfAll = true;

            if (classFeatSelections.ContainsKey(className) == false)
            {
                classFeatSelections[className] = new List<CharacterClassFeatSelection>();
                mockFeatsSelector.Setup(s => s.SelectClass(className)).Returns(classFeatSelections[className]);
            }

            classFeatSelections[className].Add(selection);
        }

        [Test]
        public void DoNotGetClassFeatsIfNone()
        {
            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            Assert.That(feats, Is.Empty);
        }

        [Test]
        public void DoNotGetClassFeatsIfNoneMatchRequirements()
        {
            characterClass.Level = 2;

            AddClassFeat(characterClass.Name, "feat 2", minimumLevel: 3);
            AddClassFeat(characterClass.Name, "feat 2", maximumLevel: 1);

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            Assert.That(feats, Is.Empty);
        }

        [Test]
        public void FeatFociAreFilled()
        {
            AddClassFeat(characterClass.Name, "feat1", focusType: "focus type");
            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("feat1", "focus type", skills, classFeatSelections[characterClass.Name][0].RequiredFeats, It.IsAny<IEnumerable<Feat>>(), characterClass))
                .Returns("focus");

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            var onlyFeat = feats.Single();

            Assert.That(onlyFeat.Name, Is.EqualTo(classFeatSelections[characterClass.Name][0].Feat));
            Assert.That(onlyFeat.Foci.Single(), Is.EqualTo("focus"));
        }

        [Test]
        public void FeatFociAreFilledIndividually()
        {
            AddClassFeat(characterClass.Name, "feat1", focusType: "focus type");
            AddClassFeat(characterClass.Name, "feat2");

            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("feat1", "focus type", skills, classFeatSelections[characterClass.Name][0].RequiredFeats, It.IsAny<IEnumerable<Feat>>(), characterClass))
                .Returns("focus");
            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("feat2", string.Empty, skills, classFeatSelections[characterClass.Name][1].RequiredFeats, It.IsAny<IEnumerable<Feat>>(), characterClass))
                .Returns(string.Empty);

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            var first = feats.First();
            var last = feats.Last();

            Assert.That(first.Name, Is.EqualTo(classFeatSelections[characterClass.Name][0].Feat));
            Assert.That(first.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(last.Name, Is.EqualTo(classFeatSelections[characterClass.Name][1].Feat));
            Assert.That(last.Foci, Is.Empty);
            mockFeatFocusGenerator.Verify(g => g.GenerateAllowingFocusOfAllFrom(It.IsAny<string>(), It.IsAny<string>(), skills, It.IsAny<IEnumerable<RequiredFeatSelection>>(), It.IsAny<IEnumerable<Feat>>(), It.IsAny<CharacterClass>()), Times.Exactly(2));
            mockFeatFocusGenerator.Verify(g => g.GenerateAllowingFocusOfAllFrom("feat1", "focus type", skills, classFeatSelections[characterClass.Name][0].RequiredFeats, It.IsAny<IEnumerable<Feat>>(), characterClass), Times.Once);
            mockFeatFocusGenerator.Verify(g => g.GenerateAllowingFocusOfAllFrom("feat2", string.Empty, skills, classFeatSelections[characterClass.Name][1].RequiredFeats, It.IsAny<IEnumerable<Feat>>(), characterClass), Times.Once);
        }

        [Test]
        public void FeatsWithFociCanBeFilledMoreThanOnce()
        {
            characterClass.Level = 2;
            AddClassFeat(characterClass.Name, "feat1", focusType: "focus type");
            AddClassFeat(characterClass.Name, "feat1", focusType: "focus type", minimumLevel: 2);

            mockFeatFocusGenerator.SetupSequence(g => g.GenerateAllowingFocusOfAllFrom("feat1", "focus type", skills, classFeatSelections[characterClass.Name][0].RequiredFeats, It.IsAny<IEnumerable<Feat>>(), characterClass))
                .Returns("focus 1").Returns("focus 2");

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            var first = feats.First();
            var last = feats.Last();

            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(first.Foci.Single(), Is.EqualTo("focus 1"));
            Assert.That(last.Name, Is.EqualTo("feat1"));
            Assert.That(last.Foci.Single(), Is.EqualTo("focus 2"));
        }

        [Test]
        public void HonorFociInClassRequirements()
        {
            AddClassFeat(characterClass.Name, "feat1", focusType: "focus type");
            AddClassFeat(characterClass.Name, "feat2", focusType: "focus type", requiredFeatNames: "feat1");

            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("feat1", "focus type", skills, classFeatSelections[characterClass.Name][0].RequiredFeats, It.IsAny<IEnumerable<Feat>>(), characterClass))
                .Returns("focus 1");
            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("feat2", "focus type", skills, classFeatSelections[characterClass.Name][1].RequiredFeats, It.Is<IEnumerable<Feat>>(fs => fs.Any(f => f.Name == "feat1")), characterClass))
                .Returns("focus 1");

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            var firstFeat = feats.First();
            var lastFeat = feats.Last();

            Assert.That(firstFeat.Name, Is.EqualTo("feat1"));
            Assert.That(firstFeat.Foci.Single(), Is.EqualTo("focus 1"));
            Assert.That(lastFeat.Name, Is.EqualTo("feat2"));
            Assert.That(lastFeat.Foci.Single(), Is.EqualTo("focus 1"));
            Assert.That(feats.Count(), Is.EqualTo(2));
        }

        [Test]
        public void StatBasedFrequenciesAreSet()
        {
            AddClassFeat(characterClass.Name, "feat1", frequencyQuantity: 1);
            classFeatSelections[characterClass.Name][0].FrequencyQuantityAbility = "stat";

            stats["stat"] = new Ability("stat");
            stats["stat"].Value = 15;

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            var onlyFeat = feats.Single();
            Assert.That(onlyFeat.Frequency.Quantity, Is.EqualTo(3));
        }

        [Test]
        public void StatBasedFrequenciesCannotBeNegative()
        {
            AddClassFeat(characterClass.Name, "feat1", frequencyQuantity: 1);
            classFeatSelections[characterClass.Name][0].FrequencyQuantityAbility = "stat";

            stats["stat"] = new Ability("stat");
            stats["stat"].Value = 1;

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            var onlyFeat = feats.Single();
            Assert.That(onlyFeat.Frequency.Quantity, Is.EqualTo(0));
        }

        [TestCase(1, 2)]
        [TestCase(2, 4)]
        [TestCase(3, 6)]
        [TestCase(4, 8)]
        [TestCase(5, 10)]
        public void RangerImprovesPowerOfFavoredEnemyFeat(int numberOfFavoredEnemies, int power)
        {
            characterClass.Name = CharacterClassConstants.Ranger;

            while (numberOfFavoredEnemies-- > 0)
                AddClassFeat(characterClass.Name, FeatConstants.FavoredEnemy, power: 2);

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            var feat1 = feats.First();
            Assert.That(feat1.Power, Is.EqualTo(power));
        }

        [Test]
        public void RangerFavoredEnemyImprovementsAreRandomForMultipleFavoredEnemies()
        {
            characterClass.Name = CharacterClassConstants.Ranger;

            AddClassFeat(characterClass.Name, FeatConstants.FavoredEnemy, focusType: "focus type", power: 2);
            AddClassFeat(characterClass.Name, FeatConstants.FavoredEnemy, focusType: "focus type", power: 2);
            AddClassFeat(characterClass.Name, FeatConstants.FavoredEnemy, focusType: "focus type", power: 2);
            AddClassFeat(characterClass.Name, FeatConstants.FavoredEnemy, focusType: "focus type", power: 2);
            AddClassFeat(characterClass.Name, FeatConstants.FavoredEnemy, focusType: "focus type", power: 2);

            var indices = new List<int>();
            indices.Add(1);
            indices.Add(1);
            indices.Add(4);
            indices.Add(0);
            var i = 0;

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<Feat>>()))
                .Returns((IEnumerable<Feat> fs) => fs.ElementAt(indices[i]))
                .Callback(() => i++);

            mockFeatFocusGenerator.SetupSequence(g => g.GenerateAllowingFocusOfAllFrom(FeatConstants.FavoredEnemy, "focus type", skills, It.IsAny<IEnumerable<RequiredFeatSelection>>(), It.IsAny<IEnumerable<Feat>>(), characterClass))
                .Returns("focus 1").Returns("focus 2").Returns("focus 3").Returns("focus 4").Returns("focus 5");

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            Assert.That(feats.First(f => f.Foci.Contains("focus 1")).Power, Is.EqualTo(4));
            Assert.That(feats.First(f => f.Foci.Contains("focus 2")).Power, Is.EqualTo(6));
            Assert.That(feats.First(f => f.Foci.Contains("focus 3")).Power, Is.EqualTo(2));
            Assert.That(feats.First(f => f.Foci.Contains("focus 4")).Power, Is.EqualTo(2));
            Assert.That(feats.First(f => f.Foci.Contains("focus 5")).Power, Is.EqualTo(4));
        }

        [Test]
        public void NonRangersDoNotGetToImproveFavoredEnemies()
        {
            AddClassFeat(characterClass.Name, FeatConstants.FavoredEnemy, focusType: "focus type", power: 2);
            AddClassFeat(characterClass.Name, FeatConstants.FavoredEnemy, focusType: "focus type", power: 2);
            AddClassFeat(characterClass.Name, FeatConstants.FavoredEnemy, focusType: "focus type", power: 2);
            AddClassFeat(characterClass.Name, FeatConstants.FavoredEnemy, focusType: "focus type", power: 2);
            AddClassFeat(characterClass.Name, FeatConstants.FavoredEnemy, focusType: "focus type", power: 2);

            var indices = new List<int>();
            indices.Add(1);
            indices.Add(1);
            indices.Add(4);
            indices.Add(0);
            var i = 0;

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<Feat>>()))
                .Returns((IEnumerable<Feat> fs) => fs.ElementAt(indices[i]))
                .Callback(() => i++);

            mockFeatFocusGenerator.SetupSequence(g => g.GenerateAllowingFocusOfAllFrom(FeatConstants.FavoredEnemy, "focus type", skills, It.IsAny<IEnumerable<RequiredFeatSelection>>(), It.IsAny<IEnumerable<Feat>>(), characterClass))
                .Returns("focus 1").Returns("focus 2").Returns("focus 3").Returns("focus 4").Returns("focus 5");

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            Assert.That(feats.First(f => f.Foci.Contains("focus 1")).Power, Is.EqualTo(2));
            Assert.That(feats.First(f => f.Foci.Contains("focus 2")).Power, Is.EqualTo(2));
            Assert.That(feats.First(f => f.Foci.Contains("focus 3")).Power, Is.EqualTo(2));
            Assert.That(feats.First(f => f.Foci.Contains("focus 4")).Power, Is.EqualTo(2));
            Assert.That(feats.First(f => f.Foci.Contains("focus 5")).Power, Is.EqualTo(2));
        }

        [Test]
        public void RacialFeatAreNotAltered()
        {
            AddClassFeat(characterClass.Name, "class feat 1", power: 9266);
            AddClassFeat(characterClass.Name, "class feat 2", frequencyPeriod: "fortnight", frequencyQuantity: 600);

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            Assert.That(racialFeats, Is.Empty);
        }

        [Test]
        public void FeatsAllowFocusForAllProficiency()
        {
            characterClass.SpecialistFields = new[] { "specialist" };

            AddClassFeat("specialist", "feat1", focusType: FeatConstants.Foci.All);
            classFeatSelections["specialist"][0].AllowFocusOfAll = true;

            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("feat1", FeatConstants.Foci.All, skills, classFeatSelections["specialist"][0].RequiredFeats, It.IsAny<IEnumerable<Feat>>(), characterClass))
                .Returns(FeatConstants.Foci.All);

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            var onlyFeat = feats.Single();

            Assert.That(onlyFeat.Name, Is.EqualTo(classFeatSelections["specialist"][0].Feat));
            Assert.That(onlyFeat.Foci.Single(), Is.EqualTo(FeatConstants.Foci.All));
        }

        [Test]
        public void FeatsDoNotAllowFocusForAllProficiency()
        {
            characterClass.SpecialistFields = new[] { "specialist" };

            AddClassFeat("specialist", "feat1", focusType: FeatConstants.Foci.All);
            classFeatSelections["specialist"][0].AllowFocusOfAll = false;

            mockFeatFocusGenerator.Setup(g => g.GenerateFrom("feat1", FeatConstants.Foci.All, skills, classFeatSelections["specialist"][0].RequiredFeats, It.IsAny<IEnumerable<Feat>>(), characterClass))
                .Returns("focus");

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            var onlyFeat = feats.Single();

            Assert.That(onlyFeat.Name, Is.EqualTo(classFeatSelections["specialist"][0].Feat));
            Assert.That(onlyFeat.Foci.Single(), Is.EqualTo("focus"));
        }

        [Test]
        public void HonorFeatRequirementsForClassFeats()
        {
            AddClassFeat(characterClass.Name, "feat with requirements 1", requiredFeatNames: new[] { "feat 1" });
            AddClassFeat(characterClass.Name, "feat with requirements 2", requiredFeatNames: new[] { "feat 2" });
            AddClassFeat(characterClass.Name, "feat with requirements 3", requiredFeatNames: new[] { "feat 3" });
            AddClassFeat(characterClass.Name, "feat with requirements 4", requiredFeatNames: new[] { "feat 4" });

            var requiredFeat3 = classFeatSelections[characterClass.Name][2].RequiredFeats.Single();
            requiredFeat3.Focus = "focus 3";
            classFeatSelections[characterClass.Name][2].RequiredFeats = new[] { requiredFeat3 };
            var requiredFeat4 = classFeatSelections[characterClass.Name][3].RequiredFeats.Single();
            requiredFeat4.Focus = "focus 4";
            classFeatSelections[characterClass.Name][3].RequiredFeats = new[] { requiredFeat4 };

            racialFeats.Add(new Feat { Name = "feat 2" });
            racialFeats.Add(new Feat { Name = "feat 3", Foci = new[] { "wrong focus" } });
            racialFeats.Add(new Feat { Name = "feat 4", Foci = new[] { "focus 4" } });

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Is.All.Not.EqualTo("feat with requirements 1"));
            Assert.That(featNames, Contains.Item("feat with requirements 2"));
            Assert.That(featNames, Is.All.Not.EqualTo("feat with requirements 3"));
            Assert.That(featNames, Contains.Item("feat with requirements 4"));
        }

        [Test]
        public void HonorFeatRequirementsForClassFeatsWhenRequirementsAreOtherClassFeats()
        {
            AddClassFeat(characterClass.Name, "feat with requirements 1", focusType: "focus type");
            AddClassFeat(characterClass.Name, "feat with requirements 2", focusType: "focus type");
            AddClassFeat(characterClass.Name, "feat with requirements 3", requiredFeatNames: new[] { "feat with requirements 1" });
            AddClassFeat(characterClass.Name, "feat with requirements 4", requiredFeatNames: new[] { "feat with requirements 2" });

            var requiredFeat3 = classFeatSelections[characterClass.Name][2].RequiredFeats.Single();
            requiredFeat3.Focus = "focus 1";
            classFeatSelections[characterClass.Name][2].RequiredFeats = new[] { requiredFeat3 };
            var requiredFeat4 = classFeatSelections[characterClass.Name][3].RequiredFeats.Single();
            requiredFeat4.Focus = "focus 2";
            classFeatSelections[characterClass.Name][3].RequiredFeats = new[] { requiredFeat4 };

            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("feat with requirements 1", "focus type", skills, classFeatSelections[characterClass.Name][0].RequiredFeats, It.IsAny<IEnumerable<Feat>>(), characterClass))
                .Returns("focus 3");
            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("feat with requirements 2", "focus type", skills, classFeatSelections[characterClass.Name][1].RequiredFeats, It.IsAny<IEnumerable<Feat>>(), characterClass))
                .Returns("focus 2");

            var feats = classFeatsGenerator.GenerateWith(characterClass, race, stats, racialFeats, skills);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Contains.Item("feat with requirements 1"));
            Assert.That(featNames, Contains.Item("feat with requirements 2"));
            Assert.That(featNames, Is.All.Not.EqualTo("feat with requirements 3"));
            Assert.That(featNames, Contains.Item("feat with requirements 4"));
        }
    }
}