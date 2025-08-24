using DnDGen.RollGen;

namespace DnDGen.CharacterGen.Abilities.Randomizers
{
    internal class HeroicAbilitiesRandomizer : BaseAbilitiesRandomizer
    {
        private readonly Dice dice;

        public HeroicAbilitiesRandomizer(Dice dice)
        {
            this.dice = dice;
        }

        protected override int RollAbility()
        {
            //INFO: 15-18
            return dice.Roll(3).d2().Plus(12).AsSum();
        }
    }
}