using IAUI.Agent.Database_Layer;
using IAUI.Agent.Plugins.FunctionPlugin;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables()
    .Build();

builder.Services.AddOpenApi();
builder.Services.AddLogging(loggingBuilder =>
    loggingBuilder.AddConsole().AddConfiguration(configuration.GetSection("Logging"))
);
builder.Services.AddOptions();
builder.Services.Configure<OpenAIConfiguration>(
    configuration.GetSection(OpenAIConfiguration.SectionName)
);
builder.Services.Configure<IAUIStoreDatabaseConfiguration>(
    configuration.GetSection(IAUIStoreDatabaseConfiguration.SectionName)
);

builder.Services.AddSingleton<IIAUIDatabaseService, IAUIDatabaseService>();
builder.Services.AddSingleton<IDataSeeder, DataSeeder>();
builder.Services.AddSingleton<IProfileScoringService, ProfileScoringService>();

// var kernelBuilder = builder.Services.AddKernel();

builder.Services.AddSingleton<IChatCompletionService>(sp =>
{
    var openAIConfiguration =
        configuration.GetSection(OpenAIConfiguration.SectionName).Get<OpenAIConfiguration>()
        ?? throw new Exception("OpenAI configuration is missing");

    return new OpenAIChatCompletionService(openAIConfiguration.Model, openAIConfiguration.ApiKey);
});

builder.Services.AddSingleton<TimePlugin>();
builder.Services.AddSingleton<ProfileDataFetchPlugin>();
builder.Services.AddKeyedSingleton<ProfileScoringPlugin>(nameof(ProfileScoringPlugin));
builder.Services.AddKeyedTransient(
    "IAUIKKernel",
    (sp, key) =>
    {
        KernelPluginCollection kernelFunctions = [];
        kernelFunctions.AddFromObject(sp.GetRequiredService<TimePlugin>());
        //kernelFunctions.AddFromObject(sp.GetRequiredService<ProfileDataFetchPlugin>());
        kernelFunctions.AddFromObject(
            sp.GetRequiredKeyedService<ProfileScoringPlugin>(nameof(ProfileScoringPlugin))
        );

        return new Kernel(sp, kernelFunctions);
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// await app.Services.GetRequiredService<IDataSeeder>().SeedData();
//await app.Services.GetRequiredService<IDataSeeder>().SeedData();
await app
    .Services.GetRequiredService<IProfileScoringService>()
    .CalculateProfileScoresForAllUserProfile();
