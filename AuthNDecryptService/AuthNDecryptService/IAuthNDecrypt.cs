﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace AuthNDecryptService
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom d'interface "IService1" à la fois dans le code et le fichier de configuration.
    [ServiceContract]
    public interface IAuthNDecrypt
    {
        [OperationContract]
        Task<User> AuthenticateAsync(User user);

        [OperationContract]
        Task<User> GetUserAsync(int loginId);

        [OperationContract]
        Task<bool> SendDocumentAsync(Decrypt.AuthNDecryptService.Document document, string decryptedContent, string key);

        [OperationContract]
        Task<string> UploadDocumentAsync(string filename, string fileContent, User user);
    }


    [DataContract]
    public class User
    {
        [DataMember]
        public bool isValid { get; set; }
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
        public string filename { get; set; }
        [DataMember]
        public string fileContent { get; set; }
        [DataMember]
        public string decryptKey { get; set; }

    }
}
