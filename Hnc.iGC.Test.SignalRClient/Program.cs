using Microsoft.AspNetCore.SignalR.Client;

using System;
using System.Threading.Tasks;

namespace Hnc.iGC.Test.SignalRClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://127.0.0.1:24912/controlhub")
                .WithAutomaticReconnect()
                .Build();

            connection.On<string>("MessageInfo", message => { Console.WriteLine(message); });
            connection.On<string>("MessageWarning", message => { Console.WriteLine(message); });
            connection.On<string>("MessageError", message => { Console.WriteLine(message); });
            connection.On<string>("MessageSuccess", message => { Console.WriteLine(message); });
            connection.On<string, string>("RegisterValueChanged", (deviceId, value) => { Console.WriteLine(value); });

            try
            {
                connection.StartAsync().Wait();
                Console.WriteLine("Connection started");
                connection.InvokeAsync("KeyByte", new { BYTE1 = 0x00, BYTE2 = 0x14 }).Wait();
                var result = connection.InvokeAsync<int>("GetRegister", new { DeviceId = "1A5FAAC3FFF96DB", GetRegister = "D10" }).Result;
                Task.Delay(2000).Wait();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey(true);
                connection.StopAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
