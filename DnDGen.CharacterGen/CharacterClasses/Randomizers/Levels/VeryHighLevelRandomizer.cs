using DnDGen.RollGen;

namespace DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels
{
    internal class VeryHighLevelRandomizer : RangedLevelRandomizer
    {
        public VeryHighLevelRandomizer(Dice dice)
            : base(dice)
        {
            rollBonus = 15;
        }
    }
}