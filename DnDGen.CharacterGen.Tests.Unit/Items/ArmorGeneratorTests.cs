using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Generators;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.RollGen;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Unit.Items
{
    [TestFixture]
    public partial class ArmorGeneratorTests
    {
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<IPercentileSelector> mockPercentileSelector;
        private Mock<MundaneItemGenerator> mockMundaneArmorGenerator;
        private Mock<MagicalItemGenerator> mockMagicalArmorGenerator;
        private Mock<JustInTimeFactory> mockJustInTimeFactory;
        private Mock<Dice> mockDice;
        private IArmorGenerator armorGenerator;
        private List<Feat> additionalFeats;
        private List<Feat> classFeats;
        private List<Feat> racialFeats;
        private FeatCollections feats;
        private CharacterClass characterClass;
        private string[] armorProficiencyFeats;
        private string[] shieldProficiencyFeats;
        private List<string> proficientArmors;
        private List<string> proficientShields;
        private Armor magicalArmor;
        private Armor magicalShield;
        private Race race;
        private string powerTableName;
        private string power;

        [SetUp]
        public void Setup()
        {
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockPercentileSelector = new Mock<IPercentileSelector>();
            mockMundaneArmorGenerator = new Mock<MundaneItemGenerator>();
            mockMagicalArmorGenerator = new Mock<MagicalItemGenerator>();
            mockJustInTimeFactory = new Mock<JustInTimeFactory>();
            mockDice = new Mock<Dice>();

            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(ItemTypeConstants.Armor)).Returns(mockMundaneArmorGenerator.Object);
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>(ItemTypeConstants.Armor)).Returns(mockMagicalArmorGenerator.Object);

            armorGenerator = new ArmorGenerator(mockCollectionsSelector.Object, mockPercentileSelector.Object, mockJustInTimeFactory.Object, mockDice.Object);

            additionalFeats = [];
            classFeats = [];
            racialFeats = [];
            feats = new FeatCollections
            {
                Additional = additionalFeats,
                Class = classFeats,
                Racial = racialFeats
            };
            characterClass = new CharacterClass();
            armorProficiencyFeats = [FeatConstants.LightArmorProficiency, FeatConstants.MediumArmorProficiency, FeatConstants.HeavyArmorProficiency];
            shieldProficiencyFeats = [FeatConstants.ShieldProficiency, FeatConstants.TowerShieldProficiency];
            proficientArmors = [];
            proficientShields = [];
            race = new Race
            {
                Size = "size"
            };
            magicalArmor = CreateArmor("magical armor");
            magicalArmor.IsMagical = true;
            magicalShield = CreateShield("magical shield");
            magicalShield.IsMagical = true;

            proficientArmors.Remove(magicalArmor.Name);
            proficientArmors.Add("my armor");
            proficientArmors.Add("other armor");
            proficientArmors.Add("metal armor");
            proficientArmors.Add("specific armor");
            proficientArmors.Add("specific metal armor");
            proficientShields.Remove(magicalShield.Name);
            proficientShields.Add("my shield");
            proficientShields.Add("other shield");
            proficientShields.Add("metal shield");
            proficientShields.Add("specific shield");
            proficientShields.Add("specific metal shield");

            characterClass.Level = 9266;
            additionalFeats.Add(new Feat { Name = FeatConstants.LightArmorProficiency });
            additionalFeats.Add(new Feat { Name = FeatConstants.ShieldProficiency });
            additionalFeats.Add(new Feat { Name = "other feat" });

            powerTableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, characterClass.Level);
            power = "my power";

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, ItemTypeConstants.Armor + GroupConstants.Proficiency))
                .Returns(armorProficiencyFeats);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, AttributeConstants.Shield + GroupConstants.Proficiency))
                .Returns(shieldProficiencyFeats);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.LightArmorProficiency))
                .Returns(proficientArmors);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.ShieldProficiency))
                .Returns(proficientShields);

            var index = 0;

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<string>>()))
                .Returns((IEnumerable<string> ss) => ss.ElementAt(index++ % ss.Count()));
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<string>>()))
                .Returns((IEnumerable<string> c, IEnumerable<string> u, IEnumerable<string> r, IEnumerable<string> vr) => GetWeightedRandom(c, u, r, vr, index++));

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Specific))
                .Returns(() => new[] { magicalArmor.Name, magicalShield.Name }
                    .Union(proficientArmors)
                    .Union(proficientShields)
                    .Where(a => a.Contains("specific")));
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Metal))
                .Returns(() => new[] { magicalArmor.Name, magicalShield.Name }
                    .Union(proficientArmors)
                    .Union(proficientShields)
                    .Where(a => a.Contains("metal")));

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(It.IsAny<int>())).Returns(false);
            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(It.IsAny<double>())).Returns(false);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(It.IsAny<int>())).Returns(false);
        }

        private static string GetWeightedRandom(IEnumerable<string> common, IEnumerable<string> uncommon, IEnumerable<string> rare, IEnumerable<string> veryRare, int index)
        {
            common ??= [];
            uncommon ??= [];
            rare ??= [];
            veryRare ??= [];

            var all = common.Concat(uncommon).Concat(rare).Concat(veryRare);
            var total = all.Count();

            return all.ElementAt(index % total);
        }

        [Test]
        public void GenerateArmorFrom_GenerateNoArmor_WhenNotProficient()
        {
            additionalFeats.Remove(additionalFeats[0]);
            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.Null);
        }

        [Test]
        public void GenerateArmorFrom_GenerateMundaneArmor()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            var mundaneArmor = CreateArmor("mundane armor");

            SetupSelectRandomArmor(["my armor", "other armor", "metal armor", "mundane armor"], "my random armor");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random armor", race.Size)).Returns(mundaneArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(mundaneArmor));
        }

        private void SetupSelectRandomArmor(string[] armors, string expected)
        {
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(
                    RandomWeightedCollection<string>.EquivalentSet(armors),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    null))
                .Returns(expected);
        }

        [Test]
        public void GenerateArmorFrom_GenerateMundaneArmor_UseCumulativeProficiencies()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            additionalFeats.Add(new Feat { Name = FeatConstants.HeavyArmorProficiency });
            additionalFeats.Add(new Feat { Name = FeatConstants.MediumArmorProficiency });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.HeavyArmorProficiency))
                .Returns(RandomArmorPermutation.HeavyArmors);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.MediumArmorProficiency))
                .Returns(RandomArmorPermutation.MediumArmors);

            var mundaneArmor = CreateArmor("mundane armor");
            var heavyArmor = CreateArmor("heavy armor");
            var mediumArmor = CreateArmor("medium armor");
            proficientArmors.Remove("heavy armor");
            proficientArmors.Remove("medium armor");

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(
                    RandomWeightedCollection<string>.EquivalentSet(FeatConstants.HeavyArmorProficiency),
                    RandomWeightedCollection<string>.EquivalentSet(FeatConstants.MediumArmorProficiency),
                    RandomWeightedCollection<string>.EquivalentSet(FeatConstants.LightArmorProficiency),
                    null))
                .Returns(FeatConstants.HeavyArmorProficiency);
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(
                    RandomWeightedCollection<string>.EquivalentSet(RandomArmorPermutation.HeavyArmors),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    null))
                .Returns("my random armor");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random armor", race.Size)).Returns(heavyArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(heavyArmor));
        }

        [TestCaseSource(nameof(RandomArmorData))]
        public void GenerateArmorFrom_GenerateMundaneArmor_FromRandom(RandomArmorPermutation permutation)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.HeavyArmorProficiency))
                .Returns(RandomArmorPermutation.HeavyArmors);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.MediumArmorProficiency))
                .Returns(RandomArmorPermutation.MediumArmors);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.LightArmorProficiency))
                .Returns(RandomArmorPermutation.LightArmors);

            var expectedArmor = CreateArmor(permutation.Armors.Expected);

            permutation.Racial.SetupMock(mockCollectionsSelector);
            permutation.Class.SetupMock(mockCollectionsSelector);
            permutation.Additional.SetupMock(mockCollectionsSelector);
            permutation.Armors.SetupMock(mockCollectionsSelector);

            mockMundaneArmorGenerator.Setup(g => g.Generate(permutation.Armors.Expected, race.Size)).Returns(expectedArmor);

            var armor = armorGenerator.GenerateArmorFrom(permutation.FeatCollection, characterClass, race);

            permutation.Racial.VerifyMock(mockCollectionsSelector);
            permutation.Class.VerifyMock(mockCollectionsSelector);
            permutation.Additional.VerifyMock(mockCollectionsSelector);
            permutation.Armors.VerifyMock(mockCollectionsSelector);

            mockMundaneArmorGenerator.Verify(g => g.Generate(permutation.Armors.Expected, race.Size), Times.Once);

            Assert.That(armor, Is.EqualTo(expectedArmor));
        }

        private static IEnumerable RandomArmorData
        {
            get
            {
                var permutations = new[]
                {
                    new RandomArmorPermutation("R-L:L"),
                    new RandomArmorPermutation("R-L:L;C-M:M"),
                    new RandomArmorPermutation("R-L:L;C-M:M;A-H:H"),
                    new RandomArmorPermutation("R-L:L;C-MH:M"),
                    new RandomArmorPermutation("R-L:L;C-MH:H"),
                    new RandomArmorPermutation("R-L:L;A-M:M"),
                    new RandomArmorPermutation("R-L:L;A-MH:M"),
                    new RandomArmorPermutation("R-L:L;A-MH:H"),
                    new RandomArmorPermutation("R-LM:L"),
                    new RandomArmorPermutation("R-LM:L;C-H:H"),
                    new RandomArmorPermutation("R-LM:L;A-H:H"),
                    new RandomArmorPermutation("R-LM:M"),
                    new RandomArmorPermutation("R-LM:M;C-H:H"),
                    new RandomArmorPermutation("R-LM:M;A-H:H"),
                    new RandomArmorPermutation("R-LMH:L"),
                    new RandomArmorPermutation("R-LMH:M"),
                    new RandomArmorPermutation("R-LMH:H"),

                    new RandomArmorPermutation("C-L:L"),
                    new RandomArmorPermutation("C-L:L;A-M:M"),
                    new RandomArmorPermutation("C-L:L;A-MH:M"),
                    new RandomArmorPermutation("C-L:L;A-MH:H"),
                    new RandomArmorPermutation("C-LM:L"),
                    new RandomArmorPermutation("C-LM:L;A-H:H"),
                    new RandomArmorPermutation("C-LM:M"),
                    new RandomArmorPermutation("C-LM:M;A-H:H"),
                    new RandomArmorPermutation("C-LMH:L"),
                    new RandomArmorPermutation("C-LMH:M"),
                    new RandomArmorPermutation("C-LMH:H"),

                    new RandomArmorPermutation("A-L:L"),
                    new RandomArmorPermutation("A-LM:L"),
                    new RandomArmorPermutation("A-LM:M"),
                    new RandomArmorPermutation("A-LMH:L"),
                    new RandomArmorPermutation("A-LMH:M"),
                    new RandomArmorPermutation("A-LMH:H"),
                };

                foreach (var permutation in permutations)
                {
                    yield return new TestCaseData(permutation).SetArgDisplayNames(permutation.Key);
                }
            }
        }

        public class RandomArmorPermutation
        {
            public string Key { get; init; }
            public FeatCollections FeatCollection { get; set; }
            public RandomWeightedCollection<string> Racial { get; set; }
            public RandomWeightedCollection<string> Class { get; set; }
            public RandomWeightedCollection<string> Additional { get; set; }
            public RandomWeightedCollection<string> Armors { get; set; }

            public static readonly string[] LightArmors = ["light armor", "other light armor"];
            public static readonly string[] MediumArmors = ["medium armor", "other medium armor"];
            public static readonly string[] HeavyArmors = ["heavy armor", "other heavy armor"];

            public RandomArmorPermutation(string key)
            {
                Key = key;
                FeatCollection = new();
                Racial = new() { VeryRare = null };
                Class = new() { VeryRare = null };
                Additional = new() { VeryRare = null };
                Armors = new() { VeryRare = null, Expected = "my random armor" };

                ParseKey();
            }

            private void ParseKey()
            {
                var sections = Key.Split(';');
                foreach (var section in sections)
                {
                    var parts = section.Split(':', '-');
                    var feats = parts[0];
                    var proficiencies = parts[1];
                    var expected = parts[2];

                    switch (feats)
                    {
                        case "R":
                            FeatCollection.Racial = SetProficiencies(proficiencies);
                            Racial = SetWeights(proficiencies, expected);
                            break;
                        case "C":
                            FeatCollection.Class = SetProficiencies(proficiencies);
                            Class = SetWeights(proficiencies, expected);
                            break;
                        case "A":
                            FeatCollection.Additional = SetProficiencies(proficiencies);
                            Additional = SetWeights(proficiencies, expected);
                            break;
                    }
                }

                Armors.Common = GetArmors(Additional.Expected);
                Armors.Uncommon = GetArmors(Class.Expected);
                Armors.Rare = GetArmors(Racial.Expected);
            }

            private string[] GetArmors(string feat)
            {
                return feat switch
                {
                    FeatConstants.LightArmorProficiency => LightArmors,
                    FeatConstants.MediumArmorProficiency => MediumArmors,
                    FeatConstants.HeavyArmorProficiency => HeavyArmors,
                    _ => [],
                };
            }

            private RandomWeightedCollection<string> SetWeights(string proficiencies, string expected)
            {
                var feats = proficiencies.Select(GetFeat);

                return new()
                {
                    Common = [.. feats.Intersect([FeatConstants.HeavyArmorProficiency])],
                    Uncommon = [.. feats.Intersect([FeatConstants.MediumArmorProficiency])],
                    Rare = [.. feats.Intersect([FeatConstants.LightArmorProficiency])],
                    VeryRare = null,
                    Expected = GetFeat(expected[0])
                };
            }

            private IEnumerable<Feat> SetProficiencies(string proficiencies)
            {
                var feats = new List<string> { "other feat" };
                feats.AddRange(proficiencies.Select(GetFeat));
                feats.Add("another feat");

                return feats.Select(f => new Feat { Name = f });
            }

            private string GetFeat(char p)
            {
                return p switch
                {
                    'L' => FeatConstants.LightArmorProficiency,
                    'M' => FeatConstants.MediumArmorProficiency,
                    'H' => FeatConstants.HeavyArmorProficiency,
                    _ => null,
                };
            }
        }

        [Test]
        public void GenerateArmorFrom_GenerateMundaneArmor_CannotBeSpecific()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            var mundaneArmor = CreateArmor("mundane armor");

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(It.Is<double>(t => t <= 1))).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(It.Is<int>(t => t <= 100))).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor", "metal armor", "mundane armor"))).Returns("my random armor");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random armor", race.Size)).Returns(mundaneArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(mundaneArmor));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void GenerateArmorFrom_GenerateMundaneArmor_DruidAndNonmetal_ByRoll(bool specialMaterial, bool dragonhide)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            var mundaneArmor = CreateArmor("mundane armor");

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor", "mundane armor"))).Returns("my random armor");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random armor", race.Size)).Returns(mundaneArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(mundaneArmor));
        }

        [Test]
        public void GenerateArmorFrom_GenerateMundaneArmor_DruidAndMetal()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            var mundaneArmor = CreateArmor("mundane armor");

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor", "metal armor", "mundane armor"))).Returns("metal armor");
            mockMundaneArmorGenerator.Setup(g => g.Generate("metal armor", race.Size, TraitConstants.SpecialMaterials.Dragonhide)).Returns(mundaneArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(mundaneArmor));
        }

        [Test]
        public void GenerateArmorFrom_GenerateMundaneArmor_DruidAndNonmetal_ByRandom()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            var mundaneArmor = CreateArmor("mundane armor");

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor", "metal armor", "mundane armor"))).Returns("my random armor");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random armor", race.Size)).Returns(mundaneArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(mundaneArmor));
        }

        private Armor CreateArmor(string name)
        {
            var armor = new Armor
            {
                Name = name,
                ItemType = ItemTypeConstants.Armor,
                Size = race.Size
            };

            proficientArmors.Add(name);

            return armor;
        }

        [Test]
        public void GenerateArmorFrom_GenerateMagicalArmor()
        {
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor", "metal armor"))).Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [Test]
        public void GenerateArmorFrom_GenerateMagicalArmor_UseCumulativeProficiencies()
        {
            additionalFeats.Add(new Feat { Name = FeatConstants.HeavyArmorProficiency });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.HeavyArmorProficiency))
                .Returns(["heavy armor", "other heavy armor"]);

            var heavyArmor = CreateArmor("heavy armor");
            proficientArmors.Remove("heavy armor");

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor", "metal armor", "heavy armor", "other heavy armor")))
                .Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(heavyArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(heavyArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateArmorFrom_GenerateMagicalArmor_Specific(string power, double specificThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("specific armor", "specific metal armor"))).Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98, 92)]
        [TestCase(PowerConstants.Medium, 0.97, 64)]
        [TestCase(PowerConstants.Major, 0.97, 64)]
        public void GenerateArmorFrom_GenerateMagicalArmor_SpecificOnReroll(string power, double specificThreshold, int rollThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(rollThreshold)).Returns(true);
            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(false).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("specific armor", "specific metal armor"))).Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateArmorFrom_GenerateMagicalArmor_NotSpecific_IfNoSpecificAvailable(string power, double specificThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            proficientArmors.Remove("specific armor");
            proficientArmors.Remove("specific metal armor");

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor", "metal armor"))).Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void GenerateArmorFrom_GenerateMagicalArmor_DruidAndNonmetal_ByRoll(bool specialMaterial, bool dragonhide)
        {
            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor"))).Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98, false, false)]
        [TestCase(PowerConstants.Minor, 0.98, false, true)]
        [TestCase(PowerConstants.Minor, 0.98, true, false)]
        [TestCase(PowerConstants.Medium, 0.97, false, false)]
        [TestCase(PowerConstants.Medium, 0.97, false, true)]
        [TestCase(PowerConstants.Medium, 0.97, true, false)]
        [TestCase(PowerConstants.Major, 0.97, false, false)]
        [TestCase(PowerConstants.Major, 0.97, false, true)]
        [TestCase(PowerConstants.Major, 0.97, true, false)]
        public void GenerateArmorFrom_GenerateMagicalArmor_DruidAndNonmetal_ByRoll_Specific(string power, double specificThreshold, bool specialMaterial, bool dragonhide)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("specific armor"))).Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98, 92, false, false)]
        [TestCase(PowerConstants.Minor, 0.98, 92, false, true)]
        [TestCase(PowerConstants.Minor, 0.98, 92, true, false)]
        [TestCase(PowerConstants.Medium, 0.97, 64, false, false)]
        [TestCase(PowerConstants.Medium, 0.97, 64, false, true)]
        [TestCase(PowerConstants.Medium, 0.97, 64, true, false)]
        [TestCase(PowerConstants.Major, 0.97, 64, false, false)]
        [TestCase(PowerConstants.Major, 0.97, 64, false, true)]
        [TestCase(PowerConstants.Major, 0.97, 64, true, false)]
        public void GenerateArmorFrom_GenerateMagicalArmor_DruidAndNonmetal_ByRoll_SpecificOnReroll(
            string power,
            double specificThreshold,
            int rollThreshold,
            bool specialMaterial,
            bool dragonhide)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(rollThreshold)).Returns(true);
            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(false).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("specific armor"))).Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98, false, false)]
        [TestCase(PowerConstants.Minor, 0.98, false, true)]
        [TestCase(PowerConstants.Minor, 0.98, true, false)]
        [TestCase(PowerConstants.Medium, 0.97, false, false)]
        [TestCase(PowerConstants.Medium, 0.97, false, true)]
        [TestCase(PowerConstants.Medium, 0.97, true, false)]
        [TestCase(PowerConstants.Major, 0.97, false, false)]
        [TestCase(PowerConstants.Major, 0.97, false, true)]
        [TestCase(PowerConstants.Major, 0.97, true, false)]
        public void GenerateArmorFrom_GenerateMagicalArmor_DruidAndNonmetal_ByRoll_NotSpecific_WhenNoneAvailable(
            string power,
            double specificThreshold,
            bool specialMaterial,
            bool dragonhide)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            proficientArmors.Remove("specific armor");
            //Not removing the metal specific armor, as it should be removed by not allowing metal

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor"))).Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [Test]
        public void GenerateArmorFrom_GenerateMagicalArmor_DruidAndMetal()
        {
            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor", "metal armor"))).Returns("metal armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "metal armor", race.Size, TraitConstants.SpecialMaterials.Dragonhide)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateArmorFrom_GenerateMagicalArmor_DruidAndMetal_Specific(string power, double specificThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("specific armor"))).Returns("specific random armor");
            //Can't apply Dragonhide to specific armor
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "specific random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98, 92)]
        [TestCase(PowerConstants.Medium, 0.97, 64)]
        [TestCase(PowerConstants.Major, 0.97, 64)]
        public void GenerateArmorFrom_GenerateMagicalArmor_DruidAndMetal_SpecificOnReroll(string power, double specificThreshold, int rollThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(rollThreshold)).Returns(true);
            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(false).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("specific armor"))).Returns("specific random armor");
            //Can't apply Dragonhide to specific armor
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "specific random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateArmorFrom_GenerateMagicalArmor_DruidAndMetal_NotSpecific_WhenNoneAvailable_Proficiency(string power, double specificThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            proficientArmors.Remove("specific armor");
            proficientArmors.Remove("specific metal armor");

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor", "metal armor"))).Returns("metal armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "metal armor", race.Size, TraitConstants.SpecialMaterials.Dragonhide)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateArmorFrom_GenerateMagicalArmor_DruidAndMetal_NotSpecific_WhenNoneAvailable_AllMetal(string power, double specificThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            proficientArmors.Remove("specific armor");

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            SetupSelectRandomArmor(["my armor", "other armor", "metal armor"], "metal armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "metal armor", race.Size, TraitConstants.SpecialMaterials.Dragonhide)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [Test]
        public void GenerateArmorFrom_GenerateMagicalArmor_DruidAndNonmetal_ByRandom()
        {
            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor", "metal armor"))).Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateArmorFrom_GenerateMagicalArmor_DruidAndNonmetal_ByRandom_Specific(string power, double specificThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("specific armor"))).Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98, 92)]
        [TestCase(PowerConstants.Medium, 0.97, 64)]
        [TestCase(PowerConstants.Major, 0.97, 64)]
        public void GenerateArmorFrom_GenerateMagicalArmor_DruidAndNonmetal_ByRandom_SpecificOnReroll(string power, double specificThreshold, int rollThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(rollThreshold)).Returns(true);
            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(false).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("specific armor"))).Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateArmorFrom_GenerateMagicalArmor_DruidAndNonmetal_ByRandom_NotSpecific_WhenNoneAvailable_Proficiency(string power, double specificThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            proficientArmors.Remove("specific armor");
            proficientArmors.Remove("specific metal armor");

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor", "metal armor"))).Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateArmorFrom_GenerateMagicalArmor_DruidAndNonmetal_ByRandom_NotSpecific_WhenNoneAvailable_AllMetal(string power, double specificThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            proficientArmors.Remove("specific armor");

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor", "metal armor"))).Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [Test]
        public void GenerateShieldFrom_GenerateNoShield_WhenNotProficient()
        {
            additionalFeats.Remove(additionalFeats[0]);
            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.Null);
        }

        [Test]
        public void GenerateShieldFrom_GenerateMundaneShield()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            var mundaneShield = CreateShield("mundane shield");

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield", "mundane shield"))).Returns("my random shield");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random shield", race.Size)).Returns(mundaneShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(mundaneShield));
        }

        [Test]
        public void GenerateShieldFrom_GenerateMundaneShield_UseCumulativeProficiencies()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            additionalFeats.Add(new Feat { Name = FeatConstants.TowerShieldProficiency });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.TowerShieldProficiency))
                .Returns(["heavy shield", "other heavy shield"]);

            var mundaneShield = CreateShield("mundane shield");
            var heavyShield = CreateShield("heavy shield");
            proficientShields.Remove("heavy shield");

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield", "heavy shield", "other heavy shield", "mundane shield")))
                .Returns("my random shield");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random shield", race.Size)).Returns(heavyShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(heavyShield));
        }

        [Test]
        public void GenerateShieldFrom_GenerateMundaneShield_CannotBeSpecific()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            var mundaneShield = CreateShield("mundane shield");

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(It.Is<double>(t => t <= 1))).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(It.Is<int>(t => t <= 100))).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield", "mundane shield"))).Returns("my random shield");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random shield", race.Size)).Returns(mundaneShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(mundaneShield));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void GenerateShieldFrom_GenerateMundaneShield_DruidAndNonmetal_ByRoll(bool specialMaterial, bool dragonhide)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            var mundaneShield = CreateShield("mundane shield");

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "mundane shield"))).Returns("my random shield");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random shield", race.Size)).Returns(mundaneShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(mundaneShield));
        }

        [Test]
        public void GenerateShieldFrom_GenerateMundaneShield_DruidAndMetal()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            var mundaneShield = CreateShield("mundane shield");

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield", "mundane shield"))).Returns("metal shield");
            mockMundaneArmorGenerator.Setup(g => g.Generate("metal shield", race.Size, TraitConstants.SpecialMaterials.Dragonhide)).Returns(mundaneShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(mundaneShield));
        }

        [Test]
        public void GenerateShieldFrom_GenerateMundaneShield_DruidAndNonmetal_ByRandom()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(PowerConstants.Mundane);

            var mundaneShield = CreateShield("mundane shield");

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield", "mundane shield")))
                .Returns("my random shield");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random shield", race.Size)).Returns(mundaneShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(mundaneShield));
        }

        [Test]
        public void GenerateShieldFrom_GenerateMagicalShield()
        {
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield"))).Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [Test]
        public void GenerateShieldFrom_GenerateMagicalShield_UseCumulativeProficiencies()
        {
            additionalFeats.Add(new Feat { Name = FeatConstants.TowerShieldProficiency });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.TowerShieldProficiency))
                .Returns(["heavy shield", "other heavy shield"]);

            var heavyShield = CreateShield("heavy shield");
            proficientShields.Remove("heavy shield");

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield", "heavy shield", "other heavy shield")))
                .Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(heavyShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(heavyShield));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateShieldFrom_GenerateMagicalShield_Specific(string power, double specificThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("specific shield", "specific metal shield"))).Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(PowerConstants.Minor, 0.98, 92)]
        [TestCase(PowerConstants.Medium, 0.97, 64)]
        [TestCase(PowerConstants.Major, 0.97, 64)]
        public void GenerateShieldFrom_GenerateMagicalShield_SpecificOnReroll(string power, double specificThreshold, int rollThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(rollThreshold)).Returns(true);
            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(false).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("specific shield", "specific metal shield"))).Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateShieldFrom_GenerateMagicalShield_NotSpecific_IfNoSpecificAvailable(string power, double specificThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            proficientShields.Remove("specific shield");
            proficientShields.Remove("specific metal shield");

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield"))).Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void GenerateShieldFrom_GenerateMagicalShield_DruidAndNonmetal_ByRoll(bool specialMaterial, bool dragonhide)
        {
            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield"))).Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(PowerConstants.Minor, 0.98, false, false)]
        [TestCase(PowerConstants.Minor, 0.98, false, true)]
        [TestCase(PowerConstants.Minor, 0.98, true, false)]
        [TestCase(PowerConstants.Medium, 0.97, false, false)]
        [TestCase(PowerConstants.Medium, 0.97, false, true)]
        [TestCase(PowerConstants.Medium, 0.97, true, false)]
        [TestCase(PowerConstants.Major, 0.97, false, false)]
        [TestCase(PowerConstants.Major, 0.97, false, true)]
        [TestCase(PowerConstants.Major, 0.97, true, false)]
        public void GenerateShieldFrom_GenerateMagicalShield_DruidAndNonmetal_ByRoll_Specific(string power, double specificThreshold, bool specialMaterial, bool dragonhide)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("specific shield"))).Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(PowerConstants.Minor, 0.98, 92, false, false)]
        [TestCase(PowerConstants.Minor, 0.98, 92, false, true)]
        [TestCase(PowerConstants.Minor, 0.98, 92, true, false)]
        [TestCase(PowerConstants.Medium, 0.97, 64, false, false)]
        [TestCase(PowerConstants.Medium, 0.97, 64, false, true)]
        [TestCase(PowerConstants.Medium, 0.97, 64, true, false)]
        [TestCase(PowerConstants.Major, 0.97, 64, false, false)]
        [TestCase(PowerConstants.Major, 0.97, 64, false, true)]
        [TestCase(PowerConstants.Major, 0.97, 64, true, false)]
        public void GenerateShieldFrom_GenerateMagicalShield_DruidAndNonmetal_ByRoll_SpecificOnReroll(
            string power,
            double specificThreshold,
            int rollThreshold,
            bool specialMaterial,
            bool dragonhide)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(rollThreshold)).Returns(true);
            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(false).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("specific shield"))).Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(PowerConstants.Minor, 0.98, false, false)]
        [TestCase(PowerConstants.Minor, 0.98, false, true)]
        [TestCase(PowerConstants.Minor, 0.98, true, false)]
        [TestCase(PowerConstants.Medium, 0.97, false, false)]
        [TestCase(PowerConstants.Medium, 0.97, false, true)]
        [TestCase(PowerConstants.Medium, 0.97, true, false)]
        [TestCase(PowerConstants.Major, 0.97, false, false)]
        [TestCase(PowerConstants.Major, 0.97, false, true)]
        [TestCase(PowerConstants.Major, 0.97, true, false)]
        public void GenerateShieldFrom_GenerateMagicalShield_DruidAndNonmetal_ByRoll_NotSpecific_WhenNoneAvailable(
            string power,
            double specificThreshold,
            bool specialMaterial,
            bool dragonhide)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            proficientShields.Remove("specific shield");
            //Not removing the metal specific shield, as it should be removed by not allowing metal

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield"))).Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [Test]
        public void GenerateShieldFrom_GenerateMagicalShield_DruidAndMetal()
        {
            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield"))).Returns("metal shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "metal shield", race.Size, TraitConstants.SpecialMaterials.Dragonhide)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateShieldFrom_GenerateMagicalShield_DruidAndMetal_Specific(string power, double specificThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("specific shield"))).Returns("specific random shield");
            //Can't apply Dragonhide to specific armor
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "specific random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(PowerConstants.Minor, 0.98, 92)]
        [TestCase(PowerConstants.Medium, 0.97, 64)]
        [TestCase(PowerConstants.Major, 0.97, 64)]
        public void GenerateShieldFrom_GenerateMagicalShield_DruidAndMetal_SpecificOnReroll(string power, double specificThreshold, int rollThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(rollThreshold)).Returns(true);
            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(false).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("specific shield"))).Returns("specific random shield");
            //Can't apply Dragonhide to specific shield
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "specific random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateShieldFrom_GenerateMagicalShield_DruidAndMetal_NotSpecific_WhenNoneAvailable_Proficiency(string power, double specificThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            proficientShields.Remove("specific shield");
            proficientShields.Remove("specific metal shield");

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield"))).Returns("metal shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "metal shield", race.Size, TraitConstants.SpecialMaterials.Dragonhide)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateShieldFrom_GenerateMagicalShield_DruidAndMetal_NotSpecific_WhenNoneAvailable_AllMetal(string power, double specificThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            proficientShields.Remove("specific shield");

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield"))).Returns("metal shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "metal shield", race.Size, TraitConstants.SpecialMaterials.Dragonhide)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [Test]
        public void GenerateShieldFrom_GenerateMagicalShield_DruidAndNonmetal_ByRandom()
        {
            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield"))).Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateShieldFrom_GenerateMagicalShield_DruidAndNonmetal_ByRandom_Specific(string power, double specificThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("specific shield"))).Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(PowerConstants.Minor, 0.98, 92)]
        [TestCase(PowerConstants.Medium, 0.97, 64)]
        [TestCase(PowerConstants.Major, 0.97, 64)]
        public void GenerateShieldFrom_GenerateMagicalShield_DruidAndNonmetal_ByRandom_SpecificOnReroll(string power, double specificThreshold, int rollThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(rollThreshold)).Returns(true);
            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(false).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("specific shield"))).Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateShieldFrom_GenerateMagicalShield_DruidAndNonmetal_ByRandom_NotSpecific_WhenNoneAvailable_Proficiency(string power, double specificThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            proficientShields.Remove("specific shield");
            proficientShields.Remove("specific metal shield");

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield"))).Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateShieldFrom_GenerateMagicalShield_DruidAndNonmetal_ByRandom_NotSpecific_WhenNoneAvailable_AllMetal(string power, double specificThreshold)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, powerTableName)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            proficientShields.Remove("specific shield");

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield"))).Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        private Armor CreateShield(string name)
        {
            var shield = new Armor();
            shield.Name = name;
            shield.ItemType = ItemTypeConstants.Armor;
            shield.Attributes = new[] { AttributeConstants.Shield };
            shield.Size = race.Size;

            proficientShields.Add(name);

            return shield;
        }

        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(4, 2)]
        [TestCase(5, 2)]
        [TestCase(6, 3)]
        [TestCase(7, 3)]
        [TestCase(8, 4)]
        [TestCase(9, 4)]
        [TestCase(10, 5)]
        [TestCase(11, 5)]
        [TestCase(12, 6)]
        [TestCase(13, 6)]
        [TestCase(14, 7)]
        [TestCase(15, 7)]
        [TestCase(16, 8)]
        [TestCase(17, 8)]
        [TestCase(18, 9)]
        [TestCase(19, 9)]
        [TestCase(20, 10)]
        public void GenerateArmorFrom_NPCArmorIsHalfLevel(int npcLevel, int effectiveLevel)
        {
            characterClass.Level = npcLevel;
            characterClass.Name = "class name";
            characterClass.IsNPC = true;

            var npcArmor = CreateArmor("npc armor");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, effectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("npc power");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor", "metal armor", "npc armor"))).Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate("npc power", "my random armor", race.Size)).Returns(npcArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(npcArmor));
        }

        [Test]
        public void GenerateArmorFrom_LevelAdjustmentAffectsNPCLevelForArmor()
        {
            characterClass.Level = 9266;
            characterClass.LevelAdjustment = 90210;
            characterClass.Name = "class name";
            characterClass.IsNPC = true;

            var npcArmor = CreateArmor("npc armor");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, (9266 + 90210) / 2);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("npc power");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor", "metal armor", "npc armor"))).Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate("npc power", "my random armor", race.Size)).Returns(npcArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(npcArmor));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        [TestCase(6, 6)]
        [TestCase(7, 7)]
        [TestCase(8, 8)]
        [TestCase(9, 9)]
        [TestCase(10, 10)]
        [TestCase(11, 11)]
        [TestCase(12, 12)]
        [TestCase(13, 13)]
        [TestCase(14, 14)]
        [TestCase(15, 15)]
        [TestCase(16, 16)]
        [TestCase(17, 17)]
        [TestCase(18, 18)]
        [TestCase(19, 19)]
        [TestCase(20, 20)]
        public void GenerateArmorFrom_PlayerCharacterArmorIsFullLevel(int level, int effectiveLevel)
        {
            characterClass.Level = level;
            characterClass.Name = "class name";
            characterClass.IsNPC = false;

            var playerArmor = CreateArmor("player armor");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, effectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("player power");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor", "metal armor", "player armor"))).Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate("player power", "my random armor", race.Size)).Returns(playerArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(playerArmor));
        }

        [Test]
        public void GenerateArmorFrom_LevelAdjustmentAffectsPlayerLevelForArmor()
        {
            characterClass.Level = 9266;
            characterClass.LevelAdjustment = 90210;
            characterClass.Name = "class name";
            characterClass.IsNPC = false;

            var playerArmor = CreateArmor("player armor");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, 9266 + 90210);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("player power");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my armor", "other armor", "metal armor", "player armor"))).Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate("player power", "my random armor", race.Size)).Returns(playerArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(playerArmor));
        }

        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(4, 2)]
        [TestCase(5, 2)]
        [TestCase(6, 3)]
        [TestCase(7, 3)]
        [TestCase(8, 4)]
        [TestCase(9, 4)]
        [TestCase(10, 5)]
        [TestCase(11, 5)]
        [TestCase(12, 6)]
        [TestCase(13, 6)]
        [TestCase(14, 7)]
        [TestCase(15, 7)]
        [TestCase(16, 8)]
        [TestCase(17, 8)]
        [TestCase(18, 9)]
        [TestCase(19, 9)]
        [TestCase(20, 10)]
        public void GenerateShieldFrom_NPCShieldIsHalfLevel(int npcLevel, int effectiveLevel)
        {
            characterClass.Level = npcLevel;
            characterClass.Name = "class name";
            characterClass.IsNPC = true;

            var npcShield = CreateShield("npc shield");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, effectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("npc power");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield", "npc shield"))).Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate("npc power", "my random shield", race.Size)).Returns(npcShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(npcShield));
        }

        [Test]
        public void GenerateShieldFrom_LevelAdjustmentAffectsNPCLevelForShield()
        {
            characterClass.Level = 9266;
            characterClass.LevelAdjustment = 90210;
            characterClass.Name = "class name";
            characterClass.IsNPC = true;

            var npcShield = CreateShield("npc shield");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, (9266 + 90210) / 2);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("npc power");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield", "npc shield"))).Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate("npc power", "my random shield", race.Size)).Returns(npcShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(npcShield));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        [TestCase(6, 6)]
        [TestCase(7, 7)]
        [TestCase(8, 8)]
        [TestCase(9, 9)]
        [TestCase(10, 10)]
        [TestCase(11, 11)]
        [TestCase(12, 12)]
        [TestCase(13, 13)]
        [TestCase(14, 14)]
        [TestCase(15, 15)]
        [TestCase(16, 16)]
        [TestCase(17, 17)]
        [TestCase(18, 18)]
        [TestCase(19, 19)]
        [TestCase(20, 20)]
        public void GenerateShieldFrom_PlayerCharacterShieldIsFullLevel(int level, int effectiveLevel)
        {
            characterClass.Level = level;
            characterClass.Name = "class name";
            characterClass.IsNPC = false;

            var playerShield = CreateShield("player shield");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, effectiveLevel);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("player power");
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield", "player shield")))
                .Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate("player power", "my random shield", race.Size)).Returns(playerShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(playerShield));
        }

        [Test]
        public void GenerateShieldFrom_LevelAdjustmentAffectsPlayerLevelForShield()
        {
            characterClass.Level = 9266;
            characterClass.LevelAdjustment = 90210;
            characterClass.Name = "class name";
            characterClass.IsNPC = false;

            var playerShield = CreateShield("player shield");

            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, 9266 + 90210);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("player power");
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("my shield", "other shield", "metal shield", "player shield")))
                .Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate("player power", "my random shield", race.Size)).Returns(playerShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(playerShield));
        }
    }
}
