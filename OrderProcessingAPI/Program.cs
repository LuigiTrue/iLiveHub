using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Text.Json;
using SenderService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<Sender>();

var app = builder.Build();

// Obtém o serviço RabbitMQ
var rabbitMqService = app.Services.GetRequiredService<Sender>();

// Lê o arquivo JSON
var jsonFilePath = app.Configuration["JsonFilePath"];
var jsonContent = await File.ReadAllTextAsync(jsonFilePath);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.MapGet("/send", async (Sender senderService) =>
{
    await senderService.Send(jsonContent);
    return Results.Ok("Mensagem enviada para a fila!");
})
.WithName("SendMessage");

app.Run();
