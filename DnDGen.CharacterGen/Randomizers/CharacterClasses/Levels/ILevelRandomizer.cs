using System.Collections.Generic;

namespace DnDGen.CharacterGen.Randomizers.CharacterClasses.Levels
{
    public interface ILevelRandomizer
    {
        int Randomize();
        IEnumerable<int> GetAllPossibleResults();
    }
}