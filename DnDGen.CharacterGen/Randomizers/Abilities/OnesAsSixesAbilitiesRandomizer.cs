using DnDGen.RollGen;

namespace DnDGen.CharacterGen.Randomizers.Abilities
{
    internal class OnesAsSixesAbilitiesRandomizer : BaseAbilitiesRandomizer
    {
        private readonly Dice dice;

        public OnesAsSixesAbilitiesRandomizer(Dice dice)
        {
            this.dice = dice;
        }

        protected override int RollAbility()
        {
            return dice.Roll("3d6t1").AsSum();
        }
    }
}