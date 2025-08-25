using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Abilities.Randomizers;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using DnDGen.CharacterGen.Leaders;
using DnDGen.CharacterGen.Tests.Integration.Generators.Characters;
using NUnit.Framework;
using System;

namespace DnDGen.CharacterGen.Tests.Integration.Stress.Leaders
{
    [TestFixture]
    public class LeadershipGeneratorTests : StressTests
    {
        private ILeadershipGenerator leadershipGenerator;
        private IAbilitiesRandomizer heroicAbilitiesRandomizer;
        private ILevelRandomizer veryHighLevelRandomizer;
        private Random random;
        private IClassNameRandomizer nPCClassNameRandomizer;
        private CharacterAsserter characterAsserter;

        [SetUp]
        public void Setup()
        {
            characterAsserter = new CharacterAsserter();
            nPCClassNameRandomizer = GetNewInstanceOf<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.AnyNPC);
            random = GetNewInstanceOf<Random>();
            veryHighLevelRandomizer = GetNewInstanceOf<ILevelRandomizer>(LevelRandomizerTypeConstants.VeryHigh);
            heroicAbilitiesRandomizer = GetNewInstanceOf<IAbilitiesRandomizer>(AbilitiesRandomizerTypeConstants.Heroic);
            leadershipGenerator = GetNewInstanceOf<ILeadershipGenerator>();
        }

        [Test]
        public void StressLeadership()
        {
            stressor.Stress(GenerateAndAssertLeadership);
        }

        protected void GenerateAndAssertLeadership()
        {
            var leadership = GenerateLeadership();
            Assert.That(leadership, Is.Not.Null);
            Assert.That(leadership.FollowerQuantities.Level1, Is.AtLeast(leadership.FollowerQuantities.Level2));
            Assert.That(leadership.FollowerQuantities.Level2, Is.InRange(leadership.FollowerQuantities.Level3, leadership.FollowerQuantities.Level1));
            Assert.That(leadership.FollowerQuantities.Level3, Is.InRange(leadership.FollowerQuantities.Level4, leadership.FollowerQuantities.Level2));
            Assert.That(leadership.FollowerQuantities.Level4, Is.InRange(leadership.FollowerQuantities.Level5, leadership.FollowerQuantities.Level3));
            Assert.That(leadership.FollowerQuantities.Level5, Is.InRange(leadership.FollowerQuantities.Level6, leadership.FollowerQuantities.Level4));
            Assert.That(leadership.FollowerQuantities.Level6, Is.AtMost(leadership.FollowerQuantities.Level5));
            Assert.That(leadership.FollowerQuantities.Level1, Is.Not.Negative);
            Assert.That(leadership.FollowerQuantities.Level2, Is.Not.Negative);
            Assert.That(leadership.FollowerQuantities.Level3, Is.Not.Negative);
            Assert.That(leadership.FollowerQuantities.Level4, Is.Not.Negative);
            Assert.That(leadership.FollowerQuantities.Level5, Is.Not.Negative);
            Assert.That(leadership.FollowerQuantities.Level6, Is.Not.Negative);
            Assert.That(leadership.LeadershipModifiers, Is.Not.Null);
        }

        private Leadership GenerateLeadership()
        {
            //INFO: Generating a high-level leader takes too long. Instead, we will generate the individual arguments.  We will ignore animals for now
            var level = veryHighLevelRandomizer.Randomize();
            var abilities = heroicAbilitiesRandomizer.Randomize();

            return leadershipGenerator.GenerateLeadership(level, abilities[AbilityConstants.Charisma].Bonus, string.Empty);
        }

        [Test]
        public void StressCohort()
        {
            stressor.Stress(AssertCohort);
        }

        public void AssertCohort()
        {
            var leader = stressor.Generate(GetCharacterPrototype, p => p.CharacterClass.Level >= 6);
            var cohortScore = random.Next(3, 26);

            var cohort = leadershipGenerator.GenerateCohort(cohortScore, leader.CharacterClass.Level, leader.Alignment.Full, leader.CharacterClass.Name);
            characterAsserter.AssertCharacter(cohort);
            Assert.That(cohort.Equipment.Treasure.Items, Is.Not.Empty);
            Assert.That(cohort.Class.Level, Is.InRange(1, leader.CharacterClass.Level - 2));
        }

        [Test]
        public void StressNPCCohort()
        {
            stressor.Stress(GeneratAndAssertNPCCohort);
        }

        public void GeneratAndAssertNPCCohort()
        {
            var leader = stressor.Generate(GetCharacterPrototype, p => p.CharacterClass.Level >= 6);
            var cohortScore = random.Next(3, 26);

            var cohort = leadershipGenerator.GenerateCohort(cohortScore, leader.CharacterClass.Level, leader.Alignment.Full, leader.CharacterClass.Name);
            characterAsserter.AssertCharacter(cohort);
            Assert.That(cohort.Class.Level, Is.InRange(1, leader.CharacterClass.Level - 2));
        }

        [Test]
        public void StressFollower()
        {
            stressor.Stress(GenerateAndAssertFollower);
        }

        public void GenerateAndAssertFollower()
        {
            var leader = stressor.Generate(GetCharacterPrototype, p => p.CharacterClass.Level >= 6);
            var followerLevel = random.Next(1, 7);

            var follower = leadershipGenerator.GenerateFollower(followerLevel, leader.Alignment.Full, leader.CharacterClass.Name);
            characterAsserter.AssertCharacter(follower);
            Assert.That(follower.Equipment.Treasure.Items, Is.Not.Empty);
            Assert.That(follower.Class.Level, Is.InRange(1, followerLevel));
        }

        [Test]
        public void StressNPCFollower()
        {
            stressor.Stress(GenerateAndAssertNPCFollower);
        }

        public void GenerateAndAssertNPCFollower()
        {
            var leader = stressor.Generate(GetCharacterPrototype, p => p.CharacterClass.Level >= 6);
            var leaderClass = nPCClassNameRandomizer.Randomize(leader.Alignment);
            var followerLevel = random.Next(1, 7);

            var follower = leadershipGenerator.GenerateFollower(followerLevel, leader.Alignment.Full, leaderClass);
            characterAsserter.AssertCharacter(follower);
            Assert.That(follower.Class.Level, Is.InRange(1, followerLevel));
        }
    }
}
