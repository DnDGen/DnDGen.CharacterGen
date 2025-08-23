using System.Collections.Generic;

namespace DnDGen.CharacterGen.Randomizers.CharacterClasses
{
    public interface ILevelRandomizer
    {
        int Randomize();
        IEnumerable<int> GetAllPossibleResults();
    }
}