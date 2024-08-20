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
            Server server = new(6000);
            var expected = new List<string>
            {
                "http://localhost:6000/"
            };

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