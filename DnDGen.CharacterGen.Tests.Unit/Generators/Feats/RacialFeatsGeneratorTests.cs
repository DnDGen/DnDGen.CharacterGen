using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Generators.Feats;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Selectors.Collections;
using DnDGen.CharacterGen.Selectors.Selections;
using DnDGen.CharacterGen.Skills;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.RollGen;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Generators.Feats
{
    [TestFixture]
    public class RacialFeatsGeneratorTests
    {
        private IRacialFeatsGenerator racialFeatsGenerator;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<IAdjustmentsSelector> mockAdjustmentsSelector;
        private Mock<IFeatsSelector> mockFeatsSelector;
        private Mock<IFeatFocusGenerator> mockFeatFocusGenerator;
        private Mock<Dice> mockDice;
        private Race race;
        private List<RacialFeatSelection> baseRaceFeats;
        private List<RacialFeatSelection> metaraceFeats;
        private List<RacialFeatSelection> speciesFeats;
        private List<Skill> skills;
        private Dictionary<string, Ability> stats;

        [SetUp]
        public void Setup()
        {
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockAdjustmentsSelector = new Mock<IAdjustmentsSelector>();
            mockFeatsSelector = new Mock<IFeatsSelector>();
            mockFeatFocusGenerator = new Mock<IFeatFocusGenerator>();
            mockDice = new Mock<Dice>();
            racialFeatsGenerator = new RacialFeatsGenerator(mockCollectionsSelector.Object, mockAdjustmentsSelector.Object, mockFeatsSelector.Object, mockFeatFocusGenerator.Object, mockDice.Object);

            race = new Race();
            baseRaceFeats = new List<RacialFeatSelection>();
            metaraceFeats = new List<RacialFeatSelection>();
            speciesFeats = new List<RacialFeatSelection>();
            skills = new List<Skill>();
            stats = new Dictionary<string, Ability>();

            race.BaseRace = "base race";
            race.Metarace = "metarace";
            race.MetaraceSpecies = "metarace species";
            stats["stat"] = new Ability("stat");
            stats["stat"].Value = 14;

            mockFeatsSelector.Setup(s => s.SelectRacial("base race")).Returns(baseRaceFeats);
            mockFeatsSelector.Setup(s => s.SelectRacial("metarace")).Returns(metaraceFeats);
            mockFeatsSelector.Setup(s => s.SelectRacial("metarace species")).Returns(speciesFeats);
        }

        [Test]
        public void GetBaseRacialFeats()
        {
            var feat1 = new RacialFeatSelection();
            feat1.Feat = "base race feat 1";

            var feat2 = new RacialFeatSelection();
            feat2.Feat = "base race feat 2";
            feat2.Power = 9266;
            feat2.Frequency.Quantity = 42;
            feat2.Frequency.TimePeriod = "fortnight";

            baseRaceFeats.Add(feat1);
            baseRaceFeats.Add(feat2);

            var feats = racialFeatsGenerator.GenerateWith(race, skills, stats);
            var first = feats.First();
            var last = feats.Last();

            Assert.That(first.Name, Is.EqualTo("base race feat 1"));
            Assert.That(first.Power, Is.EqualTo(0));
            Assert.That(first.Frequency.Quantity, Is.EqualTo(0));
            Assert.That(first.Frequency.TimePeriod, Is.Empty);

            Assert.That(last.Name, Is.EqualTo("base race feat 2"));
            Assert.That(last.Power, Is.EqualTo(9266));
            Assert.That(last.Frequency.Quantity, Is.EqualTo(42));
            Assert.That(last.Frequency.TimePeriod, Is.EqualTo("fortnight"));
        }

        [Test]
        public void GetMetaracialFeats()
        {
            var feat1 = new RacialFeatSelection();
            feat1.Feat = "metarace feat 1";

            var feat2 = new RacialFeatSelection();
            feat2.Feat = "metarace feat 2";
            feat2.Power = 9266;
            feat2.Frequency.Quantity = 42;
            feat2.Frequency.TimePeriod = "fortnight";

            metaraceFeats.Add(feat1);
            metaraceFeats.Add(feat2);

            var feats = racialFeatsGenerator.GenerateWith(race, skills, stats);
            var first = feats.First();
            var last = feats.Last();

            Assert.That(first.Name, Is.EqualTo("metarace feat 1"));
            Assert.That(first.Power, Is.EqualTo(0));
            Assert.That(first.Frequency.Quantity, Is.EqualTo(0));
            Assert.That(first.Frequency.TimePeriod, Is.Empty);

            Assert.That(last.Name, Is.EqualTo("metarace feat 2"));
            Assert.That(last.Power, Is.EqualTo(9266));
            Assert.That(last.Frequency.Quantity, Is.EqualTo(42));
            Assert.That(last.Frequency.TimePeriod, Is.EqualTo("fortnight"));
        }

        [Test]
        public void GetMetaracialSpeciesFeats()
        {
            var feat1 = new RacialFeatSelection();
            feat1.Feat = "metarace species feat 1";

            var feat2 = new RacialFeatSelection();
            feat2.Feat = "metarace species feat 2";
            feat2.Power = 9266;
            feat2.Frequency.Quantity = 42;
            feat2.Frequency.TimePeriod = "fortnight";

            speciesFeats.Add(feat1);
            speciesFeats.Add(feat2);

            var feats = racialFeatsGenerator.GenerateWith(race, skills, stats);
            var first = feats.First();
            var last = feats.Last();

            Assert.That(first.Name, Is.EqualTo("metarace species feat 1"));
            Assert.That(first.Power, Is.EqualTo(0));
            Assert.That(first.Frequency.Quantity, Is.EqualTo(0));
            Assert.That(first.Frequency.TimePeriod, Is.Empty);

            Assert.That(last.Name, Is.EqualTo("metarace species feat 2"));
            Assert.That(last.Power, Is.EqualTo(9266));
            Assert.That(last.Frequency.Quantity, Is.EqualTo(42));
            Assert.That(last.Frequency.TimePeriod, Is.EqualTo("fortnight"));
        }

        [Test]
        public void GetAllRacialFeats()
        {
            var baseRaceFeat = new RacialFeatSelection();
            baseRaceFeat.Feat = "base race feat";
            baseRaceFeats.Add(baseRaceFeat);

            var metaraceFeat = new RacialFeatSelection();
            metaraceFeat.Feat = "metarace feat";
            metaraceFeats.Add(metaraceFeat);

            var speciesFeat = new RacialFeatSelection();
            speciesFeat.Feat = "metarace species feat";
            speciesFeats.Add(speciesFeat);

            var feats = racialFeatsGenerator.GenerateWith(race, skills, stats);
            Assert.That(feats.Count(), Is.EqualTo(3));
        }

        [Test]
        public void DoNotGetRacialFeatThatDoNotMeetRequirements()
        {
            var baseRaceFeat = new RacialFeatSelection();
            baseRaceFeat.Feat = "base race feat";
            baseRaceFeat.MinimumHitDieRequirement = 2;
            baseRaceFeats.Add(baseRaceFeat);

            var metaraceFeat = new RacialFeatSelection();
            metaraceFeat.Feat = "metarace feat";
            metaraceFeat.SizeRequirement = "size";
            metaraceFeats.Add(metaraceFeat);

            var speciesFeat = new RacialFeatSelection();
            speciesFeat.Feat = "metarace species feat";
            speciesFeat.MinimumAbilities["stat"] = 15;
            speciesFeats.Add(speciesFeat);

            var monsterHitDice = new Dictionary<string, int>();
            monsterHitDice[race.BaseRace] = 1;
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(TableNameConstants.Set.Adjustments.MonsterHitDice)).Returns(monsterHitDice);

            var feats = racialFeatsGenerator.GenerateWith(race, skills, stats);
            Assert.That(feats, Is.Empty);
        }

        [Test]
        public void GetRacialFeatThatMeetRequirements()
        {
            var baseRaceFeat = new RacialFeatSelection();
            baseRaceFeat.Feat = "base race feat";
            baseRaceFeat.MinimumHitDieRequirement = 2;
            baseRaceFeats.Add(baseRaceFeat);

            var metaraceFeat = new RacialFeatSelection();
            metaraceFeat.Feat = "metarace feat";
            metaraceFeat.SizeRequirement = "size";
            metaraceFeats.Add(metaraceFeat);

            var speciesFeat = new RacialFeatSelection();
            speciesFeat.Feat = "metarace species feat";
            speciesFeat.MinimumAbilities["stat"] = 14;
            speciesFeats.Add(speciesFeat);

            race.Size = "size";

            mockAdjustmentsSelector.Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.MonsterHitDice, race.BaseRace)).Returns(2);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters))
                .Returns(new[] { race.BaseRace });

            var feats = racialFeatsGenerator.GenerateWith(race, skills, stats);
            Assert.That(feats.Count(), Is.EqualTo(3));
        }

        [Test]
        public void NonMonstersHaveOneMonsterHitDieForSakeOfHitDiceRequirements()
        {
            var baseRaceFeat = new RacialFeatSelection();
            baseRaceFeat.Feat = "racial feat";
            baseRaceFeat.MinimumHitDieRequirement = 1;
            baseRaceFeats.Add(baseRaceFeat);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters))
                .Returns(new[] { "other base race" });

            var feats = racialFeatsGenerator.GenerateWith(race, skills, stats);
            var onlyFeat = feats.Single();

            Assert.That(onlyFeat.Name, Is.EqualTo("racial feat"));
            mockAdjustmentsSelector.Verify(s => s.SelectAllFrom(TableNameConstants.Set.Adjustments.MonsterHitDice), Times.Never);
        }

        [Test]
        public void GetFociForFeat()
        {
            var baseRaceFeatSelection = new RacialFeatSelection();
            baseRaceFeatSelection.Feat = "racial feat";
            baseRaceFeatSelection.FocusType = "base focus type";
            baseRaceFeats.Add(baseRaceFeatSelection);

            var metaraceFeatSelection = new RacialFeatSelection();
            metaraceFeatSelection.Feat = "metarace feat";
            metaraceFeatSelection.FocusType = "meta focus type";
            metaraceFeats.Add(metaraceFeatSelection);

            var speciesFeatSelection = new RacialFeatSelection();
            speciesFeatSelection.Feat = "metarace species feat";
            speciesFeatSelection.FocusType = "species focus type";
            speciesFeats.Add(speciesFeatSelection);

            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("racial feat", "base focus type", skills)).Returns("base focus");
            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("metarace feat", "meta focus type", skills)).Returns("meta focus");
            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("metarace species feat", "species focus type", skills)).Returns("species focus");

            var feats = racialFeatsGenerator.GenerateWith(race, skills, stats);
            var baseFeat = feats.First(f => f.Name == baseRaceFeatSelection.Feat);
            var metaFeat = feats.First(f => f.Name == metaraceFeatSelection.Feat);
            var speciesFeat = feats.First(f => f.Name == speciesFeatSelection.Feat);

            Assert.That(baseFeat.Foci.Single(), Is.EqualTo("base focus"));
            Assert.That(metaFeat.Foci.Single(), Is.EqualTo("meta focus"));
            Assert.That(speciesFeat.Foci.Single(), Is.EqualTo("species focus"));
        }

        [Test]
        public void DoNotGetEmptyFoci()
        {
            var baseRaceFeatSelection = new RacialFeatSelection();
            baseRaceFeatSelection.Feat = "racial feat";
            baseRaceFeatSelection.FocusType = string.Empty;
            baseRaceFeats.Add(baseRaceFeatSelection);

            var metaraceFeatSelection = new RacialFeatSelection();
            metaraceFeatSelection.Feat = "metarace feat";
            metaraceFeatSelection.FocusType = string.Empty;
            metaraceFeats.Add(metaraceFeatSelection);

            var speciesFeatSelection = new RacialFeatSelection();
            speciesFeatSelection.Feat = "metarace species feat";
            speciesFeatSelection.FocusType = string.Empty;
            speciesFeats.Add(speciesFeatSelection);

            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("racial feat", string.Empty, skills)).Returns(string.Empty);
            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("metarace feat", string.Empty, skills)).Returns(string.Empty);
            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("metarace species feat", string.Empty, skills)).Returns(string.Empty);

            var feats = racialFeatsGenerator.GenerateWith(race, skills, stats);
            var baseFeat = feats.First(f => f.Name == baseRaceFeatSelection.Feat);
            var metaFeat = feats.First(f => f.Name == metaraceFeatSelection.Feat);
            var speciesFeat = feats.First(f => f.Name == speciesFeatSelection.Feat);

            Assert.That(baseFeat.Foci, Is.Empty);
            Assert.That(metaFeat.Foci, Is.Empty);
            Assert.That(speciesFeat.Foci, Is.Empty);
        }

        [Test]
        public void AddMonsterHitDiceToPower()
        {
            var metaraceFeat = new RacialFeatSelection();
            metaraceFeat.Feat = "metarace feat";
            metaraceFeat.Power = 10;
            metaraceFeats.Add(metaraceFeat);

            mockAdjustmentsSelector.Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.MonsterHitDice, race.BaseRace)).Returns(2);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters))
                .Returns(new[] { race.BaseRace });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.AddMonsterHitDiceToPower))
                .Returns(new[] { metaraceFeat.Feat });

            var feats = racialFeatsGenerator.GenerateWith(race, skills, stats);
            var onlyFeat = feats.Single();

            Assert.That(onlyFeat.Name, Is.EqualTo("metarace feat"));
            Assert.That(onlyFeat.Power, Is.EqualTo(12));
        }

        [Test]
        public void DoNotAddMonsterHitDiceToPower()
        {
            var baseRaceFeatSelection = new RacialFeatSelection();
            baseRaceFeatSelection.Feat = "different feat";
            baseRaceFeatSelection.Power = 2;
            baseRaceFeats.Add(baseRaceFeatSelection);

            var metaraceFeat = new RacialFeatSelection();
            metaraceFeat.Feat = "metarace feat";
            metaraceFeat.Power = 10;
            metaraceFeats.Add(metaraceFeat);

            var speciesFeatSelection = new RacialFeatSelection();
            speciesFeatSelection.Feat = "different feat";
            speciesFeatSelection.Power = 3;
            speciesFeats.Add(speciesFeatSelection);

            var monsterHitDice = new Dictionary<string, int>();
            monsterHitDice[race.BaseRace] = 2;
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(TableNameConstants.Set.Adjustments.MonsterHitDice)).Returns(monsterHitDice);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters))
                .Returns(new[] { race.BaseRace });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.AddMonsterHitDiceToPower))
                .Returns(new[] { "different feat" });

            var feats = racialFeatsGenerator.GenerateWith(race, skills, stats);
            var first = feats.First();
            var second = feats.ElementAt(1);
            var third = feats.Last();

            Assert.That(first.Name, Is.EqualTo("different feat"));
            Assert.That(first.Power, Is.EqualTo(2));
            Assert.That(second.Name, Is.EqualTo("metarace feat"));
            Assert.That(second.Power, Is.EqualTo(10));
            Assert.That(third.Name, Is.EqualTo("different feat"));
            Assert.That(third.Power, Is.EqualTo(3));
        }

        [Test]
        public void IfNotMonster_Add1ToPower()
        {
            var metaraceFeat = new RacialFeatSelection();
            metaraceFeat.Feat = "metarace feat";
            metaraceFeat.Power = 10;
            metaraceFeats.Add(metaraceFeat);

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters)).Returns(new[] { "monster" });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.AddMonsterHitDiceToPower))
                .Returns(new[] { metaraceFeat.Feat });

            var feats = racialFeatsGenerator.GenerateWith(race, skills, stats);
            var onlyFeat = feats.Single();

            Assert.That(onlyFeat.Name, Is.EqualTo("metarace feat"));
            Assert.That(onlyFeat.Power, Is.EqualTo(11));
        }

        [Test]
        public void GetOneRandomFociForRacialFeatIfNoRandomFociQuantity()
        {
            var featSelection = new RacialFeatSelection();
            featSelection.Feat = "racial feat";
            featSelection.FocusType = "focus type";

            var count = 1;
            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("racial feat", "focus type", skills)).Returns(() => $"focus {count++}");
            mockDice.Setup(d => d.Roll("dice roll").AsSum<int>()).Returns(3);

            baseRaceFeats.Add(featSelection);

            var feats = racialFeatsGenerator.GenerateWith(race, skills, stats);
            var feat = feats.Single();

            Assert.That(feat.Name, Is.EqualTo("racial feat"));
            Assert.That(feat.Foci.Single(), Is.EqualTo("focus 1"));
        }

        [Test]
        public void GetRandomFociForRacialFeat()
        {
            var featSelection = new RacialFeatSelection();
            featSelection.Feat = "racial feat";
            featSelection.FocusType = "focus type";
            featSelection.RandomFociQuantity = "dice roll";

            var count = 1;
            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("racial feat", "focus type", skills)).Returns(() => $"focus {count++}");
            mockDice.Setup(d => d.Roll("dice roll").AsSum<int>()).Returns(3);

            baseRaceFeats.Add(featSelection);

            var feats = racialFeatsGenerator.GenerateWith(race, skills, stats);
            var feat = feats.Single();

            Assert.That(feat.Name, Is.EqualTo("racial feat"));
            Assert.That(feat.Foci, Is.EquivalentTo(new[] { "focus 1", "focus 2", "focus 3" }));
        }

        [Test]
        public void GetNoDuplicateRandomFociForRacialFeat()
        {
            var featSelection = new RacialFeatSelection();
            featSelection.Feat = "racial feat";
            featSelection.FocusType = "focus type";
            featSelection.RandomFociQuantity = "dice roll";

            var count = 1;
            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("racial feat", "focus type", skills)).Returns(() => $"focus {count++ / 2}");
            mockDice.Setup(d => d.Roll("dice roll").AsSum<int>()).Returns(3);

            baseRaceFeats.Add(featSelection);

            var feats = racialFeatsGenerator.GenerateWith(race, skills, stats);
            var feat = feats.Single();

            Assert.That(feat.Name, Is.EqualTo("racial feat"));
            Assert.That(feat.Foci, Is.Unique);
            Assert.That(feat.Foci, Is.EquivalentTo(new[] { "focus 0", "focus 1", "focus 2" }));
        }

        [Test]
        public void GetNoRandomFociForRacialFeatIfNoFocusType()
        {
            var featSelection = new RacialFeatSelection();
            featSelection.Feat = "racial feat";
            featSelection.RandomFociQuantity = "dice roll";

            var count = 1;
            mockFeatFocusGenerator.Setup(g => g.GenerateAllowingFocusOfAllFrom("racial feat", "focus type", skills)).Returns(() => $"focus {count++}");
            mockDice.Setup(d => d.Roll("dice roll").AsSum<int>()).Returns(3);

            baseRaceFeats.Add(featSelection);

            var feats = racialFeatsGenerator.GenerateWith(race, skills, stats);
            var feat = feats.Single();

            Assert.That(feat.Name, Is.EqualTo("racial feat"));
            Assert.That(feat.Foci, Is.Empty);
        }
    }
}