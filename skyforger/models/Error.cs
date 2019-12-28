using System.Collections.Generic;

namespace skyforger.models
{
    public class Error
    {
        public Error(dynamic type, string uri)
        {
            Type = type;
            Uri = uri;
            List = new List<string>();
        }
        public string Type { get; set; }
        public string Uri { get; set; }
        public List<string> List { get; set; }
    }
}