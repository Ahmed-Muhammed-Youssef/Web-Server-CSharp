using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WebServer.Components
{
    /// <summary>
    /// Basic Web Server
    /// </summary>
    public class Server
    {
        private HttpListener _listener = new();
        private Semaphore sem;

        public Server(int portNumber = 5000, int maxSimultaneousConnections = 20, HttpListener? listener = null)
        {
            _listener = listener ?? new HttpListener();

            InitializeListener(portNumber);
            sem = new(maxSimultaneousConnections, maxSimultaneousConnections);
        }

        /// <summary>
        /// Starts the web server.
        /// </summary>
        public Task StartAsync()
        {
            _listener.Start();
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

            SendResponse(context);
        }

        private void SendResponse(HttpListenerContext context)
        {
            // this code is a place holder
            string response = "Hello Browser!";
            byte[] encoded = Encoding.UTF8.GetBytes(response);
            context.Response.ContentLength64 = encoded.Length;
            context.Response.OutputStream.Write(encoded, 0, encoded.Length);
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
    }
}
