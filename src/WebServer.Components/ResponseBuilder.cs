using System.Text;
using WebServer.Components.Constants;

namespace WebServer.Components
{
    class ResponseBuilder
    {
        public static Payload Build(RouterResponse routerResponse)
        {
            Payload payload = new();
            if (routerResponse.Headers["Content-Type"] == ContentType.Html)
            {
                string fileContent = File.ReadAllText(routerResponse.Content, Encoding.UTF8);
                payload.Data = Encoding.UTF8.GetBytes(fileContent);
                payload.StatusCode = 200;
                payload.ContentType = ContentType.Html + "; charset=utf-8";
            }

            else if (routerResponse.Headers["Content-Type"] == ContentType.Ico)
            {
                byte[] fileContent = File.ReadAllBytes(routerResponse.Content);
                payload.Data = fileContent;
                payload.StatusCode = 200;
                payload.ContentType = ContentType.Ico; 
            }



            return payload;
        }
    }

    class Payload
    {
        public byte[] Data { get; set; } = [];
        public string ContentType { get; set; } = WebServer.Components.Constants.ContentType.Html;
        public int StatusCode { get; set; }
    }
}
