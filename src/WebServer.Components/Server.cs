using System.Net;
using System.Net.Sockets;

namespace WebServer.Components
{
    /// <summary>
    /// Basic Web Server
    /// </summary>
    public class Server
    {
        private HttpListener _listener = new();

        public Server(HttpListener? listener = null)
        {
            _listener = listener ?? new HttpListener();
            InitializeListener();
        }

        public List<string> GetServerAddresses()
        {
            return [.. _listener.Prefixes];
        }

        /// <summary>
        /// Returns list of IP addresses for every network interface for the current machine.
        /// It doesn't include the loopback IP.
        /// </summary>
        private static List<IPAddress> GetLocalHostIPs()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();
        }

        /// <summary>
        /// Initializes the listener to listen to all the network different interfaces
        /// </summary>
        private void InitializeListener()
        {
            var localhostIPs = GetLocalHostIPs() ?? [];

            // The local host ips doesn't include the loopback ip
            _listener.Prefixes.Add("http://localhost/");

            localhostIPs.ForEach(ip =>
            {
                // Console.WriteLine("Listening on IP " + "http://" + ip.ToString() + "/");
                _listener.Prefixes.Add("http://" + ip.ToString() + "/");
            });
        }
    }
}
