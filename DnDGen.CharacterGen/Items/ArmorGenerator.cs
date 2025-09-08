using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Items.Selectors;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Generators;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.RollGen;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Items
{
    internal class ArmorGenerator(
        ICollectionSelector collectionsSelector,
        JustInTimeFactory justInTimeFactory,
        Dice dice,
        ITreasureLevelSelector treasureLevelSelector) : IArmorGenerator
    {
        private readonly ICollectionSelector collectionsSelector = collectionsSelector;
        private readonly MundaneItemGenerator mundaneArmorGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Armor);
        private readonly MagicalItemGenerator magicalArmorGenerator = justInTimeFactory.Build<MagicalItemGenerator>(ItemTypeConstants.Armor);
        private readonly Dice dice = dice;
        private readonly ITreasureLevelSelector treasureLevelSelector = treasureLevelSelector;

        public Armor GenerateArmorFrom(FeatCollections feats, CharacterClass characterClass, Race race)
        {
            var proficiencyFeats = GetArmorProficiencyFeats(feats.All, ItemTypeConstants.Armor);
            if (!proficiencyFeats.Any())
                return null;

            var power = treasureLevelSelector.SelectPowerFrom(characterClass, race);
            var armorName = GetPreferredArmor(feats, ItemTypeConstants.Armor, characterClass, power);
            var item = GenerateArmor(power, armorName, race, characterClass);

            return item as Armor;
        }

        //Source: https://www.d20srd.org/srd/magicItems/magicArmor.htm
        private static double GetSpecificThreshold(string power) => power switch
        {
            PowerConstants.Minor => .98,
            PowerConstants.Medium or PowerConstants.Major => .97,
            _ => 1.01,
        };

        //Source: https://www.d20srd.org/srd/magicItems/magicArmor.htm
        private static int GetRollAgainThreshold(string power) => power switch
        {
            PowerConstants.Minor => 92,
            PowerConstants.Medium or PowerConstants.Major => 64,
            _ => 101,
        };

        public Armor GenerateShieldFrom(FeatCollections feats, CharacterClass characterClass, Race race)
        {
            var proficiencyFeats = GetArmorProficiencyFeats(feats.All, AttributeConstants.Shield);
            if (!proficiencyFeats.Any())
                return null;

            var power = treasureLevelSelector.SelectPowerFrom(characterClass, race);
            var shieldName = GetPreferredArmor(feats, AttributeConstants.Shield, characterClass, power);
            var item = GenerateArmor(power, shieldName, race, characterClass);

            return item as Armor;
        }

        private Item GenerateArmor(string power, string armorName, Race race, CharacterClass characterClass)
        {
            var metalArmors = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Metal);
            var traits = new[] { race.Size };

            //INFO: If we are able to select a metal armor here for a Druid, then it was rolled that it could be dragonhide
            if (characterClass.Name == CharacterClassConstants.Druid && metalArmors.Contains(armorName))
            {
                traits = [race.Size, TraitConstants.SpecialMaterials.Dragonhide];
            }

            if (power == PowerConstants.Mundane)
            {
                return mundaneArmorGenerator.Generate(armorName, traits);
            }

            return magicalArmorGenerator.Generate(power, armorName, traits);
        }

        private string GetPreferredArmor(FeatCollections feats, string armorType, CharacterClass characterClass, string power)
        {
            var specificThreshold = GetSpecificThreshold(power);
            var rollAgainThreshold = GetRollAgainThreshold(power);

            //INFO: Need to filter out specific armors, if they aren't a possibility
            //Even though TreasureGen can handle specific armors from base versions, there are some specific armors that jump proficiency categories,
            //such as full plate of speed being medium proficiency instead of heavy
            bool isSpecific;
            do
            {
                isSpecific = dice.Roll().Percentile().AsTrueOrFalse(specificThreshold);
            } while (!isSpecific && dice.Roll().Percentile().AsTrueOrFalse(rollAgainThreshold));

            //INFO: Armor has a 5% chance to be made of a special material
            //There are 3 special materials that can apply to metal armor: Adamantine, mithral, and dragonhide
            var isDragonhide = dice.Roll().Percentile().AsTrueOrFalse(.95) && dice.Roll().d3().AsTrueOrFalse(3);

            var commonArmors = GetPreferredArmors(feats.Additional, armorType, characterClass, isSpecific, isDragonhide);
            var uncommonArmors = GetPreferredArmors(feats.Class, armorType, characterClass, isSpecific, isDragonhide);
            var rareArmors = GetPreferredArmors(feats.Racial, armorType, characterClass, isSpecific, isDragonhide);

            var preferredArmor = collectionsSelector.SelectRandomFrom(commonArmors, uncommonArmors, rareArmors);
            return preferredArmor;
        }

        private IEnumerable<Feat> GetArmorProficiencyFeats(IEnumerable<Feat> feats, string armorType)
        {
            var proficiencyFeatNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, armorType + GroupConstants.Proficiency);
            return feats.Where(f => proficiencyFeatNames.Contains(f.Name));
        }

        private IEnumerable<string> GetPreferredArmors(IEnumerable<Feat> feats, string armorType, CharacterClass characterClass, bool isSpecific, bool isDragonhide)
        {
            var proficiencyFeats = GetArmorProficiencyFeats(feats, armorType);
            var proficiencyFeatNames = proficiencyFeats.Select(f => f.Name);
            if (!proficiencyFeatNames.Any(HasPossibleArmors))
                return [];

            var commonArmorFeatNames = new[] { FeatConstants.HeavyArmorProficiency, FeatConstants.TowerShieldProficiency };
            var uncommonArmorFeatNames = new[] { FeatConstants.MediumArmorProficiency, FeatConstants.ShieldProficiency };
            var rareArmorFeatNames = new[] { FeatConstants.LightArmorProficiency };

            var preferredFeatName = collectionsSelector.SelectRandomFrom(
                proficiencyFeatNames.Intersect(commonArmorFeatNames).Where(HasPossibleArmors),
                proficiencyFeatNames.Intersect(uncommonArmorFeatNames).Where(HasPossibleArmors),
                proficiencyFeatNames.Intersect(rareArmorFeatNames).Where(HasPossibleArmors));

            return GetArmors(preferredFeatName);

            bool HasPossibleArmors(string featName) => GetArmors(featName).Any();

            IEnumerable<string> GetArmors(string featName)
            {
                var armors = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, featName);

                var specificArmors = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Specific);
                if (isSpecific)
                    armors = armors.Intersect(specificArmors);
                else
                    armors = armors.Except(specificArmors);

                if (characterClass.Name != CharacterClassConstants.Druid)
                    return armors;

                var metalArmors = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Metal);
                //INFO: Even if the armor should be Dragonhide, specific armors shouldn't have modified special materials
                if (!isDragonhide || isSpecific)
                    return armors.Except(metalArmors);

                return armors;
            }
        }
    }
}
