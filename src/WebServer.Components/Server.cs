using System.Diagnostics.Metrics;
using System.Net;
using System.Reflection;
using System.Text;

namespace WebServer.Components
{
    /// <summary>
    /// Basic Web Server
    /// </summary>
    public class Server
    {
        private HttpListener _listener = new();
        private readonly TextWriter _loggerDestination;
        private Semaphore sem;
        private Router router = new Router();

        public Server(int portNumber = 5000, int maxSimultaneousConnections = 20, HttpListener? listener = null, TextWriter? loggerDestination = null)
        {
            _listener = listener ?? new HttpListener();
            this._loggerDestination = loggerDestination ?? Console.Out;
            sem = new(maxSimultaneousConnections, maxSimultaneousConnections);

            InitializeListener(portNumber);
        }

        /// <summary>
        /// Starts the web server.
        /// </summary>
        public Task StartAsync()
        {
            _listener.Start();

            var ips = GetServerAddresses();
            foreach (var ip in ips)
            {
                _loggerDestination.WriteLine("Listening on: " + ip);
            }

            return Task.Run(() => RunServer(_listener));
        }

        /// <summary>
        /// Start awaiting for connections, up to the "maxSimultaneousConnections" value.
        /// This code runs in a separate thread.
        /// </summary>
        private void RunServer(HttpListener listener)
        {
            while (true)
            {
                sem.WaitOne();
                StartConnectionListener(listener);
            }
        }

        /// <summary>
        /// Await connections.
        /// </summary>
        private async void StartConnectionListener(HttpListener listener)
        {
            // Wait for a connection. Return to caller while we wait.
            HttpListenerContext context = await listener.GetContextAsync();

            // Release the semaphore so that another listener can be immediately started up.
            sem.Release();

            LogRequest(context.Request);

            var route = router.StartRouting(context.Request);

            var payLoad = ResponseBuilder.Build(route);

            SendResponse(context, payLoad);
        }

       
        private void SendResponse(HttpListenerContext context, Payload payload)
        {
            context.Response.ContentLength64 = payload.Data.Length;
            context.Response.ContentType = payload.ContentType;
            context.Response.OutputStream.Write(payload.Data, 0, payload.Data.Length);
            context.Response.OutputStream.Close();

        }

        public List<string> GetServerAddresses()
        {
            return [.. _listener.Prefixes];
        }

        /// <summary>
        /// Initializes the listener to listen to all the network different interfaces
        /// </summary>
        private void InitializeListener(int portNumber)
        {
            _listener.Prefixes.Add("http://localhost:" + portNumber.ToString() + "/");
        }

        /// <summary>
        /// Log requests.
        /// </summary>
        public void LogRequest(HttpListenerRequest request)
        {
            var sb = new StringBuilder();

            sb.AppendLine("HTTP Request Log:");
            sb.AppendLine($"Timestamp: {DateTime.Now}");
            sb.AppendLine($"Method: {request.HttpMethod}");
            sb.AppendLine($"URL: {request.Url}");
            sb.AppendLine($"Client IP: {request.RemoteEndPoint?.Address}");

            // Log query string parameters
            if (request.QueryString.Count > 0)
            {
                sb.AppendLine("Query String Parameters:");
                foreach (string? key in request.QueryString.AllKeys)
                {
                    sb.AppendLine($"    {key}: {request.QueryString[key]}");
                }
            }

            // Log headers
            sb.AppendLine("Headers:");
            foreach (string key in request.Headers.AllKeys)
            {
                sb.AppendLine($"    {key}: {request.Headers[key]}");
            }

            // Optionally log request body if it's not a GET request and has content
            if (request.HttpMethod != "GET" && request.HasEntityBody)
            {
                sb.AppendLine("Body:");
                using (var bodyReader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    sb.AppendLine(bodyReader.ReadToEnd());
                }
            }

            sb.AppendLine("End of Request Log");
            sb.AppendLine(new string('-', 50));

            // Write the log to the configured writer
            Log(sb.ToString());
        }

        private void Log(string info)
        {
            _loggerDestination.WriteLine(info);
        }

        public static string GetWebsitePath()
        {
            // Path of our exe.
            string websitePath = Assembly.GetExecutingAssembly().Location + "\\Website";
            // websitePath = websitePath.LeftOfRightmostOf("\\").LeftOfRightmostOf("\\").LeftOfRightmostOf("\\") ;

            return websitePath;
        }

    }
}
