using MassTransit;
using SendService.Core.Commands;

using Microsoft.EntityFrameworkCore;
using WebSystemOne.Data;
using WebSystemOne.Models;
using Microsoft.AspNetCore.Identity;
using WebSystemOne.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<FeedbackService>();
builder.Services.AddTransient<GettingRecordsService>();
builder.Services.AddDbContext<ApplicationDb>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUserModel>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDb>();

builder.Services.AddMassTransit(x =>
{
    x.AddRequestClient<FeedBackResponse>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");

        cfg.Message<FeedBackRequest>(x => x.SetEntityName("FeedBackConsumerQueue"));

        cfg.Message<GettingRecordsRequest>(x => x.SetEntityName("GettingRecordsConsumerQueue"));


        int portValue = rabbitMqConfig.GetValue<int>("Port");

        ushort port = Convert.ToUInt16(portValue);

        cfg.Host(rabbitMqConfig.GetValue<string>("Hostname"), port, "/", h =>
        {
            h.Username(rabbitMqConfig.GetValue<string>("Username"));
            h.Password(rabbitMqConfig.GetValue<string>("Password"));
        });
    });
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDb>();
        StatusSeedData.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при инициализации базы данных.");
    }
}


app.Run();
