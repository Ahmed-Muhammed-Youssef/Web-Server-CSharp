using WebServer.Components;

namespace WebServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new();

            var ips = server.GetServerAddresses();

            foreach (var ip in ips) {
                Console.WriteLine("Listening on IP " + ip);
            }

        }
    }
}
