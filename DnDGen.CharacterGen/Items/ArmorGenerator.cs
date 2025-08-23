using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Generators;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.RollGen;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Generators.Items
{
    internal class ArmorGenerator : IArmorGenerator
    {
        private readonly ICollectionSelector collectionsSelector;
        private readonly IPercentileSelector percentileSelector;
        private readonly JustInTimeFactory justInTimeFactory;
        private readonly Dice dice;

        public ArmorGenerator(ICollectionSelector collectionsSelector, IPercentileSelector percentileSelector, JustInTimeFactory justInTimeFactory, Dice dice)
        {
            this.collectionsSelector = collectionsSelector;
            this.percentileSelector = percentileSelector;
            this.justInTimeFactory = justInTimeFactory;
            this.dice = dice;
        }

        public Armor GenerateArmorFrom(IEnumerable<Feat> feats, CharacterClass characterClass, Race race)
        {
            var effectiveLevel = GetEffectiveLevel(characterClass);
            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, effectiveLevel);
            var power = percentileSelector.SelectFrom(Config.Name, tableName);
            var proficientArmors = GetProficientArmors(feats, ItemTypeConstants.Armor, characterClass);

            if (proficientArmors.Any() == false)
                return null;

            var item = GenerateArmor(power, proficientArmors, race, characterClass);

            return item as Armor;
        }

        //Source: https://www.d20srd.org/srd/magicItems/magicArmor.htm
        private double GetSpecificThreshold(string power)
        {
            return power switch
            {
                PowerConstants.Minor => .98,
                PowerConstants.Medium or PowerConstants.Major => .97,
                _ => 1.01,
            };
        }

        //Source: https://www.d20srd.org/srd/magicItems/magicArmor.htm
        private int GetRollAgainThreshold(string power)
        {
            return power switch
            {
                PowerConstants.Minor => 92,
                PowerConstants.Medium or PowerConstants.Major => 64,
                _ => 101,
            };
        }

        private int GetEffectiveLevel(CharacterClass characterClass)
        {
            return (int)Math.Max(1, characterClass.EffectiveLevel);
        }

        public Armor GenerateShieldFrom(IEnumerable<Feat> feats, CharacterClass characterClass, Race race)
        {
            var effectiveLevel = GetEffectiveLevel(characterClass);
            var tableName = string.Format(TableNameConstants.Formattable.Percentile.LevelXPower, effectiveLevel);
            var power = percentileSelector.SelectFrom(Config.Name, tableName);
            var proficientShields = GetProficientArmors(feats, AttributeConstants.Shield, characterClass);

            if (proficientShields.Any() == false)
                return null;

            var item = GenerateArmor(power, proficientShields, race, characterClass);

            return item as Armor;
        }

        private Item GenerateArmor(string power, IEnumerable<string> proficientArmors, Race race, CharacterClass characterClass)
        {
            //INFO: Need to filter out specific armors, if they aren't a possibility
            //Even though TreasureGen can handle specific armors from base versions, there are some specific armors that jump proficiency categories,
            //such as full plate of speed being medium proficiency instead of heavy
            var isSpecific = false;
            var specificThreshold = GetSpecificThreshold(power);
            var rollAgainThreshold = GetRollAgainThreshold(power);

            do
            {
                isSpecific = dice.Roll().Percentile().AsTrueOrFalse(specificThreshold);
            } while (!isSpecific && dice.Roll().Percentile().AsTrueOrFalse(rollAgainThreshold));

            var specificArmors = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Specific);
            var metalArmors = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Metal);

            if (isSpecific && proficientArmors.Intersect(specificArmors).Any()
                && (characterClass.Name != CharacterClassConstants.Druid || proficientArmors.Intersect(specificArmors.Except(metalArmors)).Any()))
            {
                if (characterClass.Name == CharacterClassConstants.Druid)
                    proficientArmors = proficientArmors.Intersect(specificArmors.Except(metalArmors));
                else
                    proficientArmors = proficientArmors.Intersect(specificArmors);
            }
            else
            {
                proficientArmors = proficientArmors.Except(specificArmors);
            }

            var armorName = collectionsSelector.SelectRandomFrom(proficientArmors);
            var traits = new[] { race.Size };

            //INFO: If we are able to select a metal armor here for a Druid, then it was rolled that it could be dragonhide
            if (characterClass.Name == CharacterClassConstants.Druid && metalArmors.Contains(armorName))
            {
                traits = new[] { race.Size, TraitConstants.SpecialMaterials.Dragonhide };
            }

            if (power == PowerConstants.Mundane)
            {
                var mundaneArmorGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Armor);
                return mundaneArmorGenerator.Generate(armorName, traits);
            }

            var magicalArmorGenerator = justInTimeFactory.Build<MagicalItemGenerator>(ItemTypeConstants.Armor);
            return magicalArmorGenerator.Generate(power, armorName, traits);
        }

        private IEnumerable<string> GetProficientArmors(IEnumerable<Feat> feats, string armorType, CharacterClass characterClass)
        {
            var proficiencyFeatNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, armorType + GroupConstants.Proficiency);
            var proficiencyFeats = feats.Where(f => proficiencyFeatNames.Contains(f.Name));
            var proficientArmors = new List<string>();

            foreach (var feat in proficiencyFeats)
            {
                var featArmors = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, feat.Name);
                proficientArmors.AddRange(featArmors);
            }

            if (characterClass.Name != CharacterClassConstants.Druid)
            {
                return proficientArmors;
            }

            //INFO: Armor has a 5% chance to be made of a special material
            var isSpecialMaterial = dice.Roll().Percentile().AsTrueOrFalse(.95);
            //There are 3 special materials that can apply to metal armor: Adamantine, mithral, and dragonhide
            var isDragonhide = dice.Roll().d3().AsTrueOrFalse(3);

            if (!isSpecialMaterial || !isDragonhide)
            {
                var metalArmors = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ItemGroups, AttributeConstants.Metal);

                return proficientArmors.Except(metalArmors);
            }

            return proficientArmors;
        }
    }
}
