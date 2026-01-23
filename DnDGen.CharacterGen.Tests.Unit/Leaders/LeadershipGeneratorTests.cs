using DnDGen.CharacterGen.Abilities.Randomizers;
using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Alignments.Randomizers;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using DnDGen.CharacterGen.Characters;
using DnDGen.CharacterGen.Leaders;
using DnDGen.CharacterGen.Leaders.Selectors;
using DnDGen.CharacterGen.Races.Randomizers;
using DnDGen.CharacterGen.Selectors;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Leaders
{
    [TestFixture]
    public class LeadershipGeneratorTests
    {
        private ILeadershipGenerator leadershipGenerator;
        private Mock<ICharacterGenerator> mockCharacterGenerator;
        private Mock<ILeadershipSelector> mockLeadershipSelector;
        private Mock<IPercentileSelector> mockPercentileSelector;
        private Mock<IAdjustmentsSelector> mockAdjustmentsSelector;
        private Mock<ISetLevelRandomizer> mockSetLevelRandomizer;
        private Mock<ISetAlignmentRandomizer> mockSetAlignmentRandomizer;
        private Mock<IClassNameRandomizer> mockAnyPlayerClassNameRandomizer;
        private Mock<RaceRandomizer> mockBaseRaceRandomizer;
        private Mock<RaceRandomizer> mockMetaraceRandomizer;
        private Mock<IAbilitiesRandomizer> mockAbilityRandomizer;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<IClassNameRandomizer> mockAnyNPCClassNameRandomizer;
        private Mock<JustInTimeFactory> mockJustInTimeFactory;
        private List<string> allowedAlignments;
        private string leaderAlignment;
        private FollowerQuantities followerQuantities;
        private List<string> npcClasses;

        [SetUp]
        public void Setup()
        {
            mockCharacterGenerator = new Mock<ICharacterGenerator>();
            mockLeadershipSelector = new Mock<ILeadershipSelector>();
            mockPercentileSelector = new Mock<IPercentileSelector>();
            mockAdjustmentsSelector = new Mock<IAdjustmentsSelector>();
            mockSetLevelRandomizer = new Mock<ISetLevelRandomizer>();
            mockSetAlignmentRandomizer = new Mock<ISetAlignmentRandomizer>();
            mockAnyPlayerClassNameRandomizer = new Mock<IClassNameRandomizer>();
            mockBaseRaceRandomizer = new Mock<RaceRandomizer>();
            mockMetaraceRandomizer = new Mock<RaceRandomizer>();
            mockAbilityRandomizer = new Mock<IAbilitiesRandomizer>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockAnyNPCClassNameRandomizer = new Mock<IClassNameRandomizer>();
            mockJustInTimeFactory = new Mock<JustInTimeFactory>();
            leadershipGenerator = new LeadershipGenerator(
                mockCharacterGenerator.Object,
                mockLeadershipSelector.Object,
                mockPercentileSelector.Object,
                mockAdjustmentsSelector.Object,
                mockCollectionsSelector.Object,
                mockJustInTimeFactory.Object);

            allowedAlignments = [];
            followerQuantities = new FollowerQuantities();
            npcClasses = [];

            mockLeadershipSelector.Setup(s => s.SelectFollowerQuantitiesFor(It.IsAny<int>())).Returns(new FollowerQuantities());
            mockSetLevelRandomizer.SetupAllProperties();
            mockSetAlignmentRandomizer.SetupAllProperties();
            leaderAlignment = "leader alignment";
            allowedAlignments.Add(leaderAlignment);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.AlignmentGroups, leaderAlignment))
                .Returns(allowedAlignments);

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.NPCs)).Returns(npcClasses);

            var index = 0;
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.AlignmentGroups, leaderAlignment))
                .Returns(allowedAlignments);
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<string>>()))
                .Returns((IEnumerable<string> cc) => cc.ElementAt(index++ % cc.Count()));

            mockJustInTimeFactory.Setup(f => f.Build<ISetAlignmentRandomizer>()).Returns(mockSetAlignmentRandomizer.Object);
            mockJustInTimeFactory.Setup(f => f.Build<ISetLevelRandomizer>()).Returns(mockSetLevelRandomizer.Object);
            mockJustInTimeFactory.Setup(f => f.Build<RaceRandomizer>(RaceRandomizerTypeConstants.BaseRace.AnyBase)).Returns(mockBaseRaceRandomizer.Object);
            mockJustInTimeFactory.Setup(f => f.Build<RaceRandomizer>(RaceRandomizerTypeConstants.Metarace.AnyMeta)).Returns(mockMetaraceRandomizer.Object);
            mockJustInTimeFactory.Setup(f => f.Build<IAbilitiesRandomizer>(AbilitiesRandomizerTypeConstants.BestOfFour)).Returns(mockAbilityRandomizer.Object);
            mockJustInTimeFactory.Setup(f => f.Build<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.AnyNPC)).Returns(mockAnyNPCClassNameRandomizer.Object);
            mockJustInTimeFactory.Setup(f => f.Build<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.AnyPlayer)).Returns(mockAnyPlayerClassNameRandomizer.Object);
        }

        [Test]
        public void LeadershipScoreIsLevelPlusCharismaModifier()
        {
            var leadership = leadershipGenerator.GenerateLeadership(9266, 90210, string.Empty);
            Assert.That(leadership.Score, Is.EqualTo(99476));
            Assert.That(leadership.CohortScore, Is.EqualTo(99476));
        }

        [Test]
        public void CharacterReputationGenerated()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Percentile.Reputation)).Returns("reputable");

            var reputationAjustments = new Dictionary<string, int>
            {
                ["reputable"] = 0
            };
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(TableNameConstants.Set.Adjustments.LeadershipModifiers)).Returns(reputationAjustments);

            var leadership = leadershipGenerator.GenerateLeadership(9266, 90210, string.Empty);
            Assert.That(leadership.LeadershipModifiers, Contains.Item("reputable"));
        }

        [Test]
        public void CharacterReputationAdjustmentIsApplied()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Percentile.Reputation)).Returns("reputable");

            var reputationAjustments = new Dictionary<string, int>
            {
                ["reputable"] = 42
            };
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(TableNameConstants.Set.Adjustments.LeadershipModifiers)).Returns(reputationAjustments);

            var leadership = leadershipGenerator.GenerateLeadership(9266, 90210, string.Empty);
            Assert.That(leadership.LeadershipModifiers, Contains.Item("reputable"));
            Assert.That(leadership.Score, Is.EqualTo(99518));
            Assert.That(leadership.CohortScore, Is.EqualTo(99518));
        }

        [Test]
        public void NegativeCharacterReputationAdjustmentIsApplied()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Percentile.Reputation)).Returns("reputable");

            var reputationAjustments = new Dictionary<string, int>
            {
                ["reputable"] = -42
            };
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(TableNameConstants.Set.Adjustments.LeadershipModifiers)).Returns(reputationAjustments);

            var leadership = leadershipGenerator.GenerateLeadership(9266, 90210, string.Empty);
            Assert.That(leadership.LeadershipModifiers, Contains.Item("reputable"));
            Assert.That(leadership.Score, Is.EqualTo(99434));
            Assert.That(leadership.CohortScore, Is.EqualTo(99434));
        }

        [Test]
        public void AnimalsDecreaseCohortScoreBy2()
        {
            var leadership = leadershipGenerator.GenerateLeadership(9266, 90210, "animal");
            Assert.That(leadership.Score, Is.EqualTo(99476));
            Assert.That(leadership.CohortScore, Is.EqualTo(99474));
        }

        [Test]
        public void KillingCohortsDecreasesScoreOfAttractingCohorts()
        {
            mockPercentileSelector.SetupSequence(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.KilledCohort)).Returns(true).Returns(false);

            var leadership = leadershipGenerator.GenerateLeadership(9266, 90210, string.Empty);
            Assert.That(leadership.Score, Is.EqualTo(99476));
            Assert.That(leadership.CohortScore, Is.EqualTo(99474));
            Assert.That(leadership.LeadershipModifiers, Contains.Item("Caused the death of 1 cohort(s)"));
        }

        [Test]
        public void KillingMultipleCohortsDecreasesScoreOfAttractingCohorts()
        {
            mockPercentileSelector.SetupSequence(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.KilledCohort)).Returns(true).Returns(true).Returns(false);

            var leadership = leadershipGenerator.GenerateLeadership(9266, 90210, string.Empty);
            Assert.That(leadership.Score, Is.EqualTo(99476));
            Assert.That(leadership.CohortScore, Is.EqualTo(99472));
            Assert.That(leadership.LeadershipModifiers, Contains.Item("Caused the death of 2 cohort(s)"));
        }

        [Test]
        public void GetFollowerQuantities()
        {
            mockLeadershipSelector.Setup(s => s.SelectFollowerQuantitiesFor(99476)).Returns(followerQuantities);
            var leadership = leadershipGenerator.GenerateLeadership(9266, 90210, string.Empty);
            Assert.That(leadership.FollowerQuantities, Is.EqualTo(followerQuantities));
        }

        [Test]
        public void GenerateLeadershipMovementFactorsAndApplyThem()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Percentile.LeadershipMovement)).Returns("moves");

            var leadershipAdjustments = new Dictionary<string, int>
            {
                ["moves"] = 42,
                ["murders"] = -5
            };

            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(TableNameConstants.Set.Adjustments.LeadershipModifiers)).Returns(leadershipAdjustments);
            mockLeadershipSelector.Setup(s => s.SelectFollowerQuantitiesFor(99518)).Returns(followerQuantities);

            var leadership = leadershipGenerator.GenerateLeadership(9266, 90210, string.Empty);
            Assert.That(leadership.Score, Is.EqualTo(99476));
            Assert.That(leadership.CohortScore, Is.EqualTo(99476));
            Assert.That(leadership.FollowerQuantities, Is.EqualTo(followerQuantities));
        }

        [Test]
        public void CharacterDoesNotHaveEmptyStringLeadershipModifiers()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Percentile.LeadershipMovement)).Returns(string.Empty);
            var leadership = leadershipGenerator.GenerateLeadership(9266, 90210, string.Empty);
            Assert.That(leadership.LeadershipModifiers, Is.Empty);
        }

        [Test]
        public void GenerateWhetherCharacterHasCausedFollowerDeathsAndApply()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.KilledFollowers)).Returns(true);
            mockLeadershipSelector.Setup(s => s.SelectFollowerQuantitiesFor(99475)).Returns(followerQuantities);

            var leadership = leadershipGenerator.GenerateLeadership(9266, 90210, string.Empty);
            Assert.That(leadership.Score, Is.EqualTo(99476));
            Assert.That(leadership.CohortScore, Is.EqualTo(99476));
            Assert.That(leadership.FollowerQuantities, Is.EqualTo(followerQuantities));
        }

        [Test]
        public void GenerateCohort()
        {
            mockLeadershipSelector.Setup(s => s.SelectCohortLevelFor(9266)).Returns(42);

            var cohort = new Character();
            mockCharacterGenerator
                .Setup(g => g.GenerateWith(
                    mockSetAlignmentRandomizer.Object,
                    mockAnyPlayerClassNameRandomizer.Object,
                    mockSetLevelRandomizer.Object,
                    mockBaseRaceRandomizer.Object,
                    mockMetaraceRandomizer.Object,
                    mockAbilityRandomizer.Object))
                .Returns(cohort);

            var generatedCohort = leadershipGenerator.GenerateCohort(9266, 90210, leaderAlignment, "class name");
            Assert.That(generatedCohort, Is.EqualTo(cohort));
            mockSetLevelRandomizer.VerifySet(r => r.SetLevel = 42);
        }

        [Test]
        public void GenerateNPCCohort()
        {
            mockLeadershipSelector.Setup(s => s.SelectCohortLevelFor(9266)).Returns(42);
            npcClasses.Add("class name");

            var cohort = new Character();
            mockCharacterGenerator
                .Setup(g => g.GenerateWith(
                    mockSetAlignmentRandomizer.Object,
                    mockAnyNPCClassNameRandomizer.Object,
                    mockSetLevelRandomizer.Object,
                    mockBaseRaceRandomizer.Object,
                    mockMetaraceRandomizer.Object,
                    mockAbilityRandomizer.Object))
                .Returns(cohort);

            var generatedCohort = leadershipGenerator.GenerateCohort(9266, 90210, leaderAlignment, "class name");
            Assert.That(generatedCohort, Is.EqualTo(cohort));
            mockSetLevelRandomizer.VerifySet(r => r.SetLevel = 42);
        }

        [Test]
        public void CohortLevelIs2LessThanLeaderLevel()
        {
            mockLeadershipSelector.Setup(s => s.SelectCohortLevelFor(9266)).Returns(90210);

            var cohort = new Character();
            mockCharacterGenerator
                .Setup(g => g.GenerateWith(
                    mockSetAlignmentRandomizer.Object,
                    mockAnyPlayerClassNameRandomizer.Object,
                    mockSetLevelRandomizer.Object,
                    mockBaseRaceRandomizer.Object,
                    mockMetaraceRandomizer.Object,
                    mockAbilityRandomizer.Object))
                .Returns(cohort);

            var generatedCohort = leadershipGenerator.GenerateCohort(9266, 42, leaderAlignment, "class name");
            Assert.That(generatedCohort, Is.EqualTo(cohort));
            mockSetLevelRandomizer.VerifySet(r => r.SetLevel = 40);
        }

        [Test]
        public void AttractCohortOfSameAlignment()
        {
            mockLeadershipSelector.Setup(s => s.SelectCohortLevelFor(9266)).Returns(42);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.AttractCohortOfDifferentAlignment)).Returns(false);

            var cohortAlignment = new Alignment("cohort alignment");
            var leadersAlignment = new Alignment(leaderAlignment);
            allowedAlignments.Add(cohortAlignment.ToString());

            var cohort = new Character();
            mockCharacterGenerator
                .Setup(g => g.GenerateWith(
                    mockSetAlignmentRandomizer.Object,
                    mockAnyPlayerClassNameRandomizer.Object,
                    mockSetLevelRandomizer.Object,
                    mockBaseRaceRandomizer.Object,
                    mockMetaraceRandomizer.Object,
                    mockAbilityRandomizer.Object))
                .Returns(cohort);

            var generatedCohort = leadershipGenerator.GenerateCohort(9266, 90210, leaderAlignment, "class name");
            Assert.That(generatedCohort, Is.EqualTo(cohort));
            mockSetAlignmentRandomizer.VerifySet(r => r.SetAlignment = leadersAlignment);
        }

        [Test]
        public void AttractCohortOfDifferingAlignment()
        {
            mockLeadershipSelector.Setup(s => s.SelectCohortLevelFor(9265)).Returns(42);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.AttractCohortOfDifferentAlignment)).Returns(true);

            var cohortAlignment = new Alignment("cohort alignment");
            allowedAlignments.Add(cohortAlignment.ToString());

            var cohort = new Character();
            mockCharacterGenerator
                .Setup(g => g.GenerateWith(
                    mockSetAlignmentRandomizer.Object,
                    mockAnyPlayerClassNameRandomizer.Object,
                    mockSetLevelRandomizer.Object,
                    mockBaseRaceRandomizer.Object,
                    mockMetaraceRandomizer.Object,
                    mockAbilityRandomizer.Object))
                .Returns(cohort);

            var generatedCohort = leadershipGenerator.GenerateCohort(9266, 90210, leaderAlignment, "class name");
            Assert.That(generatedCohort, Is.EqualTo(cohort));
            mockSetAlignmentRandomizer.VerifySet(r => r.SetAlignment = cohortAlignment);
        }

        [Test]
        public void IfSelectedCohortLevelIs0_DoNotGenerateCohort()
        {
            mockLeadershipSelector.Setup(s => s.SelectCohortLevelFor(9266)).Returns(0);

            var cohort = new Character();
            mockCharacterGenerator
                .Setup(g => g.GenerateWith(
                    mockSetAlignmentRandomizer.Object,
                    mockAnyPlayerClassNameRandomizer.Object,
                    mockSetLevelRandomizer.Object,
                    mockBaseRaceRandomizer.Object,
                    mockMetaraceRandomizer.Object,
                    mockAbilityRandomizer.Object))
                .Returns(cohort);

            var generatedCohort = leadershipGenerator.GenerateCohort(9266, 90210, leaderAlignment, "class name");
            Assert.That(generatedCohort, Is.Null);
        }

        [Test]
        public void FollowerGenerated()
        {
            var follower = new Character();
            mockCharacterGenerator
                .Setup(g => g.GenerateWith(
                    mockSetAlignmentRandomizer.Object,
                    mockAnyPlayerClassNameRandomizer.Object,
                    mockSetLevelRandomizer.Object,
                    mockBaseRaceRandomizer.Object,
                    mockMetaraceRandomizer.Object,
                    mockAbilityRandomizer.Object))
                .Returns(follower);

            var generatedFollower = leadershipGenerator.GenerateFollower(9266, leaderAlignment, "class name");
            Assert.That(generatedFollower, Is.EqualTo(follower));
            mockSetLevelRandomizer.VerifySet(r => r.SetLevel = 9266);
        }

        [Test]
        public void NPCFollowerGenerated()
        {
            var follower = new Character();
            mockCharacterGenerator
                .Setup(g => g.GenerateWith(
                    mockSetAlignmentRandomizer.Object,
                    mockAnyNPCClassNameRandomizer.Object,
                    mockSetLevelRandomizer.Object,
                    mockBaseRaceRandomizer.Object,
                    mockMetaraceRandomizer.Object,
                    mockAbilityRandomizer.Object))
                .Returns(follower);
            npcClasses.Add("class name");

            var generatedFollower = leadershipGenerator.GenerateFollower(9266, leaderAlignment, "class name");
            Assert.That(generatedFollower, Is.EqualTo(follower));
            mockSetLevelRandomizer.VerifySet(r => r.SetLevel = 9266);
        }

        [Test]
        public void FollowerCannotOpposeAlignment()
        {
            var followerAlignment = new Alignment("cohort alignment");

            allowedAlignments.Clear();
            allowedAlignments.Add(followerAlignment.ToString());

            var follower = new Character();
            mockCharacterGenerator
                .Setup(g => g.GenerateWith(
                    mockSetAlignmentRandomizer.Object,
                    mockAnyPlayerClassNameRandomizer.Object,
                    mockSetLevelRandomizer.Object,
                    mockBaseRaceRandomizer.Object,
                    mockMetaraceRandomizer.Object,
                    mockAbilityRandomizer.Object))
                .Returns(follower);

            var generatedFollower = leadershipGenerator.GenerateFollower(9266, leaderAlignment, "class name");
            Assert.That(generatedFollower, Is.EqualTo(follower));
            mockSetLevelRandomizer.VerifySet(r => r.SetLevel = 9266);
            mockSetAlignmentRandomizer.VerifySet(r => r.SetAlignment = followerAlignment);
        }
    }
}
