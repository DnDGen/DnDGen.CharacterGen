using System.Collections.Generic;

namespace DnDGen.CharacterGen.Selectors
{
    internal interface IAdjustmentsSelector
    {
        Dictionary<string, int> SelectAllFrom(string tableName);
        int SelectFrom(string tableName, string name);
    }
}