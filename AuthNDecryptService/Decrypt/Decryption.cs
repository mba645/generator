using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Decrypt.AuthNDecryptService;

namespace Decrypt
{
    public sealed class Decryption
    {
        public static readonly string[] validChars = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        List<Task> taskList = new List<Task>();
        AuthNDecryptClient client = new AuthNDecryptClient();

        public void addTask(CancellationToken ct, Document document)
        {
            taskList.Add(Task.Factory.StartNew(() => {
                GenerateOperation(string.Empty, 0, 6, document);
            }, ct));
        }

        /// <summary>
        /// Goes through every key in a range
        /// </summary>
        /// <param name="prefix">if you have the start of the key set it here</param>
        /// <param name="level">key length to begin with ex : 2 would start with key "aa"</param>
        /// <param name="maxlength">maximum key length</param>
        /// <param name="document">encrypted document</param>
        public async void GenerateOperation(string prefix, int level, int maxlength, Document document)
        {
            level += 1;
            string test = "void";
            

            foreach (string c in validChars)
            {
                string key = prefix + c;
                
                //displays the generated key
                Console.WriteLine("key =  {0}", key);

                await client.SendDocumentAsync(document, XOR(document.fileContent, key), key);

                if (level < maxlength) GenerateOperation(prefix + c, level, maxlength, document);
            }
        }

        public string XOR(string msg, string key)
        {
            var result = new StringBuilder();

            for (int c = 0; c < msg.Length; c++)
                result.Append((char)((uint)msg[c] ^ (uint)key[c % key.Length]));

            return result.ToString();
        }
    }
}
