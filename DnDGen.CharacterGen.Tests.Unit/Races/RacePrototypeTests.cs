using DnDGen.CharacterGen.Races;
using NUnit.Framework;
using System;

namespace DnDGen.CharacterGen.Tests.Unit.Races
{
    [TestFixture]
    public class RacePrototypeTests
    {
        private RacePrototype race;

        [SetUp]
        public void Setup()
        {
            race = new RacePrototype();
        }

        [Test]
        public void Summary_ReturnsBaseRace()
        {
            race.BaseRace = "base race";

            Assert.That(race.Summary, Is.EqualTo("base race"));
        }

        [Test]
        public void Summary_ReturnsBaseRace_NoMetarace()
        {
            race.BaseRace = "base race";
            race.Metarace = RaceConstants.Metaraces.None;

            Assert.That(race.Summary, Is.EqualTo("base race"));
        }

        [Test]
        public void Summary_ReturnsBaseRaceAndMetarace()
        {
            race.BaseRace = "base race";
            race.Metarace = "metarace";

            Assert.That(race.Summary, Is.EqualTo("metarace base race"));
        }

        [Test]
        public void PrototypeInitialized()
        {
            Assert.That(race.BaseRace, Is.Empty);
            Assert.That(race.Metarace, Is.Empty);
        }

        [Test]
        public void ToStringIsSummary()
        {
            race.BaseRace = Guid.NewGuid().ToString();

            Assert.That(race.ToString(), Is.EqualTo(race.Summary));
        }

        [Test]
        public void ToStringIsSummary_WithMetarace()
        {
            race.BaseRace = Guid.NewGuid().ToString();
            race.Metarace = Guid.NewGuid().ToString();

            Assert.That(race.ToString(), Is.EqualTo(race.Summary));
        }

        [Test]
        public void ConvertingToStringUsesSummary()
        {
            race.BaseRace = Guid.NewGuid().ToString();

            var alignmentString = Convert.ToString(race);
            Assert.That(alignmentString, Is.EqualTo(race.Summary));
        }

        [Test]
        public void ConvertingToStringUsesSummary_WithMetarace()
        {
            race.BaseRace = Guid.NewGuid().ToString();
            race.Metarace = Guid.NewGuid().ToString();

            var alignmentString = Convert.ToString(race);
            Assert.That(alignmentString, Is.EqualTo(race.Summary));
        }

        [Test]
        public void PrototypeIsNotEqualIfOtherItemNotRacePrototype()
        {
            race.BaseRace = Guid.NewGuid().ToString();
            var otherRace = new object();

            Assert.That(race, Is.Not.EqualTo(otherRace));
        }

        [Test]
        public void PrototypeIsNotEqualIfBaseRaceDiffers()
        {
            race.BaseRace = "My Base Race";
            race.Metarace = "My Metarace";

            var otherRace = new RacePrototype();
            otherRace.BaseRace = "My Other Base Race";
            otherRace.Metarace = "My Metarace";

            Assert.That(race, Is.Not.EqualTo(otherRace));
        }

        [Test]
        public void PrototypeIsNotEqualIfMetaraceDiffers()
        {
            race.BaseRace = "My Base Race";
            race.Metarace = "My Metarace";

            var otherRace = new RacePrototype();
            otherRace.BaseRace = "My Base Race";
            otherRace.Metarace = "My Other Metarace";

            Assert.That(race, Is.Not.EqualTo(otherRace));
        }

        [Test]
        public void PrototypeIsEqualIfBaseRacesAndMetaracesMatch()
        {
            race.BaseRace = "My Base Race";
            race.Metarace = "My Metarace";

            var otherRace = new RacePrototype();
            otherRace.BaseRace = "My Base Race";
            otherRace.Metarace = "My Metarace";

            Assert.That(race, Is.EqualTo(otherRace));
        }

        [Test]
        public void HashCodeIsHashOfSummary()
        {
            race.BaseRace = "My Base Race";
            race.Metarace = "My Metarace";

            var raceHash = race.GetHashCode();
            var raceToStringHash = race.ToString().GetHashCode();

            Assert.That(raceHash, Is.EqualTo(raceToStringHash));
        }
    }
}