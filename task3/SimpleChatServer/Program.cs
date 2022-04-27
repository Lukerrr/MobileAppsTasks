using System;
using System.Threading;
using Grpc.Core;
using SimpleChatApp.GrpcService;
using SimpleChatApp.Server;

namespace SimpleChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server...");
            RunServer("192.168.0.104", 30051);
        }

        static private void RunServer(string ip, int port)
        {
            ChatServerModel serverModel = new ChatServerModel();
            Server server = new Server
            {
                Services =
                {
                    ChatService.BindService(new GrpcChatService(serverModel))
                },
                Ports =
                {
                    new ServerPort(ip, port, ServerCredentials.Insecure)
                }
            };

            try
            {
                server.Start();
                Console.WriteLine($"Server is listening on {ip}:{port}");
                while (true) Thread.Sleep(1000);
            }
            finally
            {
                serverModel.ClearAllConnections().WaitAsync(new TimeSpan(200000000L));
                try
                {
                    server.ShutdownAsync().WaitAsync(new TimeSpan(200000000L));
                }
                catch (Exception value)
                {
                    Console.WriteLine(value);
                }

                serverModel.Dispose();
                Console.WriteLine("Server closed");
            }
        }
    }
}
