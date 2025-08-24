using DnDGen.RollGen;

namespace DnDGen.CharacterGen.Abilities.Randomizers
{
    internal class PoorAbilitiesRandomizer : BaseAbilitiesRandomizer
    {
        private readonly Dice dice;

        public PoorAbilitiesRandomizer(Dice dice)
        {
            this.dice = dice;
        }

        protected override int RollAbility()
        {
            //INFO: 3-9
            return dice.Roll(3).d3().AsSum();
        }
    }
}