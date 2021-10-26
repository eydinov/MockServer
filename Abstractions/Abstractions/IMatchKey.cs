using System.Text.RegularExpressions;

namespace MockServer.Environment.Abstractions
{
    public interface IMatchKey
    {
        string Method { get; set; }
        string Path { get; set; }
        public Regex Regex { get; set; }
        bool IsMatch(string method, string path);
    }
}
