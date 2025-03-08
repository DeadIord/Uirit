using MassTransit;
using MessageApi.Data;
using MessageApi.Rabbit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<FeedBackHandler>();

    config.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");

        cfg.ReceiveEndpoint("GettingDocumentConsumerQueue", x =>
        {
            x.ConfigureConsumer<GettingDocumentHandler>(context);
            x.ConfigureConsumeTopology = false;
            x.Bind("GettingDocumentConsumerQueue");
        });
        cfg.ReceiveEndpoint("FeedBackConsumerQueue", x =>
        {
            x.ConfigureConsumer<FeedBackHandler>(context);
            x.ConfigureConsumeTopology = false;
            x.Bind("FeedBackConsumerQueue");
        });

        cfg.Host(rabbitMqConfig.GetValue<string>("Hostname"), rabbitMqConfig.GetValue<ushort>("Port"), "/", h =>
        {
            h.Username(rabbitMqConfig.GetValue<string>("Username"));
            h.Password(rabbitMqConfig.GetValue<string>("Password"));
        });
    });

});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
