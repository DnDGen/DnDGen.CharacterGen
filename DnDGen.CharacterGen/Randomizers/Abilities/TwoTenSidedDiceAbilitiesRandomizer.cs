using DnDGen.RollGen;

namespace DnDGen.CharacterGen.Randomizers.Abilities
{
    internal class TwoTenSidedDiceAbilitiesRandomizer : BaseAbilitiesRandomizer
    {
        private readonly Dice dice;

        public TwoTenSidedDiceAbilitiesRandomizer(Dice dice)
        {
            this.dice = dice;
        }

        protected override int RollAbility()
        {
            return dice.Roll(2).d10().AsSum();
        }
    }
}