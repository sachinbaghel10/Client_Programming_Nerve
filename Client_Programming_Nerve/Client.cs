// Client.cs
using System;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace TcpClient
{
    public class Client
    {
        private readonly string _serverIp;
        private readonly int _port;
        private readonly byte[] _key = Encoding.UTF8.GetBytes("YourSecretKey123");
        private readonly byte[] _iv = Encoding.UTF8.GetBytes("YourSecretIV1234");

        public Client(string serverIp, int port)
        {
            _serverIp = serverIp;
            _port = port;
        }

        public async Task SendRequestAsync(string request)
        {
            try
            {
                using (System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient())
                {
                    await client.ConnectAsync(_serverIp, _port);
                    Console.WriteLine("Connected to server!");

                    using (NetworkStream stream = client.GetStream())
                    {
                        string encryptedRequest = Encrypt(request);
                        byte[] requestBytes = Encoding.UTF8.GetBytes(encryptedRequest);
                        await stream.WriteAsync(requestBytes, 0, requestBytes.Length);
                        Console.WriteLine($"Sent request: {request}");

                        byte[] buffer = new byte[1024];
                        while (true)
                        {
                            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                            if (bytesRead == 0) break;

                            string encryptedResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            string response = Decrypt(encryptedResponse);
                            Console.WriteLine($"{response}");

                            if (response == "EMPTY") break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex.Message}");
            }
        }

        private string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                ICryptoTransform encryptor = aes.CreateEncryptor();
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                return Convert.ToBase64String(encryptedBytes);
            }
        }

        private string Decrypt(string cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                ICryptoTransform decryptor = aes.CreateDecryptor();
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}

