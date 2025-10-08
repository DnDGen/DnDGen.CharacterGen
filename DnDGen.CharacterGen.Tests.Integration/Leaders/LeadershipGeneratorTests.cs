using DnDGen.CharacterGen.Leaders;
using NUnit.Framework;

namespace DnDGen.CharacterGen.Tests.Integration.Leaders
{
    [TestFixture]
    public class LeadershipGeneratorTests : IntegrationTests
    {
        private ILeadershipGenerator leadershipGenerator;
        private int attempts;

        [SetUp]
        public void Setup()
        {
            leadershipGenerator = GetNewInstanceOf<ILeadershipGenerator>();
            attempts = 10;
        }

        [Test]
        public void NoFollowersHappen()
        {
            var hasFollowers = true;
            var leadership = new Leadership();

            while (attempts-- > 0 && hasFollowers)
            {
                leadership = leadershipGenerator.GenerateLeadership(6, -2, string.Empty);
                hasFollowers = leadership.FollowerQuantities.Level1 > 0;
            }

            Assert.That(hasFollowers, Is.False);

            Assert.That(leadership, Is.Not.Null);
            Assert.That(leadership.FollowerQuantities.Level1, Is.AtLeast(leadership.FollowerQuantities.Level2));
            Assert.That(leadership.FollowerQuantities.Level2, Is.InRange(leadership.FollowerQuantities.Level3, leadership.FollowerQuantities.Level1));
            Assert.That(leadership.FollowerQuantities.Level3, Is.InRange(leadership.FollowerQuantities.Level4, leadership.FollowerQuantities.Level2));
            Assert.That(leadership.FollowerQuantities.Level4, Is.InRange(leadership.FollowerQuantities.Level5, leadership.FollowerQuantities.Level3));
            Assert.That(leadership.FollowerQuantities.Level5, Is.InRange(leadership.FollowerQuantities.Level6, leadership.FollowerQuantities.Level4));
            Assert.That(leadership.FollowerQuantities.Level6, Is.InRange(0, leadership.FollowerQuantities.Level5));
            Assert.That(leadership.FollowerQuantities.Level1, Is.Zero);
            Assert.That(leadership.FollowerQuantities.Level2, Is.Zero);
            Assert.That(leadership.FollowerQuantities.Level3, Is.Zero);
            Assert.That(leadership.FollowerQuantities.Level4, Is.Zero);
            Assert.That(leadership.FollowerQuantities.Level5, Is.Zero);
            Assert.That(leadership.FollowerQuantities.Level6, Is.Zero);
            Assert.That(leadership.LeadershipModifiers, Is.Not.Null);
        }

        [Test]
        public void FollowersHappen()
        {
            var hasFollowers = false;
            var leadership = new Leadership();

            while (attempts-- > 0 && !hasFollowers)
            {
                leadership = leadershipGenerator.GenerateLeadership(10, 0, string.Empty);
                hasFollowers = leadership.FollowerQuantities.Level1 > 0;
            }

            Assert.That(hasFollowers, Is.True);

            Assert.That(leadership, Is.Not.Null);
            Assert.That(leadership.FollowerQuantities.Level1, Is.GreaterThan(leadership.FollowerQuantities.Level2));
            Assert.That(leadership.FollowerQuantities.Level2, Is.InRange(leadership.FollowerQuantities.Level3, leadership.FollowerQuantities.Level1));
            Assert.That(leadership.FollowerQuantities.Level3, Is.InRange(leadership.FollowerQuantities.Level4, leadership.FollowerQuantities.Level2));
            Assert.That(leadership.FollowerQuantities.Level4, Is.InRange(leadership.FollowerQuantities.Level5, leadership.FollowerQuantities.Level3));
            Assert.That(leadership.FollowerQuantities.Level5, Is.InRange(leadership.FollowerQuantities.Level6, leadership.FollowerQuantities.Level4));
            Assert.That(leadership.FollowerQuantities.Level6, Is.InRange(0, leadership.FollowerQuantities.Level5));
            Assert.That(leadership.FollowerQuantities.Level1, Is.Positive);
            Assert.That(leadership.FollowerQuantities.Level2, Is.Not.Negative);
            Assert.That(leadership.FollowerQuantities.Level3, Is.Not.Negative);
            Assert.That(leadership.FollowerQuantities.Level4, Is.Not.Negative);
            Assert.That(leadership.FollowerQuantities.Level5, Is.Not.Negative);
            Assert.That(leadership.FollowerQuantities.Level6, Is.Not.Negative);
            Assert.That(leadership.LeadershipModifiers, Is.Not.Null);
        }

        [Test]
        public void SomeFollowersHappen()
        {
            var hasFollowers = false;
            var leadership = new Leadership();

            while (attempts-- > 0 && !hasFollowers)
            {
                leadership = leadershipGenerator.GenerateLeadership(15, 2, string.Empty);
                hasFollowers = leadership.FollowerQuantities.Level1 > 0;
            }

            Assert.That(hasFollowers, Is.True);

            Assert.That(leadership, Is.Not.Null);
            Assert.That(leadership.FollowerQuantities.Level1, Is.GreaterThan(leadership.FollowerQuantities.Level2));
            Assert.That(leadership.FollowerQuantities.Level2, Is.InRange(leadership.FollowerQuantities.Level3, leadership.FollowerQuantities.Level1));
            Assert.That(leadership.FollowerQuantities.Level3, Is.InRange(leadership.FollowerQuantities.Level4, leadership.FollowerQuantities.Level2));
            Assert.That(leadership.FollowerQuantities.Level4, Is.InRange(leadership.FollowerQuantities.Level5, leadership.FollowerQuantities.Level3));
            Assert.That(leadership.FollowerQuantities.Level5, Is.InRange(leadership.FollowerQuantities.Level6, leadership.FollowerQuantities.Level4));
            Assert.That(leadership.FollowerQuantities.Level6, Is.InRange(0, leadership.FollowerQuantities.Level5));
            Assert.That(leadership.FollowerQuantities.Level1, Is.Positive);
            Assert.That(leadership.FollowerQuantities.Level2, Is.Not.Negative);
            Assert.That(leadership.FollowerQuantities.Level3, Is.Not.Negative);
            Assert.That(leadership.FollowerQuantities.Level4, Is.Not.Negative);
            Assert.That(leadership.FollowerQuantities.Level5, Is.Not.Negative);
            Assert.That(leadership.FollowerQuantities.Level6, Is.Zero);
            Assert.That(leadership.LeadershipModifiers, Is.Not.Null);
        }

        [Test]
        public void AllFollowersHappen()
        {
            var hasFollowers = false;
            var leadership = new Leadership();

            while (attempts-- > 0 && !hasFollowers)
            {
                leadership = leadershipGenerator.GenerateLeadership(20, 5, string.Empty);
                hasFollowers = leadership.FollowerQuantities.Level6 > 0;
            }

            Assert.That(hasFollowers, Is.True);

            Assert.That(leadership, Is.Not.Null);
            Assert.That(leadership.FollowerQuantities.Level1, Is.GreaterThan(leadership.FollowerQuantities.Level2));
            Assert.That(leadership.FollowerQuantities.Level2, Is.InRange(leadership.FollowerQuantities.Level3, leadership.FollowerQuantities.Level1));
            Assert.That(leadership.FollowerQuantities.Level3, Is.InRange(leadership.FollowerQuantities.Level4, leadership.FollowerQuantities.Level2));
            Assert.That(leadership.FollowerQuantities.Level4, Is.InRange(leadership.FollowerQuantities.Level5, leadership.FollowerQuantities.Level3));
            Assert.That(leadership.FollowerQuantities.Level5, Is.InRange(leadership.FollowerQuantities.Level6, leadership.FollowerQuantities.Level4));
            Assert.That(leadership.FollowerQuantities.Level6, Is.InRange(0, leadership.FollowerQuantities.Level5));
            Assert.That(leadership.FollowerQuantities.Level1, Is.Positive);
            Assert.That(leadership.FollowerQuantities.Level2, Is.Positive);
            Assert.That(leadership.FollowerQuantities.Level3, Is.Positive);
            Assert.That(leadership.FollowerQuantities.Level4, Is.Positive);
            Assert.That(leadership.FollowerQuantities.Level5, Is.Positive);
            Assert.That(leadership.FollowerQuantities.Level6, Is.Positive);
            Assert.That(leadership.LeadershipModifiers, Is.Not.Null);
        }
    }
}
