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

builder.Services.Configure<GoogleAIConfiguration>(
    configuration.GetSection(GoogleAIConfiguration.SectionName)
);

builder.Services.Configure<ProfileScoringAlgorithmConfiguration>(
    configuration.GetSection(ProfileScoringAlgorithmConfiguration.SectionName)
);

builder.Services.AddSingleton<IIAUIDatabaseService, IAUIDatabaseService>();
builder.Services.AddSingleton<IDataSeeder, DataSeeder>();
builder.Services.AddSingleton<IProfileScoringService, ProfileScoringService>();

builder.Services.AddSingleton<IChatCompletionService>(sp =>
{
    var openAIConfiguration =
        configuration.GetSection(OpenAIConfiguration.SectionName).Get<OpenAIConfiguration>()
        ?? throw new Exception("OpenAI configuration is missing");

    var googleAIConfiguration =
        configuration.GetSection(GoogleAIConfiguration.SectionName).Get<GoogleAIConfiguration>()
        ?? throw new Exception("Google AI configuration is missing");

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    // return new GoogleAIGeminiChatCompletionService(
    //     googleAIConfiguration.Model,
    //     googleAIConfiguration.ApiKey
    // );
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    return new OpenAIChatCompletionService(openAIConfiguration.Model, openAIConfiguration.ApiKey);
});

builder.Services.AddTransient<TimePlugin>();

//builder.Services.AddSingleton<ProfileDataFetchPlugin>();
builder.Services.AddKeyedTransient<ProfileScoringPlugin>(nameof(ProfileScoringPlugin));

builder.Services.AddTransient<UIComponentLibraryPlugin>();
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
        kernelFunctions.AddFromObject(sp.GetRequiredService<UIComponentLibraryPlugin>());

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

//await app.Services.GetRequiredService<IDataSeeder>().SeedData();
await app.Services.GetRequiredService<IProfileScoringService>().GetUIComponentLibrariesAsync();
