using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace AuthNDecryptService
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" dans le code, le fichier svc et le fichier de configuration.
    // REMARQUE : pour lancer le client test WCF afin de tester ce service, sélectionnez Service1.svc ou Service1.svc.cs dans l'Explorateur de solutions et démarrez le débogage.
    public class AuthNDecrypt : IAuthNDecrypt
    {
        private string tokenApp = "KwdcVAQVK0KzabAM62qp0g==";
        private User user = new User();
        DocumentValidatorService.DocumentVerificationEndpointClient epClient = new DocumentValidatorService.DocumentVerificationEndpointClient();

        public string Authenticate(User user)
        {
            bool isAllowed = false;

            using (var cnn = new MySqlConnection(Properties.Settings.Default.DBConnectionString))
            {
                using (var cmd = new MySqlCommand(
                    "SELECT id_user, username, password FROM user ", cnn))
                {
                    cnn.Open();
                    using (MySqlDataReader dataReader = cmd.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                for (int i = 0; i < dataReader.FieldCount; i = i + 3)
                                {
                                    if (user.username == dataReader.GetString(i + 1) && user.userPassword == dataReader.GetString(i + 2))
                                    {
                                        isAllowed = true;
                                        user.userId = Convert.ToInt32(dataReader.GetString(i));
                                        user.tokenUser = Tokenify(user);
                                        this.user = user;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (isAllowed)
                {
                    int rowsChanged = 0;
                    using (var cmd = new MySqlCommand(
                        "UPDATE user " +
                        "SET token_user = @tokenUser " +
                        "WHERE id_user = @userId", cnn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@tokenUser", user.tokenUser));
                        cmd.Parameters.Add(new MySqlParameter("@userId", user.userId));
                        rowsChanged = (int)cmd.ExecuteNonQuery();
                        if (rowsChanged != 0)
                        {
                            return user.tokenUser;
                        }
                    }
                }
            }

            return ("false");
        }

        public async Task<string> AuthenticateAsync(User user)
        {
            return await Task.Run(() => Authenticate(user));
        }

        public User GetUser(int loginId)
        {
            User user = new User();
            using (var cnn = new MySqlConnection(Properties.Settings.Default.DBConnectionString))
            {
                using (var cmd = new MySqlCommand(
                    "SELECT username, password, token_user FROM user " +
                    "WHERE id_user = @loginId", cnn))
                {
                    cmd.Parameters.Add(new MySqlParameter("@loginId", loginId));

                    cnn.Open();
                    using (MySqlDataReader dataReader = cmd.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                user.username = dataReader.GetString(0);
                                user.userPassword = dataReader.GetString(1);
                                user.tokenUser = dataReader.GetString(2);
                            }
                        }
                    }
                }
            }
            return user;
        }

        public async Task<User> GetUserAsync(int loginId)
        {
            return await Task.Run(() => GetUser(loginId));
        }

        public bool SendDocument(Document document)
        {
            epClient.documentVerificationOperation(document.fileContent, document.filename, document.decryptKey);
            return true;
            //if (this.user != new User() && tokenApp == this.tokenApp && tokenUser == GetUser(user.userId).tokenUser)
            //{

            //    epClient.documentVerificationOperation(document.fileContent, document.fileName, document.decryptKey);
            //    return true;
            //}
            //return false;

        }

        public async Task<bool> SendDocumentAsync(Document document)
        {
            return await Task.Run(() => SendDocument(document));
        }

        public string UploadDocument(string filename, string fileContent, string tokenApp, string tokenUser)
        {
            if (this.user != new User())
            {
                if (tokenApp == this.tokenApp && tokenUser == GetUser(user.userId).tokenUser)
                {
                    Document document = new Document();
                    document.filename = filename;
                    document.fileContent = fileContent;
                    return "Done";
                }
                else
                {
                    return "False tokenuser or tokenapp";
                }
            }
            return "No user logged";

        }

        public async Task<string> UploadDocumentAsync(string filename, string fileContent, string tokenApp, string tokenUser)
        {
            return await Task.Run(() => UploadDocument(filename, fileContent, tokenApp, tokenUser));
        }

        public string Tokenify(User user)
        {
            user.tokenUser += user.username.GetHashCode();
            user.tokenUser += user.userPassword.GetHashCode();
            user.tokenUser += user.tokenApp.GetHashCode();
            user.tokenUser += DateTime.Now.GetHashCode();
            user.tokenUser = user.tokenUser.GetHashCode().ToString();

            return user.tokenUser;
        }
    }
}
