using System.ComponentModel;
using System.Text.Json;
using IAUI.Agent.Models.Dtos;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace IAUI.Agent.Plugins.FunctionPlugin;

public class ProfileScoringPlugin(ILogger<ProfileScoringPlugin> logger)
{
    [KernelFunction("score_user_profile")]
    [Description("Score user profile based on various parameters. using LLM prompting.")]
    public async Task<UserProfileScore> GetUserProfileScore(
        [Description("user profile information for scoring")] UserProfile userProfile,
        Kernel kernel
    )
    {
        var handlebarsPromptYaml = EmbeddedResource.Read("ProfileScoringPromptYamlTemplate.yaml");
        var templateFactory = new HandlebarsPromptTemplateFactory();
        var function = kernel.CreateFunctionFromPromptYaml(handlebarsPromptYaml, templateFactory);
        var user = new UserProfileInfoProfiScoringDto()
        {
            Id = userProfile.Id,
            DateOfBirth = userProfile.DateOfBirth,
            Address = userProfile.Address,
            AccountCreationDate = userProfile.AccountCreationDate,
            LoginHistory = userProfile.LoginHistory,
        };
        var arguments = new KernelArguments() { { "user", user } };

        var response = await kernel.InvokeAsync(function, arguments);

        var cleansedResponse = response
            .ToString()
            .Replace("```json", "")
            .Replace("`", "")
            .Replace("\n", "");
        logger.LogInformation("Response: {Response}", cleansedResponse);

        var userProfileScore = JsonSerializer.Deserialize<UserProfileScore>(cleansedResponse);

        return userProfileScore ?? throw new Exception("Failed to deserialize UserProfileScore");
    }
}
