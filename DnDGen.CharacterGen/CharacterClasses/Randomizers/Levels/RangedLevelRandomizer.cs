using DnDGen.RollGen;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels
{
    internal abstract class RangedLevelRandomizer : ILevelRandomizer
    {
        protected int rollBonus;
        private readonly Dice dice;

        public RangedLevelRandomizer(Dice dice)
        {
            this.dice = dice;
        }

        public int Randomize()
        {
            var roll = dice.Roll().d(5).AsSum();
            return roll + rollBonus;
        }

        public IEnumerable<int> GetAllPossibleResults()
        {
            return Enumerable.Range(1 + rollBonus, 5);
        }
    }
}