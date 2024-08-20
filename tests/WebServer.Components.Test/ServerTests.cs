using System.Net;
using System.Net.Sockets;

namespace WebServer.Components.Test
{
    public class ServerTests
    {
        [Fact]
        public void InitializeListener_ShouldAddNetworkIPs()
        {
            // Arrange
            Server server = new();
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            var expected = host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).Select(ip => "http://" + ip.ToString() + "/").ToList();
            expected.Add("http://localhost/");

            var actualOutPut = server.GetServerAddresses();

            // Act
            // The constructor of `Server` class calls `InitializeListener` automatically.

            // Assert
            Assert.Equal(expected.Count, actualOutPut.Count);

            foreach (var ip in actualOutPut)
            {
                Assert.Contains(ip, expected);
            }

        }
    }
}