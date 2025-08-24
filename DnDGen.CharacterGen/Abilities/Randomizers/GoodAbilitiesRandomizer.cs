using DnDGen.RollGen;

namespace DnDGen.CharacterGen.Abilities.Randomizers
{
    internal class GoodAbilitiesRandomizer : BaseAbilitiesRandomizer
    {
        private readonly Dice dice;

        public GoodAbilitiesRandomizer(Dice dice)
        {
            this.dice = dice;
        }

        protected override int RollAbility()
        {
            //INFO: 13-16
            return dice.Roll(3).d2().Plus(10).AsSum();
        }
    }
}