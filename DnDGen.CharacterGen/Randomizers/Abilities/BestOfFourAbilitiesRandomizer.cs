using DnDGen.RollGen;

namespace DnDGen.CharacterGen.Randomizers.Abilities
{
    internal class BestOfFourAbilitiesRandomizer : BaseAbilitiesRandomizer
    {
        private readonly Dice dice;

        public BestOfFourAbilitiesRandomizer(Dice dice)
        {
            this.dice = dice;
        }

        protected override int RollAbility()
        {
            return dice.Roll("4d6k3").AsSum();
        }
    }
}