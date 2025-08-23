using System.Collections.Generic;

namespace DnDGen.CharacterGen.Randomizers.CharacterClasses.Levels
{
    internal class SetLevelRandomizer : ISetLevelRandomizer
    {
        public int SetLevel { get; set; }

        public int Randomize()
        {
            return SetLevel;
        }

        public IEnumerable<int> GetAllPossibleResults()
        {
            return new[] { SetLevel };
        }
    }
}