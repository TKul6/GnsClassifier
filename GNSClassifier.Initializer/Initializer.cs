using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GnsClassifier.Common;
using Newtonsoft.Json;

namespace GnsClassifier.Initializer
{
    public class Initializer
    {
        public Dictionary<string, ClassifierResult> CreateClassifierResultsDictionaryFromFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var resultsDictionary = lines.ToDictionary(line => line, line => ClassifierResult.Unknown);
            return resultsDictionary;
        }

        public void WriteDictionaryToFile(Dictionary<string, ClassifierResult> dictionary, string filePath)
        {
            var dirPath = Path.GetDirectoryName(filePath);

            Debug.Assert(dirPath != null, "dirPath != null");

            Directory.CreateDirectory(dirPath);
            var serialized = JsonConvert.SerializeObject(dictionary);
            File.WriteAllText(filePath, serialized);
        }
    }
}
