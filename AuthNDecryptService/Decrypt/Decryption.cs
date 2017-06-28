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

        public void addTask(CancellationToken ct)
        {
            taskList.Add(Task.Factory.StartNew(() => {
                GenerateOperation(string.Empty, 0, 6, "0d1d050d0a");
            }, ct));
        }

        /// <summary>
        /// Goes through every key in a range
        /// </summary>
        /// <param name="prefix">if you have the start of the key set it here</param>
        /// <param name="level">key length to begin with ex : 2 would start with key "aa"</param>
        /// <param name="maxlength">maximum key length</param>
        /// <param name="msg">encryptedMsg</param>
        public string GenerateOperation(string prefix, int level, int maxlength, string msg)
        {
            StringBuilder msgBytes = new StringBuilder();
            level += 1;

            foreach (char c in msg)
            {
                msgBytes.Append(string.Join(" ", Encoding.ASCII.GetBytes(c.ToString()).Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0'))));
            }

            foreach (string c in validChars)
            {
                string key = prefix + c;
                //displays the generated key
                Console.WriteLine("key =  {0}", key);

                DecryptMsg(msgBytes.ToString(), key);

                if (level < maxlength) GenerateOperation(prefix + c, level, maxlength, msg);
            }
        }

        private string DecryptMsg(string msg, string key)
        {
            StringBuilder result = new StringBuilder();
            StringBuilder keyBytes = new StringBuilder();

            foreach(char c in key)
            {
                keyBytes.Append(string.Join(" ", Encoding.ASCII.GetBytes(c.ToString()).Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0'))));
            }

            result.Append(XOR(msg, keyBytes, 0));

            char[] reverse = result.ToString().ToCharArray();
            Array.Reverse(reverse);

            result = new StringBuilder();
            result.Append(reverse);

            return result.ToString();
        }

        private string XOR(string msg, StringBuilder key, int level)
        {
            StringBuilder result = new StringBuilder();

            if(level < msg.Length-1)
            {
                result.Append(XOR(msg, key, level + 1));
            }
            
            return result.Append(msg[level] ^ key[level % key.Length]).ToString();
        }
    }
}
