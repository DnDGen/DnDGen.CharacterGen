using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items;
using DnDGen.CharacterGen.Races;
using DnDGen.TreasureGen.Items;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.CharacterGen.Tests.Integration.Items
{
    public class EquipmentGeneratorTests : IntegrationTests
    {
        private IEquipmentGenerator equipmentGenerator;

        [SetUp]
        public void Setup()
        {
            equipmentGenerator = GetNewInstanceOf<IEquipmentGenerator>();
        }

        [Test]
        public void GenerateGenericWeapons()
        {
            var feats = new FeatCollections
            {
                Class = [
                    new() { Name = FeatConstants.SimpleWeaponProficiency, Foci = [FeatConstants.Foci.All] },
                    new() { Name = FeatConstants.MartialWeaponProficiency, Foci = [FeatConstants.Foci.All] },
                ],
            };
            var characterClass = new CharacterClass
            {
                Name = CharacterClassConstants.Fighter,
                Level = 20,
            };
            var race = new Race { BaseRace = RaceConstants.BaseRaces.Human, Size = RaceConstants.Sizes.Medium };

            var isSpecific = true;
            var iterations = 1000;

            while (iterations-- > 0 && isSpecific)
            {
                var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
                isSpecific = equipment.PrimaryHand.Attributes.Contains(AttributeConstants.Specific);
            }

            Assert.That(isSpecific, Is.False);
        }

        [Test]
        public void GenerateSpecificWeapons()
        {
            var feats = new FeatCollections
            {
                Class = [
                    new() { Name = FeatConstants.SimpleWeaponProficiency, Foci = [FeatConstants.Foci.All] },
                    new() { Name = FeatConstants.MartialWeaponProficiency, Foci = [FeatConstants.Foci.All] },
                ],
            };
            var characterClass = new CharacterClass
            {
                Name = CharacterClassConstants.Fighter,
                Level = 20,
            };
            var race = new Race { BaseRace = RaceConstants.BaseRaces.Human, Size = RaceConstants.Sizes.Medium };

            var isSpecific = false;
            var iterations = 1000;

            while (iterations-- > 0 && !isSpecific)
            {
                var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
                isSpecific = equipment.PrimaryHand.Attributes.Contains(AttributeConstants.Specific);
            }

            Assert.That(isSpecific, Is.True);
        }

        [Test]
        public void GenerateGenericArmor()
        {
            var feats = new FeatCollections
            {
                Class = [
                    new() { Name = FeatConstants.LightArmorProficiency },
                    new() { Name = FeatConstants.MediumArmorProficiency },
                    new() { Name = FeatConstants.HeavyArmorProficiency },
                ],
            };
            var characterClass = new CharacterClass
            {
                Name = CharacterClassConstants.Fighter,
                Level = 20,
            };
            var race = new Race { BaseRace = RaceConstants.BaseRaces.Human, Size = RaceConstants.Sizes.Medium };

            var isSpecific = true;
            var iterations = 1000;

            while (iterations-- > 0 && isSpecific)
            {
                var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
                isSpecific = equipment.Armor.Attributes.Contains(AttributeConstants.Specific);
            }

            Assert.That(isSpecific, Is.False);
        }

        [Test]
        public void GenerateSpecificArmor()
        {
            var feats = new FeatCollections
            {
                Class = [
                    new() { Name = FeatConstants.LightArmorProficiency },
                    new() { Name = FeatConstants.MediumArmorProficiency },
                    new() { Name = FeatConstants.HeavyArmorProficiency },
                ],
            };
            var characterClass = new CharacterClass
            {
                Name = CharacterClassConstants.Fighter,
                Level = 20,
            };
            var race = new Race { BaseRace = RaceConstants.BaseRaces.Human, Size = RaceConstants.Sizes.Medium };

            var isSpecific = false;
            var iterations = 1000;

            while (iterations-- > 0 && !isSpecific)
            {
                var equipment = equipmentGenerator.GenerateWith(feats, characterClass, race);
                isSpecific = equipment.Armor.Attributes.Contains(AttributeConstants.Specific);
            }

            Assert.That(isSpecific, Is.True);
        }
    }
}
