using System;
using System.Threading.Tasks;

namespace TcpClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("TCP Client Starting...");

            Console.WriteLine("Enter server IP (press Enter for localhost):");
            string serverIp = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(serverIp))
                serverIp = "127.0.0.1";

            Console.WriteLine("Enter server port:");
            int port = int.Parse(Console.ReadLine());

            Client client = new Client(serverIp, port);

            while (true)
            {
                Console.WriteLine("\nEnter request (e.g., SetAOne) or 'exit' to quit:");
                string request = Console.ReadLine();

                if (request?.ToLower() == "exit") break;

                await client.SendRequestAsync(request);
            }
        }
    }
}
