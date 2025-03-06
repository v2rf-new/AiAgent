using AiAgent.API.Configuration;
using AiAgent.API.Service;
using AiAgent.API.Service.Abstract;
using AiAgent.API.Service.ChatManager;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.ChatCompletion;

//TODOES
//TODO: Add a History reducer based on tokens.
//TODO: Add 
//TODO: Parse the content outside of the ai reasoning (<think/>)
//TODO: Improve logging.
//TODO: Implement OpenAI service.

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("AgentConfiguration.json", optional: true, reloadOnChange: true);
builder.Services.Configure<AgentConfiguration>(builder.Configuration.GetSection("AgentConfiguration"));

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<HttpService>();
builder.Services.AddSingleton<ChatManagerService>();
builder.Services.AddSingleton<OllamaService>();
builder.Services.AddSingleton<EmbeddingService>();
builder.Services.AddScoped<ITextCompletionService, OllamaTextCompletionService>();
# pragma warning disable SKEXP0001
builder.Services.AddTransient<IChatHistoryReducer, ChatHistoryReducer>();
builder.Services.AddTransient(provider => new Lazy<IChatHistoryReducer>(provider.GetRequiredService<IChatHistoryReducer>));



builder.Services.AddLogging(builder =>
{
    //builder.ClearProviders();
    builder.AddConsole();
    builder.AddDebug();
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.UseRouting();
app.MapControllers();

app.Run();
