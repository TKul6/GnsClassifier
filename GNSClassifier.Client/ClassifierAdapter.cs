using System;
using System.Net.Http;
using System.Threading.Tasks;
using GnsClassifier.Common;
using Newtonsoft.Json;

namespace GnsClassifier.Client
{
    public class ClassifierAdapter
    {
        private readonly HttpClient _client;
        private readonly string _baseAddress;

        public ClassifierAdapter(string address)
        {
            var clientHandler = new HttpClientHandler()
            {
                UseDefaultCredentials = true
            };
            _client = new HttpClient(clientHandler);
            _baseAddress = address;
        }

        public void SubmitResult(string word, ClassifierResult result)
        {
            var address = $"{_baseAddress}/SubmitResult?word={word}&result={result}";
            Task.Run(() => _client.GetAsync(new Uri(address)));
        }

        public string GetWordToClassify()
        {
            var address = $"{_baseAddress}/GetWordToClassify";
            var serializedWord =  _client.GetStringAsync(new Uri(address)).Result;
            var word = JsonConvert.DeserializeObject<string>(serializedWord);
            return word;
        }
    }
}
