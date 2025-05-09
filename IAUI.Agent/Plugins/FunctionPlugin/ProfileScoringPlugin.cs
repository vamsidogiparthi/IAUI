using System.ComponentModel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace IAUI.Agent.Plugins.FunctionPlugin;

public class ProfileScoringPlugin(
    ILogger<ProfileScoringPlugin> logger,
    [FromKeyedServices("IAUIKKernel")] Kernel kernel
)
{
    [KernelFunction("score_user_profile")]
    [Description("Score user profile based on various parameters. using LLM prompting.")]
    public async Task<UserProfileScore> GetUserProfileScore(
        [Description("user primary key for fetching user information for scoring")] string userId
    )
    {
        var handlebarsPromptYaml = EmbeddedResource.Read("TextSummarizationYamlTemplate.yaml");
        var templateFactory = new HandlebarsPromptTemplateFactory();
        var function = kernel.CreateFunctionFromPromptYaml(handlebarsPromptYaml, templateFactory);
        var userProfile = new UserProfile();
        var arguments = new KernelArguments() { { "input", userProfile } };

        var response = await kernel.InvokeAsync(function, arguments);
        logger.LogInformation("Response: {Response}", response);
        return new UserProfileScore
        {
            UserId = long.Parse(userId),
            ScoreId = 1,
            Score = 85,
            Category = "User Engagement",
            Algorithm = "LLM",
            Reason = "High engagement with the platform.",
        };
    }
}
