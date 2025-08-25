using DnDGen.RollGen;

namespace DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels
{
    internal class MediumLevelRandomizer : RangedLevelRandomizer
    {
        public MediumLevelRandomizer(Dice dice)
            : base(dice)
        {
            rollBonus = 5;
        }
    }
}