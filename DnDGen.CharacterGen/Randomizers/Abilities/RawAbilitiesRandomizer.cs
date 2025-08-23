using DnDGen.RollGen;

namespace DnDGen.CharacterGen.Randomizers.Abilities
{
    internal class RawAbilitiesRandomizer : BaseAbilitiesRandomizer
    {
        private readonly Dice dice;

        public RawAbilitiesRandomizer(Dice dice)
        {
            this.dice = dice;
        }

        protected override int RollAbility()
        {
            return dice.Roll(3).d6().AsSum();
        }
    }
}