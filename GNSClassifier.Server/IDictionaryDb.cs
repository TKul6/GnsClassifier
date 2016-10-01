using System.Collections.Generic;

namespace GnsClassifier.Server
{
    public interface IDictionaryDb<TKey, TValue>
    {
        IDictionary<TKey, TValue> GetEntries();
        void UpdateEntry(TKey word, TValue result);
    }
}