using System.Collections.Generic;

namespace GnsClassifier.Common
{
    public interface IDictionaryDb<TKey, TValue>
    {
        IDictionary<TKey, TValue> GetEntries();
        void UpdateEntry(TKey word, TValue result);
    }
}