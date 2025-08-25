using DnDGen.Infrastructure.Selectors.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.CharacterGen.Selectors
{
    internal class AdjustmentsSelector : IAdjustmentsSelector
    {
        private readonly ICollectionSelector collectionsSelector;

        public AdjustmentsSelector(ICollectionSelector collectionsSelector)
        {
            this.collectionsSelector = collectionsSelector;
        }

        public Dictionary<string, int> SelectAllFrom(string tableName)
        {
            var collectionTable = collectionsSelector.SelectAllFrom(Config.Name, tableName);
            var adjustmentTable = new Dictionary<string, int>();

            foreach (var kvp in collectionTable)
            {
                adjustmentTable[kvp.Key] = GetAdjustment(kvp.Value);
            }

            return adjustmentTable;
        }

        public int SelectFrom(string tableName, string name)
        {
            var collection = collectionsSelector.SelectFrom(Config.Name, tableName, name);
            var adjustment = GetAdjustment(collection);

            return adjustment;
        }

        private int GetAdjustment(IEnumerable<string> collection)
        {
            var firstItem = collection.First();
            var adjustment = Convert.ToInt32(firstItem);

            return adjustment;
        }
    }
}