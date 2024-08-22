using System.Net;
using System.Reflection;
using WebServer.Components.Constants;

namespace WebServer.Components
{
    internal class Router
    {
        private string _websitePath { get; set; }

        public Router(string? websitePath = null)
        {
            _websitePath = websitePath ?? AppDomain.CurrentDomain.BaseDirectory;
        }

        public void StartRouting(HttpListenerRequest request)
        {
            string verb = request.HttpMethod;
                          
            string path = request.RawUrl ?? "";

            int beginingOfQueryStrings = path.IndexOf('?');
            string queryStrings = beginingOfQueryStrings < 0 ? "" : path[(beginingOfQueryStrings + 1)..];

            if (beginingOfQueryStrings > 0 && beginingOfQueryStrings < path.Length)
            {
                path = path[..beginingOfQueryStrings];
            }

            Dictionary<string, string> queryParameters = ExtractQueryStrings(queryStrings);
        }

        /// <summary>
        /// Returns the query strings as a dictionary
        /// </summary>
        /// <param name="queryStrings"></param>
        /// <returns></returns>
        private static Dictionary<string, string> ExtractQueryStrings(string queryStrings)
        {
            Dictionary<string, string> queryParameters = [];

            var queryPairs = queryStrings.Split('&', StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in queryPairs)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0];
                    string value = keyValue[1];
                    queryParameters[key] = value;
                }
                else if (keyValue.Length == 1)
                {
                    // Handle cases where there might be a key without a value
                    string key = keyValue[0];
                    queryParameters[key] = string.Empty;
                }
            }

            return queryParameters;
        }

    }
    public class RouterResponse
    {
        /// <summary>
        /// Default to 200 OK
        /// </summary>
        public int StatusCode { get; set; } = 200; 
        
        /// <summary>
        /// Response headers added by the router
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = [];
        
        /// <summary>
        /// Could be a file path or text content
        /// </summary>
        public string Content { get; set; } = "";

        /// <summary>
        /// True if Content is a file path
        /// </summary>
        public bool IsFile { get; set; }

    }

    
}
