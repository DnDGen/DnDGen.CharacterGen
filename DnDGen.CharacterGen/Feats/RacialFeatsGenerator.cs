using DnDGen.CharacterGen.Abilities;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Selectors.Collections;
using DnDGen.CharacterGen.Selectors.Selections;
using DnDGen.CharacterGen.Skills;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.RollGen;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Feats
{
    internal class RacialFeatsGenerator : IRacialFeatsGenerator
    {
        private readonly ICollectionSelector collectionsSelector;
        private readonly IAdjustmentsSelector adjustmentsSelector;
        private readonly IFeatsSelector featsSelector;
        private readonly IFeatFocusGenerator featFocusGenerator;
        private readonly Dice dice;

        public RacialFeatsGenerator(
            ICollectionSelector collectionsSelector,
            IAdjustmentsSelector adjustmentsSelector,
            IFeatsSelector featsSelector,
            IFeatFocusGenerator featFocusGenerator,
            Dice dice)
        {
            this.collectionsSelector = collectionsSelector;
            this.adjustmentsSelector = adjustmentsSelector;
            this.featsSelector = featsSelector;
            this.featFocusGenerator = featFocusGenerator;
            this.dice = dice;
        }

        public IEnumerable<Feat> GenerateWith(Race race, IEnumerable<Skill> skills, Dictionary<string, Ability> abilities)
        {
            var baseRacialFeatSelections = featsSelector.SelectRacial(race.BaseRace);
            var metaracialFeatSelections = featsSelector.SelectRacial(race.Metarace);
            var featToIncreasePower = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.FeatGroups, GroupConstants.AddMonsterHitDiceToPower);
            var monsterHitDice = GetMonsterHitDice(race.BaseRace);

            foreach (var selection in metaracialFeatSelections)
                if (featToIncreasePower.Contains(selection.Feat))
                    selection.Power += monsterHitDice;

            var metaraceSpeciesFeatSelections = featsSelector.SelectRacial(race.MetaraceSpecies);
            var allRacialFeatSelections = baseRacialFeatSelections.Union(metaracialFeatSelections).Union(metaraceSpeciesFeatSelections);
            var feats = new List<Feat>();

            foreach (var racialFeatSelection in allRacialFeatSelections)
            {
                if (racialFeatSelection.RequirementsMet(race, monsterHitDice, abilities, feats) == false)
                    continue;

                var feat = new Feat();
                feat.Name = racialFeatSelection.Feat;
                feat.Foci = GetFoci(racialFeatSelection, skills);

                feat.Frequency = racialFeatSelection.Frequency;
                feat.Power = racialFeatSelection.Power;

                feats.Add(feat);
            }

            return feats;
        }

        private IEnumerable<string> GetFoci(RacialFeatSelection racialFeatSelection, IEnumerable<Skill> skills)
        {
            if (string.IsNullOrEmpty(racialFeatSelection.FocusType))
                return Enumerable.Empty<string>();

            var foci = new HashSet<string>();

            var fociQuantity = 1;
            if (racialFeatSelection.RandomFociQuantity.Any())
                fociQuantity = dice.Roll(racialFeatSelection.RandomFociQuantity).AsSum();

            while (fociQuantity > foci.Count)
            {
                var focus = featFocusGenerator.GenerateAllowingFocusOfAllFrom(racialFeatSelection.Feat, racialFeatSelection.FocusType, skills);
                if (string.IsNullOrEmpty(focus) == false)
                    foci.Add(focus);
            }

            return foci;
        }

        private int GetMonsterHitDice(string baseRace)
        {
            var monsters = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.BaseRaceGroups, GroupConstants.Monsters);
            if (monsters.Contains(baseRace) == false)
                return 1;

            var hitDice = adjustmentsSelector.SelectFrom(TableNameConstants.Set.Adjustments.MonsterHitDice, baseRace);
            return hitDice;
        }
    }
}