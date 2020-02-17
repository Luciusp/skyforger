using System.Collections.Generic;

namespace skyforger.models
{
    public class Error
    {
        public Error(string type, string uri, List<string> errors)
        {
            Type = type;
            Uri = uri;
            Errors = errors;
        }
        public string Type { get; set; }
        public string Uri { get; set; }
        public List<string> Errors { get; set; }
    }
}