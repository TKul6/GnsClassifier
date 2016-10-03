using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace GnsClassifier.Common
{
    public class DictionaryFileDb<TKey, TValue> : IDictionaryDb<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _entries;
        private readonly string _dbLocation;

        private readonly object _fileLockObject = new object();

        public DictionaryFileDb(string dbLocation)
        {
            _dbLocation = dbLocation;
            _entries = DeserializeEntries();
        }

        public IDictionary<TKey, TValue> GetEntries()
        {
            return _entries;
        }

        public void UpdateEntry(TKey word, TValue result)
        {
            _entries[word] = result;
            SerializeEntries();
        }

        private IDictionary<TKey, TValue> DeserializeEntries()
        {
            var resultsString = File.ReadAllText(_dbLocation);
            
            var results = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(resultsString);
            if (results == null)
            {
                results = new Dictionary<TKey, TValue>();
            }
            return results;
        }

        private void SerializeEntries()
        {
            var json = JsonConvert.SerializeObject(_entries);
            lock (_fileLockObject)
            {
                File.WriteAllText(_dbLocation, json);
            }
        }
    }
}
