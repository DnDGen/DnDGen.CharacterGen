using DnDGen.CharacterGen.Selectors;
using DnDGen.CharacterGen.Skills.Selectors;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Feats.Selectors
{
    internal class FeatsSelector : IFeatsSelector
    {
        private readonly ICollectionSelector collectionsSelector;
        private readonly IAdjustmentsSelector adjustmentsSelector;

        public FeatsSelector(ICollectionSelector collectionsSelector, IAdjustmentsSelector adjustmentsSelector)
        {
            this.collectionsSelector = collectionsSelector;
            this.adjustmentsSelector = adjustmentsSelector;
        }

        public IEnumerable<RacialFeatSelection> SelectRacial(string race)
        {
            var tableName = string.Format(TableNameConstants.Formattable.Collection.RACEFeatData, race);
            var featData = collectionsSelector.SelectAllFrom(Config.Name, tableName);
            var racialFeatSelections = new List<RacialFeatSelection>();

            foreach (var dataKVP in featData)
            {
                //INFO: Calling ToArray so we can access indices
                var data = dataKVP.Value.ToArray();

                var racialFeatSelection = new RacialFeatSelection();
                racialFeatSelection.Feat = data[DataIndexConstants.RacialFeatData.FeatNameIndex];
                racialFeatSelection.SizeRequirement = data[DataIndexConstants.RacialFeatData.SizeRequirementIndex];
                racialFeatSelection.Power = Convert.ToInt32(data[DataIndexConstants.RacialFeatData.PowerIndex]);
                racialFeatSelection.MinimumHitDieRequirement = Convert.ToInt32(data[DataIndexConstants.RacialFeatData.MinimumHitDiceRequirementIndex]);
                racialFeatSelection.FocusType = data[DataIndexConstants.RacialFeatData.FocusIndex];
                racialFeatSelection.Frequency.Quantity = Convert.ToInt32(data[DataIndexConstants.RacialFeatData.FrequencyQuantityIndex]);
                racialFeatSelection.Frequency.TimePeriod = data[DataIndexConstants.RacialFeatData.FrequencyTimePeriodIndex];
                racialFeatSelection.MaximumHitDieRequirement = Convert.ToInt32(data[DataIndexConstants.RacialFeatData.MaximumHitDiceRequirementIndex]);
                racialFeatSelection.RandomFociQuantity = data[DataIndexConstants.RacialFeatData.RandomFociQuantity];
                racialFeatSelection.RequiredFeats = GetRequiredFeats(dataKVP.Key);

                var statNames = data[DataIndexConstants.RacialFeatData.RequiredStatIndex].Split(',');
                var statValue = Convert.ToInt32(data[DataIndexConstants.RacialFeatData.RequiredStatMinimumValueIndex]);

                if (string.IsNullOrEmpty(statNames[0]) == false)
                    for (var i = 0; i < statNames.Length; i++)
                        racialFeatSelection.MinimumAbilities[statNames[i]] = statValue;

                racialFeatSelections.Add(racialFeatSelection);
            }

            return racialFeatSelections;
        }

        public IEnumerable<AdditionalFeatSelection> SelectAdditional()
        {
            var featData = collectionsSelector.SelectAllFrom(Config.Name, TableNameConstants.Set.Collection.AdditionalFeatData);
            var additionalFeatSelections = new List<AdditionalFeatSelection>();

            foreach (var dataKVP in featData)
            {
                var additionalFeatSelection = SelectAdditional(dataKVP);
                additionalFeatSelections.Add(additionalFeatSelection);
            }

            return additionalFeatSelections;
        }

        private AdditionalFeatSelection SelectAdditional(KeyValuePair<string, IEnumerable<string>> dataKVP)
        {
            var additionalFeatSelection = new AdditionalFeatSelection();
            additionalFeatSelection.Feat = dataKVP.Key;

            var data = dataKVP.Value.ToArray();
            additionalFeatSelection.RequiredBaseAttack = Convert.ToInt32(data[DataIndexConstants.AdditionalFeatData.BaseAttackRequirementIndex]);
            additionalFeatSelection.FocusType = data[DataIndexConstants.AdditionalFeatData.FocusTypeIndex];
            additionalFeatSelection.Frequency.Quantity = Convert.ToInt32(data[DataIndexConstants.AdditionalFeatData.FrequencyQuantityIndex]);
            additionalFeatSelection.Frequency.TimePeriod = data[DataIndexConstants.AdditionalFeatData.FrequencyTimePeriodIndex];
            additionalFeatSelection.Power = Convert.ToInt32(data[DataIndexConstants.AdditionalFeatData.PowerIndex]);
            additionalFeatSelection.RequiredFeats = GetRequiredFeats(additionalFeatSelection.Feat);

            var featsWithClassRequirements = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.HasClassRequirements);
            if (featsWithClassRequirements.Contains(dataKVP.Key))
            {
                var tableName = string.Format(TableNameConstants.Formattable.Adjustments.FEATClassRequirements, dataKVP.Key);
                additionalFeatSelection.RequiredCharacterClasses = adjustmentsSelector.SelectAllFrom(tableName);
            }

            var featsWithSkillRequirements = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.HasSkillRequirements);
            if (featsWithSkillRequirements.Contains(dataKVP.Key))
            {
                additionalFeatSelection.RequiredSkills = GetRequiredSkills(additionalFeatSelection.Feat);
            }

            var featsWithStatRequirements = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.HasAbilityRequirements);
            if (featsWithStatRequirements.Contains(dataKVP.Key))
            {
                var tableName = string.Format(TableNameConstants.Formattable.Adjustments.FEATAbilityRequirements, dataKVP.Key);
                additionalFeatSelection.RequiredAbilities = adjustmentsSelector.SelectAllFrom(tableName);
            }

            var featsTakenMultipleTimes = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.TakenMultipleTimes);
            additionalFeatSelection.CanBeTakenMultipleTimes = featsTakenMultipleTimes.Contains(additionalFeatSelection.Feat);

            return additionalFeatSelection;
        }

        public IEnumerable<CharacterClassFeatSelection> SelectClass(string characterClassName)
        {
            var tableName = string.Format(TableNameConstants.Formattable.Collection.CLASSFeatData, characterClassName);
            var featData = collectionsSelector.SelectAllFrom(Config.Name, tableName);
            var classFeatSelections = new List<CharacterClassFeatSelection>();

            foreach (var dataKVP in featData)
            {
                var data = dataKVP.Value.ToArray();

                var classFeatSelection = new CharacterClassFeatSelection();
                classFeatSelection.Feat = data[DataIndexConstants.CharacterClassFeatData.FeatNameIndex];
                classFeatSelection.FocusType = data[DataIndexConstants.CharacterClassFeatData.FocusTypeIndex];
                classFeatSelection.Frequency.Quantity = Convert.ToInt32(data[DataIndexConstants.CharacterClassFeatData.FrequencyQuantityIndex]);
                classFeatSelection.Frequency.TimePeriod = data[DataIndexConstants.CharacterClassFeatData.FrequencyTimePeriodIndex];
                classFeatSelection.MinimumLevel = Convert.ToInt32(data[DataIndexConstants.CharacterClassFeatData.MinimumLevelRequirementIndex]);
                classFeatSelection.Power = Convert.ToInt32(data[DataIndexConstants.CharacterClassFeatData.PowerIndex]);
                classFeatSelection.MaximumLevel = Convert.ToInt32(data[DataIndexConstants.CharacterClassFeatData.MaximumLevelRequirementIndex]);
                classFeatSelection.FrequencyQuantityAbility = data[DataIndexConstants.CharacterClassFeatData.FrequencyQuantityStatIndex];
                classFeatSelection.RequiredFeats = GetRequiredFeats(dataKVP.Key);
                classFeatSelection.SizeRequirement = data[DataIndexConstants.CharacterClassFeatData.SizeRequirementIndex];
                classFeatSelection.AllowFocusOfAll = Convert.ToBoolean(data[DataIndexConstants.CharacterClassFeatData.AllowFocusOfAllIndex]);

                classFeatSelections.Add(classFeatSelection);
            }

            return classFeatSelections;
        }

        private IEnumerable<RequiredFeatSelection> GetRequiredFeats(string feat)
        {
            var allRequiredFeats = collectionsSelector.SelectAllFrom(Config.Name, TableNameConstants.Set.Collection.RequiredFeats);
            var requiredFeats = new List<RequiredFeatSelection>();

            if (allRequiredFeats.ContainsKey(feat) == false)
                return requiredFeats;

            var requiredFeatsData = allRequiredFeats[feat];

            foreach (var requiredFeatData in requiredFeatsData)
            {
                var splitData = requiredFeatData.Split('/');
                var requiredFeat = new RequiredFeatSelection();
                requiredFeat.Feat = splitData[0];

                if (splitData.Length > 1)
                    requiredFeat.Focus = splitData[1];

                requiredFeats.Add(requiredFeat);
            }

            return requiredFeats;
        }

        private IEnumerable<RequiredSkillSelection> GetRequiredSkills(string feat)
        {
            var tableName = string.Format(TableNameConstants.Formattable.Adjustments.FEATSkillRankRequirements, feat);
            var requiredSkillsAndRanks = adjustmentsSelector.SelectAllFrom(tableName);
            var requiredSkillSelections = new List<RequiredSkillSelection>();

            foreach (var kvp in requiredSkillsAndRanks)
            {
                var splitData = kvp.Key.Split('/');
                var requiredSkill = new RequiredSkillSelection();
                requiredSkill.Skill = splitData[0];
                requiredSkill.Ranks = kvp.Value;

                if (splitData.Length > 1)
                    requiredSkill.Focus = splitData[1];

                requiredSkillSelections.Add(requiredSkill);
            }

            return requiredSkillSelections;
        }
    }
}