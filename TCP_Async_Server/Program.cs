using System.Net;

namespace TCP_Async_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerFactorialAsync server = new ServerFactorialAsync(IPAddress.Any, 2048);
            server.Start();
        }
    }
}
