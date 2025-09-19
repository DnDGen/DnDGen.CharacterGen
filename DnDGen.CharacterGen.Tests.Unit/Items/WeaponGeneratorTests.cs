using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items;
using DnDGen.CharacterGen.Items.Selectors;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Generators;
using DnDGen.Infrastructure.Selectors.Collections;
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
    public class WeaponGeneratorTests
    {
        private IWeaponGenerator weaponGenerator;
        private Mock<ITreasureLevelSelector> mockTreasureLevelSelector;
        private Mock<MundaneItemGenerator> mockMundaneWeaponGenerator;
        private Mock<MagicalItemGenerator> mockMagicalWeaponGenerator;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<JustInTimeFactory> mockJustInTimeFactory;
        private Weapon magicalWeapon;
        private FeatCollections feats;
        private List<Feat> additionalFeats;
        private List<Feat> classFeats;
        private List<Feat> racialFeats;
        private List<string> proficiencyFeats;
        private CharacterClass characterClass;
        private List<string> allWeapons;
        private List<string> allAmmunitions;
        private List<string> allMeleeWeapons;
        private List<string> allRangedWeapons;
        private List<string> allTwoHandedWeapons;
        private List<string> allProficientWeapons;
        private Race race;
        private string power;

        [SetUp]
        public void Setup()
        {
            mockTreasureLevelSelector = new Mock<ITreasureLevelSelector>();
            mockMundaneWeaponGenerator = new Mock<MundaneItemGenerator>();
            mockMagicalWeaponGenerator = new Mock<MagicalItemGenerator>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockJustInTimeFactory = new Mock<JustInTimeFactory>();

            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon)).Returns(mockMundaneWeaponGenerator.Object);
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>(ItemTypeConstants.Weapon)).Returns(mockMagicalWeaponGenerator.Object);

            weaponGenerator = new WeaponGenerator(mockCollectionsSelector.Object, mockJustInTimeFactory.Object, mockTreasureLevelSelector.Object);

            magicalWeapon = new Weapon();
            additionalFeats = [];
            classFeats = [];
            racialFeats = [];
            feats = new FeatCollections { Additional = additionalFeats, Class = classFeats, Racial = racialFeats };
            characterClass = new CharacterClass();
            proficiencyFeats =
            [
                FeatConstants.SimpleWeaponProficiency,
                FeatConstants.MartialWeaponProficiency,
                FeatConstants.ExoticWeaponProficiency,
            ];
            allWeapons = [];
            allAmmunitions = [];
            allMeleeWeapons = [];
            allRangedWeapons = [];
            allTwoHandedWeapons = [];
            allProficientWeapons = [];
            race = new Race
            {
                Size = "size",
                BaseRace = "base race"
            };
            magicalWeapon = CreateOneHandedMeleeWeapon("magical weapon");
            magicalWeapon.IsMagical = true;
            characterClass.Name = "class name";
            characterClass.Level = 9266;
            additionalFeats.Add(new Feat { Name = FeatConstants.SimpleWeaponProficiency, Foci = [FeatConstants.Foci.All] });

            allWeapons.Add("other weapon");
            allWeapons.Add("other melee");
            allWeapons.Add("other ranged");
            allWeapons.Add("other two-handed");
            allWeapons.Add("other ammo");
            allAmmunitions.Add("other ammo");
            allMeleeWeapons.Add("other weapon");
            allMeleeWeapons.Add("other melee");
            allMeleeWeapons.Add("other two-handed");
            allRangedWeapons.Add("other ranged");
            allRangedWeapons.Add("other ammo");
            allTwoHandedWeapons.Add("other two-handed");
            allProficientWeapons.Add("other weapon");
            allProficientWeapons.Add("other melee");
            allProficientWeapons.Add("other ranged");
            allProficientWeapons.Add("other two-handed");
            allProficientWeapons.Add("other ammo");

            power = "power";
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(power);

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
                .Returns(GetWeightedRandom<string>);
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(
                    It.IsAny<IEnumerable<Feat>>(),
                    It.IsAny<IEnumerable<Feat>>(),
                    It.IsAny<IEnumerable<Feat>>(),
                    It.IsAny<IEnumerable<Feat>>()))
                .Returns(GetWeightedRandom<Feat>);

            T GetWeightedRandom<T>(IEnumerable<T> common, IEnumerable<T> uncommon, IEnumerable<T> rare, IEnumerable<T> veryRare)
            {
                common ??= [];
                uncommon ??= [];
                rare ??= [];
                veryRare ??= [];

                var all = common.Concat(uncommon).Concat(rare).Concat(veryRare);
                var total = all.Count();

                return all.ElementAt(index++ % total);
            }

            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, ItemTypeConstants.Weapon + GroupConstants.Proficiency))
                .Returns(proficiencyFeats);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatFoci, additionalFeats[0].Name))
                .Returns(allProficientWeapons);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, ItemTypeConstants.Weapon))
                .Returns(allWeapons);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Ammunition))
                .Returns(allAmmunitions);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Melee))
                .Returns(allMeleeWeapons);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Ranged))
                .Returns(allRangedWeapons);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.TwoHanded))
                .Returns(allTwoHandedWeapons);
        }

        private void SetupSelectRandomWeapon(string[] weapons, string expected)
        {
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(
                    RandomWeightedCollection<string>.EquivalentSet(weapons),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    RandomWeightedCollection<string>.EquivalentSet(),
                    null))
                .Returns(expected);
        }

        [Test]
        public void GenerateFrom_GenerateNoWeapon()
        {
            additionalFeats.Clear();
            additionalFeats.Add(new Feat { Name = "feat 1" });
            additionalFeats.Add(new Feat { Name = "feat 2", Foci = [FeatConstants.Foci.UnarmedStrike] });

            proficiencyFeats.Clear();
            proficiencyFeats.Add(additionalFeats[0].Name);
            proficiencyFeats.Add(additionalFeats[1].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.Null);
        }

        [Test]
        public void GenerateFrom_GenerateMundaneWeapon()
        {
            var mundaneWeapon = CreateOneHandedMeleeWeapon("mundane weapon");
            var wrongMundaneWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            allProficientWeapons.Remove(magicalWeapon.Name);

            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);
            SetupSelectRandomWeapon([.. allProficientWeapons.Except(allAmmunitions)], "my random weapon");
            mockMundaneWeaponGenerator.Setup(g => g.Generate("my random weapon", race.Size)).Returns(mundaneWeapon);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(mundaneWeapon));
        }

        private Weapon CreateWeapon(string name)
        {
            var weapon = new Weapon
            {
                Name = name,
                ItemType = ItemTypeConstants.Weapon,
                Size = race.Size
            };

            allWeapons.Add(name);
            allProficientWeapons.Add(name);

            return weapon;
        }

        private Weapon CreateOneHandedMeleeWeapon(string name)
        {
            var weapon = CreateWeapon(name);
            weapon.Attributes = [AttributeConstants.Melee];

            allMeleeWeapons.Add(name);

            return weapon;
        }

        private Weapon CreateTwoHandedMeleeWeapon(string name)
        {
            var weapon = CreateWeapon(name);
            weapon.Attributes = [AttributeConstants.Melee, AttributeConstants.TwoHanded];

            allMeleeWeapons.Add(name);
            allTwoHandedWeapons.Add(name);

            return weapon;
        }

        private Weapon CreateRangedWeapon(string name)
        {
            var weapon = CreateWeapon(name);
            weapon.Attributes = [AttributeConstants.Ranged];

            allRangedWeapons.Add(name);

            return weapon;
        }

        private Weapon CreateAmmunition(string name)
        {
            var weapon = CreateWeapon(name);
            weapon.Attributes = [AttributeConstants.Ranged, AttributeConstants.Ammunition];

            allRangedWeapons.Add(name);
            allAmmunitions.Add(name);

            return weapon;
        }

        [TestCaseSource(nameof(RandomWeaponData))]
        public void GenerateFrom_GenerateMundaneWeapon_FromRandom(RandomWeaponPermutation permutation)
        {
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            allWeapons.AddRange(RandomWeaponPermutation.AllWeapons);

            var expectedWeapon = CreateWeapon(permutation.Weapons.Expected);

            permutation.Racial.SetupMock(mockCollectionsSelector);
            permutation.Class.SetupMock(mockCollectionsSelector);
            permutation.Additional.SetupMock(mockCollectionsSelector);
            permutation.Weapons.SetupMock(mockCollectionsSelector);

            mockMundaneWeaponGenerator.Setup(g => g.Generate(permutation.Weapons.Expected, race.Size)).Returns(expectedWeapon);

            var weapon = weaponGenerator.GenerateFrom(permutation.FeatCollection, characterClass, race);

            permutation.Racial.VerifyMock(mockCollectionsSelector);
            permutation.Class.VerifyMock(mockCollectionsSelector);
            permutation.Additional.VerifyMock(mockCollectionsSelector);
            permutation.Weapons.VerifyMock(mockCollectionsSelector);

            mockMundaneWeaponGenerator.Verify(g => g.Generate(permutation.Weapons.Expected, race.Size), Times.Once);

            Assert.That(weapon, Is.EqualTo(expectedWeapon));
        }

        private static IEnumerable RandomWeaponData
        {
            get
            {
                var sources = new[] { "R", "C", "A" };
                var featPerms = new[]
                {
                    "S:S",
                    "SM:S",
                    "SM:M",
                    "SE:S",
                    "SE:E",
                    "SN:S",
                    "SN:N",
                    "SME:S",
                    "SME:M",
                    "SME:E",
                    "SMN:S",
                    "SMN:M",
                    "SMN:N",
                    "SEN:S",
                    "SEN:E",
                    "SEN:N",
                    "SMEN:S",
                    "SMEN:M",
                    "SMEN:E",
                    "SMEN:N",

                    "M:M",
                    "ME:M",
                    "ME:E",
                    "MN:M",
                    "MN:N",
                    "MEN:M",
                    "MEN:E",
                    "MEN:N",

                    "E:E",
                    "EN:E",
                    "EN:N",

                    "N:N",
                };

                foreach (var featPerm in featPerms)
                {
                    foreach (var source in sources)
                    {
                        var key = $"{source}-{featPerm}";
                        yield return new TestCaseData(new RandomWeaponPermutation(key)).SetArgDisplayNames(key);

                        var otherSource1 = sources.Except([source]).First();
                        var otherSource2 = sources.Except([source]).Last();
                        foreach (var featPerm2 in featPerms)
                        {
                            var key2 = $"{otherSource1}-{featPerm};{otherSource2}-{featPerm2}";
                            yield return new TestCaseData(new RandomWeaponPermutation(key2)).SetArgDisplayNames(key2);
                        }
                    }

                    foreach (var featPerm2 in featPerms)
                    {
                        foreach (var featPerm3 in featPerms)
                        {
                            var key3 = $"{sources[0]}-{featPerm};{sources[1]}-{featPerm2};{sources[2]}-{featPerm3}";
                            yield return new TestCaseData(new RandomWeaponPermutation(key3)).SetArgDisplayNames(key3);
                        }
                    }
                }
            }
        }

        public class RandomWeaponPermutation
        {
            public string Key { get; init; }
            public FeatCollections FeatCollection { get; set; }
            public RandomWeightedCollection<Feat> Racial { get; set; }
            public RandomWeightedCollection<Feat> Class { get; set; }
            public RandomWeightedCollection<Feat> Additional { get; set; }
            public RandomWeightedCollection<string> Weapons { get; set; }

            private static readonly string[] BaseWeapons = [
                "Racial weapon", "other Racial weapon",
                "Class weapon", "other Class weapon",
                "Additional weapon", "other Additional weapon",
            ];
            public static readonly string[] SpecialistWeapons = [.. BaseWeapons.Select(w => $"Specialist {w}")];
            public static readonly string[] SimpleWeapons = [.. BaseWeapons.Select(w => $"Simple {w}")];
            public static readonly string[] MartialWeapons = [.. BaseWeapons.Select(w => $"Martial {w}")];
            public static readonly string[] ExoticWeapons = [.. BaseWeapons.Select(w => $"Exotic {w}")];
            public static readonly string[] AllWeapons = [.. SpecialistWeapons, .. SimpleWeapons, .. MartialWeapons, .. ExoticWeapons];

            private const string SpecialistFeatName = "Non-Proficiency Weapon Feat";

            public RandomWeaponPermutation(string key)
            {
                Key = key;
                FeatCollection = new();
                Racial = new();
                Class = new();
                Additional = new();
                Weapons = new() { VeryRare = null, Expected = "my random weapon" };

                ParseKey();
            }

            public override string ToString() => Key;

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
                            FeatCollection.Racial = SetProficiencies(proficiencies, "Racial");
                            Racial = SetWeights(FeatCollection.Racial, expected);
                            break;
                        case "C":
                            FeatCollection.Class = SetProficiencies(proficiencies, "Class");
                            Class = SetWeights(FeatCollection.Class, expected);
                            break;
                        case "A":
                            FeatCollection.Additional = SetProficiencies(proficiencies, "Additional");
                            Additional = SetWeights(FeatCollection.Additional, expected);
                            break;
                        default: throw new ArgumentException($"Unknown feat source '{feats}'");
                    }
                }

                Weapons.Common = Additional.Expected?.Foci.Intersect(AllWeapons).ToArray() ?? [];
                Weapons.Uncommon = Class.Expected?.Foci.Intersect(AllWeapons).ToArray() ?? [];
                Weapons.Rare = Racial.Expected?.Foci.Intersect(AllWeapons).ToArray() ?? [];
            }

            private RandomWeightedCollection<Feat> SetWeights(IEnumerable<Feat> feats, string expected) => new()
            {
                Common = [.. feats.Where(f => f.Name == SpecialistFeatName)],
                Uncommon = [.. feats.Where(f => f.Name == FeatConstants.MartialWeaponProficiency)],
                Rare = [.. feats.Where(f => f.Name == FeatConstants.SimpleWeaponProficiency)],
                VeryRare = [.. feats.Where(f => f.Name == FeatConstants.ExoticWeaponProficiency)],
                Expected = feats.First(f => f.Name == GetFeatName(expected[0]))
            };

            private IEnumerable<Feat> SetProficiencies(string proficiencies, string filter)
            {
                var feats = new List<Feat> { new() { Name = "other feat" } };
                feats.AddRange(proficiencies.Select(p => BuildFeat(p, filter)));
                feats.Add(new() { Name = "another feat" });

                return feats;
            }

            private static string[] GetFoci(char? p, string filter) => p switch
            {
                'S' => GetFilteredWeapons(SimpleWeapons, filter),
                'M' => GetFilteredWeapons(MartialWeapons, filter),
                'E' => GetFilteredWeapons(ExoticWeapons, filter),
                'N' => GetFilteredWeapons(SpecialistWeapons, filter),
                _ => [],
            };

            private static string[] GetFilteredWeapons(string[] weapons, string filter) => [.. weapons.Where(w => w.Contains(filter))];

            private static string GetFeatName(char p) => p switch
            {
                'S' => FeatConstants.SimpleWeaponProficiency,
                'M' => FeatConstants.MartialWeaponProficiency,
                'E' => FeatConstants.ExoticWeaponProficiency,
                'N' => SpecialistFeatName,
                _ => throw new ArgumentException($"Unknown feat character '{p}'"),
            };

            private static Feat BuildFeat(char p, string filter) => new() { Name = GetFeatName(p), Foci = GetFoci(p, filter).Concat(["not a weapon"]) };
        }

        [Test]
        public void GenerateFrom_CanWieldSpecificMundaneWeaponProficiency()
        {
            var mundaneWeapon = CreateOneHandedMeleeWeapon("mundane weapon");
            var wrongMundaneWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            var specialties = new[] { mundaneWeapon.Name };
            SetupSelectRandomWeapon(specialties, "my random weapon");
            mockMundaneWeaponGenerator.Setup(g => g.Generate("my random weapon", race.Size)).Returns(mundaneWeapon);

            additionalFeats.Add(new Feat { Name = "feat2", Foci = specialties });
            proficiencyFeats.Add(additionalFeats[1].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(mundaneWeapon));
        }

        [Test]
        public void GenerateFrom_PreferMundaneWeaponsPickedAsFocusForNonProficiencyFeats()
        {
            var mundaneWeapon = CreateOneHandedMeleeWeapon("mundane weapon");
            var wrongMundaneWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            var specialties = new[] { mundaneWeapon.Name };
            var wrongSpecialties = new[] { wrongMundaneWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(specialties))).Returns("my random weapon");
            mockMundaneWeaponGenerator.Setup(g => g.Generate("my random weapon", race.Size)).Returns(mundaneWeapon);

            additionalFeats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            additionalFeats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add(additionalFeats[1].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(mundaneWeapon));
        }

        [Test]
        public void GenerateFrom_DoNotPreferMundaneWeaponPickedAsFocusForWeaponFamiliarity()
        {
            var mundaneWeapon = CreateOneHandedMeleeWeapon("mundane weapon");
            var wrongMundaneWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            var specialties = new[] { mundaneWeapon.Name };
            var wrongSpecialties = new[] { wrongMundaneWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(specialties))).Returns("my random weapon");
            mockMundaneWeaponGenerator.Setup(g => g.Generate("my random weapon", race.Size)).Returns(mundaneWeapon);

            additionalFeats.Add(new Feat { Name = FeatConstants.WeaponFamiliarity, Foci = wrongSpecialties });
            additionalFeats.Add(new Feat { Name = "feat3", Foci = specialties });

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(mundaneWeapon), weapon.Name);
        }

        [Test]
        public void GenerateFrom_PreferAnyMundaneWeaponsPickedAsFocusForNonProficiencyFeats()
        {
            var mundaneWeapon = CreateOneHandedMeleeWeapon("mundane weapon");
            var otherMundaneWeapon = CreateOneHandedMeleeWeapon("other mundane weapon");
            var wrongMundaneWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            var specialties = new[] { mundaneWeapon.Name };
            var wrongSpecialties = new[] { wrongMundaneWeapon.Name };
            var multipleSpecialties = new[] { mundaneWeapon.Name, otherMundaneWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(multipleSpecialties))).Returns("my random weapon");
            mockMundaneWeaponGenerator.Setup(g => g.Generate("my random weapon", race.Size)).Returns(otherMundaneWeapon);

            additionalFeats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            additionalFeats.Add(new Feat { Name = "feat3", Foci = multipleSpecialties });
            additionalFeats.Add(new Feat { Name = "feat4", Foci = specialties });
            proficiencyFeats.Add(additionalFeats[1].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(otherMundaneWeapon), weapon.Name);
        }

        [Test]
        public void GenerateFrom_PreferMundaneWeaponsPickedAsFocusForProficiencyFeats()
        {
            var mundaneWeapon = CreateOneHandedMeleeWeapon("mundane weapon");
            var wrongMundaneWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            var specialties = new[] { mundaneWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(specialties))).Returns("my random weapon");
            mockMundaneWeaponGenerator.Setup(g => g.Generate("my random weapon", race.Size)).Returns(mundaneWeapon);

            additionalFeats.Add(new Feat { Name = "feat2", Foci = specialties });
            proficiencyFeats.Add(additionalFeats[1].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(mundaneWeapon));
        }

        [Test]
        public void GenerateFrom_PreferAnyMundaneWeaponsPickedAsFocusForProficiencyFeats()
        {
            var mundaneWeapon = CreateOneHandedMeleeWeapon("mundane weapon");
            var otherMundaneWeapon = CreateOneHandedMeleeWeapon("other mundane weapon");
            var wrongMundaneWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);

            var specialties = new[] { mundaneWeapon.Name };
            var multipleSpecialties = new[] { mundaneWeapon.Name, otherMundaneWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(multipleSpecialties))).Returns("my random weapon");
            mockMundaneWeaponGenerator.Setup(g => g.Generate("my random weapon", race.Size)).Returns(otherMundaneWeapon);

            additionalFeats.Add(new Feat { Name = "feat2", Foci = multipleSpecialties });
            additionalFeats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add(additionalFeats[1].Name);
            proficiencyFeats.Add(additionalFeats[2].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(otherMundaneWeapon));
        }

        [Test]
        public void GenerateFrom_NoPreferenceForMundaneWeapons()
        {
            var mundaneWeapon = CreateOneHandedMeleeWeapon("mundane weapon");
            var wrongMundaneWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            mockTreasureLevelSelector.Setup(s => s.SelectPowerFrom(characterClass, race)).Returns(PowerConstants.Mundane);
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(allProficientWeapons.Except(allAmmunitions).ToArray())))
                .Returns("my random weapon");
            mockMundaneWeaponGenerator.Setup(g => g.Generate("my random weapon", race.Size)).Returns(mundaneWeapon);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(mundaneWeapon));
        }

        [Test]
        public void GenerateFrom_GenerateMagicalWeapon()
        {
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(allProficientWeapons.Except(allAmmunitions).ToArray()))).Returns("my random weapon");

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon));
        }

        [Test]
        public void GenerateFrom_CanWieldSpecificMagicalWeaponProficiency()
        {
            var wrongMagicalWeapon = CreateOneHandedMeleeWeapon("wrong weapon");
            var specialties = new[] { magicalWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(specialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.SetupSequence(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon).Returns(wrongMagicalWeapon);

            additionalFeats.Add(new Feat { Name = "feat2", Foci = new[] { magicalWeapon.Name } });
            proficiencyFeats.Add(additionalFeats[1].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon));
        }

        [Test]
        public void GenerateFrom_PreferMagicalWeaponsPickedAsFocusForNonProficiencyFeats()
        {
            var wrongMagicalWeapon = CreateOneHandedMeleeWeapon("wrong weapon");

            var specialties = new[] { magicalWeapon.Name };
            var wrongSpecialties = new[] { wrongMagicalWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(specialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            additionalFeats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            additionalFeats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add(additionalFeats[1].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon));
        }

        [Test]
        public void GenerateFrom_DoNotPreferMagicalWeaponsPickedAsFocusForWeaponFamiliarity()
        {
            var wrongMagicalWeapon = CreateOneHandedMeleeWeapon("wrong weapon");

            var specialties = new[] { magicalWeapon.Name };
            var wrongSpecialties = new[] { wrongMagicalWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(specialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            additionalFeats.Add(new Feat { Name = FeatConstants.WeaponFamiliarity, Foci = wrongSpecialties });
            additionalFeats.Add(new Feat { Name = "feat3", Foci = specialties });

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateFrom_PreferAnyMagicalWeaponsPickedAsFocusForNonProficiencyFeats()
        {
            var otherMagicalWeapon = CreateOneHandedMeleeWeapon("other magical weapon");
            var wrongMagicalWeapon = CreateOneHandedMeleeWeapon("wrong weapon");

            var specialties = new[] { magicalWeapon.Name };
            var multipleSpecialties = new[] { magicalWeapon.Name, otherMagicalWeapon.Name };
            var wrongSpecialties = new[] { wrongMagicalWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(multipleSpecialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(otherMagicalWeapon);

            additionalFeats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            additionalFeats.Add(new Feat { Name = "feat3", Foci = multipleSpecialties });
            additionalFeats.Add(new Feat { Name = "feat4", Foci = specialties });
            proficiencyFeats.Add(additionalFeats[1].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(otherMagicalWeapon));
        }

        [Test]
        public void GenerateFrom_PreferMagicalWeaponsPickedAsFocusForProficiencyFeats()
        {
            var wrongMagicalWeapon = CreateOneHandedMeleeWeapon("wrong weapon");

            var specialties = new[] { magicalWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(specialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            additionalFeats.Add(new Feat { Name = "feat2", Foci = specialties });
            proficiencyFeats.Add(additionalFeats[1].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon));
        }

        [Test]
        public void GenerateFrom_PreferAnyMagicalWeaponsPickedAsFocusForProficiencyFeats()
        {
            var otherMagicalWeapon = CreateOneHandedMeleeWeapon("other magical weapon");
            var wrongMagicalWeapon = CreateOneHandedMeleeWeapon("wrong weapon");

            var specialties = new[] { magicalWeapon.Name };
            var multipleSpecialties = new[] { magicalWeapon.Name, otherMagicalWeapon.Name };
            var wrongSpecialties = new[] { wrongMagicalWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(multipleSpecialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(otherMagicalWeapon);

            additionalFeats.Add(new Feat { Name = "feat2", Foci = multipleSpecialties });
            additionalFeats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add(additionalFeats[1].Name);
            proficiencyFeats.Add(additionalFeats[2].Name);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(otherMagicalWeapon));
        }

        [Test]
        public void GenerateFrom_NoPreferenceForMagicalWeapons()
        {
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(allProficientWeapons.Except(allAmmunitions).ToArray()))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateFrom_SaveBonusesOfAllDoNotCountAsProficiencyFeats()
        {
            additionalFeats.Add(new Feat { Name = FeatConstants.SaveBonus, Foci = [FeatConstants.Foci.All] });
            additionalFeats[0].Foci = [magicalWeapon.Name];

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(magicalWeapon.Name))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        //INFO: Example here is Shurikens
        [Test]
        public void GenerateFrom_ThrownAmmunitionIsAllowed()
        {
            var shuriken = CreateRangedWeapon("thrown ammo");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(allProficientWeapons.Except(allAmmunitions).ToArray()))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(shuriken);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(shuriken), weapon.Name);
        }

        [Test]
        public void GenerateFrom_AmmunitionIsNotAllowed()
        {
            var ammo = CreateAmmunition("my ammo");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(allProficientWeapons.Except(allAmmunitions).ToArray()))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            var weapon = weaponGenerator.GenerateFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateAmmunition_ReturnsAmmunition()
        {
            var ammunition = CreateAmmunition("my ammo");

            var ammunitions = new[] { "ammo" };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(ammunitions))).Returns("my random ammo");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random ammo", race.Size)).Returns(ammunition);

            var generatedAmmunition = weaponGenerator.GenerateAmmunition(characterClass, race, "ammo");
            Assert.That(generatedAmmunition, Is.EqualTo(ammunition), generatedAmmunition.Name);
        }

        [Test]
        public void GenerateMeleeFrom_MeleeWeaponMustBeMelee()
        {
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(allProficientWeapons.Intersect(allMeleeWeapons).ToArray()))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            var weapon = weaponGenerator.GenerateMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateMeleeFrom_IfGenerationOfMeleeWeaponFails_TryWithoutNonProficiencyWeaponFoci()
        {
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            var otherMagicalWeapon = CreateTwoHandedMeleeWeapon("two-handed melee");

            var wrongSpecialties = new[] { rangedWeapon.Name };
            var specialties = new[] { otherMagicalWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(specialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(otherMagicalWeapon);

            additionalFeats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            additionalFeats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add("feat3");

            var weapon = weaponGenerator.GenerateMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(otherMagicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateMeleeFrom_IfGenerationOfMeleeWeaponFailsAgain_TryWithoutSpecificProficiencyWeaponFoci()
        {
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            var otherRangedWeapon = CreateRangedWeapon("other ranged weapon");

            var wrongSpecialties = new[] { rangedWeapon.Name };
            var specialties = new[] { otherRangedWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(allProficientWeapons.Intersect(allMeleeWeapons).ToArray()))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            additionalFeats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            additionalFeats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add("feat3");

            var weapon = weaponGenerator.GenerateMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateMeleeFrom_IfNoMeleeWeaponsPossible_ReturnNothing()
        {
            allProficientWeapons.Clear();
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            var otherRangedWeapon = CreateRangedWeapon("other ranged weapon");

            var weapon = weaponGenerator.GenerateMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.Null);
            mockMagicalWeaponGenerator.Verify(g => g.Generate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Never);
        }

        [Test]
        public void GenerateOneHandedMeleeFrom_OneHandedMeleeWeaponMustBeMelee()
        {
            var rangedWeapon = CreateRangedWeapon("ranged weapon");

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(allProficientWeapons.Intersect(allMeleeWeapons).Except(allTwoHandedWeapons).ToArray())))
                .Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            var weapon = weaponGenerator.GenerateOneHandedMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateOneHandedMeleeFrom_OneHandedMeleeWeaponMustBeOneHanded()
        {
            var twoHandedWeapon = CreateTwoHandedMeleeWeapon("two-handed weapon");

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(allProficientWeapons.Intersect(allMeleeWeapons).Except(allTwoHandedWeapons).ToArray())))
                .Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(magicalWeapon);

            var weapon = weaponGenerator.GenerateOneHandedMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateOneHandedMeleeFrom_IfGenerationOfOneHandedMeleeWeaponFails_TryWithoutNonProficiencyWeaponFoci()
        {
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            var otherMagicalWeapon = CreateOneHandedMeleeWeapon("one-handed weapon");

            var wrongSpecialties = new[] { rangedWeapon.Name };
            var specialties = new[] { otherMagicalWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(specialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(otherMagicalWeapon);

            additionalFeats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            additionalFeats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add("feat3");

            var weapon = weaponGenerator.GenerateOneHandedMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(otherMagicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateOneHandedMeleeFrom_IfGenerationOfOneHandedMeleeWeaponFailsAgain_TryWithoutSpecificProficiencyWeaponFoci()
        {
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            var twoHandedWeapon = CreateTwoHandedMeleeWeapon("two-handed weapon");

            var wrongSpecialties = new[] { rangedWeapon.Name };
            var specialties = new[] { twoHandedWeapon.Name };
            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(allProficientWeapons.Intersect(allMeleeWeapons).Except(allTwoHandedWeapons).ToArray()))
                .Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon")).Returns(magicalWeapon);

            additionalFeats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            additionalFeats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add("feat3");

            var weapon = weaponGenerator.GenerateOneHandedMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(magicalWeapon), weapon.Name);
        }

        [Test]
        public void GenerateOneHandedMeleeFrom_IfNoOneHandedMeleeWeaponsPossible_ReturnNothing()
        {
            allProficientWeapons.Clear();
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            var twoHandedWeapon = CreateTwoHandedMeleeWeapon("two-handed weapon");

            var weapon = weaponGenerator.GenerateOneHandedMeleeFrom(feats, characterClass, race);
            Assert.That(weapon, Is.Null);
            mockMagicalWeaponGenerator.Verify(g => g.Generate(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GenerateRangedFrom_RangedWeaponMustNotBeMelee()
        {
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("ranged weapon", "other ranged"))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(rangedWeapon);

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(rangedWeapon), weapon.Name);
        }

        [Test]
        public void GenerateRangedFrom_IfGenerationOfRangedWeaponFails_TryWithoutNonProficiencyWeaponFoci()
        {
            var meleeWeapon = CreateOneHandedMeleeWeapon("melee weapon");
            var rangedWeapon = CreateRangedWeapon("ranged weapon");

            var wrongSpecialties = new[] { meleeWeapon.Name };
            var specialties = new[] { rangedWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet(specialties))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(rangedWeapon);

            additionalFeats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            additionalFeats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add("feat3");

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(rangedWeapon), weapon.Name);
        }

        [Test]
        public void GenerateRangedFrom_IfGenerationOfRangedWeaponFailsAgain_TryWithoutSpecificProficiencyWeaponFoci()
        {
            var meleeWeapon = CreateOneHandedMeleeWeapon("melee weapon");
            var otherMeleeWeapon = CreateOneHandedMeleeWeapon("other melee weapon");
            var rangedWeapon = CreateRangedWeapon("ranged weapon");

            var wrongSpecialties = new[] { meleeWeapon.Name };
            var specialties = new[] { otherMeleeWeapon.Name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("ranged weapon", "other ranged"))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(rangedWeapon);

            additionalFeats.Add(new Feat { Name = "feat2", Foci = wrongSpecialties });
            additionalFeats.Add(new Feat { Name = "feat3", Foci = specialties });
            proficiencyFeats.Add("feat3");

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.Not.Null);
            Assert.That(weapon, Is.EqualTo(rangedWeapon), weapon.Name);
        }

        [Test]
        public void GenerateRangedFrom_RangedWeaponCannotBeAmmunition()
        {
            var ammunition = CreateAmmunition("my ammo");
            var rangedWeapon = CreateRangedWeapon("ranged weapon");
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("ranged weapon", "other ranged"))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(rangedWeapon);

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(rangedWeapon), weapon.Name);
        }

        //INFO: Example here is Shurikens
        [Test]
        public void GenerateRangedFrom_RangedThrownAmmunitionIsAllowed()
        {
            var thrown = CreateRangedWeapon("thrown weapon");
            var rangedWeapon = CreateRangedWeapon("ranged weapon");

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(RandomWeightedCollection<string>.EquivalentSet("thrown weapon", "ranged weapon", "other ranged"))).Returns("my random weapon");
            mockMagicalWeaponGenerator.Setup(g => g.Generate(power, "my random weapon", race.Size)).Returns(thrown);

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.EqualTo(thrown), weapon.Name);
        }

        [Test]
        public void GenerateRangedFrom_GenerateNoRangedWeapon()
        {
            additionalFeats.Add(new Feat { Name = "feat2", Foci = [magicalWeapon.Name] });
            proficiencyFeats.Add("feat2");

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.Null);
        }

        [Test]
        public void GenerateRangedFrom_IfNoRangedWeaponsPossible_ReturnNothing()
        {
            allProficientWeapons.Clear();
            var meleeWeapon = CreateOneHandedMeleeWeapon("melee weapon");
            var otherMeleeWeapon = CreateTwoHandedMeleeWeapon("two-handed weapon");
            var ammunition = CreateAmmunition("my ammo");

            var weapon = weaponGenerator.GenerateRangedFrom(feats, characterClass, race);
            Assert.That(weapon, Is.Null);
            mockMagicalWeaponGenerator.Verify(g => g.Generate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Never);
        }
    }
}
