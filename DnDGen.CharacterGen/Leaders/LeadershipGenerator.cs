using DnDGen.CharacterGen.Abilities.Randomizers;
using DnDGen.CharacterGen.Alignments;
using DnDGen.CharacterGen.Alignments.Randomizers;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.ClassNames;
using DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels;
using DnDGen.CharacterGen.Characters;
using DnDGen.CharacterGen.Leaders.Selectors;
using DnDGen.CharacterGen.Races.Randomizers;
using DnDGen.CharacterGen.Selectors;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Leaders
{
    internal class LeadershipGenerator(
        ICharacterGenerator characterGenerator,
        ILeadershipSelector leadershipSelector,
        IPercentileSelector percentileSelector,
        IAdjustmentsSelector adjustmentsSelector,
        ICollectionSelector collectionsSelector,
        JustInTimeFactory justInTimeFactrory) : ILeadershipGenerator
    {
        private readonly ICharacterGenerator characterGenerator = characterGenerator;
        private readonly ILeadershipSelector leadershipSelector = leadershipSelector;
        private readonly IPercentileSelector percentileSelector = percentileSelector;
        private readonly IAdjustmentsSelector adjustmentsSelector = adjustmentsSelector;
        private readonly ICollectionSelector collectionsSelector = collectionsSelector;
        private readonly JustInTimeFactory justInTimeFactrory = justInTimeFactrory;

        public Leadership GenerateLeadership(int level, int charismaBonus, string leaderAnimal)
        {
            var leadership = new Leadership
            {
                Score = level + charismaBonus
            };

            var leadershipModifiers = new List<string>();
            var reputation = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Set.Percentile.Reputation);
            var leadershipAdjustments = adjustmentsSelector.SelectAllFrom(TableNameConstants.Set.Adjustments.LeadershipModifiers);

            if (string.IsNullOrEmpty(reputation) == false)
            {
                leadershipModifiers.Add(reputation);
                leadership.Score += leadershipAdjustments[reputation];
            }

            leadership.CohortScore = leadership.Score;
            var cohortDeaths = 0;

            while (percentileSelector.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.KilledCohort))
                cohortDeaths++;

            leadership.CohortScore -= cohortDeaths * 2;

            if (cohortDeaths > 0)
            {
                var modifier = string.Format("Caused the death of {0} cohort(s)", cohortDeaths);
                leadershipModifiers.Add(modifier);
            }

            if (string.IsNullOrEmpty(leaderAnimal) == false)
                leadership.CohortScore -= 2;

            var followerScore = leadership.Score;
            var leaderMovement = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Set.Percentile.LeadershipMovement);

            if (string.IsNullOrEmpty(leaderMovement) == false)
            {
                leadershipModifiers.Add(leaderMovement);
                followerScore += leadershipAdjustments[leaderMovement];
            }

            if (percentileSelector.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.KilledFollowers))
            {
                leadershipModifiers.Add("Caused the death of followers");
                followerScore--;
            }

            leadership.LeadershipModifiers = leadershipModifiers;
            leadership.FollowerQuantities = leadershipSelector.SelectFollowerQuantitiesFor(followerScore);

            return leadership;
        }

        public Character GenerateCohort(int cohortScore, int leaderLevel, string leaderAlignment, string leaderClass)
        {
            var alignmentDiffers = percentileSelector.SelectFrom<bool>(Config.Name, TableNameConstants.Set.TrueOrFalse.AttractCohortOfDifferentAlignment);
            if (alignmentDiffers)
                cohortScore--;

            var cohortLevel = leadershipSelector.SelectCohortLevelFor(cohortScore);
            cohortLevel = Math.Min(leaderLevel - 2, cohortLevel);

            if (cohortLevel <= 0)
                return null;

            var alignment = new Alignment(leaderAlignment);

            if (alignmentDiffers)
                alignment = GetAlignment(leaderAlignment, false);

            return GenerateFollower(alignment, cohortLevel, leaderClass);
        }

        public Character GenerateFollower(int level, string leaderAlignment, string leaderClass)
        {
            var alignment = GetAlignment(leaderAlignment);
            return GenerateFollower(alignment, level, leaderClass);
        }

        private Character GenerateFollower(Alignment alignment, int level, string leaderClass)
        {
            var setLevelRandomizer = justInTimeFactrory.Build<ISetLevelRandomizer>();
            var setAlignmentRandomizer = justInTimeFactrory.Build<ISetAlignmentRandomizer>();
            var baseRaceRandomizer = justInTimeFactrory.Build<RaceRandomizer>(RaceRandomizerTypeConstants.BaseRace.AnyBase);
            var metaraceRandomizer = justInTimeFactrory.Build<RaceRandomizer>(RaceRandomizerTypeConstants.Metarace.AnyMeta);
            var abilitiesRandomizer = justInTimeFactrory.Build<IAbilitiesRandomizer>(AbilitiesRandomizerTypeConstants.BestOfFour);

            setLevelRandomizer.SetLevel = level;
            setAlignmentRandomizer.SetAlignment = alignment;

            var npcs = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.ClassNameGroups, GroupConstants.NPCs);

            if (npcs.Contains(leaderClass))
            {
                var anyNPCClassNameRandomizer = justInTimeFactrory.Build<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.AnyNPC);
                return characterGenerator.GenerateWith(
                    setAlignmentRandomizer,
                    anyNPCClassNameRandomizer,
                    setLevelRandomizer,
                    baseRaceRandomizer,
                    metaraceRandomizer,
                    abilitiesRandomizer);
            }

            var anyPlayerClassNameRandomizer = justInTimeFactrory.Build<IClassNameRandomizer>(ClassNameRandomizerTypeConstants.AnyPlayer);
            return characterGenerator.GenerateWith(
                setAlignmentRandomizer,
                anyPlayerClassNameRandomizer,
                setLevelRandomizer,
                baseRaceRandomizer,
                metaraceRandomizer,
                abilitiesRandomizer);
        }

        private Alignment GetAlignment(string leaderAlignment, bool allowLeaderAlignment = true)
        {
            var possibleAlignments = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Set.Collection.AlignmentGroups, leaderAlignment);

            if (!allowLeaderAlignment)
                possibleAlignments = possibleAlignments.Where(a => a != leaderAlignment);

            var alignment = collectionsSelector.SelectRandomFrom(possibleAlignments);

            return new Alignment(alignment);
        }
    }
}
