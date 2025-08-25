using DnDGen.RollGen;

namespace DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels
{
    internal class HighLevelRandomizer : RangedLevelRandomizer
    {
        public HighLevelRandomizer(Dice dice)
            : base(dice)
        {
            rollBonus = 10;
        }
    }
}