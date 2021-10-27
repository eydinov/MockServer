namespace MockServer.Environment
{
    public class Request : PropsBasedObject
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public string PathRegex { get; set; }
        public Authorization[] Authorization { get; set; }

    }
}
