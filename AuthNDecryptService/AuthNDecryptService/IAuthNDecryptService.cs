using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AuthNDecryptService
{
    [ServiceContract]
    public interface IAuthNDecryptService
    {
        //[OperationContract]
        //string Authenticate(User user);

        [OperationContract]
        Task<string> AuthenticateAsync(User user);

        [OperationContract]
        User GetUser(int loginId);

        //[OperationContract]
        //Task<User> GetUserAsync(int loginId);

        [OperationContract]
        bool SendDocument(Document document, String tokenApp, String tokenUser);

        //[OperationContract]
        //Task<bool> SendDocumentAsync(Document document, String tokenApp, String tokenUser);
    }


    [DataContract]
    public class User
    {
        [DataMember]
        public int userId { get; set; }
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public string userPassword { get; set; }
        [DataMember]
        public string tokenApp { get; set; }
        [DataMember]
        public string tokenUser { get; set; }

    }

    [DataContract]
    public class Document
    {
        [DataMember]
        public string fileName { get; set; }
        [DataMember]
        public string fileContent { get; set; }
        [DataMember]
        public string decryptKey { get; set; }

    }
}
