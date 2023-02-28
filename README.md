# Multiplayer Checkers Application with Authentication and Chat
A multiplayer checkers application built with ASP.NET Core 6, Entity Framework Core, MS SQL, SignalR, and Angular for the frontend. The application also includes user authentication and JWT bearer tokens for secure communication.

### Server
The server is built with ASP.NET Core 6 and Entity Framework Core, and uses MS SQL for database management. It implements the following features:

* User authentication with Identity library and JWT bearer tokens
* Checkers game logic and management, including capturing, turning into a king, and switching turns
* Chat functionality for players to communicate during the game
  
### Getting Started
1. Clone the repository to your local machine
2. Open the solution in Visual Studio
3. Restore the NuGet packages
4. Update the connection string in the appsettings.json file to point to your local MS SQL database - replace the connection with this: "Server=\<YourServer\>;Database=\<DataBaseNameYouWant\>;Trusted_Connection=True;TrustServerCertificate=True"
5. Run the migrations to create the database and tables
6. Run the application
  
### Prerequisites
* Visual Studio 2019 or later
* MS SQL Server
* .NET Core 6 SDK
  
### Built With
* ASP.NET Core 6
* Entity Framework Core
* MS SQL
* SignalR
