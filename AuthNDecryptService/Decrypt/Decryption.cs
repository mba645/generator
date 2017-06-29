using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Decrypt
{
    public sealed class Decryption
    {
        public static readonly string[] validChars = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        List<Task> taskList = new List<Task>();
        DocumentValidatorService.DocumentVerificationEndpointClient epClient = new DocumentValidatorService.DocumentVerificationEndpointClient();

        public void addTask(string filename, string filecontent)
        {
            taskList.Add(Task.Factory.StartNew(() => {
                GenerateOperation(string.Empty, 0, 3, filename, filecontent);
            }));
        }

        /// <summary>
        /// Goes through every key in a range
        /// </summary>
        /// <param name="prefix">if you have the start of the key set it here</param>
        /// <param name="level">key length to begin with ex : 2 would start with key "aa"</param>
        /// <param name="maxlength">maximum key length</param>
        /// <param name="document">encrypted document</param>
        public async void GenerateOperation(string prefix, int level, int maxlength, string filename, string fileContent)
        {
            level += 1;
            string key = "ety67";

            string output = JsonConvert.SerializeObject(XOR(fileContent, key));

            epClient.documentVerificationOperation(output, filename, key);

            //foreach (string c in validChars)
            //{
            //    string key = prefix + c + "67";

            //    //displays the generated key
            //    Console.WriteLine("key =  {0}", key);

            //    string output = JsonConvert.SerializeObject(XOR(fileContent, key));

            //    epClient.documentVerificationOperation(output, filename, key);

            //    if (level < maxlength) GenerateOperation(prefix + c, level, maxlength, filename, fileContent);
            //}
        }

        public string XOR(string msg, string key)
        {
            char[] output = new char[msg.Length];

            for (int c = 0; c < msg.Length; c++)
                output[c] = (char)(msg[c] ^ key[c % key.Length]);

            return new string(output);
        }
    }
}
