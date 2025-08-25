using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Combats;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Selectors;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.RollGen;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Combats
{
    [TestFixture]
    public class HitPointsGeneratorTests
    {
        private Mock<Dice> mockDice;
        private Mock<IAdjustmentsSelector> mockAdjustmentsSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private IHitPointsGenerator hitPointsGenerator;

        private CharacterClass characterClass;
        private Race race;
        private int constitutionBonus;
        private Dictionary<string, int> hitDice;
        private List<Feat> feats;

        [SetUp]
        public void Setup()
        {
            mockDice = new Mock<Dice>();
            mockAdjustmentsSelector = new Mock<IAdjustmentsSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            hitPointsGenerator = new HitPointsGenerator(mockDice.Object, mockAdjustmentsSelector.Object, mockCollectionsSelector.Object);

            characterClass = new CharacterClass();
            race = new Race();
            feats = [];
            hitDice = [];

            characterClass.Level = 1;
            characterClass.Name = "class name";
            race.Metarace = "metarace";
            constitutionBonus = 0;
            hitDice[characterClass.Name] = 9266;
            hitDice["otherclassname"] = 42;

            SetUpRoll(0, 8, 0);
            SetUpRoll(1, 9266, 42);

            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.ClassHitDice, It.IsAny<string>()))
                .Returns((string t, string n) => hitDice[n]);
        }

        private void SetUpRoll(int quantity, int die, params int[] rolls)
        {
            mockDice.Setup(d => d.Roll(quantity).d(die).AsIndividualRolls<int>()).Returns(rolls);

            var sumMock = new Mock<PartialRoll>();
            var plus = 0;
            sumMock.Setup(r => r.AsSum<int>()).Returns(() => rolls.Sum() + plus);
            mockDice.Setup(d => d.Roll(quantity).d(die).Plus(It.IsAny<double>()))
                .Callback((double p) => plus = (int)p)
                .Returns(sumMock.Object);
        }

        [Test]
        public void GetHitDieFromAdjustments()
        {
            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(42));
        }

        [Test]
        public void RollHitPointsPerLevel()
        {
            characterClass.Level = 600;
            SetUpRoll(600, 9266, 90210);

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(90210));
        }

        [Test]
        public void ConstitutionBonusAppliedPerLevel()
        {
            characterClass.Level = 2;
            constitutionBonus = 5;
            SetUpRoll(2, 9266, 90210, 42);

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(90262));
            mockDice.Verify(d => d.Roll(2).d(9266).Plus(10).AsSum<int>(), Times.Once);
            mockDice.Verify(d => d.Roll(2).d(9266).AsIndividualRolls<int>(), Times.Never);
        }

        [Test]
        public void CannotGainFewerThan1HitPointPerLevel()
        {
            characterClass.Level = 5;
            constitutionBonus = int.MinValue;
            SetUpRoll(5, 9266, 90210, 42, 600, 1337, 1234);

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(5));
            mockDice.Verify(d => d.Roll(5).d(9266).Plus(It.IsAny<int>()).AsSum<int>(), Times.Never);
            mockDice.Verify(d => d.Roll(5).d(9266).AsIndividualRolls<int>(), Times.Once);
        }

        [Test]
        public void NonMonsterDoNotGetAdditionalHitDice()
        {
            characterClass.Level = 2;
            SetUpRoll(2, 9266, 90210, 42);

            race.BaseRace = "differentbaserace";
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters))
                .Returns(new[] { "otherbaserace", "baserace" });

            var monsterHitDice = new Dictionary<string, int>();
            monsterHitDice["monster"] = 1;
            monsterHitDice["baserace"] = 3;
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(TableNameConstants.Set.Adjustments.MonsterHitDice)).Returns(monsterHitDice);

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(90252));
            mockAdjustmentsSelector.Verify(s => s.SelectAllFrom(TableNameConstants.Set.Adjustments.MonsterHitDice), Times.Never);
        }

        [Test]
        public void MonstersGetAdditionalHitDice()
        {
            characterClass.Level = 2;
            SetUpRoll(2, 9266, 90210, 42);

            race.BaseRace = "baserace";
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters))
                .Returns(new[] { "otherbaserace", "baserace" });

            var monsterHitDice = new Dictionary<string, int>();
            monsterHitDice["monster"] = 1;
            monsterHitDice["baserace"] = 3;
            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.MonsterHitDice, It.IsAny<string>()))
                .Returns((string t, string n) => monsterHitDice[n]);

            SetUpRoll(3, 8, 600, 1337, 1234);

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(93423));
        }

        [Test]
        public void MonstersGetNoAdditionalHitDice()
        {
            characterClass.Level = 2;
            SetUpRoll(2, 9266, 90210, 42);

            race.BaseRace = "baserace";
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters))
                .Returns(new[] { "otherbaserace", "baserace" });

            var monsterHitDice = new Dictionary<string, int>();
            monsterHitDice["monster"] = 1;
            monsterHitDice["baserace"] = 0;
            mockAdjustmentsSelector.Setup(s => s.SelectAllFrom(TableNameConstants.Set.Adjustments.MonsterHitDice)).Returns(monsterHitDice);

            SetUpRoll(0, 8, 600, 1337, 1234);

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(90252));
        }

        [Test]
        public void MonstersApplyConstitutionBonusForEachAdditionalHitDie()
        {
            characterClass.Level = 2;
            SetUpRoll(2, 9266, 90210, 42);

            race.BaseRace = "baserace";
            constitutionBonus = 5;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters))
                .Returns(new[] { "otherbaserace", "baserace" });

            var monsterHitDice = new Dictionary<string, int>();
            monsterHitDice["monster"] = 1;
            monsterHitDice["baserace"] = 3;
            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.MonsterHitDice, It.IsAny<string>()))
                .Returns((string t, string n) => monsterHitDice[n]);

            SetUpRoll(3, 8, 600, 1337, 1234);

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(93448));
        }

        [Test]
        public void MonstersCannotGainFewerThan1HitPointPerHitDie()
        {
            characterClass.Level = 2;
            SetUpRoll(2, 9266, 90210, 42);

            race.BaseRace = "baserace";
            constitutionBonus = int.MinValue;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters))
                .Returns(new[] { "otherbaserace", "baserace" });

            var monsterHitDice = new Dictionary<string, int>();
            monsterHitDice["monster"] = 1;
            monsterHitDice["baserace"] = 3;
            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.MonsterHitDice, It.IsAny<string>()))
                .Returns((string t, string n) => monsterHitDice[n]);

            SetUpRoll(3, 8, 600, 1337, 1234);

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(5));
        }

        [Test]
        public void HalfDragonIncreasesMonsterHitDie()
        {
            characterClass.Level = 2;
            SetUpRoll(2, 9266, 90210, 42);

            race.BaseRace = "baserace";
            race.Metarace = RaceConstants.Metaraces.HalfDragon;
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters))
                .Returns(["otherbaserace", "baserace"]);

            var monsterHitDice = new Dictionary<string, int>();
            monsterHitDice["monster"] = 1;
            monsterHitDice["baserace"] = 3;
            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.MonsterHitDice, It.IsAny<string>()))
                .Returns((string t, string n) => monsterHitDice[n]);

            SetUpRoll(3, 10, 2345, 3456, 4567);
            SetUpRoll(3, 8, 600, 1337, 1234);

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(100620));
            mockDice.Verify(d => d.Roll(It.IsAny<int>()).d(8), Times.Never);
            mockDice.Verify(d => d.Roll(3).d(10), Times.Once);
            mockDice.Verify(d => d.Roll(It.IsAny<int>()).d(12), Times.Never);
            mockDice.Verify(d => d.Roll(2).d(9266), Times.Once);
        }

        [Test]
        public void UndeadIncreasesMonsterHitDie_Metarace()
        {
            characterClass.Level = 2;
            SetUpRoll(2, 9266, 666, 666);
            SetUpRoll(2, 12, 90210, 42);

            race.BaseRace = "baserace";
            race.Metarace = "undead";
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters))
                .Returns(["otherbaserace", "baserace"]);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, GroupConstants.Undead))
                .Returns(["undead", "other undead"]);

            var monsterHitDice = new Dictionary<string, int>();
            monsterHitDice["monster"] = 1;
            monsterHitDice["baserace"] = 3;
            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.MonsterHitDice, It.IsAny<string>()))
                .Returns((string t, string n) => monsterHitDice[n]);

            SetUpRoll(3, 12, 5678, 6789, 7890);
            SetUpRoll(3, 10, 2345, 3456, 4567);
            SetUpRoll(3, 8, 600, 1337, 1234);

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(110609));
            mockDice.Verify(d => d.Roll(It.IsAny<int>()).d(8), Times.Never);
            mockDice.Verify(d => d.Roll(It.IsAny<int>()).d(10), Times.Never);
            mockDice.Verify(d => d.Roll(3).d(12), Times.Once);
            mockDice.Verify(d => d.Roll(2).d(12), Times.Once);
        }

        [Test]
        public void UndeadIncreasesMonsterHitDie_Mummy()
        {
            characterClass.Level = 2;
            SetUpRoll(2, 9266, 90210, 42);
            SetUpRoll(2, 12, 666, 666);

            race.BaseRace = RaceConstants.BaseRaces.Mummy;
            race.Metarace = RaceConstants.Metaraces.None;
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters))
                .Returns(["otherbaserace", "baserace", RaceConstants.BaseRaces.Mummy]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, GroupConstants.Undead))
                .Returns(["undead", "other undead"]);

            var monsterHitDice = new Dictionary<string, int>
            {
                ["monster"] = 1,
                ["baserace"] = 3,
                [RaceConstants.BaseRaces.Mummy] = 5,
            };
            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.MonsterHitDice, It.IsAny<string>()))
                .Returns((string t, string n) => monsterHitDice[n]);

            SetUpRoll(5, 12, 5678, 6789);
            SetUpRoll(5, 10, 666, 666);
            SetUpRoll(5, 8, 666, 666);

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(102719));
            mockDice.Verify(d => d.Roll(It.IsAny<int>()).d(8), Times.Never);
            mockDice.Verify(d => d.Roll(It.IsAny<int>()).d(10), Times.Never);
            mockDice.Verify(d => d.Roll(5).d(12), Times.Once);
            mockDice.Verify(d => d.Roll(2).d(9266), Times.Once);
        }

        [Test]
        public void UndeadSetsClassHitDieTo12_Metarace()
        {
            characterClass.Level = 2;
            SetUpRoll(2, 9266, 666, 666);
            SetUpRoll(2, 12, 90210, 42);

            race.Metarace = "undead";
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, GroupConstants.Undead))
                .Returns(["undead", "other undead"]);

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(90252));
            mockDice.Verify(d => d.Roll(It.IsAny<int>()).d(8), Times.Never);
            mockDice.Verify(d => d.Roll(It.IsAny<int>()).d(10), Times.Never);
            mockDice.Verify(d => d.Roll(2).d(12), Times.Once);
            mockDice.Verify(d => d.Roll(It.IsAny<int>()).d(9266), Times.Never);
        }

        [Test]
        public void UndeadDoesNotSetClassHitDieTo12_Mummy()
        {
            characterClass.Level = 2;
            SetUpRoll(2, 9266, 90210, 42);
            SetUpRoll(2, 12, 666, 666);

            race.BaseRace = RaceConstants.BaseRaces.Mummy;
            race.Metarace = RaceConstants.Metaraces.None;
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, GroupConstants.Undead))
                .Returns(["undead", "other undead"]);

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(90252));
            mockDice.Verify(d => d.Roll(It.IsAny<int>()).d(8), Times.Never);
            mockDice.Verify(d => d.Roll(It.IsAny<int>()).d(10), Times.Never);
            mockDice.Verify(d => d.Roll(It.IsAny<int>()).d(12), Times.Never);
            mockDice.Verify(d => d.Roll(2).d(9266), Times.Once);
        }

        [Test]
        public void ToughnessIncreassHitPoints()
        {
            feats.Add(new Feat { Name = FeatConstants.Toughness, Power = 3 });

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(45));
        }

        [Test]
        public void ToughnessIncreassHitPointsMultipleTimes()
        {
            feats.Add(new Feat { Name = FeatConstants.Toughness, Power = 3 });
            feats.Add(new Feat { Name = FeatConstants.Toughness, Power = 3 });

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(48));
        }

        [Test]
        public void MinimumCheckAppliedPerLevel()
        {
            characterClass.Level = 3;
            constitutionBonus = -2;

            SetUpRoll(3, 9266, 1, 2, 4);

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(4));
            mockDice.Verify(d => d.Roll(3).d(9266).Plus(It.IsAny<int>()).AsSum<int>(), Times.Never);
            mockDice.Verify(d => d.Roll(3).d(9266).AsIndividualRolls<int>(), Times.Once);
        }

        [Test]
        public void MinimumCheckAppliedPerMonsterHitDie()
        {
            characterClass.Level = 2;
            mockDice.Setup(d => d.Roll(2).d(9266).AsIndividualRolls<int>()).Returns(new[] { 90210, 42 });

            race.BaseRace = "baserace";
            constitutionBonus = -2;

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters))
                .Returns(["otherbaserace", "baserace"]);

            var monsterHitDice = new Dictionary<string, int>();
            monsterHitDice["monster"] = 1234;
            monsterHitDice["baserace"] = 3;
            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.MonsterHitDice, It.IsAny<string>()))
                .Returns((string t, string n) => monsterHitDice[n]);

            SetUpRoll(3, 8, 1, 2, 4);

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(90252));
            mockDice.Verify(d => d.Roll(3).d(8).Plus(It.IsAny<int>()).AsSum<int>(), Times.Never);
            mockDice.Verify(d => d.Roll(3).d(8).AsIndividualRolls<int>(), Times.Once);
        }

        [Test]
        public void MinimumCheckAppliedPerUndeadHitDie()
        {
            characterClass.Level = 5;
            var mockClassPartialRoll = new Mock<PartialRoll>();
            mockClassPartialRoll.Setup(r => r.d(12).AsIndividualRolls<int>()).Returns(new[] { 4, 5, 7, 10, 12 });
            mockDice.Setup(d => d.Roll(5)).Returns(mockClassPartialRoll.Object);
            constitutionBonus = -5;

            race.BaseRace = "baserace";
            race.Metarace = "undead";
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters))
                .Returns(new[] { "otherbaserace", "baserace" });
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.MetaraceGroups, GroupConstants.Undead))
                .Returns(new[] { "undead", "other undead" });

            var monsterHitDice = new Dictionary<string, int>();
            monsterHitDice["monster"] = 1;
            monsterHitDice["baserace"] = 3;
            mockAdjustmentsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Set.Adjustments.MonsterHitDice, It.IsAny<string>()))
                .Returns((string t, string n) => monsterHitDice[n]);

            var mockRacePartialRoll = new Mock<PartialRoll>();
            mockRacePartialRoll.Setup(r => r.d(12).AsIndividualRolls<int>()).Returns(new[] { 1, 3, 8 });
            mockDice.Setup(d => d.Roll(3)).Returns(mockRacePartialRoll.Object);

            var hitPoints = hitPointsGenerator.GenerateWith(characterClass, constitutionBonus, race, feats);
            Assert.That(hitPoints, Is.EqualTo(21));
        }
    }
}