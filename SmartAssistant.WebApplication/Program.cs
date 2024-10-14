using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartAssistant.WebApplication.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using SmartAssistant.Shared.Repositories;
using SmartAssistant.Shared.Services;
using SmartAssistant.WebApp.Data.Entities;
using SmartAssistant.Shared.Mapping;
using SmartAssistant.Shared;
using SmartAssistant.Shared.Hubs;
using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Repositories.Event;
using SmartAssistant.Shared.Services.Event;
using SmartAssistant.Shared.Services.CleanUps;
using SmartAssistant.Shared.Interfaces.Reminder;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Interfaces.Team;
using SmartAssistant.Shared.Interfaces.User;
using SmartAssistant.Shared.Repositories.Team;
using SmartAssistant.Shared.Repositories.User;
using SmartAssistant.Shared.Services.Teams;
using SmartAssistant.Shared.Services.User;
using SmartAssistant.Shared.Services.Speech;

var builder = WebApplication.CreateBuilder(args);

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"C:\Users\rauga\OneDrive\Работен плот\The new project\SmartAssistant.WebApplication\SmartAssistant.WebApplication\Config\smartassistant-credentials.json");
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

builder.Services.AddHostedService<TaskReminderCleanupService>();

builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IUserRepository, UserRepository>(); 
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();

builder.Services.AddHostedService<EventCleanupService>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<GoogleSpeechService>();


builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNamingPolicy = null; // Keep property names as-is
});


builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserResolver>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notificationHub");
    endpoints.MapHub<ChatHub>("/chatHub");
});

app.Run();
