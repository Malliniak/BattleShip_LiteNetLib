# BattleShip_LiteNetLib
WIP prototype to create BattleShip hame with custom C# server with LiteNetLib.

Folder BattleShipCoreServer is a LiteNetLib custom build server without any middleware. In order to test it, run it in your IDE or on a dedicated server, i have tested only Linux maachines.

In order to change server IP Adress inside Unity project navigate to NetHub.cs class. 

```cs
            _connectionManager.Connect("localhost", 3000, NETHUB_ACCESS_TOKEN);
```

This line - 39 - takes a desired server connection parameters.
