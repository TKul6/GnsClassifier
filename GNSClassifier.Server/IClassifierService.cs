using System.ServiceModel;
using System.ServiceModel.Web;
using GnsClassifier.Common;

namespace GnsClassifier.Server
{
    [ServiceContract]
    [ServiceKnownType(typeof(ClassifierResult))]
    public interface IClassifierService
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string GetWordToClassify();

        [OperationContract]
        [WebGet(UriTemplate = "SubmitResult?word={word}&result={result}")]
        void SubmitClassifierResult(string word, ClassifierResult result);
    }
}
