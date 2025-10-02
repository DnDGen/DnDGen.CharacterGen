using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Combats;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Skills;
using DnDGen.CharacterGen.Tables;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

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
            stats = [];
            skills = [];
            baseAttack = new BaseAttack();
            racialFeats = [];

            mockRacialFeatsGenerator.Setup(g => g.GenerateWith(race, skills, stats)).Returns(racialFeats);
        }

        [Test]
        public void GetRacialFeats()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Foci = ["focus"];
            racialFeats[0].Name = "racialFeat1";
            racialFeats[0].Power = 9266;
            racialFeats[1].Frequency.Quantity = 42;
            racialFeats[1].Frequency.TimePeriod = "fortnight";
            racialFeats[1].Name = "racialFeat2";

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            Assert.That(feats.Racial, Is.EquivalentTo(racialFeats));
        }

        [Test]
        public void GetRacialFeats_CombineFoci()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat with foci";
            racialFeats[0].Foci = ["focus 1", "focus 2"];
            racialFeats[1].Name = "feat with other foci";
            racialFeats[1].Foci = ["focus 3", "focus 4"];
            racialFeats[2].Name = "feat without foci";
            racialFeats[2].Foci = [];
            racialFeats[3].Name = "feat with foci";
            racialFeats[3].Foci = ["focus 5", "focus 6"];

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            var actualFeats = feats.Racial.ToArray();
            Assert.That(actualFeats[0].Name, Is.EqualTo("feat without foci"));
            Assert.That(actualFeats[0].Foci, Is.Empty);
            Assert.That(actualFeats[1].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[1].Foci, Is.EquivalentTo(["focus 1", "focus 2", "focus 5", "focus 6"]));
            Assert.That(actualFeats[2].Name, Is.EqualTo("feat with other foci"));
            Assert.That(actualFeats[2].Foci, Is.EquivalentTo(["focus 3", "focus 4"]));
            Assert.That(actualFeats, Has.Length.EqualTo(3));
        }

        [Test]
        public void GetRacialFeats_CombineFoci_WithAllFoci()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat with foci";
            racialFeats[0].Foci = [GroupConstants.All];
            racialFeats[1].Name = "feat with other foci";
            racialFeats[1].Foci = ["focus 3", "focus 4"];
            racialFeats[2].Name = "feat with foci";
            racialFeats[2].Foci = ["focus 5", "focus 6"];

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            var actualFeats = feats.Racial.ToArray();
            Assert.That(actualFeats[0].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[0].Foci, Is.EquivalentTo([GroupConstants.All]));
            Assert.That(actualFeats[1].Name, Is.EqualTo("feat with other foci"));
            Assert.That(actualFeats[1].Foci, Is.EquivalentTo(["focus 3", "focus 4"]));
            Assert.That(actualFeats, Has.Length.EqualTo(2));
        }

        [Test]
        public void GetRacialFeats_CombineFoci_SkipDifferentPowers()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat with foci";
            racialFeats[0].Foci = ["focus 1", "focus 2"];
            racialFeats[0].Power = 9266;
            racialFeats[1].Name = "feat with foci";
            racialFeats[1].Foci = ["focus 3", "focus 4"];
            racialFeats[1].Power = 42;
            racialFeats[2].Name = "feat with foci";
            racialFeats[2].Foci = ["focus 5", "focus 6"];
            racialFeats[2].Power = 9266;

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            var actualFeats = feats.Racial.ToArray();
            Assert.That(actualFeats[0].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[0].Foci, Is.EquivalentTo(["focus 1", "focus 2", "focus 5", "focus 6"]));
            Assert.That(actualFeats[0].Power, Is.EqualTo(9266));
            Assert.That(actualFeats[1].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[1].Foci, Is.EquivalentTo(["focus 3", "focus 4"]));
            Assert.That(actualFeats[1].Power, Is.EqualTo(42));
            Assert.That(actualFeats, Has.Length.EqualTo(2));
        }

        [Test]
        public void GetRacialFeats_CombineFoci_SkipDifferentFrequencies()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat with foci";
            racialFeats[0].Foci = ["focus 1", "focus 2"];
            racialFeats[0].Frequency.Quantity = 9266;
            racialFeats[0].Frequency.TimePeriod = "sometimes";
            racialFeats[1].Name = "feat with foci";
            racialFeats[1].Foci = ["focus 3", "focus 4"];
            racialFeats[1].Frequency.Quantity = 42;
            racialFeats[1].Frequency.TimePeriod = "sometimes";
            racialFeats[2].Name = "feat with foci";
            racialFeats[2].Foci = ["focus 5", "focus 6"];
            racialFeats[2].Frequency.Quantity = 9266;
            racialFeats[2].Frequency.TimePeriod = "sometimes";
            racialFeats[3].Name = "feat with foci";
            racialFeats[3].Foci = ["focus 7", "focus 8"];
            racialFeats[3].Frequency.Quantity = 9266;
            racialFeats[3].Frequency.TimePeriod = "often";

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            var actualFeats = feats.Racial.ToArray();
            Assert.That(actualFeats[0].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[0].Foci, Is.EquivalentTo(["focus 1", "focus 2", "focus 5", "focus 6"]));
            Assert.That(actualFeats[0].Frequency.Quantity, Is.EqualTo(9266));
            Assert.That(actualFeats[0].Frequency.TimePeriod, Is.EqualTo("sometimes"));
            Assert.That(actualFeats[1].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[1].Foci, Is.EquivalentTo(["focus 3", "focus 4"]));
            Assert.That(actualFeats[1].Frequency.Quantity, Is.EqualTo(42));
            Assert.That(actualFeats[1].Frequency.TimePeriod, Is.EqualTo("sometimes"));
            Assert.That(actualFeats[2].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[2].Foci, Is.EquivalentTo(["focus 7", "focus 8"]));
            Assert.That(actualFeats[2].Frequency.Quantity, Is.EqualTo(9266));
            Assert.That(actualFeats[2].Frequency.TimePeriod, Is.EqualTo("often"));
            Assert.That(actualFeats, Has.Length.EqualTo(3));
        }

        [Test]
        public void GetClassFeats()
        {
            var classFeats = new List<Feat>
            {
                new(),
                new()
            };

            classFeats[0].Name = "classFeat1";
            classFeats[0].Foci = ["focus"];
            classFeats[0].Power = 9266;
            classFeats[1].Name = "classFeat2";
            classFeats[1].Frequency.Quantity = 42;
            classFeats[1].Frequency.TimePeriod = "fortnight";

            mockClassFeatsGenerator.Setup(g => g.GenerateWith(characterClass, race, stats, racialFeats, skills)).Returns(classFeats);

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            Assert.That(feats.Class, Is.EquivalentTo(classFeats));
        }

        [Test]
        public void GetClassFeats_CombineFoci()
        {
            var classFeats = new List<Feat>
            {
                new(),
                new(),
                new(),
                new(),
            };

            classFeats[0].Name = "feat with foci";
            classFeats[0].Foci = ["focus 1", "focus 2"];
            classFeats[1].Name = "feat with other foci";
            classFeats[1].Foci = ["focus 3", "focus 4"];
            classFeats[2].Name = "feat without foci";
            classFeats[2].Foci = [];
            classFeats[3].Name = "feat with foci";
            classFeats[3].Foci = ["focus 5", "focus 6"];

            mockClassFeatsGenerator.Setup(g => g.GenerateWith(characterClass, race, stats, racialFeats, skills)).Returns(classFeats);

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            var actualFeats = feats.Class.ToArray();
            Assert.That(actualFeats[0].Name, Is.EqualTo("feat without foci"));
            Assert.That(actualFeats[0].Foci, Is.Empty);
            Assert.That(actualFeats[1].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[1].Foci, Is.EquivalentTo(["focus 1", "focus 2", "focus 5", "focus 6"]));
            Assert.That(actualFeats[2].Name, Is.EqualTo("feat with other foci"));
            Assert.That(actualFeats[2].Foci, Is.EquivalentTo(["focus 3", "focus 4"]));
            Assert.That(actualFeats, Has.Length.EqualTo(3));
        }

        [Test]
        public void GetClassFeats_CombineFoci_WithAllFoci()
        {
            var classFeats = new List<Feat>
            {
                new(),
                new(),
                new(),
            };

            classFeats[0].Name = "feat with foci";
            classFeats[0].Foci = ["focus 1", "focus 2"];
            classFeats[1].Name = "feat with other foci";
            classFeats[1].Foci = ["focus 3", "focus 4"];
            classFeats[2].Name = "feat with foci";
            classFeats[2].Foci = [GroupConstants.All];

            mockClassFeatsGenerator.Setup(g => g.GenerateWith(characterClass, race, stats, racialFeats, skills)).Returns(classFeats);

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            var actualFeats = feats.Class.ToArray();
            Assert.That(actualFeats[0].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[0].Foci, Is.EquivalentTo([GroupConstants.All]));
            Assert.That(actualFeats[1].Name, Is.EqualTo("feat with other foci"));
            Assert.That(actualFeats[1].Foci, Is.EquivalentTo(["focus 3", "focus 4"]));
            Assert.That(actualFeats, Has.Length.EqualTo(2));
        }

        [Test]
        public void GetClassFeats_CombineFoci_SkipDifferentPowers()
        {
            var classFeats = new List<Feat>
            {
                new(),
                new(),
                new(),
            };

            classFeats[0].Name = "feat with foci";
            classFeats[0].Foci = ["focus 1", "focus 2"];
            classFeats[0].Power = 9266;
            classFeats[1].Name = "feat with foci";
            classFeats[1].Foci = ["focus 3", "focus 4"];
            classFeats[1].Power = 42;
            classFeats[2].Name = "feat with foci";
            classFeats[2].Foci = ["focus 5", "focus 6"];
            classFeats[2].Power = 9266;

            mockClassFeatsGenerator.Setup(g => g.GenerateWith(characterClass, race, stats, racialFeats, skills)).Returns(classFeats);

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            var actualFeats = feats.Class.ToArray();
            Assert.That(actualFeats[0].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[0].Foci, Is.EquivalentTo(["focus 1", "focus 2", "focus 5", "focus 6"]));
            Assert.That(actualFeats[0].Power, Is.EqualTo(9266));
            Assert.That(actualFeats[1].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[1].Foci, Is.EquivalentTo(["focus 3", "focus 4"]));
            Assert.That(actualFeats[1].Power, Is.EqualTo(42));
            Assert.That(actualFeats, Has.Length.EqualTo(2));
        }

        [Test]
        public void GetClassFeats_CombineFoci_SkipDifferentFrequencies()
        {
            var classFeats = new List<Feat>
            {
                new(),
                new(),
                new(),
                new(),
            };

            classFeats[0].Name = "feat with foci";
            classFeats[0].Foci = ["focus 1", "focus 2"];
            classFeats[0].Frequency.Quantity = 9266;
            classFeats[0].Frequency.TimePeriod = "sometimes";
            classFeats[1].Name = "feat with foci";
            classFeats[1].Foci = ["focus 3", "focus 4"];
            classFeats[1].Frequency.Quantity = 42;
            classFeats[1].Frequency.TimePeriod = "sometimes";
            classFeats[2].Name = "feat with foci";
            classFeats[2].Foci = ["focus 5", "focus 6"];
            classFeats[2].Frequency.Quantity = 9266;
            classFeats[2].Frequency.TimePeriod = "sometimes";
            classFeats[3].Name = "feat with foci";
            classFeats[3].Foci = ["focus 7", "focus 8"];
            classFeats[3].Frequency.Quantity = 9266;
            classFeats[3].Frequency.TimePeriod = "often";

            mockClassFeatsGenerator.Setup(g => g.GenerateWith(characterClass, race, stats, racialFeats, skills)).Returns(classFeats);

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            var actualFeats = feats.Class.ToArray();
            Assert.That(actualFeats[0].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[0].Foci, Is.EquivalentTo(["focus 1", "focus 2", "focus 5", "focus 6"]));
            Assert.That(actualFeats[0].Frequency.Quantity, Is.EqualTo(9266));
            Assert.That(actualFeats[0].Frequency.TimePeriod, Is.EqualTo("sometimes"));
            Assert.That(actualFeats[1].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[1].Foci, Is.EquivalentTo(["focus 3", "focus 4"]));
            Assert.That(actualFeats[1].Frequency.Quantity, Is.EqualTo(42));
            Assert.That(actualFeats[1].Frequency.TimePeriod, Is.EqualTo("sometimes"));
            Assert.That(actualFeats[2].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[2].Foci, Is.EquivalentTo(["focus 7", "focus 8"]));
            Assert.That(actualFeats[2].Frequency.Quantity, Is.EqualTo(9266));
            Assert.That(actualFeats[2].Frequency.TimePeriod, Is.EqualTo("often"));
            Assert.That(actualFeats, Has.Length.EqualTo(3));
        }

        [Test]
        public void GetAdditionalFeats()
        {
            var additionalFeats = new List<Feat>
            {
                new(),
                new()
            };

            additionalFeats[0].Foci = ["focus"];
            additionalFeats[0].Name = "feat1";
            additionalFeats[0].Power = 9266;
            additionalFeats[1].Frequency.Quantity = 42;
            additionalFeats[1].Frequency.TimePeriod = "fortnight";
            additionalFeats[1].Name = "feat2";

            mockAdditionalFeatsGenerator.Setup(g => g.GenerateWith(characterClass, race, stats, skills, baseAttack, It.IsAny<IEnumerable<Feat>>())).Returns(additionalFeats);

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            Assert.That(feats.Additional, Is.EquivalentTo(additionalFeats));
        }

        [Test]
        public void GetAdditionalFeats_CombineFoci()
        {
            var additionalFeats = new List<Feat>
            {
                new(),
                new(),
                new(),
                new(),
            };

            additionalFeats[0].Name = "feat with foci";
            additionalFeats[0].Foci = ["focus 1", "focus 2"];
            additionalFeats[1].Name = "feat with other foci";
            additionalFeats[1].Foci = ["focus 3", "focus 4"];
            additionalFeats[2].Name = "feat without foci";
            additionalFeats[2].Foci = [];
            additionalFeats[3].Name = "feat with foci";
            additionalFeats[3].Foci = ["focus 5", "focus 6"];

            mockAdditionalFeatsGenerator.Setup(g => g.GenerateWith(characterClass, race, stats, skills, baseAttack, It.IsAny<IEnumerable<Feat>>())).Returns(additionalFeats);

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            var actualFeats = feats.Additional.ToArray();
            Assert.That(actualFeats[0].Name, Is.EqualTo("feat without foci"));
            Assert.That(actualFeats[0].Foci, Is.Empty);
            Assert.That(actualFeats[1].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[1].Foci, Is.EquivalentTo(["focus 1", "focus 2", "focus 5", "focus 6"]));
            Assert.That(actualFeats[2].Name, Is.EqualTo("feat with other foci"));
            Assert.That(actualFeats[2].Foci, Is.EquivalentTo(["focus 3", "focus 4"]));
            Assert.That(actualFeats, Has.Length.EqualTo(3));
        }

        [Test]
        public void GetAdditionalFeats_CombineFoci_WithAllFoci()
        {
            var additionalFeats = new List<Feat>
            {
                new(),
                new(),
                new(),
            };

            additionalFeats[0].Name = "feat with foci";
            additionalFeats[0].Foci = ["focus 1", GroupConstants.All];
            additionalFeats[1].Name = "feat with other foci";
            additionalFeats[1].Foci = ["focus 3", "focus 4"];
            additionalFeats[2].Name = "feat with foci";
            additionalFeats[2].Foci = ["focus 5", "focus 6"];

            mockAdditionalFeatsGenerator.Setup(g => g.GenerateWith(characterClass, race, stats, skills, baseAttack, It.IsAny<IEnumerable<Feat>>())).Returns(additionalFeats);

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            var actualFeats = feats.Additional.ToArray();
            Assert.That(actualFeats[0].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[0].Foci, Is.EquivalentTo([GroupConstants.All]));
            Assert.That(actualFeats[1].Name, Is.EqualTo("feat with other foci"));
            Assert.That(actualFeats[1].Foci, Is.EquivalentTo(["focus 3", "focus 4"]));
            Assert.That(actualFeats, Has.Length.EqualTo(2));
        }

        [Test]
        public void GetAdditionalFeats_CombineFoci_SkipDifferentPowers()
        {
            var additionalFeats = new List<Feat>
            {
                new(),
                new(),
                new(),
            };

            additionalFeats[0].Name = "feat with foci";
            additionalFeats[0].Foci = ["focus 1", "focus 2"];
            additionalFeats[0].Power = 9266;
            additionalFeats[1].Name = "feat with foci";
            additionalFeats[1].Foci = ["focus 3", "focus 4"];
            additionalFeats[1].Power = 42;
            additionalFeats[2].Name = "feat with foci";
            additionalFeats[2].Foci = ["focus 5", "focus 6"];
            additionalFeats[2].Power = 9266;

            mockAdditionalFeatsGenerator.Setup(g => g.GenerateWith(characterClass, race, stats, skills, baseAttack, It.IsAny<IEnumerable<Feat>>())).Returns(additionalFeats);

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            var actualFeats = feats.Additional.ToArray();
            Assert.That(actualFeats[0].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[0].Foci, Is.EquivalentTo(["focus 1", "focus 2", "focus 5", "focus 6"]));
            Assert.That(actualFeats[0].Power, Is.EqualTo(9266));
            Assert.That(actualFeats[1].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[1].Foci, Is.EquivalentTo(["focus 3", "focus 4"]));
            Assert.That(actualFeats[1].Power, Is.EqualTo(42));
            Assert.That(actualFeats, Has.Length.EqualTo(2));
        }

        [Test]
        public void GetAdditionalFeats_CombineFoci_SkipDifferentFrequencies()
        {
            var additionalFeats = new List<Feat>
            {
                new(),
                new(),
                new(),
            };

            additionalFeats[0].Name = "feat with foci";
            additionalFeats[0].Foci = ["focus 1", "focus 2"];
            additionalFeats[0].Frequency.Quantity = 9266;
            additionalFeats[0].Frequency.TimePeriod = "sometimes";
            additionalFeats[1].Name = "feat with other foci";
            additionalFeats[1].Foci = ["focus 3", "focus 4"];
            additionalFeats[1].Frequency.Quantity = 42;
            additionalFeats[1].Frequency.TimePeriod = "sometimes";
            additionalFeats[2].Name = "feat with foci";
            additionalFeats[2].Foci = ["focus 5", "focus 6"];
            additionalFeats[2].Frequency.Quantity = 9266;
            additionalFeats[2].Frequency.TimePeriod = "sometimes";
            additionalFeats.Add(new()
            {
                Name = "feat with foci",
                Foci = ["focus 7", "focus 8"],
                Frequency = new Frequency
                {
                    Quantity = 9266,
                    TimePeriod = "often"
                }
            });

            mockAdditionalFeatsGenerator.Setup(g => g.GenerateWith(characterClass, race, stats, skills, baseAttack, It.IsAny<IEnumerable<Feat>>())).Returns(additionalFeats);

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            var actualFeats = feats.Additional.ToArray();
            Assert.That(actualFeats[0].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[0].Foci, Is.EquivalentTo(["focus 1", "focus 2", "focus 5", "focus 6"]));
            Assert.That(actualFeats[0].Frequency.Quantity, Is.EqualTo(9266));
            Assert.That(actualFeats[0].Frequency.TimePeriod, Is.EqualTo("sometimes"));
            Assert.That(actualFeats[1].Name, Is.EqualTo("feat with other foci"));
            Assert.That(actualFeats[1].Foci, Is.EquivalentTo(["focus 3", "focus 4"]));
            Assert.That(actualFeats[1].Frequency.Quantity, Is.EqualTo(42));
            Assert.That(actualFeats[1].Frequency.TimePeriod, Is.EqualTo("sometimes"));
            Assert.That(actualFeats[2].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[2].Foci, Is.EquivalentTo(["focus 7", "focus 8"]));
            Assert.That(actualFeats[2].Frequency.Quantity, Is.EqualTo(9266));
            Assert.That(actualFeats[2].Frequency.TimePeriod, Is.EqualTo("often"));
            Assert.That(actualFeats, Has.Length.EqualTo(3));
        }

        [Test]
        public void ConsolidateAllFeatsWithFoci()
        {
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());
            racialFeats.Add(new Feat());

            racialFeats[0].Name = "feat with foci";
            racialFeats[0].Foci = ["focus 1", "focus 2"];
            racialFeats[1].Name = "feat with other foci";
            racialFeats[1].Foci = ["focus 3", "focus 4"];
            racialFeats[2].Name = "feat with foci";
            racialFeats[2].Foci = ["focus 5", "focus 6"];

            var classFeats = new List<Feat>
            {
                new(),
                new(),
                new(),
            };

            classFeats[0].Name = "feat with foci";
            classFeats[0].Foci = ["focus 7", "focus 8"];
            classFeats[1].Name = "feat with other foci";
            classFeats[1].Foci = ["focus 9", "focus 10"];
            classFeats[2].Name = "feat with foci";
            classFeats[2].Foci = [GroupConstants.All];

            mockClassFeatsGenerator.Setup(g => g.GenerateWith(characterClass, race, stats, racialFeats, skills)).Returns(classFeats);

            var additionalFeats = new List<Feat>
            {
                new(),
                new(),
                new(),
            };

            additionalFeats[0].Name = "feat with foci";
            additionalFeats[0].Foci = ["focus 11", "focus 12"];
            additionalFeats[1].Name = "feat with other foci";
            additionalFeats[1].Foci = ["focus 13", "focus 14"];
            additionalFeats[2].Name = "feat with foci";
            additionalFeats[2].Foci = ["focus 15", "focus 16"];

            mockAdditionalFeatsGenerator.Setup(g => g.GenerateWith(characterClass, race, stats, skills, baseAttack, It.IsAny<IEnumerable<Feat>>())).Returns(additionalFeats);

            var feats = featsGenerator.GenerateWith(characterClass, race, stats, skills, baseAttack);
            var actualFeats = feats.Racial.ToArray();
            Assert.That(actualFeats[0].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[0].Foci, Is.EquivalentTo(["focus 1", "focus 2", "focus 5", "focus 6"]));
            Assert.That(actualFeats[1].Name, Is.EqualTo("feat with other foci"));
            Assert.That(actualFeats[1].Foci, Is.EquivalentTo(["focus 3", "focus 4"]));
            Assert.That(actualFeats, Has.Length.EqualTo(2));

            actualFeats = [.. feats.Class];
            Assert.That(actualFeats[0].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[0].Foci, Is.EquivalentTo([GroupConstants.All]));
            Assert.That(actualFeats[1].Name, Is.EqualTo("feat with other foci"));
            Assert.That(actualFeats[1].Foci, Is.EquivalentTo(["focus 9", "focus 10"]));
            Assert.That(actualFeats, Has.Length.EqualTo(2));

            actualFeats = [.. feats.Additional];
            Assert.That(actualFeats[0].Name, Is.EqualTo("feat with foci"));
            Assert.That(actualFeats[0].Foci, Is.EquivalentTo(["focus 11", "focus 12", "focus 15", "focus 16"]));
            Assert.That(actualFeats[1].Name, Is.EqualTo("feat with other foci"));
            Assert.That(actualFeats[1].Foci, Is.EquivalentTo(["focus 13", "focus 14"]));
            Assert.That(actualFeats, Has.Length.EqualTo(2));
        }
    }
}