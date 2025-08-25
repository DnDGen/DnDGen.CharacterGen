using DnDGen.CharacterGen.Tables;
using DnDGen.CharacterGen.Leaders;
using System;
using System.Linq;
using DnDGen.CharacterGen.Selectors;

namespace DnDGen.CharacterGen.Leaders.Selectors
{
    internal class LeadershipSelector : ILeadershipSelector
    {
        private readonly IAdjustmentsSelector adjustmentsSelector;

        public LeadershipSelector(IAdjustmentsSelector adjustmentsSelector)
        {
            this.adjustmentsSelector = adjustmentsSelector;
        }

        public int SelectCohortLevelFor(int leadershipScore)
        {
            return GetAppropriateAdjustment(leadershipScore, TableNameConstants.Set.Adjustments.CohortLevels);
        }

        public FollowerQuantities SelectFollowerQuantitiesFor(int leadershipScore)
        {
            var followerQuantities = new FollowerQuantities();

            followerQuantities.Level1 = GetQuantityOfFollowersAtLevel(1, leadershipScore);
            followerQuantities.Level2 = GetQuantityOfFollowersAtLevel(2, leadershipScore);
            followerQuantities.Level3 = GetQuantityOfFollowersAtLevel(3, leadershipScore);
            followerQuantities.Level4 = GetQuantityOfFollowersAtLevel(4, leadershipScore);
            followerQuantities.Level5 = GetQuantityOfFollowersAtLevel(5, leadershipScore);
            followerQuantities.Level6 = GetQuantityOfFollowersAtLevel(6, leadershipScore);

            return followerQuantities;
        }

        private int GetQuantityOfFollowersAtLevel(int level, int leadershipScore)
        {
            var tableName = string.Format(TableNameConstants.Formattable.Adjustments.LevelXFollowerQuantities, level);
            return GetAppropriateAdjustment(leadershipScore, tableName);
        }

        private int GetAppropriateAdjustment(int leadershipScore, string tableName)
        {
            var adjustments = adjustmentsSelector.SelectAllFrom(tableName);
            var numericScores = adjustments.Keys.Select(k => Convert.ToInt32(k));
            var maxScore = numericScores.Max();

            if (leadershipScore > maxScore)
                return adjustments[maxScore.ToString()];

            var minScore = numericScores.Min();

            if (leadershipScore < minScore)
                return 0;

            return adjustments[leadershipScore.ToString()];
        }
    }
}
