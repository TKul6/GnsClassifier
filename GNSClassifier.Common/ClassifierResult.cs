using System.Runtime.Serialization;

namespace GnsClassifier.Common
{
    [DataContract]
    public enum ClassifierResult
    {
        [EnumMember]
        Unknown =-1,
        [EnumMember]
        Unsafe =0,
        [EnumMember]
        Safe =1,
    }
}
