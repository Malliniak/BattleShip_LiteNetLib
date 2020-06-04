using System;
using System.Threading;
using LiteNetLib;
using Serilog;

namespace BattleShipCoreServer.Core
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            Run();
        }
        
        static void Run()
        {
            Server listener = new Server();
            NetManager netManager = new NetManager(listener);
            netManager.Start(3000);
            netManager.DiscoveryEnabled = true;
            netManager.AutoRecycle = true;
            listener.NetManager = netManager;

            while (!Console.KeyAvailable)
            {
                netManager.PollEvents();
                Thread.Sleep(15);
            }
            
            netManager.Stop();
        }
    }
}