# Multiplayer Checkers Application with Authentication and Chat
A multiplayer checkers application built with ASP.NET Core 6, Entity Framework Core, MS SQL, SignalR, and Angular for the frontend. The application also includes user authentication and JWT bearer tokens for secure communication.

### Server
The server is built with ASP.NET Core 6 and Entity Framework Core, and uses MS SQL for database management (there is also an option to work with sqlite). It implements the following features:

* User authentication with Identity library and JWT bearer tokens
* Checkers game logic and management with signalR
* Chat functionality for users to communicate via signalR

### Prerequisites
* Visual Studio 2019 or later
* MS SQL Server (optional)
* .NET Core 6 SDK
  
### Getting Started
1. Clone the repository to your local machine
2. Open the solution in Visual Studio
3. Restore the NuGet packages (build the projects)
4. Update the connection string in the appsettings.json file to point to your local MS SQL database - replace the connection with this line: 
`Server=<YourServer>;Database=<DataBaseNameYouWant>;Trusted_Connection=True;TrustServerCertificate=True`
5. Run the application

#### For SqLite
* Instead of doing step 4 (changing the connection string), in program.cs comment out the line: `builder.Services.AddDbContext...UseSqlServer...`(disable the usage of sqlServer)
* uncomment the line: `builder.Services.AddDbContext...UseSqlite...`(enable the usage of sqlite)
* If you want to specify the name and location of the db file, in appsettings.json change SqLiteConnection to `Data Source=<FileLocation>/<DbFileName>.db`
  
### Built With
* ASP.NET Core 6
* Entity Framework Core
* MS SQL
* SignalR
* JWT bearer

### Frontend Project
<https://github.com/Yarins2000/TalkBack-Frontend>
