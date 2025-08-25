using System.Collections.Generic;

namespace DnDGen.CharacterGen.CharacterClasses.Randomizers.Levels
{
    public interface ILevelRandomizer
    {
        int Randomize();
        IEnumerable<int> GetAllPossibleResults();
    }
}