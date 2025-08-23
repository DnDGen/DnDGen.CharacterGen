using DnDGen.RollGen;

namespace DnDGen.CharacterGen.Randomizers.CharacterClasses.Levels
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