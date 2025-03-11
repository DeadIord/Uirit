using MassTransit;
using Microsoft.EntityFrameworkCore;
using SendService.Core.Commands;
using WebSystemOne.Controllers;
using WebSystemTwo.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<testService>();

builder.Services.AddMassTransit(x =>
{
    // Регистрация клиента запросов с указанием корректного типа
    x.AddRequestClient<GettingDocumentRequset>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");

        cfg.Message<GettingDocumentRequset>(x => x.SetEntityName("GettingDocumentConsumerQueue"));

        int portValue = rabbitMqConfig.GetValue<int>("Port");
        ushort port = Convert.ToUInt16(portValue);

        cfg.Host(rabbitMqConfig.GetValue<string>("Hostname"), port, "/", h =>
        {
            h.Username(rabbitMqConfig.GetValue<string>("Username"));
            h.Password(rabbitMqConfig.GetValue<string>("Password"));
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        StatusSeedData.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при инициализации базы данных.");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();