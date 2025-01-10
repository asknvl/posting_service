
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using posting.Models.LoaderBot;
using posting.Services.LoaderBotService;
using posting.Services.MongoDBService;
using posting.Services.S3Service;
using posting.Utils.MessageConstructor;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.Configure<List<LoaderBotModel>>(builder.Configuration.GetSection("LoaderBots"));

builder.Services.AddSingleton<IMongoDBService, MongoDBService>();
builder.Services.AddSingleton<IS3Service, S3Service>();

builder.Services.AddSingleton<IMessageConstructorFactory, MessageConstructorFactory>();
builder.Services.AddSingleton<ILoaderBotFactory, LoaderBotFactory>();

builder.Services.AddSingleton<LoaderBotService>(); // ќдин экземпл€р сервиса
builder.Services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<LoaderBotService>()); // »спользовать как хост-сервис
builder.Services.AddSingleton<ILoaderBotService>(sp => sp.GetRequiredService<LoaderBotService>()); // »спользовать как обычный сервис


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen( options => {
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
