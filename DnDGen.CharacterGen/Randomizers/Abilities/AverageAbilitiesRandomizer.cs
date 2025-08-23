using DnDGen.RollGen;

namespace DnDGen.CharacterGen.Generators.Randomizers.Abilities
{
    internal class AverageAbilitiesRandomizer : BaseAbilitiesRandomizer
    {
        private readonly Dice dice;

        public AverageAbilitiesRandomizer(Dice dice)
        {
            this.dice = dice;
        }

        protected override int RollAbility()
        {
            //INFO: 10-13
            return dice.Roll(3).d2().Plus(7).AsSum();
        }
    }
}