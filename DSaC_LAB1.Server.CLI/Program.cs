using DSaC_LAB1.Networking;
using DSaC_LAB1.Server.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;

namespace DSaC_LAB1.Server.CLI;

public class Program
{
    private const int PORT = 11000;

    public static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<ISocketServer, SocketServer>()
            .AddSingleton<INotificationServer, NotificationServer>()
            .BuildServiceProvider();

        Console.InputEncoding = Encoding.Unicode;
        Console.OutputEncoding = Encoding.Unicode;
        Console.WriteLine("Starting application");

        INotificationServer server = serviceProvider.GetService<INotificationServer>();
        server.Start(IPAddress.Any, PORT, 100);
        Console.WriteLine("Server is listening....");

        Console.ReadLine();
        serviceProvider.Dispose();
    }
}