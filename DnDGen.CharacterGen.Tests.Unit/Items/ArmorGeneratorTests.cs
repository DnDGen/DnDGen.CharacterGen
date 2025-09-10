using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items;
using DnDGen.CharacterGen.Items.Selectors;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Generators;
using DnDGen.Infrastructure.Selectors.Collections;
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
    public class ArmorGeneratorTests
    {
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<ITreasureLevelSelector> mockTreasureLevelSelector;
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
        private Dictionary<string, List<string>> proficientArmors;
        private Dictionary<string, List<string>> proficientShields;
        private Armor magicalArmor;
        private Armor magicalShield;
        private Race race;
        private string power;

        [SetUp]
        public void Setup()
        {
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockTreasureLevelSelector = new Mock<ITreasureLevelSelector>();
            mockMundaneArmorGenerator = new Mock<MundaneItemGenerator>();
            mockMagicalArmorGenerator = new Mock<MagicalItemGenerator>();
            mockJustInTimeFactory = new Mock<JustInTimeFactory>();
            mockDice = new Mock<Dice>();

            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(ItemTypeConstants.Armor)).Returns(mockMundaneArmorGenerator.Object);
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>(ItemTypeConstants.Armor)).Returns(mockMagicalArmorGenerator.Object);

            armorGenerator = new ArmorGenerator(mockCollectionsSelector.Object, mockJustInTimeFactory.Object, mockDice.Object, mockTreasureLevelSelector.Object);

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
            proficientArmors = new()
            {
                [FeatConstants.LightArmorProficiency] = [],
                [FeatConstants.MediumArmorProficiency] = [],
                [FeatConstants.HeavyArmorProficiency] = [],
            };
            proficientShields = new()
            {
                [FeatConstants.ShieldProficiency] = [],
                [FeatConstants.TowerShieldProficiency] = [],
            };
            race = new Race
            {
                Size = "size"
            };
            magicalArmor = CreateArmor("magical armor");
            magicalArmor.IsMagical = true;
            magicalShield = CreateShield("magical shield");
            magicalShield.IsMagical = true;

            proficientArmors[FeatConstants.LightArmorProficiency].Add("my armor");
            proficientArmors[FeatConstants.LightArmorProficiency].Add("other armor");
            proficientArmors[FeatConstants.LightArmorProficiency].Add("metal armor");
            proficientArmors[FeatConstants.LightArmorProficiency].Add("specific armor");
            proficientArmors[FeatConstants.LightArmorProficiency].Add("specific metal armor");
            proficientShields.Remove(magicalShield.Name);
            proficientShields[FeatConstants.ShieldProficiency].Add("my shield");
            proficientShields[FeatConstants.ShieldProficiency].Add("other shield");
            proficientShields[FeatConstants.ShieldProficiency].Add("metal shield");
            proficientShields[FeatConstants.ShieldProficiency].Add("specific shield");
            proficientShields[FeatConstants.ShieldProficiency].Add("specific metal shield");

            characterClass.Level = 9266;
            additionalFeats.Add(new Feat { Name = FeatConstants.LightArmorProficiency });
            additionalFeats.Add(new Feat { Name = FeatConstants.ShieldProficiency });
            additionalFeats.Add(new Feat { Name = "other feat" });

            power = "my power";
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, ItemTypeConstants.Armor + GroupConstants.Proficiency))
                .Returns(armorProficiencyFeats);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, AttributeConstants.Shield + GroupConstants.Proficiency))
                .Returns(shieldProficiencyFeats);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.LightArmorProficiency))
                .Returns(proficientArmors[FeatConstants.LightArmorProficiency]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.MediumArmorProficiency))
                .Returns(proficientArmors[FeatConstants.MediumArmorProficiency]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.HeavyArmorProficiency))
                .Returns(proficientArmors[FeatConstants.HeavyArmorProficiency]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.ShieldProficiency))
                .Returns(proficientShields[FeatConstants.ShieldProficiency]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, FeatConstants.TowerShieldProficiency))
                .Returns(proficientShields[FeatConstants.TowerShieldProficiency]);

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
                .Returns(GetWeightedRandom);

            string GetWeightedRandom(IEnumerable<string> common, IEnumerable<string> uncommon, IEnumerable<string> rare, IEnumerable<string> veryRare)
            {
                common ??= [];
                uncommon ??= [];
                rare ??= [];
                veryRare ??= [];

                var all = common.Concat(uncommon).Concat(rare).Concat(veryRare);
                var total = all.Count();

                return all.ElementAt(index++ % total);
            }

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Specific))
                .Returns(() => new[] { magicalArmor.Name, magicalShield.Name }
                    .Union(proficientArmors.Values.SelectMany(a => a))
                    .Union(proficientShields.Values.SelectMany(a => a))
                    .Where(a => a.Contains("specific")));
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Metal))
                .Returns(() => new[] { magicalArmor.Name, magicalShield.Name }
                    .Union(proficientArmors.Values.SelectMany(a => a))
                    .Union(proficientShields.Values.SelectMany(a => a))
                    .Where(a => a.Contains("metal")));

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(It.IsAny<double>())).Returns(false);
            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(It.IsAny<int>())).Returns(false);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(It.IsAny<int>())).Returns(false);
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
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            var mundaneArmor = CreateArmor("mundane armor", FeatConstants.LightArmorProficiency);

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
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            additionalFeats.Add(new Feat { Name = FeatConstants.HeavyArmorProficiency });
            additionalFeats.Add(new Feat { Name = FeatConstants.MediumArmorProficiency });

            proficientArmors[FeatConstants.HeavyArmorProficiency].AddRange(RandomArmorPermutation.HeavyArmors);
            proficientArmors[FeatConstants.MediumArmorProficiency].AddRange(RandomArmorPermutation.MediumArmors);

            var mundaneArmor = CreateArmor("mundane armor");
            var heavyArmor = CreateArmor("heavy armor");
            var mediumArmor = CreateArmor("medium armor");

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
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            proficientArmors[FeatConstants.HeavyArmorProficiency].AddRange(RandomArmorPermutation.HeavyArmors);
            proficientArmors[FeatConstants.MediumArmorProficiency].AddRange(RandomArmorPermutation.MediumArmors);
            proficientArmors[FeatConstants.LightArmorProficiency].Clear();
            proficientArmors[FeatConstants.LightArmorProficiency].AddRange(RandomArmorPermutation.LightArmors);

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
                        default: throw new ArgumentException($"Unknown feat source '{feats}'");
                    }
                }

                Armors.Common = GetArmors(Additional.Expected);
                Armors.Uncommon = GetArmors(Class.Expected);
                Armors.Rare = GetArmors(Racial.Expected);
            }

            private static string[] GetArmors(string feat) => feat switch
            {
                FeatConstants.LightArmorProficiency => LightArmors,
                FeatConstants.MediumArmorProficiency => MediumArmors,
                FeatConstants.HeavyArmorProficiency => HeavyArmors,
                _ => [],
            };

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

            private string GetFeat(char p) => p switch
            {
                'L' => FeatConstants.LightArmorProficiency,
                'M' => FeatConstants.MediumArmorProficiency,
                'H' => FeatConstants.HeavyArmorProficiency,
                _ => throw new ArgumentException($"Unknown feat character '{p}'"),
            };
        }

        [Test]
        public void GenerateArmorFrom_GenerateMundaneArmor_NoDruidYesSpecificYesRerollNoDragonhide_MundaneCannotBeSpecific()
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            var mundaneArmor = CreateArmor("mundane armor", FeatConstants.LightArmorProficiency);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(It.Is<double>(t => t <= 1))).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(It.Is<int>(t => t <= 100))).Returns(true);

            SetupSelectRandomArmor(["my armor", "other armor", "metal armor", "mundane armor"], "my random armor");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random armor", race.Size)).Returns(mundaneArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(mundaneArmor));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void GenerateArmorFrom_GenerateMundaneArmor_YesDruidNoSpecific0RerollNoDragonhide_ByRoll_ReturnNonmetalArmor(bool specialMaterial, bool dragonhide)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            var mundaneArmor = CreateArmor("mundane armor", FeatConstants.LightArmorProficiency);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            SetupSelectRandomArmor(["my armor", "other armor", "mundane armor"], "my random armor");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random armor", race.Size)).Returns(mundaneArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(mundaneArmor));
        }

        [Test]
        public void GenerateArmorFrom_GenerateMundaneArmor_YesDruidNoSpecific0RerollYesDragonhide_ReturnsDragonhideArmor()
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            var mundaneArmor = CreateArmor("mundane armor", FeatConstants.LightArmorProficiency);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            SetupSelectRandomArmor(["my armor", "other armor", "metal armor", "mundane armor"], "metal armor");
            mockMundaneArmorGenerator.Setup(g => g.Generate("metal armor", race.Size, TraitConstants.SpecialMaterials.Dragonhide)).Returns(mundaneArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(mundaneArmor));
        }

        [Test]
        public void GenerateArmorFrom_GenerateMundaneArmor_YesDruidNoSpecific0RerollYesDragonhide_ReturnsNonmetalArmor()
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            var mundaneArmor = CreateArmor("mundane armor", FeatConstants.LightArmorProficiency);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            SetupSelectRandomArmor(["my armor", "other armor", "metal armor", "mundane armor"], "my random armor");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random armor", race.Size)).Returns(mundaneArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(mundaneArmor));
        }

        private Armor CreateArmor(string name, string feat = null)
        {
            var armor = new Armor
            {
                Name = name,
                ItemType = ItemTypeConstants.Armor,
                Size = race.Size
            };

            if (feat is not null)
                proficientArmors[feat].Add(name);

            return armor;
        }

        [Test]
        public void GenerateArmorFrom_GenerateMagicalArmor()
        {
            SetupSelectRandomArmor(["my armor", "other armor", "metal armor"], "my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [Test]
        public void GenerateArmorFrom_GenerateMagicalArmor_UseCumulativeProficiencies()
        {
            additionalFeats.Add(new Feat { Name = FeatConstants.HeavyArmorProficiency });
            var heavyArmor = CreateArmor("heavy armor", FeatConstants.HeavyArmorProficiency);
            proficientArmors[FeatConstants.HeavyArmorProficiency].Add("other heavy armor");

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(
                    RandomWeightedCollection<string>.EquivalentSet(FeatConstants.HeavyArmorProficiency),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    RandomWeightedCollection<string>.EquivalentSet(FeatConstants.LightArmorProficiency),
                    null))
                .Returns(FeatConstants.HeavyArmorProficiency);
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(
                    RandomWeightedCollection<string>.EquivalentSet("heavy armor", "other heavy armor"),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    null))
                .Returns("my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(heavyArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(heavyArmor));
        }

        [TestCaseSource(nameof(RandomArmorData))]
        public void GenerateArmorFrom_GenerateMagicalArmor_FromRandom(RandomArmorPermutation permutation)
        {
            proficientArmors[FeatConstants.HeavyArmorProficiency].AddRange(RandomArmorPermutation.HeavyArmors);
            proficientArmors[FeatConstants.MediumArmorProficiency].AddRange(RandomArmorPermutation.MediumArmors);
            proficientArmors[FeatConstants.LightArmorProficiency].Clear();
            proficientArmors[FeatConstants.LightArmorProficiency].AddRange(RandomArmorPermutation.LightArmors);

            var expectedArmor = CreateArmor(permutation.Armors.Expected);

            permutation.Racial.SetupMock(mockCollectionsSelector);
            permutation.Class.SetupMock(mockCollectionsSelector);
            permutation.Additional.SetupMock(mockCollectionsSelector);
            permutation.Armors.SetupMock(mockCollectionsSelector);

            mockMagicalArmorGenerator.Setup(g => g.Generate(power, permutation.Armors.Expected, race.Size)).Returns(expectedArmor);

            var armor = armorGenerator.GenerateArmorFrom(permutation.FeatCollection, characterClass, race);

            permutation.Racial.VerifyMock(mockCollectionsSelector);
            permutation.Class.VerifyMock(mockCollectionsSelector);
            permutation.Additional.VerifyMock(mockCollectionsSelector);
            permutation.Armors.VerifyMock(mockCollectionsSelector);

            mockMagicalArmorGenerator.Verify(g => g.Generate(power, permutation.Armors.Expected, race.Size), Times.Once);

            Assert.That(armor, Is.EqualTo(expectedArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateArmorFrom_GenerateMagicalArmor_NoDruidNoSpecific0RerollNoDragonhide(string power, double specificThreshold)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(false);

            SetupSelectRandomArmor(["my armor", "other armor", "metal armor"], "my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateArmorFrom_GenerateMagicalArmor_NoDruidYesSpecific0RerollNoDragonhide(string power, double specificThreshold)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            SetupSelectRandomArmor(["specific armor", "specific metal armor"], "my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98, 92)]
        [TestCase(PowerConstants.Medium, 0.97, 64)]
        [TestCase(PowerConstants.Major, 0.97, 64)]
        public void GenerateArmorFrom_GenerateMagicalArmor_NoDruidNoSpecific1RerollNoDragonhide(string power, double specificThreshold, int rerollThreshold)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(false);
            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(rerollThreshold)).Returns(true).Returns(false);

            SetupSelectRandomArmor(["my armor", "other armor", "metal armor"], "my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98, 92)]
        [TestCase(PowerConstants.Medium, 0.97, 64)]
        [TestCase(PowerConstants.Major, 0.97, 64)]
        public void GenerateArmorFrom_GenerateMagicalArmor_NoDruidYesSpecific1RerollNoDragonhide(string power, double specificThreshold, int rerollThreshold)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(false).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(rerollThreshold)).Returns(true);

            SetupSelectRandomArmor(["specific armor", "specific metal armor"], "my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98, 92)]
        [TestCase(PowerConstants.Medium, 0.97, 64)]
        [TestCase(PowerConstants.Major, 0.97, 64)]
        public void GenerateArmorFrom_GenerateMagicalArmor_NoDruidNoSpecific2RerollNoDragonhide(string power, double specificThreshold, int rerollThreshold)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(false);
            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(rerollThreshold)).Returns(true).Returns(true).Returns(false);

            SetupSelectRandomArmor(["my armor", "other armor", "metal armor"], "my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98, 92)]
        [TestCase(PowerConstants.Medium, 0.97, 64)]
        [TestCase(PowerConstants.Major, 0.97, 64)]
        public void GenerateArmorFrom_GenerateMagicalArmor_NoDruidYesSpecific2RerollNoDragonhide(string power, double specificThreshold, int rerollThreshold)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(false).Returns(false).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(rerollThreshold)).Returns(true);

            SetupSelectRandomArmor(["specific armor", "specific metal armor"], "my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void GenerateArmorFrom_GenerateMagicalArmor_YesDruidNoSpecific0RerollNoDragonhide_ByRoll_ReturnNonmetalArmor(bool specialMaterial, bool dragonhide)
        {
            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            SetupSelectRandomArmor(["my armor", "other armor"], "my random armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random armor", race.Size)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [Test]
        public void GenerateArmorFrom_GenerateMagicalArmor_YesDruidNoSpecific0RerollYesDragonhide_ReturnsDragonhideArmor()
        {
            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            SetupSelectRandomArmor(["my armor", "other armor", "metal armor"], "metal armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "metal armor", race.Size, TraitConstants.SpecialMaterials.Dragonhide)).Returns(magicalArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(magicalArmor));
        }

        [Test]
        public void GenerateArmorFrom_GenerateMagicalArmor_YesDruidNoSpecific0RerollYesDragonhide_ReturnsNonmetalArmor()
        {
            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            SetupSelectRandomArmor(["my armor", "other armor", "metal armor"], "other armor");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "other armor", race.Size)).Returns(magicalArmor);

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
        public void GenerateArmorFrom_GenerateMagicalArmor_YesDruidYesSpecific0RerollNoDragonhide_OnlyGetNonMetalSpecific(
            string power,
            double specificThreshold,
            bool specialMaterial,
            bool dragonhide)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            additionalFeats.Clear();
            additionalFeats.Add(new Feat { Name = FeatConstants.HeavyArmorProficiency });
            classFeats.Add(new Feat { Name = FeatConstants.MediumArmorProficiency });
            classFeats.Add(new Feat { Name = FeatConstants.LightArmorProficiency });

            proficientArmors[FeatConstants.HeavyArmorProficiency].Clear();
            proficientArmors[FeatConstants.HeavyArmorProficiency].AddRange(
                ["heavy metal armor", "other heavy metal armor", "specific heavy metal armor", "other specific heavy metal armor"]);
            proficientArmors[FeatConstants.MediumArmorProficiency].Clear();
            proficientArmors[FeatConstants.MediumArmorProficiency].AddRange(
                ["medium metal armor", "other medium metal armor", "specific medium metal armor", "other specific medium metal armor",
                    "medium armor", "other medium armor", "specific medium armor", "other specific medium armor"]);
            proficientArmors[FeatConstants.LightArmorProficiency].Clear();
            proficientArmors[FeatConstants.LightArmorProficiency].AddRange(
                ["light metal armor", "other light metal armor", "specific light metal armor", "other specific light metal armor",
                    "light armor", "other light armor"]);

            //Class
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(
                    RandomWeightedCollection<string>.EquivalentSet(),
                    RandomWeightedCollection<string>.EquivalentSet(FeatConstants.MediumArmorProficiency),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    null))
                .Returns(FeatConstants.MediumArmorProficiency);

            //Armors
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(
                    RandomWeightedCollection<string>.EquivalentSet(),
                    RandomWeightedCollection<string>.EquivalentSet("specific medium armor", "other specific medium armor"),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    null))
                .Returns("other specific medium armor");

            var specificArmor = CreateArmor("other specific medium armor");
            mockMagicalArmorGenerator
                .Setup(g => g.Generate(power, "other specific medium armor", race.Size))
                .Returns(specificArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(specificArmor));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateArmorFrom_GenerateMagicalArmor_YesDruidYesSpecific0RerollYesDragonhide_OnlyGetNonMetalSpecific(string power, double specificThreshold)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            additionalFeats.Clear();
            additionalFeats.Add(new Feat { Name = FeatConstants.HeavyArmorProficiency });
            classFeats.Add(new Feat { Name = FeatConstants.MediumArmorProficiency });
            classFeats.Add(new Feat { Name = FeatConstants.LightArmorProficiency });

            proficientArmors[FeatConstants.HeavyArmorProficiency].Clear();
            proficientArmors[FeatConstants.HeavyArmorProficiency].AddRange(
                ["heavy metal armor", "other heavy metal armor", "specific heavy metal armor", "other specific heavy metal armor"]);
            proficientArmors[FeatConstants.MediumArmorProficiency].Clear();
            proficientArmors[FeatConstants.MediumArmorProficiency].AddRange(
                ["medium metal armor", "other medium metal armor", "specific medium metal armor", "other specific medium metal armor",
                    "medium armor", "other medium armor", "specific medium armor", "other specific medium armor"]);
            proficientArmors[FeatConstants.LightArmorProficiency].Clear();
            proficientArmors[FeatConstants.LightArmorProficiency].AddRange(
                ["light metal armor", "other light metal armor", "specific light metal armor", "other specific light metal armor",
                    "light armor", "other light armor"]);

            //Class
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(
                    RandomWeightedCollection<string>.EquivalentSet(),
                    RandomWeightedCollection<string>.EquivalentSet(FeatConstants.MediumArmorProficiency),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    null))
                .Returns(FeatConstants.MediumArmorProficiency);

            //Armors
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(
                    RandomWeightedCollection<string>.EquivalentSet(),
                    RandomWeightedCollection<string>.EquivalentSet("specific medium armor", "other specific medium armor"),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    null))
                .Returns("other specific medium armor");

            var specificArmor = CreateArmor("other specific medium armor");
            mockMagicalArmorGenerator
                .Setup(g => g.Generate(power, "other specific medium armor", race.Size))
                .Returns(specificArmor);

            var armor = armorGenerator.GenerateArmorFrom(feats, characterClass, race);
            Assert.That(armor, Is.EqualTo(specificArmor));
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
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            var mundaneShield = CreateShield("mundane shield", FeatConstants.ShieldProficiency);

            SetupSelectRandomArmor(["my shield", "other shield", "metal shield", "mundane shield"], "my random shield");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random shield", race.Size)).Returns(mundaneShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(mundaneShield));
        }

        [Test]
        public void GenerateShieldFrom_GenerateMundaneShield_UseCumulativeProficiencies()
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            additionalFeats.Add(new Feat { Name = FeatConstants.TowerShieldProficiency });
            proficientShields[FeatConstants.TowerShieldProficiency].AddRange(RandomShieldPermutation.TowerShields);

            var heavyShield = CreateShield("heavy shield");

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(
                    RandomWeightedCollection<string>.EquivalentSet(FeatConstants.TowerShieldProficiency),
                    RandomWeightedCollection<string>.EquivalentSet(FeatConstants.ShieldProficiency),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    null))
                .Returns(FeatConstants.TowerShieldProficiency);
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(
                    RandomWeightedCollection<string>.EquivalentSet(RandomShieldPermutation.TowerShields),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    null))
                .Returns("my random shield");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random shield", race.Size)).Returns(heavyShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(heavyShield));
        }

        [TestCaseSource(nameof(RandomShieldData))]
        public void GenerateShieldFrom_GenerateMundaneShield_FromRandom(RandomShieldPermutation permutation)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            proficientShields[FeatConstants.TowerShieldProficiency].AddRange(RandomShieldPermutation.TowerShields);
            proficientShields[FeatConstants.ShieldProficiency].Clear();
            proficientShields[FeatConstants.ShieldProficiency].AddRange(RandomShieldPermutation.NormalShields);

            var expectedShield = CreateShield(permutation.Shields.Expected);

            permutation.Racial.SetupMock(mockCollectionsSelector);
            permutation.Class.SetupMock(mockCollectionsSelector);
            permutation.Additional.SetupMock(mockCollectionsSelector);
            permutation.Shields.SetupMock(mockCollectionsSelector);

            mockMundaneArmorGenerator.Setup(g => g.Generate(permutation.Shields.Expected, race.Size)).Returns(expectedShield);

            var shield = armorGenerator.GenerateShieldFrom(permutation.FeatCollection, characterClass, race);

            permutation.Racial.VerifyMock(mockCollectionsSelector);
            permutation.Class.VerifyMock(mockCollectionsSelector);
            permutation.Additional.VerifyMock(mockCollectionsSelector);
            permutation.Shields.VerifyMock(mockCollectionsSelector);

            mockMundaneArmorGenerator.Verify(g => g.Generate(permutation.Shields.Expected, race.Size), Times.Once);

            Assert.That(shield, Is.EqualTo(expectedShield));
        }

        private static IEnumerable RandomShieldData
        {
            get
            {
                var permutations = new[]
                {
                    new RandomShieldPermutation("R-S:S"),
                    new RandomShieldPermutation("R-S:S;C-T:T"),
                    new RandomShieldPermutation("R-S:S;A-T:T"),
                    new RandomShieldPermutation("R-ST:S"),
                    new RandomShieldPermutation("R-ST:T"),

                    new RandomShieldPermutation("C-S:S"),
                    new RandomShieldPermutation("C-S:S;A-T:T"),
                    new RandomShieldPermutation("C-ST:S"),
                    new RandomShieldPermutation("C-ST:T"),

                    new RandomShieldPermutation("A-S:S"),
                    new RandomShieldPermutation("A-ST:S"),
                    new RandomShieldPermutation("A-ST:T"),
                };

                foreach (var permutation in permutations)
                {
                    yield return new TestCaseData(permutation).SetArgDisplayNames(permutation.Key);
                }
            }
        }

        public class RandomShieldPermutation
        {
            public string Key { get; init; }
            public FeatCollections FeatCollection { get; set; }
            public RandomWeightedCollection<string> Racial { get; set; }
            public RandomWeightedCollection<string> Class { get; set; }
            public RandomWeightedCollection<string> Additional { get; set; }
            public RandomWeightedCollection<string> Shields { get; set; }

            public static readonly string[] NormalShields = ["normal shield", "other normal shield"];
            public static readonly string[] TowerShields = ["tower shield", "other tower shield"];

            public RandomShieldPermutation(string key)
            {
                Key = key;
                FeatCollection = new();
                Racial = new() { VeryRare = null };
                Class = new() { VeryRare = null };
                Additional = new() { VeryRare = null };
                Shields = new() { VeryRare = null, Expected = "my random shield" };

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
                        default: throw new ArgumentException($"Unknown feat source '{feats}'");
                    }
                }

                Shields.Common = GetShields(Additional.Expected);
                Shields.Uncommon = GetShields(Class.Expected);
                Shields.Rare = GetShields(Racial.Expected);
            }

            private static string[] GetShields(string feat) => feat switch
            {
                FeatConstants.ShieldProficiency => NormalShields,
                FeatConstants.TowerShieldProficiency => TowerShields,
                _ => [],
            };

            private RandomWeightedCollection<string> SetWeights(string proficiencies, string expected)
            {
                var feats = proficiencies.Select(GetFeat);

                return new()
                {
                    Common = [.. feats.Intersect([FeatConstants.TowerShieldProficiency])],
                    Uncommon = [.. feats.Intersect([FeatConstants.ShieldProficiency])],
                    Rare = [],
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

            private string GetFeat(char p) => p switch
            {
                'S' => FeatConstants.ShieldProficiency,
                'T' => FeatConstants.TowerShieldProficiency,
                _ => throw new ArgumentException($"Unknown feat character '{p}'"),
            };
        }

        [Test]
        public void GenerateShieldFrom_GenerateMundaneShield_NoDruidYesSpecificYesRerollNoDragonhide_CannotBeSpecific()
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            var mundaneShield = CreateShield("mundane shield", FeatConstants.ShieldProficiency);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(It.Is<double>(t => t <= 1))).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(It.Is<int>(t => t <= 100))).Returns(true);

            SetupSelectRandomArmor(["my shield", "other shield", "metal shield", "mundane shield"], "my random shield");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random shield", race.Size)).Returns(mundaneShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(mundaneShield));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void GenerateShieldFrom_GenerateMundaneShield_YesDruidNoSpecific0RerollNoDragonhide_ByRoll(bool specialMaterial, bool dragonhide)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            var mundaneShield = CreateShield("mundane shield", FeatConstants.ShieldProficiency);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            SetupSelectRandomArmor(["my shield", "other shield", "mundane shield"], "my random shield");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random shield", race.Size)).Returns(mundaneShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(mundaneShield));
        }

        [Test]
        public void GenerateShieldFrom_GenerateMundaneShield_YesDruidNoSpecific0RerollYesDragonhide_ReturnsDragonhideShield()
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            var mundaneShield = CreateShield("metal shield", FeatConstants.ShieldProficiency);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            SetupSelectRandomArmor(["my shield", "other shield", "metal shield"], "metal shield");
            mockMundaneArmorGenerator.Setup(g => g.Generate("metal shield", race.Size, TraitConstants.SpecialMaterials.Dragonhide)).Returns(mundaneShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(mundaneShield));
        }

        [Test]
        public void GenerateShieldFrom_GenerateMundaneShield_YesDruidNoSpecific0RerollYesDragonhide_ReturnsNonmetalShield()
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            var mundaneShield = CreateShield("mundane shield", FeatConstants.ShieldProficiency);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            SetupSelectRandomArmor(["my shield", "other shield", "metal shield", "mundane shield"], "my random shield");
            mockMundaneArmorGenerator.Setup(g => g.Generate("my random shield", race.Size)).Returns(mundaneShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(mundaneShield));
        }

        [Test]
        public void GenerateShieldFrom_GenerateMagicalShield()
        {
            SetupSelectRandomArmor(["my shield", "other shield", "metal shield"], "my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [Test]
        public void GenerateShieldFrom_GenerateMagicalShield_UseCumulativeProficiencies()
        {
            additionalFeats.Add(new Feat { Name = FeatConstants.TowerShieldProficiency });
            proficientShields[FeatConstants.TowerShieldProficiency].AddRange(RandomShieldPermutation.TowerShields);

            var heavyShield = CreateShield("heavy shield");

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(
                    RandomWeightedCollection<string>.EquivalentSet(FeatConstants.TowerShieldProficiency),
                    RandomWeightedCollection<string>.EquivalentSet(FeatConstants.ShieldProficiency),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    null))
                .Returns(FeatConstants.TowerShieldProficiency);
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(
                    RandomWeightedCollection<string>.EquivalentSet(RandomShieldPermutation.TowerShields),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    null))
                .Returns("my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(heavyShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(heavyShield));
        }

        [TestCaseSource(nameof(RandomShieldData))]
        public void GenerateShieldFrom_GenerateMagicalShield_FromRandom(RandomShieldPermutation permutation)
        {
            proficientShields[FeatConstants.TowerShieldProficiency].AddRange(RandomShieldPermutation.TowerShields);
            proficientShields[FeatConstants.ShieldProficiency].Clear();
            proficientShields[FeatConstants.ShieldProficiency].AddRange(RandomShieldPermutation.NormalShields);

            var expectedShield = CreateShield(permutation.Shields.Expected);

            permutation.Racial.SetupMock(mockCollectionsSelector);
            permutation.Class.SetupMock(mockCollectionsSelector);
            permutation.Additional.SetupMock(mockCollectionsSelector);
            permutation.Shields.SetupMock(mockCollectionsSelector);

            mockMagicalArmorGenerator.Setup(g => g.Generate(power, permutation.Shields.Expected, race.Size)).Returns(expectedShield);

            var shield = armorGenerator.GenerateShieldFrom(permutation.FeatCollection, characterClass, race);

            permutation.Racial.VerifyMock(mockCollectionsSelector);
            permutation.Class.VerifyMock(mockCollectionsSelector);
            permutation.Additional.VerifyMock(mockCollectionsSelector);
            permutation.Shields.VerifyMock(mockCollectionsSelector);

            mockMagicalArmorGenerator.Verify(g => g.Generate(power, permutation.Shields.Expected, race.Size), Times.Once);

            Assert.That(shield, Is.EqualTo(expectedShield));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateShieldFrom_GenerateMagicalShield_NoDruidYesSpecific0RerollNoDragonhide(string power, double specificThreshold)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            SetupSelectRandomArmor(["specific shield", "specific metal shield"], "my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateShieldFrom_GenerateMagicalShield_NoDruidYesSpecific0RerollNoDragonhide_OnlySpecific(string power, double specificThreshold)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

            // INFO: There are no specific tower shields, so test that it doesn't show up as an option when shield should be specific
            additionalFeats.Add(new Feat { Name = FeatConstants.TowerShieldProficiency });
            proficientShields[FeatConstants.TowerShieldProficiency].AddRange(RandomShieldPermutation.TowerShields);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(
                    RandomWeightedCollection<string>.EquivalentSet(),
                    RandomWeightedCollection<string>.EquivalentSet(FeatConstants.ShieldProficiency),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    null))
                .Returns(FeatConstants.ShieldProficiency);

            SetupSelectRandomArmor(["specific shield", "specific metal shield"], "my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(PowerConstants.Minor, 0.98, 92)]
        [TestCase(PowerConstants.Medium, 0.97, 64)]
        [TestCase(PowerConstants.Major, 0.97, 64)]
        public void GenerateShieldFrom_GenerateMagicalShield_NoDruidYesSpecific1RerollNoDragonhide(string power, double specificThreshold, int rollThreshold)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(false).Returns(true);
            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(rollThreshold)).Returns(true).Returns(false);

            SetupSelectRandomArmor(["specific shield", "specific metal shield"], "my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(PowerConstants.Minor, 0.98, 92)]
        [TestCase(PowerConstants.Medium, 0.97, 64)]
        [TestCase(PowerConstants.Major, 0.97, 64)]
        public void GenerateShieldFrom_GenerateMagicalShield_NoDruidYesSpecific2RerollNoDragonhide(string power, double specificThreshold, int rollThreshold)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(false).Returns(false).Returns(true);
            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(rollThreshold)).Returns(true).Returns(true).Returns(false);

            SetupSelectRandomArmor(["specific shield", "specific metal shield"], "my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void GenerateShieldFrom_GenerateMagicalShield_YesDruidNoSpecific0RerollNoDragonhide_ByRoll(bool specialMaterial, bool dragonhide)
        {
            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            SetupSelectRandomArmor(["my shield", "other shield"], "my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [Test]
        public void GenerateShieldFrom_GenerateMagicalShield_YesDruidNoSpecific0RerollYesDragonhide_ReturnsDragonhideShield()
        {
            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            SetupSelectRandomArmor(["my shield", "other shield", "metal shield"], "metal shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "metal shield", race.Size, TraitConstants.SpecialMaterials.Dragonhide)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [Test]
        public void GenerateShieldFrom_GenerateMagicalShield_YesDruidNoSpecific0RerollYesDragonhide_ReturnsNonmetalShield()
        {
            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            SetupSelectRandomArmor(["my shield", "other shield", "metal shield"], "other shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "other shield", race.Size)).Returns(magicalShield);

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
        public void GenerateShieldFrom_GenerateMagicalShield_YesDruidYesSpecific0RerollNoDragonhide_ByRoll(
            string power,
            double specificThreshold,
            bool specialMaterial,
            bool dragonhide)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            SetupSelectRandomArmor(["specific shield"], "my random shield");
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
        public void GenerateShieldFrom_GenerateMagicalShield_YesDruidYesSpecific1RerollNoDragonhide_ByRoll(
            string power,
            double specificThreshold,
            int rollThreshold,
            bool specialMaterial,
            bool dragonhide)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(false).Returns(true);
            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(rollThreshold)).Returns(true).Returns(false);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            SetupSelectRandomArmor(["specific shield"], "my random shield");
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
        public void GenerateShieldFrom_GenerateMagicalShield_YesDruidYesSpecific2RerollNoDragonhide_ByRoll(
            string power,
            double specificThreshold,
            int rollThreshold,
            bool specialMaterial,
            bool dragonhide)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(false).Returns(false).Returns(true);
            mockDice.SetupSequence(d => d.Roll(1).d(100).AsTrueOrFalse(rollThreshold)).Returns(true).Returns(true).Returns(false);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(specialMaterial);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(dragonhide);

            SetupSelectRandomArmor(["specific shield"], "my random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "my random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        [TestCase(PowerConstants.Minor, 0.98)]
        [TestCase(PowerConstants.Medium, 0.97)]
        [TestCase(PowerConstants.Major, 0.97)]
        public void GenerateShieldFrom_GenerateMagicalShield_YesDruidYesSpecific0RerollYesDragonhide_ReturnsNonmetalSpecific(string power, double specificThreshold)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(specificThreshold)).Returns(true);

            characterClass.Name = CharacterClassConstants.Druid;

            mockDice.Setup(d => d.Roll(1).d(100).AsTrueOrFalse(.95)).Returns(true);
            mockDice.Setup(d => d.Roll(1).d(3).AsTrueOrFalse(3)).Returns(true);

            //Can't apply Dragonhide to specific armor
            SetupSelectRandomArmor(["specific shield"], "specific random shield");
            mockMagicalArmorGenerator.Setup(g => g.Generate(power, "specific random shield", race.Size)).Returns(magicalShield);

            var shield = armorGenerator.GenerateShieldFrom(feats, characterClass, race);
            Assert.That(shield, Is.EqualTo(magicalShield));
        }

        private Armor CreateShield(string name, string feat = null)
        {
            var shield = new Armor
            {
                Name = name,
                ItemType = ItemTypeConstants.Armor,
                Attributes = [AttributeConstants.Shield],
                Size = race.Size
            };

            if (feat != null)
                proficientShields[feat].Add(name);

            return shield;
        }
    }
}
