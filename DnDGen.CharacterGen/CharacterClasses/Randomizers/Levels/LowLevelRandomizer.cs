using DnDGen.RollGen;

namespace DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels
{
    internal class LowLevelRandomizer : RangedLevelRandomizer
    {
        public LowLevelRandomizer(Dice dice)
            : base(dice)
        {
            rollBonus = 0;
        }
    }
}