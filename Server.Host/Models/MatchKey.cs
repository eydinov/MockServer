using MockServer.Environment;
using MockServer.Environment.Abstractions;
using System;
using System.Text.RegularExpressions;

namespace ServerHost.Models
{
    public class MatchKey : IMatchKey
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public Regex Regex { get; set; }

        /// <summary>
        /// Check if HttpContext method and path match with current Key
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool IsMatch(string method, string path)
        {
            return Method == method && ((Path != null && Path.Equals(path, StringComparison.InvariantCultureIgnoreCase)) || (Regex != null && Regex.IsMatch(path)));
        }
    }
}
