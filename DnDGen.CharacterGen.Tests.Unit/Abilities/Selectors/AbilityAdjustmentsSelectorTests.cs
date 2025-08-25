using DnDGen.CharacterGen.Abilities.Selectors;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Selectors;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Unit.Abilities.Selectors
{
    [TestFixture]
    public class AbilityAdjustmentsSelectorTests
    {
        private IAbilityAdjustmentsSelector selector;
        private Race race;
        private Mock<IAdjustmentsSelector> mockInnerSelector;
        private Dictionary<string, Dictionary<string, int>> allAdjustments;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private List<string> abilityNames;

        [SetUp]
        public void Setup()
        {
            mockInnerSelector = new Mock<IAdjustmentsSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            selector = new AbilityAdjustmentsSelector(mockInnerSelector.Object, mockCollectionsSelector.Object);
            race = new Race();
            allAdjustments = new Dictionary<string, Dictionary<string, int>>();
            abilityNames = new List<string>();

            race.BaseRace = "base race";
            race.Metarace = "metarace";
            race.Age.Description = "super old";

            allAdjustments[race.BaseRace] = new Dictionary<string, int>();
            allAdjustments[race.Metarace] = new Dictionary<string, int>();
            allAdjustments[race.Age.Description] = new Dictionary<string, int>();

            abilityNames.Add("first ability");
            abilityNames.Add("second ability");

            var tableName = string.Format(TableNameConstants.Formattable.Adjustments.AGEAbilityAdjustments, race.Age.Description);
            mockInnerSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(allAdjustments[race.Age.Description]);

            tableName = string.Format(TableNameConstants.Formattable.Adjustments.RACEAbilityAdjustments, race.BaseRace);
            mockInnerSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(allAdjustments[race.BaseRace]);

            tableName = string.Format(TableNameConstants.Formattable.Adjustments.RACEAbilityAdjustments, race.Metarace);
            mockInnerSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(allAdjustments[race.Metarace]);

            foreach (var ability in abilityNames)
            {
                allAdjustments[race.Metarace][ability] = 0;
                allAdjustments[race.BaseRace][ability] = 0;
                allAdjustments[race.Age.Description][ability] = 0;
            }

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.AbilityGroups, GroupConstants.All)).Returns(abilityNames);
        }

        [Test]
        public void AdjustmentsContainAllAbilities()
        {
            var adjustments = selector.SelectFor(race);
            Assert.That(adjustments.Keys, Is.EquivalentTo(abilityNames));
        }

        [Test]
        public void ApplyBaseRaceAdjustmentsToAbilityAdjustments()
        {
            var adjustment = 1;
            foreach (var ability in abilityNames)
            {
                allAdjustments[race.BaseRace][ability] = adjustment++;
            }

            var adjustments = selector.SelectFor(race);
            Assert.That(adjustments.Keys, Is.EquivalentTo(abilityNames));

            for (var i = 0; i < abilityNames.Count; i++)
            {
                Assert.That(adjustments[abilityNames[i]], Is.EqualTo(i + 1));
            }
        }

        [Test]
        public void ApplyMetaraceAdjustmentsToAbilityAdjustments()
        {
            var adjustment = 1;
            foreach (var ability in abilityNames)
            {
                allAdjustments[race.Metarace][ability] = adjustment++;
            }

            var adjustments = selector.SelectFor(race);
            Assert.That(adjustments.Keys, Is.EquivalentTo(abilityNames));

            for (var i = 0; i < abilityNames.Count; i++)
            {
                Assert.That(adjustments[abilityNames[i]], Is.EqualTo(i + 1));
            }
        }

        [Test]
        public void ApplyAgingEffectsToAbilityAdjustments()
        {
            var adjustment = 1;
            foreach (var ability in abilityNames)
            {
                allAdjustments[race.Age.Description][ability] = adjustment++;
            }

            var adjustments = selector.SelectFor(race);
            Assert.That(adjustments.Keys, Is.EquivalentTo(abilityNames));

            for (var i = 0; i < abilityNames.Count; i++)
            {
                Assert.That(adjustments[abilityNames[i]], Is.EqualTo(i + 1));
            }
        }

        [Test]
        public void ApplyAllAdjustmentsToAbilityAdjustments()
        {
            var adjustment = 1;
            foreach (var ability in abilityNames)
            {
                allAdjustments[race.BaseRace][ability] = adjustment++;
                allAdjustments[race.Metarace][ability] = adjustment++;
                allAdjustments[race.Age.Description][ability] = adjustment++;
            }

            var adjustments = selector.SelectFor(race);
            Assert.That(adjustments.Keys, Is.EquivalentTo(abilityNames));

            for (var i = 0; i < abilityNames.Count; i++)
            {
                var expected = 9 * i + 6;
                Assert.That(adjustments[abilityNames[i]], Is.EqualTo(expected));
            }
        }

        [Test]
        public void ApplyPositiveAndNegativeAdjustmentsToAbilityAdjustments()
        {
            var adjustment = 1;
            foreach (var ability in abilityNames)
            {
                allAdjustments[race.BaseRace][ability] = adjustment++ * Convert.ToInt32(Math.Pow(-1, adjustment));
                allAdjustments[race.Metarace][ability] = adjustment++ * Convert.ToInt32(Math.Pow(-1, adjustment));
                allAdjustments[race.Age.Description][ability] = adjustment++ * Convert.ToInt32(Math.Pow(-1, adjustment));
            }

            var adjustments = selector.SelectFor(race);
            Assert.That(adjustments.Keys, Is.EquivalentTo(abilityNames));

            for (var i = 0; i < abilityNames.Count; i++)
            {
                var expected = (3 * i + 2) * Math.Pow(-1, i);
                Assert.That(adjustments[abilityNames[i]], Is.EqualTo(expected));
            }
        }
    }
}