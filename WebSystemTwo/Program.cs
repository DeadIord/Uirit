using MassTransit;
using SendService.Core.Commands;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
builder.Services.AddMassTransit(x =>
{
    x.AddRequestClient<GettingDocumentResponse>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");

        cfg.Message<GettingDocumentRequset>(x => x.SetEntityName("GettingDocumentConsumerQueue"));

        int portValue = rabbitMqConfig.GetValue<int>("Port");

        // Преобразование в ushort
        ushort port = Convert.ToUInt16(portValue);

        cfg.Host(rabbitMqConfig.GetValue<string>("Hostname"), port, "/", h =>
        {
            h.Username(rabbitMqConfig.GetValue<string>("Username"));
            h.Password(rabbitMqConfig.GetValue<string>("Password"));
        });
    });
});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
