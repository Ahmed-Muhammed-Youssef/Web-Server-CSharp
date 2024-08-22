using WebServer.Components;

namespace WebServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new();

            Task t = server.StartAsync();
            
            Console.ReadLine();
        }
    }
}
