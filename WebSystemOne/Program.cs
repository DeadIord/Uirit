using MassTransit;
using SendService.Core.Commands;

using Microsoft.EntityFrameworkCore;
using WebSystemOne.Data;
using WebSystemOne.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

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

        int portValue = rabbitMqConfig.GetValue<int>("Port");

        // �������������� � ushort
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

app.Run();
