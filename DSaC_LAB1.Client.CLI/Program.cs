using DSaC_LAB1.Networking;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace DSaC_LAB1.Server.CLI;

public class Program
{
    const string ipAddress = "192.168.31.8";
    const int port = 11000;

    public static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<ISocketClient, SocketClient>()
            .BuildServiceProvider();

        

        ISocketClient client = serviceProvider.GetService<ISocketClient>();
        
        client.DataReceived += OnReceivedMessage;
        await client.Connect(IPAddress.Parse(ipAddress), port);

        while (true)
        {
            string input = Console.ReadLine() ?? String.Empty;
            if (input.Equals("/exit"))
                break;
            else
            {
                int bytesSent = await client.Send(input);
            }
        }

        serviceProvider.Dispose();
    }

    private static void OnReceivedMessage(string data)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        if (Console.CursorLeft != 0)
        {
            Console.WriteLine();
        }
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write("Server:\n\t");
        Console.ForegroundColor = originalColor;
        Console.WriteLine(data);
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write("You:\n\t");
        Console.ForegroundColor = originalColor;
    }
}