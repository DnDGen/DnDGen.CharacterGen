using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Combats;
using DnDGen.CharacterGen.Feats;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Selectors.Collections;
using DnDGen.CharacterGen.Selectors.Selections;
using DnDGen.CharacterGen.Skills;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Generators.Feats
{
    internal class AdditionalFeatsGenerator : IAdditionalFeatsGenerator
    {
        private readonly ICollectionSelector collectionsSelector;
        private readonly IFeatsSelector featsSelector;
        private readonly IFeatFocusGenerator featFocusGenerator;
        private readonly IAdjustmentsSelector adjustmentsSelector;

        public AdditionalFeatsGenerator(
            ICollectionSelector collectionsSelector,
            IFeatsSelector featsSelector,
            IFeatFocusGenerator featFocusGenerator,
            IAdjustmentsSelector adjustmentsSelector)
        {
            this.collectionsSelector = collectionsSelector;
            this.featsSelector = featsSelector;
            this.featFocusGenerator = featFocusGenerator;
            this.adjustmentsSelector = adjustmentsSelector;
        }

        public IEnumerable<Feat> GenerateWith(
            CharacterClass characterClass,
            Race race,
            Dictionary<string, Ability> abilities,
            IEnumerable<Skill> skills,
            BaseAttack baseAttack,
            IEnumerable<Feat> preselectedFeats)
        {
            var additionalFeats = GetAdditionalFeats(characterClass, race, abilities, skills, baseAttack, preselectedFeats);
            var allButBonusFeats = preselectedFeats.Union(additionalFeats);
            var bonusFeats = GetBonusFeats(characterClass, abilities, skills, baseAttack, allButBonusFeats);

            return additionalFeats.Union(bonusFeats);
        }

        private IEnumerable<Feat> GetAdditionalFeats(
            CharacterClass characterClass,
            Race race,
            Dictionary<string, Ability> abilities,
            IEnumerable<Skill> skills,
            BaseAttack baseAttack,
            IEnumerable<Feat> preselectedFeats)
        {
            var numberOfAdditionalFeats = GetAdditionalFeatQuantity(characterClass, race);
            var feats = GetFeats(numberOfAdditionalFeats, string.Empty, characterClass, abilities, skills, baseAttack, preselectedFeats);

            return feats;
        }

        private int GetAdditionalFeatQuantity(CharacterClass characterClass, Race race)
        {
            var numberOfAdditionalFeats = characterClass.Level / 3 + 1;

            if (race.BaseRace == RaceConstants.BaseRaces.Human)
                numberOfAdditionalFeats++;

            if (characterClass.Name == CharacterClassConstants.Rogue && characterClass.Level >= 10)
                numberOfAdditionalFeats += (characterClass.Level - 10) / 3 + 1;

            var monsters = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters);
            if (monsters.Contains(race.BaseRace))
            {
                var monsterHitDice = adjustmentsSelector.SelectFrom(TableNameConstants.Set.Adjustments.MonsterHitDice, race.BaseRace);
                numberOfAdditionalFeats += monsterHitDice / 3 + 1;
            }

            return numberOfAdditionalFeats;
        }

        private IEnumerable<Feat> GetBonusFeats(
            CharacterClass characterClass,
            Dictionary<string, Ability> abilities,
            IEnumerable<Skill> skills,
            BaseAttack baseAttack,
            IEnumerable<Feat> preselectedFeats)
        {
            if (characterClass.Name == CharacterClassConstants.Fighter)
                return GetFighterFeats(characterClass, abilities, skills, baseAttack, preselectedFeats);
            else if (characterClass.Name == CharacterClassConstants.Wizard)
                return GetWizardBonusFeats(characterClass, abilities, skills, baseAttack, preselectedFeats);

            return Enumerable.Empty<Feat>();
        }

        private List<Feat> PopulateFeatsFrom(
            CharacterClass characterClass,
            Dictionary<string, Ability> abilities,
            IEnumerable<Skill> skills,
            IEnumerable<Feat> preselectedFeats,
            IEnumerable<AdditionalFeatSelection> sourceFeatSelections,
            int quantity)
        {
            var feats = new List<Feat>();
            var chosenFeats = new List<Feat>();
            chosenFeats.AddRange(preselectedFeats);

            var chosenFeatSelections = new List<AdditionalFeatSelection>();
            var preselectedFeatSelections = GetSelectedSelections(sourceFeatSelections, preselectedFeats);
            chosenFeatSelections.AddRange(preselectedFeatSelections);

            var availableFeatSelections = new List<AdditionalFeatSelection>();

            var newAvailableFeatSelections = AddNewlyAvailableFeatSelections(availableFeatSelections, sourceFeatSelections, chosenFeatSelections, chosenFeats);
            availableFeatSelections.AddRange(newAvailableFeatSelections);

            while (quantity-- > 0 && availableFeatSelections.Any())
            {
                var featSelection = collectionsSelector.SelectRandomFrom(availableFeatSelections);

                var preliminaryFocus = featFocusGenerator.GenerateFrom(
                    featSelection.Feat,
                    featSelection.FocusType,
                    skills,
                    featSelection.RequiredFeats,
                    chosenFeats,
                    characterClass);
                if (preliminaryFocus == FeatConstants.Foci.All)
                {
                    quantity++;

                    chosenFeatSelections.Add(featSelection);
                    availableFeatSelections.Remove(featSelection);

                    newAvailableFeatSelections = AddNewlyAvailableFeatSelections(availableFeatSelections, sourceFeatSelections, chosenFeatSelections, chosenFeats);
                    availableFeatSelections.AddRange(newAvailableFeatSelections);

                    continue;
                }

                var feat = new Feat();
                var hasMatchingFeat = feats.Any(f => FeatsWithFociMatch(f, featSelection));

                if (hasMatchingFeat)
                {
                    feat = feats.First(f => FeatsWithFociMatch(f, featSelection));
                }
                else
                {
                    feat.Name = featSelection.Feat;
                    feat.Frequency = featSelection.Frequency;
                    feat.Power = featSelection.Power;
                    feat.CanBeTakenMultipleTimes = featSelection.CanBeTakenMultipleTimes;

                    if (featSelection.Feat == FeatConstants.SpellMastery)
                        feat.Power = abilities[AbilityConstants.Intelligence].Bonus;

                    feats.Add(feat);
                    chosenFeats.Add(feat);
                }

                if (!FeatSelectionCanBeSelectedAgain(featSelection))
                {
                    chosenFeatSelections.Add(featSelection);
                    availableFeatSelections.Remove(featSelection);
                }

                newAvailableFeatSelections = AddNewlyAvailableFeatSelections(availableFeatSelections, sourceFeatSelections, chosenFeatSelections, chosenFeats);
                availableFeatSelections.AddRange(newAvailableFeatSelections);

                if (string.IsNullOrEmpty(preliminaryFocus) == false)
                    feat.Foci = feat.Foci.Union(new[] { preliminaryFocus });

                var featFociQuantity = 0;
                if (featSelection.Feat == FeatConstants.SkillMastery)
                    featFociQuantity = abilities[AbilityConstants.Intelligence].Bonus + featSelection.Power - 1;

                while (featFociQuantity-- > 0 && preliminaryFocus != FeatConstants.Foci.All && string.IsNullOrEmpty(preliminaryFocus) == false)
                {
                    preliminaryFocus = featFocusGenerator.GenerateFrom(
                        featSelection.Feat,
                        featSelection.FocusType,
                        skills,
                        featSelection.RequiredFeats,
                        chosenFeats,
                        characterClass);
                    feat.Foci = feat.Foci.Union(new[] { preliminaryFocus });
                }
            }

            return feats;
        }

        private IEnumerable<AdditionalFeatSelection> GetSelectedSelections(IEnumerable<AdditionalFeatSelection> sourceFeatSelections, IEnumerable<Feat> preselectedFeats)
        {
            var featNames = preselectedFeats.Select(f => f.Name);
            var featSelections = sourceFeatSelections.Where(s => featNames.Contains(s.Feat));
            var nonRepeatableFeatSelections = featSelections.Where(s => !FeatSelectionCanBeSelectedAgain(s));

            return nonRepeatableFeatSelections;
        }

        private bool FeatSelectionCanBeSelectedAgain(AdditionalFeatSelection featSelection)
        {
            return !string.IsNullOrEmpty(featSelection.FocusType) || featSelection.CanBeTakenMultipleTimes;
        }

        private bool FeatsWithFociMatch(Feat feat, AdditionalFeatSelection featSelection)
        {
            return feat.Frequency.TimePeriod == string.Empty
                && feat.Name == featSelection.Feat
                && feat.Power == featSelection.Power
                && feat.Foci.Any()
                && !string.IsNullOrEmpty(featSelection.FocusType);
        }

        private IEnumerable<AdditionalFeatSelection> AddNewlyAvailableFeatSelections(
            IEnumerable<AdditionalFeatSelection> currentAdditionalFeatSelections,
            IEnumerable<AdditionalFeatSelection> sourceFeatSelections,
            IEnumerable<AdditionalFeatSelection> chosenFeatSelections,
            IEnumerable<Feat> chosenFeats)
        {
            var missingSelections = sourceFeatSelections.Except(currentAdditionalFeatSelections);
            var newPossibleSelections = missingSelections.Except(chosenFeatSelections);
            var newFeatSelectionsWithRequirementsMet = newPossibleSelections.Where(f => f.MutableRequirementsMet(chosenFeats));

            return newFeatSelectionsWithRequirementsMet;
        }

        private IEnumerable<Feat> GetFighterFeats(
            CharacterClass characterClass,
            Dictionary<string, Ability> stats,
            IEnumerable<Skill> skills,
            BaseAttack baseAttack,
            IEnumerable<Feat> selectedFeats)
        {
            var numberOfFighterFeats = characterClass.Level / 2 + 1;
            var feats = GetFeats(numberOfFighterFeats, GroupConstants.FighterBonusFeats, characterClass, stats, skills, baseAttack, selectedFeats);

            return feats;
        }

        private IEnumerable<Feat> GetFeats(
            int quantity,
            string groupName,
            CharacterClass characterClass,
            Dictionary<string, Ability> stats,
            IEnumerable<Skill> skills,
            BaseAttack baseAttack,
            IEnumerable<Feat> selectedFeats)
        {
            var featSelections = featsSelector.SelectAdditional();

            if (!string.IsNullOrEmpty(groupName))
            {
                var groupFeatNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, groupName);
                featSelections = featSelections.Where(f => groupFeatNames.Contains(f.Feat));
            }

            //INFO: Calling immediate execution, so this doesn't reevaluate every time the collection is called
            var availableFeats = featSelections.Where(f => f.ImmutableRequirementsMet(baseAttack, stats, skills, characterClass)).ToArray();
            var feats = PopulateFeatsFrom(characterClass, stats, skills, selectedFeats, availableFeats, quantity);

            return feats;
        }

        private IEnumerable<Feat> GetWizardBonusFeats(
            CharacterClass characterClass,
            Dictionary<string, Ability> stats,
            IEnumerable<Skill> skills,
            BaseAttack baseAttack,
            IEnumerable<Feat> selectedFeats)
        {
            var numberOfWizardFeats = characterClass.Level / 5;
            var feats = GetFeats(numberOfWizardFeats, GroupConstants.WizardBonusFeats, characterClass, stats, skills, baseAttack, selectedFeats);

            return feats;
        }
    }
}