using DnDGen.RollGen;

namespace DnDGen.CharacterGen.Randomizers.CharacterClasses.Levels
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