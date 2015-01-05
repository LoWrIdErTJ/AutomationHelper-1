using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace AutomationHelper
{
    public class TextHelper
    {
        private static readonly Random Rng = new Random();
        private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static Regex isGuid = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled);

        /// <summary>
        /// Generate random string
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string RandomString(int size)
        {
            var buffer = new char[size];

            for (var i = 0; i < size; i++)
            {
                buffer[i] = Chars[Rng.Next(Chars.Length)];
            }
            return new string(buffer);
        }

        /// <summary>
        /// verify if the string is an int
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt(string str)
        {
            var regex = new Regex("^[0-9]*[1-9][0-9]*$");
            return regex.IsMatch(str.Trim());
        }

        /// <summary>
        /// verify if the string is numeric
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumeric(string str)
        {
            var reg1 = new Regex(@"^[-]?\d+[.]?\d*$");
            return reg1.IsMatch(str);
        }
        
        /// <summary>
        /// verify if the string is GUID
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsGuid(string str)
        {
            var isValid = false;
            if (str != null)
            {
                if (isGuid.IsMatch(str))
                {
                    isValid = true;
                }
            }
            return isValid;
        }

        public static Dictionary<string, int> GetKeywords(string str)
        {
            Dictionary<string, int> keywords = new Dictionary<string, int>();

            //Replace all return to space
            str = str.Replace("\r", " ").Replace("\n", " ");

            // Seperate to sentence.
            string[] sentences = str.Split(new char[] { ',', '.', ':', '\r', '\n' });

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"D:\newsresult.txt"))
            {
                foreach (string s in sentences)
                {
                    string newsentence = null;
                        
                    // Cleanup sentence
                    MatchCollection mCollection = Regex.Matches(s, @"\w(?:[-\w]*\w)?", RegexOptions.IgnoreCase);
                    foreach(Match m in mCollection)
                    {
                        newsentence = string.Format("{0} {1}", newsentence, m.Groups[0].Value);
                    }

                    // Seperate words by stop word
                    if (!Regex.IsMatch(newsentence, ""))
                    {
                        foreach (string c in newsentence.Trim().Split(new char[] { ' ' }))
                        {
                            if (keywords.ContainsKey(c))
                                keywords[c]++;
                            else
                                keywords.Add(c, 1);
                        }
                    }
                    else
                    {

                    }
                }

                foreach(KeyValuePair<string, int> kv in keywords)
                {
                    sw.WriteLine(string.Format("{0} {1}", kv.Value, kv.Key));
                }
                
                sw.Flush();
                sw.Close();
            }


            return keywords;
        }
    }
}
