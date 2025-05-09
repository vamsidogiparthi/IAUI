using System.ComponentModel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace IAUI.Agent.Plugins.FunctionPlugin;

public class ProfileScoringPlugin(
    ILogger<ProfileScoringPlugin> logger
// [FromKeyedServices("IAUIKKernel")] Kernel kernel
)
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
        var arguments = new KernelArguments() { { "user", userProfile } };

        var response = await kernel.InvokeAsync(function, arguments);
        logger.LogInformation("Response: {Response}", response);
        return new UserProfileScore
        {
            UserId = userProfile.Id,
            ScoreId = 1,
            Score = 85,
            Category = "User Engagement",
            Algorithm = "LLM",
            Reason = "High engagement with the platform.",
        };
    }
}
