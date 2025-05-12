using System.Text.Json;
using IAUI.Agent.Models.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace IAUI.Agent.Plugins.FunctionPlugin;

public class ProfileScoringPlugin(
    ILogger<ProfileScoringPlugin> logger,
    IOptions<OpenAIConfiguration> openAIConfiguration,
    IOptions<ProfileScoringAlgorithmConfiguration> profileScoringAlgorithmConfiguration
)
{
    [KernelFunction("score_user_profile")]
    [Description("Score user profile based on various parameters. using LLM prompting.")]
    public async Task<UserProfileScore> CalculateUserProfileScore(
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
        var arguments = new KernelArguments()
        {
            { "user", user },
            { "scoringAlgorithm", profileScoringAlgorithmConfiguration.Value.Algorithm },
        };

        var response = await kernel.InvokeAsync(function, arguments);

        var cleansedResponse = response
            .ToString()
            .Replace("```json", "")
            .Replace("`", "")
            .Replace("\n", "");
        logger.LogInformation("Response: {Response}", cleansedResponse);

        var userProfileScore = JsonSerializer.Deserialize<UserProfileScore>(cleansedResponse);
        userProfileScore!.CreatedAt = DateTime.UtcNow;
        userProfileScore!.ScoreId =
            userProfile.ProfileScores.Length != 0
                ? userProfile.ProfileScores.Max(x => x.ScoreId) + 1
                : 1;
        userProfileScore!.AIModelUsed = openAIConfiguration.Value.Model;

        return userProfileScore ?? throw new Exception("Failed to deserialize UserProfileScore");
    }
}
