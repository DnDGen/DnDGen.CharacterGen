using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Combats;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Selectors.Collections;
using DnDGen.CharacterGen.Selectors.Selections;
using DnDGen.CharacterGen.Skills;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Generators.Feats
{
    [TestFixture]
    public class AdditionalFeatsGeneratorTests
    {
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<IFeatsSelector> mockFeatsSelector;
        private Mock<IFeatFocusGenerator> mockFeatFocusGenerator;
        private Mock<IAdjustmentsSelector> mockAdjustmentsSelector;
        private IAdditionalFeatsGenerator additionalFeatsGenerator;
        private CharacterClass characterClass;
        private Race race;
        private Dictionary<string, Ability> stats;
        private List<Skill> skills;
        private List<AdditionalFeatSelection> additionalFeatSelections;
        private BaseAttack baseAttack;
        private List<Feat> preselectedFeats;
        private List<string> fighterBonusFeats;
        private List<string> wizardBonusFeats;
        private List<string> monsters;

        [SetUp]
        public void Setup()
        {
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockFeatsSelector = new Mock<IFeatsSelector>();
            mockFeatFocusGenerator = new Mock<IFeatFocusGenerator>();
            mockAdjustmentsSelector = new Mock<IAdjustmentsSelector>();
            additionalFeatsGenerator = new AdditionalFeatsGenerator(mockCollectionsSelector.Object, mockFeatsSelector.Object, mockFeatFocusGenerator.Object, mockAdjustmentsSelector.Object);
            characterClass = new CharacterClass();
            race = new Race();
            stats = new Dictionary<string, Ability>();
            skills = new List<Skill>();
            additionalFeatSelections = new List<AdditionalFeatSelection>();
            baseAttack = new BaseAttack();
            stats[AbilityConstants.Intelligence] = new Ability(AbilityConstants.Intelligence);
            preselectedFeats = new List<Feat>();
            fighterBonusFeats = new List<string>();
            wizardBonusFeats = new List<string>();
            monsters = new List<string>();

            monsters.Add("monster");

            mockFeatsSelector.Setup(s => s.SelectAdditional()).Returns(additionalFeatSelections);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.FighterBonusFeats))
                .Returns(fighterBonusFeats);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.WizardBonusFeats)).Returns(wizardBonusFeats);
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<AdditionalFeatSelection>>()))
                .Returns((IEnumerable<AdditionalFeatSelection> fs) => fs.First());
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters)).Returns(monsters);
        }

        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(4, 2)]
        [TestCase(5, 2)]
        [TestCase(6, 3)]
        [TestCase(7, 3)]
        [TestCase(8, 3)]
        [TestCase(9, 4)]
        [TestCase(10, 4)]
        [TestCase(11, 4)]
        [TestCase(12, 5)]
        [TestCase(13, 5)]
        [TestCase(14, 5)]
        [TestCase(15, 6)]
        [TestCase(16, 6)]
        [TestCase(17, 6)]
        [TestCase(18, 7)]
        [TestCase(19, 7)]
        [TestCase(20, 7)]
        public void GetAdditionalFeats(int level, int numberOfFeats)
        {
            characterClass.Level = level;
            AddFeatSelections(8);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);

            for (var i = 0; i < numberOfFeats; i++)
                Assert.That(featNames, Contains.Item(additionalFeatSelections[i].Feat));

            Assert.That(featNames.Count(), Is.EqualTo(numberOfFeats));
        }

        private void AddFeatSelections(int quantity)
        {
            for (var i = 0; i < quantity; i++)
            {
                var selection = new AdditionalFeatSelection();
                selection.Feat = $"feat{i + 1}";

                additionalFeatSelections.Add(selection);
            }
        }

        [Test]
        public void DoNotGetAdditionalFeatIfNoneAvailable()
        {
            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            Assert.That(feats, Is.Empty);
        }

        [Test]
        public void DoNotGetMoreAdditionalFeatsIfNoneAvailable()
        {
            characterClass.Level = 1;
            AddFeatSelections(1);
            additionalFeatSelections[0].RequiredBaseAttack = 9266;

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            Assert.That(feats, Is.Empty);
        }

        [Test]
        public void FighterFeatsCanBeAdditionalFeats()
        {
            characterClass.Level = 1;
            AddFeatSelections(1);
            fighterBonusFeats.Add(additionalFeatSelections[0].Feat);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Contains.Item(additionalFeatSelections[0].Feat));
            Assert.That(featNames.Count(), Is.EqualTo(1));
        }

        [Test]
        public void AdditionalFeatsPickedAtRandom()
        {
            characterClass.Level = 3;
            AddFeatSelections(3);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.Is<IEnumerable<AdditionalFeatSelection>>(fs => fs.Count() == 3)))
                .Returns((IEnumerable<AdditionalFeatSelection> fs) => fs.Last());
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.Is<IEnumerable<AdditionalFeatSelection>>(fs => fs.Count() == 2)))
                .Returns((IEnumerable<AdditionalFeatSelection> fs) => fs.First());

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Contains.Item(additionalFeatSelections[0].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[2].Feat));
            Assert.That(featNames.Count(), Is.EqualTo(2));
        }

        [Test]
        public void HumansGetAdditionalFeat()
        {
            race.BaseRace = RaceConstants.BaseRaces.Human;
            characterClass.Level = 1;
            AddFeatSelections(3);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.Is<IEnumerable<AdditionalFeatSelection>>(fs => fs.Count() == 3)))
                .Returns((IEnumerable<AdditionalFeatSelection> fs) => fs.Last());
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.Is<IEnumerable<AdditionalFeatSelection>>(fs => fs.Count() == 2)))
                .Returns((IEnumerable<AdditionalFeatSelection> fs) => fs.First());

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Contains.Item(additionalFeatSelections[0].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[2].Feat));
            Assert.That(featNames.Count(), Is.EqualTo(2));
        }

        [Test]
        public void DoNotGetFeatsWithUnmetPrerequisite()
        {
            characterClass.Level = 1;
            AddFeatSelections(2);
            additionalFeatSelections[0].RequiredBaseAttack = 9266;

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Contains.Item(additionalFeatSelections[1].Feat));
            Assert.That(featNames.Count(), Is.EqualTo(1));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 2)]
        [TestCase(4, 3)]
        [TestCase(5, 3)]
        [TestCase(6, 4)]
        [TestCase(7, 4)]
        [TestCase(8, 5)]
        [TestCase(9, 5)]
        [TestCase(10, 6)]
        [TestCase(11, 6)]
        [TestCase(12, 7)]
        [TestCase(13, 7)]
        [TestCase(14, 8)]
        [TestCase(15, 8)]
        [TestCase(16, 9)]
        [TestCase(17, 9)]
        [TestCase(18, 10)]
        [TestCase(19, 10)]
        [TestCase(20, 11)]
        public void FightersGetBonusFighterFeats(int level, int numberOfBonusFeats)
        {
            characterClass.Name = CharacterClassConstants.Fighter;
            characterClass.Level = level;
            AddFeatSelections(20);

            foreach (var selection in additionalFeatSelections)
                fighterBonusFeats.Add(selection.Feat);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);
            var additionalFeats = level / 3 + 1;
            var totalFeats = numberOfBonusFeats + additionalFeats;

            for (var i = 0; i < totalFeats; i++)
                Assert.That(featNames, Contains.Item(additionalFeatSelections[i].Feat));

            Assert.That(featNames.Count(), Is.EqualTo(totalFeats));
        }

        [Test]
        public void DoNotGetNonFighterFeats()
        {
            characterClass.Name = CharacterClassConstants.Fighter;
            characterClass.Level = 1;
            AddFeatSelections(3);
            fighterBonusFeats.Add(additionalFeatSelections[2].Feat);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Contains.Item(additionalFeatSelections[0].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[2].Feat));
            Assert.That(featNames.Count(), Is.EqualTo(2));
        }

        [Test]
        public void DoNotGetFighterFeatsIfNoneAvailable()
        {
            characterClass.Name = CharacterClassConstants.Fighter;
            characterClass.Level = 1;
            AddFeatSelections(1);
            fighterBonusFeats.Add(additionalFeatSelections[0].Feat);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Contains.Item(additionalFeatSelections[0].Feat));
            Assert.That(featNames.Count(), Is.EqualTo(1));
        }

        [Test]
        public void DoNotGetMoreFighterFeatsIfNoneAvailable()
        {
            characterClass.Name = CharacterClassConstants.Fighter;
            characterClass.Level = 2;
            AddFeatSelections(3);
            additionalFeatSelections[1].RequiredBaseAttack = 9266;
            fighterBonusFeats.Add(additionalFeatSelections[1].Feat);
            fighterBonusFeats.Add(additionalFeatSelections[2].Feat);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Contains.Item(additionalFeatSelections[0].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[2].Feat));
            Assert.That(featNames.Count(), Is.EqualTo(2));
        }

        [Test]
        public void FighterFeatsPickedAtRandom()
        {
            characterClass.Name = CharacterClassConstants.Fighter;
            characterClass.Level = 2;
            AddFeatSelections(4);

            foreach (var selection in additionalFeatSelections)
                fighterBonusFeats.Add(selection.Feat);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.Is<IEnumerable<AdditionalFeatSelection>>(fs => fs.Count() == 4)))
                .Returns((IEnumerable<AdditionalFeatSelection> fs) => fs.Last());
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.Is<IEnumerable<AdditionalFeatSelection>>(fs => fs.Count() == 3)))
                .Returns((IEnumerable<AdditionalFeatSelection> fs) => fs.First());
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.Is<IEnumerable<AdditionalFeatSelection>>(fs => fs.Count() == 2)))
                .Returns((IEnumerable<AdditionalFeatSelection> fs) => fs.Last());

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Contains.Item(additionalFeatSelections[0].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[2].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[3].Feat));
            Assert.That(featNames.Count(), Is.EqualTo(3));
        }

        [Test]
        public void DoNotGetFighterFeatWithUnmetPrerequisite()
        {
            characterClass.Name = CharacterClassConstants.Fighter;
            characterClass.Level = 1;
            AddFeatSelections(3);
            additionalFeatSelections[1].RequiredFeats = new[] { new RequiredFeatSelection { Feat = "other feat" } };
            fighterBonusFeats.Add(additionalFeatSelections[1].Feat);
            fighterBonusFeats.Add(additionalFeatSelections[2].Feat);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Contains.Item(additionalFeatSelections[0].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[2].Feat));
            Assert.That(featNames.Count(), Is.EqualTo(2));
        }

        [Test]
        public void IfNoFighterFeatsAvailable_ThenStop()
        {
            characterClass.Name = CharacterClassConstants.Fighter;
            characterClass.Level = 1;
            AddFeatSelections(3);
            fighterBonusFeats.Add(additionalFeatSelections[1].Feat);
            additionalFeatSelections[1].RequiredFeats = new[] { new RequiredFeatSelection { Feat = "other feat" } };

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.Is<IEnumerable<AdditionalFeatSelection>>(fs => fs.Count() == 2)))
                .Returns((IEnumerable<AdditionalFeatSelection> fs) => fs.Last());

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Contains.Item(additionalFeatSelections[2].Feat));
            Assert.That(featNames.Count(), Is.EqualTo(1));
        }

        [TestCase(1, 0)]
        [TestCase(2, 0)]
        [TestCase(3, 0)]
        [TestCase(4, 0)]
        [TestCase(5, 1)]
        [TestCase(6, 1)]
        [TestCase(7, 1)]
        [TestCase(8, 1)]
        [TestCase(9, 1)]
        [TestCase(10, 2)]
        [TestCase(11, 2)]
        [TestCase(12, 2)]
        [TestCase(13, 2)]
        [TestCase(14, 2)]
        [TestCase(15, 3)]
        [TestCase(16, 3)]
        [TestCase(17, 3)]
        [TestCase(18, 3)]
        [TestCase(19, 3)]
        [TestCase(20, 4)]
        public void WizardsGetBonusWizardFeats(int level, int numberOfBonusFeats)
        {
            characterClass.Name = CharacterClassConstants.Wizard;
            characterClass.Level = level;
            AddFeatSelections(20);

            foreach (var selection in additionalFeatSelections)
                wizardBonusFeats.Add(selection.Feat);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);
            var additionalFeats = level / 3 + 1;
            var totalFeats = numberOfBonusFeats + additionalFeats;

            for (var i = 0; i < totalFeats; i++)
                Assert.That(featNames, Contains.Item(additionalFeatSelections[i].Feat));

            Assert.That(featNames.Count(), Is.EqualTo(totalFeats));
        }

        [Test]
        public void DoNotGetNonWizardFeats()
        {
            characterClass.Name = CharacterClassConstants.Wizard;
            characterClass.Level = 5;
            AddFeatSelections(5);
            wizardBonusFeats.Add(additionalFeatSelections[3].Feat);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Contains.Item(additionalFeatSelections[0].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[1].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[3].Feat));
            Assert.That(featNames.Count(), Is.EqualTo(3));
        }

        [Test]
        public void DoNotGetWizardFeatsIfNoneAvailable()
        {
            characterClass.Name = CharacterClassConstants.Wizard;
            characterClass.Level = 5;
            AddFeatSelections(4);
            wizardBonusFeats.Add(additionalFeatSelections[0].Feat);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Contains.Item(additionalFeatSelections[0].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[1].Feat));
            Assert.That(featNames.Count(), Is.EqualTo(2));
        }

        [Test]
        public void IfNoWizardFeatsAvailable_ThenStop()
        {
            characterClass.Name = CharacterClassConstants.Wizard;
            characterClass.Level = 10;
            AddFeatSelections(6);
            wizardBonusFeats.Add(additionalFeatSelections[1].Feat);
            wizardBonusFeats.Add(additionalFeatSelections[5].Feat);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Contains.Item(additionalFeatSelections[0].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[1].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[2].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[3].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[5].Feat));
            Assert.That(featNames.Count(), Is.EqualTo(5));
        }

        [Test]
        public void WizardFeatsPickedAtRandom()
        {
            characterClass.Name = CharacterClassConstants.Wizard;
            characterClass.Level = 10;
            AddFeatSelections(7);

            foreach (var selection in additionalFeatSelections)
                wizardBonusFeats.Add(selection.Feat);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<AdditionalFeatSelection>>()))
                .Returns((IEnumerable<AdditionalFeatSelection> fs) => fs.Last());
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.Is<IEnumerable<AdditionalFeatSelection>>(fs => fs.Count() == 3)))
                .Returns((IEnumerable<AdditionalFeatSelection> fs) => fs.First());

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Contains.Item(additionalFeatSelections[0].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[2].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[3].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[4].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[5].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[6].Feat));
            Assert.That(featNames.Count(), Is.EqualTo(6));
        }

        [Test]
        public void ReassessPrerequisitesEveryFeat()
        {
            characterClass.Level = 3;
            AddFeatSelections(3);
            additionalFeatSelections[1].RequiredFeats = new[] { new RequiredFeatSelection { Feat = additionalFeatSelections[0].Feat } };

            var index = 0;
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<AdditionalFeatSelection>>())).Returns((IEnumerable<AdditionalFeatSelection> fs) => fs.ElementAt(index++ % fs.Count()));

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);

            Assert.That(featNames, Contains.Item(additionalFeatSelections[0].Feat));
            Assert.That(featNames, Contains.Item(additionalFeatSelections[1].Feat));
            Assert.That(featNames.Count(), Is.EqualTo(2));
        }

        [Test]
        public void CannotPickAPreselectedFeat()
        {
            var feat = new Feat();
            feat.Name = "feat1";
            preselectedFeats.Add(feat);

            AddFeatSelections(2);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);
            Assert.That(featNames.Single(), Is.EqualTo("feat2"));
        }

        [Test]
        public void FeatFociAreFilled()
        {
            AddFeatSelections(1);
            additionalFeatSelections[0].FocusType = "focus type";
            mockFeatFocusGenerator.Setup(g => g.GenerateFrom("feat1", "focus type", skills, additionalFeatSelections[0].RequiredFeats, It.IsAny<IEnumerable<Feat>>(), characterClass))
                .Returns("focus");

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var onlyFeat = feats.Single();

            Assert.That(onlyFeat.Name, Is.EqualTo(additionalFeatSelections[0].Feat));
            Assert.That(onlyFeat.Foci.Single(), Is.EqualTo("focus"));
        }

        [Test]
        public void FeatFociAreFilledIndividually()
        {
            characterClass.Level = 3;
            AddFeatSelections(2);
            additionalFeatSelections[0].FocusType = "focus type 1";
            additionalFeatSelections[1].FocusType = "focus type 2";

            mockFeatFocusGenerator.Setup(g => g.GenerateFrom("feat1", "focus type 1", skills, additionalFeatSelections[0].RequiredFeats, It.IsAny<IEnumerable<Feat>>(), characterClass))
                .Returns("focus 1");
            mockFeatFocusGenerator.Setup(g => g.GenerateFrom("feat2", "focus type 2", skills, additionalFeatSelections[1].RequiredFeats, It.IsAny<IEnumerable<Feat>>(), characterClass))
                .Returns("focus 2");

            var index = 0;
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<AdditionalFeatSelection>>()))
                .Returns(() => additionalFeatSelections[index])
                .Callback(() => index++);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            Assert.That(feats.Count(), Is.EqualTo(2));

            var first = feats.First();
            var last = feats.Last();

            Assert.That(first.Name, Is.EqualTo(additionalFeatSelections[0].Feat));
            Assert.That(first.Foci.Single(), Is.EqualTo("focus 1"));
            Assert.That(last.Name, Is.EqualTo(additionalFeatSelections[1].Feat));
            Assert.That(last.Foci.Single(), Is.EqualTo("focus 2"));
        }

        [Test]
        public void FeatsWithFociCanBeFilledMoreThanOnce()
        {
            characterClass.Level = 3;
            AddFeatSelections(1);
            additionalFeatSelections[0].FocusType = "focus type";

            mockFeatFocusGenerator.SetupSequence(g => g.GenerateFrom("feat1", "focus type", skills, additionalFeatSelections[0].RequiredFeats, It.IsAny<IEnumerable<Feat>>(), characterClass))
                .Returns("focus 1").Returns("focus 2");

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var onlyFeat = feats.Single();

            Assert.That(onlyFeat.Name, Is.EqualTo(additionalFeatSelections[0].Feat));
            Assert.That(onlyFeat.Foci, Contains.Item("focus 1"));
            Assert.That(onlyFeat.Foci, Contains.Item("focus 2"));
            Assert.That(onlyFeat.Foci.Count(), Is.EqualTo(2));
        }

        [Test]
        public void SpellMasteryStrengthIsIntelligenceBonus()
        {
            characterClass.Level = 1;
            AddFeatSelections(1);
            additionalFeatSelections[0].Feat = FeatConstants.SpellMastery;
            stats[AbilityConstants.Intelligence].Value = 14;

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var firstFeat = feats.First();

            Assert.That(firstFeat.Name, Is.EqualTo(FeatConstants.SpellMastery));
            Assert.That(firstFeat.Power, Is.EqualTo(stats[AbilityConstants.Intelligence].Bonus));
            Assert.That(feats.Count(), Is.EqualTo(1));
        }

        [Test]
        public void FeatsThatCanBeTakenMultipleTimesWithoutFociAreAllowed()
        {
            AddFeatSelections(1);
            characterClass.Level = 3;
            additionalFeatSelections[0].CanBeTakenMultipleTimes = true;

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var first = feats.First();
            var last = feats.Last();

            Assert.That(first.Name, Is.EqualTo("feat1"));
            Assert.That(last.Name, Is.EqualTo("feat1"));
            Assert.That(feats.Count(), Is.EqualTo(2));
        }

        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(4, 2)]
        [TestCase(5, 2)]
        [TestCase(6, 3)]
        [TestCase(7, 3)]
        [TestCase(8, 3)]
        [TestCase(9, 4)]
        [TestCase(10, 5)]
        [TestCase(11, 5)]
        [TestCase(12, 6)]
        [TestCase(13, 7)]
        [TestCase(14, 7)]
        [TestCase(15, 8)]
        [TestCase(16, 9)]
        [TestCase(17, 9)]
        [TestCase(18, 10)]
        [TestCase(19, 11)]
        [TestCase(20, 11)]
        public void RoguesGetBonusFeats(int level, int quantity)
        {
            characterClass.Level = level;
            characterClass.Name = CharacterClassConstants.Rogue;
            AddFeatSelections(12);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var featNames = feats.Select(f => f.Name);

            for (var i = 0; i < quantity; i++)
                Assert.That(featNames, Contains.Item(additionalFeatSelections[i].Feat));

            Assert.That(featNames.Count(), Is.EqualTo(quantity));
        }

        [Test]
        public void IfAllFocusGenerated_CannotSelectFeat()
        {
            AddFeatSelections(1);
            additionalFeatSelections[0].FocusType = "focus type";

            mockFeatFocusGenerator.Setup(g => g.GenerateFrom(additionalFeatSelections[0].Feat, "focus type", skills, additionalFeatSelections[0].RequiredFeats, preselectedFeats, characterClass)).Returns(FeatConstants.Foci.All);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            Assert.That(feats, Is.Empty);
        }

        [Test]
        public void IfAllFocusGenerated_DoNotTryToSelectFeatAgain()
        {
            AddFeatSelections(2);
            additionalFeatSelections[0].FocusType = "focus type";

            mockFeatFocusGenerator.Setup(g => g.GenerateFrom(additionalFeatSelections[0].Feat, "focus type", skills, additionalFeatSelections[0].RequiredFeats, preselectedFeats, characterClass)).Returns(FeatConstants.Foci.All);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var onlyFeat = feats.Single();
            Assert.That(onlyFeat.Name, Is.EqualTo(additionalFeatSelections[1].Feat));
        }

        [Test]
        public void CanHaveFeatWithoutFocus()
        {
            AddFeatSelections(1);
            mockFeatFocusGenerator.Setup(g => g.GenerateFrom(additionalFeatSelections[0].Feat, string.Empty, skills, additionalFeatSelections[0].RequiredFeats, preselectedFeats, characterClass))
                .Returns(string.Empty);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var feat = feats.Single();

            Assert.That(feat.Name, Is.EqualTo(additionalFeatSelections[0].Feat));
            Assert.That(feat.Foci, Is.Empty);
        }

        [Test]
        public void SkillMasteryIsHasMultipleFoci()
        {
            AddFeatSelections(1);
            additionalFeatSelections[0].Feat = FeatConstants.SkillMastery;
            additionalFeatSelections[0].Power = 2;
            additionalFeatSelections[0].FocusType = GroupConstants.Skills;
            stats[AbilityConstants.Intelligence].Value = 12;

            mockFeatFocusGenerator.SetupSequence(g => g.GenerateFrom(FeatConstants.SkillMastery, GroupConstants.Skills, skills, additionalFeatSelections[0].RequiredFeats, It.IsAny<IEnumerable<Feat>>(), characterClass))
                .Returns("skill 1").Returns("skill 2").Returns("skill 3").Returns("skill 4");

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var onlyFeat = feats.Single();

            Assert.That(onlyFeat.Name, Is.EqualTo(FeatConstants.SkillMastery));
            Assert.That(onlyFeat.Power, Is.EqualTo(2));
            Assert.That(onlyFeat.Foci, Contains.Item("skill 1"));
            Assert.That(onlyFeat.Foci, Contains.Item("skill 2"));
            Assert.That(onlyFeat.Foci, Contains.Item("skill 3"));
            Assert.That(onlyFeat.Foci, Is.All.Not.EqualTo("skill 4"));
            Assert.That(onlyFeat.Foci.Count(), Is.EqualTo(3));
        }

        [Test]
        public void SkillMasteryDoesNotRepeatFociWithinOneFeatSelection()
        {
            AddFeatSelections(1);
            additionalFeatSelections[0].Feat = FeatConstants.SkillMastery;
            additionalFeatSelections[0].Power = 1;
            additionalFeatSelections[0].FocusType = GroupConstants.Skills;
            stats[AbilityConstants.Intelligence].Value = 12;

            mockFeatFocusGenerator.Setup(g => g.GenerateFrom(FeatConstants.SkillMastery, GroupConstants.Skills, skills, additionalFeatSelections[0].RequiredFeats, preselectedFeats, characterClass))
                .Returns("skill 1");
            mockFeatFocusGenerator.Setup(g => g.GenerateFrom(FeatConstants.SkillMastery, GroupConstants.Skills, skills, additionalFeatSelections[0].RequiredFeats, It.Is<IEnumerable<Feat>>(fs => fs.Any(f => f.Name == FeatConstants.SkillMastery)), characterClass))
                .Returns("skill 2");

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var onlyFeat = feats.Single();

            Assert.That(onlyFeat.Name, Is.EqualTo(FeatConstants.SkillMastery));
            Assert.That(onlyFeat.Power, Is.EqualTo(1));
            Assert.That(onlyFeat.Foci, Contains.Item("skill 1"));
            Assert.That(onlyFeat.Foci, Contains.Item("skill 2"));
            Assert.That(onlyFeat.Foci.Count(), Is.EqualTo(2));
        }

        [Test]
        public void SkillMasteryDoesNotRepeatFociAcrossFeatSelections()
        {
            AddFeatSelections(1);
            additionalFeatSelections[0].Feat = FeatConstants.SkillMastery;
            additionalFeatSelections[0].Power = 1;
            additionalFeatSelections[0].FocusType = GroupConstants.Skills;
            characterClass.Level = 3;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TakenMultipleTimes))
                .Returns(new[] { FeatConstants.SkillMastery });

            mockFeatFocusGenerator.Setup(g => g.GenerateFrom(FeatConstants.SkillMastery, GroupConstants.Skills, skills, additionalFeatSelections[0].RequiredFeats, preselectedFeats, characterClass))
                .Returns("skill 1");
            mockFeatFocusGenerator.Setup(g => g.GenerateFrom(FeatConstants.SkillMastery, GroupConstants.Skills, skills, additionalFeatSelections[0].RequiredFeats, It.Is<IEnumerable<Feat>>(fs => fs.Any(f => f.Name == FeatConstants.SkillMastery)), characterClass))
                .Returns("skill 2");

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var onlyFeat = feats.Single();

            Assert.That(onlyFeat.Name, Is.EqualTo(FeatConstants.SkillMastery));
            Assert.That(onlyFeat.Power, Is.EqualTo(1));
            Assert.That(onlyFeat.Foci, Contains.Item("skill 1"));
            Assert.That(onlyFeat.Foci, Contains.Item("skill 2"));
            Assert.That(onlyFeat.Foci.Count(), Is.EqualTo(2));
        }

        [Test]
        public void IfOutOfSkills_StopAddingSkillFoci()
        {
            AddFeatSelections(1);
            additionalFeatSelections[0].Feat = FeatConstants.SkillMastery;
            additionalFeatSelections[0].Power = 1;
            additionalFeatSelections[0].FocusType = GroupConstants.Skills;
            characterClass.Level = 3;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TakenMultipleTimes))
                .Returns(new[] { FeatConstants.SkillMastery });

            mockFeatFocusGenerator.Setup(g => g.GenerateFrom(FeatConstants.SkillMastery, GroupConstants.Skills, skills, additionalFeatSelections[0].RequiredFeats, preselectedFeats, characterClass))
                .Returns("skill 1");
            mockFeatFocusGenerator.Setup(g => g.GenerateFrom(FeatConstants.SkillMastery, GroupConstants.Skills, skills, additionalFeatSelections[0].RequiredFeats, It.Is<IEnumerable<Feat>>(fs => fs.Any(f => f.Name == FeatConstants.SkillMastery)), characterClass))
                .Returns(FeatConstants.Foci.All);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var onlyFeat = feats.Single();

            Assert.That(onlyFeat.Name, Is.EqualTo(FeatConstants.SkillMastery));
            Assert.That(onlyFeat.Foci.Single(), Is.EqualTo("skill 1"));
            Assert.That(onlyFeat.Power, Is.EqualTo(1));
        }

        [Test]
        public void IfOutOfSkills_DoNotTakeSkillMastery()
        {
            AddFeatSelections(1);
            additionalFeatSelections[0].Feat = FeatConstants.SkillMastery;
            additionalFeatSelections[0].Power = 1;
            additionalFeatSelections[0].FocusType = GroupConstants.Skills;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TakenMultipleTimes))
                .Returns(new[] { FeatConstants.SkillMastery });

            mockFeatFocusGenerator.Setup(g => g.GenerateFrom(FeatConstants.SkillMastery, GroupConstants.Skills, skills, additionalFeatSelections[0].RequiredFeats, preselectedFeats, characterClass))
                .Returns(FeatConstants.Foci.All);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            Assert.That(feats, Is.Empty);
        }

        [Test]
        public void IfOutOfSkills_CanTakeDifferentFeat()
        {
            AddFeatSelections(2);
            additionalFeatSelections[0].Feat = FeatConstants.SkillMastery;
            additionalFeatSelections[0].Power = 1;
            additionalFeatSelections[0].FocusType = GroupConstants.Skills;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TakenMultipleTimes))
                .Returns(new[] { FeatConstants.SkillMastery });

            mockFeatFocusGenerator.Setup(g => g.GenerateFrom(FeatConstants.SkillMastery, GroupConstants.Skills, skills, additionalFeatSelections[0].RequiredFeats, preselectedFeats, characterClass))
                .Returns(FeatConstants.Foci.All);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var onlyFeat = feats.Single();
            Assert.That(onlyFeat.Name, Is.EqualTo("feat2"));
        }

        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(4, 2)]
        [TestCase(5, 2)]
        [TestCase(6, 3)]
        [TestCase(7, 3)]
        [TestCase(8, 3)]
        [TestCase(9, 4)]
        [TestCase(10, 4)]
        public void MonsterCanSelectAdditionalFeats(int monsterHitDie, int monsterFeatQuantity)
        {
            race.BaseRace = monsters[0];
            AddFeatSelections(monsterFeatQuantity + 10);

            mockAdjustmentsSelector.Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.MonsterHitDice, race.BaseRace)).Returns(monsterHitDie);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            Assert.That(feats.Count(), Is.EqualTo(monsterFeatQuantity + 1));
        }

        [Test]
        public void NonMonstersDoNotGetAdditionalFeats()
        {
            race.BaseRace = "not a monster";
            AddFeatSelections(10);

            mockAdjustmentsSelector.Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.MonsterHitDice, race.BaseRace)).Returns(10);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            Assert.That(feats.Count(), Is.EqualTo(1));
        }

        [Test]
        public void AllDataFromFeatSelectionIsCopiedToFeat()
        {
            var selection = new AdditionalFeatSelection();
            selection.Feat = "additional feat";
            selection.FocusType = "focus type";
            selection.Frequency.Quantity = 9266;
            selection.Frequency.TimePeriod = "frequency time period";
            selection.Power = 12345;

            additionalFeatSelections.Add(selection);

            mockFeatFocusGenerator.SetupSequence(g => g.GenerateFrom("additional feat", "focus type", skills, additionalFeatSelections[0].RequiredFeats, It.IsAny<IEnumerable<Feat>>(), characterClass))
                .Returns("focus").Returns(FeatConstants.Foci.All);

            var feats = additionalFeatsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack, preselectedFeats);
            var feat = feats.Single();
            Assert.That(feat.Foci.Single(), Is.EqualTo("focus"));
            Assert.That(feat.Frequency.Quantity, Is.EqualTo(9266));
            Assert.That(feat.Frequency.TimePeriod, Is.EqualTo("frequency time period"));
            Assert.That(feat.Name, Is.EqualTo("additional feat"));
            Assert.That(feat.Power, Is.EqualTo(12345));
        }
    }
}