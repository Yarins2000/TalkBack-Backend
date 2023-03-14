using TalkBack.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TalkBack.Models;
using TalkBack.JwtService;
using TalkBack.API.JwtServices;
using TalkBack.IdentityServices;
using TalkBack.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IJWTTokenGenerator, JWTTokenGenerator>();

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    //options.KeepAliveInterval = TimeSpan.FromMinutes(10);
    //options.ClientTimeoutInterval = TimeSpan.FromMinutes(20);
});

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    //policy.WithOrigins("http://localhost:4200/").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddScoped<IIdentityService, IdentityService>();

builder.Services.AddDbContext<ContactsDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"),
                                                            b => b.MigrationsAssembly("TalkBack.Data")));

/*builder.Services.AddDbContext<ContactsDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("SqLiteConnection"),
                                                            b => b.MigrationsAssembly("TalkBack.Data")));*/
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<ContactsDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
});

builder.Services.AddJwtAuthentication(builder.Configuration);
var app = builder.Build();

/*using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ContactsDbContext>();
        if (dbContext.Database.EnsureCreated())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            if (!dbContext.Users.Any(u => u.UserName == "user1"))
            {
                var user1 = new IdentityUser()
                {
                    UserName = "user1"
                };
                await userManager.CreateAsync(user1, "User123@");
            }

            if (!dbContext.Users.Any(u => u.UserName == "user2"))
            {
                var user2 = new IdentityUser()
                {
                    UserName = "user2"
                };
                await userManager.CreateAsync(user2, "User123@");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the SQLite database.");
    }
}*/

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<ContactsDbContext>();

    if (ctx.Database.EnsureCreated())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        if (!ctx.Users.Any(u => u.UserName == "user1"))
        {
            var user1 = new IdentityUser()
            {
                UserName = "user1"
            };
            await userManager.CreateAsync(user1, "User123@");
        }

        if (!ctx.Users.Any(u => u.UserName == "user2"))
        {
            var user2 = new IdentityUser()
            {
                UserName = "user2"
            };
            await userManager.CreateAsync(user2, "User123@");
        }
    }
}

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", context =>
    {
        context.Response.Redirect("swagger");
        return Task.CompletedTask;
    });
    endpoints.MapHub<ContactsHub>("hubs/Contacts");
    endpoints.MapHub<ChatHub>("hubs/Chat");
    endpoints.MapHub<GameHub>("hubs/Game");
    endpoints.MapControllers();
});

app.Run();

//Add-Migration InitialCreate -Project Data -StartupProject TalkBack.ContactsDB -Context Data.ContactsDbContext