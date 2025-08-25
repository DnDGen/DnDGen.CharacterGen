using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Combats;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Skills;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.CharacterGen.Tests.Unit.Feats
{
    [TestFixture]
    public class FeatsGeneratorTests
    {
        private IFeatsGenerator featsGenerator;
        private Mock<IRacialFeatsGenerator> mockRacialFeatsGenerator;
        private Mock<IClassFeatsGenerator> mockClassFeatsGenerator;
        private Mock<IAdditionalFeatsGenerator> mockAdditionalFeatsGenerator;
        private CharacterClass characterClass;
        private Race race;
        private Dictionary<string, Ability> stats;
        private List<Skill> skills;
        private BaseAttack baseAttack;
        private List<Feat> racialFeats;

        [SetUp]
        public void Setup()
        {
            mockRacialFeatsGenerator = new Mock<IRacialFeatsGenerator>();
            mockClassFeatsGenerator = new Mock<IClassFeatsGenerator>();
            mockAdditionalFeatsGenerator = new Mock<IAdditionalFeatsGenerator>();
            featsGenerator = new FeatsGenerator(mockRacialFeatsGenerator.Object, mockClassFeatsGenerator.Object, mockAdditionalFeatsGenerator.Object);

            characterClass = new CharacterClass();
            race = new Race();
            stats = new Dictionary<string, Ability>();
            skills = new List<Skill>();
            baseAttack = new BaseAttack();
            racialFeats = new List<Feat>();

            mockRacialFeatsGenerator.Setup(g => g.GenerateWith(race, skills, stats)).Returns(racialFeats);
        }

        [Test]
        public void GetRacialFeats()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Foci = new[] { "focus" };
            racialFeats[0].Name = "racialFeat1";
            racialFeats[0].Power = 9266;
            racialFeats[1].Frequency.Quantity = 42;
            racialFeats[1].Frequency.TimePeriod = "fortnight";
            racialFeats[1].Name = "racialFeat2";

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            Assert.That(feats.Racial, Is.EqualTo(racialFeats));
        }

        [Test]
        public void GetClassFeats()
        {
            var classFeats = new List<Feat>();
            classFeats.Add(new Feat());
            classFeats.Add(new Feat());

            classFeats[0].Foci = new[] { "focus" };
            classFeats[0].Name = "classFeat1";
            classFeats[0].Power = 9266;
            classFeats[1].Frequency.Quantity = 42;
            classFeats[1].Frequency.TimePeriod = "fortnight";
            classFeats[1].Name = "classFeat2";

            mockClassFeatsGenerator.Setup(g => g.GenerateWith(characterClass, race, stats, racialFeats, skills)).Returns(classFeats);

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            Assert.That(feats.Class, Is.EqualTo(classFeats));
        }

        [Test]
        public void GetAdditionalFeats()
        {
            var additionalFeats = new List<Feat>();
            additionalFeats.Add(new Feat());
            additionalFeats.Add(new Feat());

            additionalFeats[0].Foci = new[] { "focus" };
            additionalFeats[0].Name = "feat1";
            additionalFeats[0].Power = 9266;
            additionalFeats[1].Frequency.Quantity = 42;
            additionalFeats[1].Frequency.TimePeriod = "fortnight";
            additionalFeats[1].Name = "feat2";

            mockAdditionalFeatsGenerator.Setup(g => g.GenerateWith(characterClass, race, stats, skills, baseAttack, It.IsAny<IEnumerable<Feat>>())).Returns(additionalFeats);

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            Assert.That(feats.Additional, Is.EqualTo(additionalFeats));
        }
    }
}