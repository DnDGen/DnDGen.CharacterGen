using DnDGen.CharacterGen.CharacterClasses;
using DnDGen.CharacterGen.Races;
using DnDGen.CharacterGen.Tables;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System;

namespace DnDGen.CharacterGen.Items.Selectors
{
    internal class TreasureLevelSelector(IPercentileSelector percentileSelector) : ITreasureLevelSelector
    {
        private readonly IPercentileSelector percentileSelector = percentileSelector;

        public int SelectLevelFrom(CharacterClass characterClass, Race race)
        {
            var level = characterClass.Level;

            if (characterClass.IsNPC)
                level += race.NPCChallengeRatingAdjustment;

            return Math.Max(1, level);
        }

        public string SelectPowerFrom(CharacterClass characterClass, Race race)
        {
            var treasureLevel = SelectLevelFrom(characterClass, race);
            var tableName = TableNameConstants.Formattable.Percentile.LevelXPower(treasureLevel);
            var power = percentileSelector.SelectFrom(Config.Name, tableName);

            return power;
        }
    }
}
