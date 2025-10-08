using DnDGen.CharacterGen.Feats;
using Newtonsoft.Json;
using NUnit.Framework;
using System;

namespace DnDGen.CharacterGen.Tests.Unit.Feats
{
    [TestFixture]
    public class FeatTests
    {
        private Feat feat;
        private Random random;

        [SetUp]
        public void Setup()
        {
            feat = new Feat();
            random = new Random();
        }

        [Test]
        public void FeatInitialized()
        {
            Assert.That(feat.Name, Is.Not.Null);
            Assert.That(feat.Foci, Is.Empty);
            Assert.That(feat.Power, Is.EqualTo(0));
            Assert.That(feat.Frequency, Is.Not.Null);
        }

        [Test]
        public void CloneFeat()
        {
            feat.CanBeTakenMultipleTimes = Convert.ToBoolean(random.Next(2));
            feat.Foci = [Guid.NewGuid().ToString(), Guid.NewGuid().ToString()];
            feat.Frequency.Quantity = random.Next();
            feat.Frequency.TimePeriod = Guid.NewGuid().ToString();
            feat.Name = Guid.NewGuid().ToString();
            feat.Power = random.Next();

            var clone = feat.Clone();
            Assert.That(clone, Is.Not.SameAs(feat));
            Assert.That(JsonConvert.SerializeObject(clone), Is.EqualTo(JsonConvert.SerializeObject(feat)));
            Assert.That(clone.CanBeTakenMultipleTimes, Is.EqualTo(feat.CanBeTakenMultipleTimes));
            Assert.That(clone.Foci, Is.Not.SameAs(feat.Foci));
            Assert.That(clone.Foci, Is.EqualTo(feat.Foci));
            Assert.That(clone.Frequency.Quantity, Is.EqualTo(feat.Frequency.Quantity));
            Assert.That(clone.Frequency.TimePeriod, Is.EqualTo(feat.Frequency.TimePeriod));
            Assert.That(clone.Name, Is.EqualTo(feat.Name));
            Assert.That(clone.Power, Is.EqualTo(feat.Power));
        }

        [Test]
        public void Summary_ReturnsName()
        {
            feat.Name = "my feat";
            Assert.That(feat.Summary, Is.EqualTo("my feat"));
        }

        [Test]
        public void Summary_ReturnsNameWithFocus()
        {
            feat.Name = "my feat";
            feat.Foci = ["my focus"];

            Assert.That(feat.Summary, Is.EqualTo("my feat (my focus)"));
        }

        [Test]
        public void Summary_ReturnsNameWithFoci()
        {
            feat.Name = "my feat";
            feat.Foci = ["my focus", "my other focus"];

            Assert.That(feat.Summary, Is.EqualTo("my feat (my focus, my other focus)"));
        }

        [Test]
        public void ToString_ReturnsSummary()
        {
            feat.Name = "my feat";
            feat.Foci = ["my focus", "my other focus"];

            Assert.That(feat.ToString(), Is.EqualTo("my feat (my focus, my other focus)"));
        }
    }
}