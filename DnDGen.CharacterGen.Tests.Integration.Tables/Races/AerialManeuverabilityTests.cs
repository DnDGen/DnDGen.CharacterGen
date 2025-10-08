using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Races;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Tables.Races
{
    [TestFixture]
    public class AerialManeuverabilityTests : CollectionTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Set.Collection.AerialManeuverability; }
        }

        [Test]
        public override void CollectionNames()
        {
            var baseRaceGroups = GetTable(TableNameConstants.Set.Collection.BaseRaceGroups);
            var metaraceGroups = GetTable(TableNameConstants.Set.Collection.MetaraceGroups);

            var names = baseRaceGroups[GroupConstants.All].Union(metaraceGroups[GroupConstants.All]);
            AssertCollectionNames(names);
        }

        [TestCase(RaceConstants.BaseRaces.Gargoyle, "Average Maneuverability")]
        [TestCase(RaceConstants.BaseRaces.Harpy, "Average Maneuverability")]
        [TestCase(RaceConstants.BaseRaces.Janni, "Perfect Maneuverability")]
        [TestCase(RaceConstants.BaseRaces.OgreMage, "Good Maneuverability")]
        [TestCase(RaceConstants.BaseRaces.Pixie, "Good Maneuverability")]
        [TestCase(RaceConstants.Metaraces.Ghost, "Perfect Maneuverability")]
        [TestCase(RaceConstants.Metaraces.HalfCelestial, "Good Maneuverability")]
        [TestCase(RaceConstants.Metaraces.HalfDragon, "Average Maneuverability")]
        [TestCase(RaceConstants.Metaraces.HalfFiend, "Average Maneuverability")]
        public void AerialManeuverability(string name, string maneuverability)
        {
            AssertDistinctCollection(name, maneuverability);
        }

        [Test]
        public void AerialSpeedOf0HasEmptyManeuverability()
        {
            var aerialSpeeds = GetTable(TableNameConstants.Set.Adjustments.AerialSpeeds);
            var aerialManeuverabilities = GetTable(TableNameConstants.Set.Collection.AerialManeuverability);

            var racesWithNoAerialSpeed = aerialSpeeds.Where(s => s.Value.Single() == "0").Select(s => s.Key);

            Assert.That(racesWithNoAerialSpeed, Is.SubsetOf(aerialManeuverabilities.Keys));

            foreach (var race in racesWithNoAerialSpeed)
            {
                Assert.That(aerialManeuverabilities.Keys, Contains.Item(race));
                Assert.That(aerialManeuverabilities[race].Single(), Is.Empty, race);
            }
        }

        [Test]
        public void PositiveAerialSpeedHasManeuverability()
        {
            var aerialSpeeds = GetTable(TableNameConstants.Set.Adjustments.AerialSpeeds);
            var aerialManeuverabilities = GetTable(TableNameConstants.Set.Collection.AerialManeuverability);

            var racesWithAerialSpeed = aerialSpeeds.Where(s => s.Value.Single() != "0").Select(s => s.Key);

            Assert.That(racesWithAerialSpeed, Is.SubsetOf(aerialManeuverabilities.Keys));

            foreach (var race in racesWithAerialSpeed)
            {
                Assert.That(aerialManeuverabilities.Keys, Contains.Item(race));
                Assert.That(aerialManeuverabilities[race].Single(), Is.Not.Empty, race);
            }
        }
    }
}