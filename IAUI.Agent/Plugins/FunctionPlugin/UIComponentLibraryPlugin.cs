using System.Text.Json;
using IAUI.Agent.Models.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace IAUI.Agent.Plugins.FunctionPlugin;

public class UIComponentLibraryPlugin(
    IIAUIDatabaseService databaseService,
    IOptions<OpenAIConfiguration> openAIConfiguration,
    ILogger<UIComponentLibraryPlugin> logger
)
{
    [KernelFunction("get_ui_components_by_score")]
    [Description("Get the list of UI Component Libraries for the given profile score.")]
    public async Task<IEnumerable<UIComponentLibrary>> GetUIComponentsForScore(
        [Description("profileScore to fetch the specific library components")] int profileScore
    )
    {
        var uiComponentLibrary = await databaseService.GetUIComponentLibraryAsyncByProfileScore(
            profileScore
        );

        return uiComponentLibrary;
    }

    [KernelFunction("get_all_ui_component_libraries")]
    [Description("Get all the UI Component Libraries.")]
    public async Task<IEnumerable<UIComponentLibrary>> GetUIComponentLibrariesAsync(
        [Description("user unique id to fetch the specific library components")] int userId
    )
    {
        logger.LogInformation("All the UI component libraries available");

        var uiComponentLibrary = await databaseService.GetAllUIComponentLibrariesAsync();

        return uiComponentLibrary;
    }

    [KernelFunction("get_all_ui_component_with_context_by_user_profile")]
    [Description("Get all the UI Components with context for the given user.")]
    public async Task<UIComponentsByUserProfileDto> GetUIComponentsByUserProfileAsync(
        [Description("user unique id to fetch the specific library components")] long userId,
        Kernel kernel
    )
    {
        logger.LogInformation("Fetching all the UI component libraries for the user profile");

        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Required(),
            ModelId = openAIConfiguration.Value.Model,
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        };

        var userProfile = await databaseService.GetUserProfileAsync(userId);
        ArgumentNullException.ThrowIfNull(userProfile);
        var uiComponentLibrary = await databaseService.GetAllUIComponentLibrariesAsync();

        var userCurrentDateTime = await kernel.InvokeAsync(
            "TimePlugin",
            "get_current_date_time_in_time_zone",
            new KernelArguments() { { "timeZone", userProfile.Address.TimeZone } }
        );

        logger.LogInformation("Response: {Response}", userCurrentDateTime.ToString());

        var handlebarsPromptYaml = EmbeddedResource.Read("UIAssignmentPluginTemplate.yaml");
        var templateFactory = new HandlebarsPromptTemplateFactory();
        var function = kernel.CreateFunctionFromPromptYaml(handlebarsPromptYaml, templateFactory);

        var arguments = new KernelArguments()
        {
            { "user", userProfile },
            {
                "userProfileScore",
                userProfile.ProfileScores.OrderByDescending(s => s.ScoreId).FirstOrDefault()
            },
            { "uiComponents", uiComponentLibrary },
            { "userDateTime", userCurrentDateTime.ToString() },
            { "executionSettings", openAIPromptExecutionSettings },
        };

        var response = await kernel.InvokeAsync(function, arguments);
        var cleansedResponse = response
            .ToString()
            .Replace("```json", "")
            .Replace("`", "")
            .Replace("\n", "");
        logger.LogInformation("Response: {Response}", cleansedResponse);

        var uIComponentsByUserProfileDto = JsonSerializer.Deserialize<UIComponentsByUserProfileDto>(
            cleansedResponse
        );

        return uIComponentsByUserProfileDto
            ?? throw new Exception("Failed to deserialize UIComponentsByUserProfileDto");
    }

    // [KernelFunction("get_all_ui_component_with_context_by_user_profile")]
    // [Description("Get all the UI Components with context for the given user.")]
    // public async Task<UIComponentsByUserProfileDto> GetUIComponentsByUserProfileAsync(
    //     [Description("user unique id to fetch the specific library components")] long userId,
    //     Kernel kernel
    // )
    // {
    //     logger.LogInformation("Fetching all the UI component libraries for the user profile");

    //     OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    //     {
    //         FunctionChoiceBehavior = FunctionChoiceBehavior.Required(),
    //         ModelId = openAIConfiguration.Value.Model,
    //         ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
    //     };

    //     var userProfile = await databaseService.GetUserProfileAsync(userId);
    //     ArgumentNullException.ThrowIfNull(userProfile);

    //     var handlebarsPromptYaml = EmbeddedResource.Read("PromptTemplateTest.yaml");
    //     var templateFactory = new HandlebarsPromptTemplateFactory();
    //     var function = kernel.CreateFunctionFromPromptYaml(handlebarsPromptYaml, templateFactory);

    //     var arguments = new KernelArguments()
    //     {
    //         { "user", userProfile },
    //         { "executionSettings", openAIPromptExecutionSettings },
    //     };

    //     var response = await kernel.InvokeAsync(function, arguments);
    //     var cleansedResponse = response
    //         .ToString()
    //         .Replace("```json", "")
    //         .Replace("`", "")
    //         .Replace("\n", "");
    //     logger.LogInformation("Response: {Response}", cleansedResponse);

    //     var uIComponentsByUserProfileDto = JsonSerializer.Deserialize<UIComponentsByUserProfileDto>(
    //         cleansedResponse
    //     );

    //     return uIComponentsByUserProfileDto
    //         ?? throw new Exception("Failed to deserialize UIComponentsByUserProfileDto");
    // }
}
