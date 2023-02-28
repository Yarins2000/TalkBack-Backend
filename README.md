# TalkBack-Backend
The server side of TalkBack project. Using Asp.Net core 6, EF core and Identity library

# How to use
To use this project, you will need to update the connection string to match your own database. 

1. Open the `appsettings.json` file located in the project's root directory.
2. Locate the `"ConnectionStrings"` section.
3. Update the `"DefaultConnection"` value with your own connection string - replace the connection with this: "Server=<YourServer>;Database=<DataBaseNameYouWant>;Trusted_Connection=True;TrustServerCertificate=True".
4. Save the file and run the project.
