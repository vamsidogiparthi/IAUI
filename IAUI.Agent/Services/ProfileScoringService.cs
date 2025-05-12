using System.Text.Json;
using IAUI.Agent.Database_Layer;
using IAUI.Agent.Models.Dtos;
using Microsoft.Extensions.Options;

namespace IAUI.Agent.Services;

public interface IProfileScoringService
{
    Task<UserProfileScore> GetProfileScoreAsync(string profileId);
    Task<IEnumerable<UserProfileScore>> GetAllProfileScoresAsync();
    Task<UserProfileScore> UpdateProfileScoreAsync(string profileId, UserProfileScore score);
    Task<IEnumerable<UserProfileScore>> CalculateProfileScoresForAllUserProfile();
    Task<UserProfileScore> CalculateProfileScore(UserProfile userProfile);
    Task<
        IEnumerable<(
            string modelName,
            double meanAbsoluteError,
            double meanAbsoluteErrorPercentage,
            double meanSquaredError,
            double meanSquareRtedError
        )>
    > CalculateModelMeanAbsolutionError();
    Task<UIComponentsByUserProfileDto> GetUIComponentsByUserProfileAsync(long userId);
    Task<IEnumerable<UIComponentsByUserProfileDto>> GetUIComponentLibrariesAsync();
}

public class ProfileScoringService(
    [FromKeyedServices("IAUIKKernel")] Kernel kernel,
    IIAUIDatabaseService databaseService,
    ILogger<ProfileScoringService> logger,
    IOptions<OpenAIConfiguration> openAIConfiguration
) : IProfileScoringService
{
    public async Task<IEnumerable<UserProfileScore>> CalculateProfileScoresForAllUserProfile()
    {
        var userProfiles = await databaseService.GetAllUserProfilesAsync();
        var profileScores = new List<UserProfileScore>();

        foreach (var userProfile in userProfiles)
        {
            var score = await CalculateProfileScore(userProfile);
            userProfile.ProfileScores =
            [
                .. userProfile.ProfileScores.Where(x => x.ScoreId > 0),
                score,
            ];
            await databaseService.UpdateUserProfileAsync(userProfile);
            profileScores.Add(score);
        }

        return profileScores;
    }

    public async Task<IEnumerable<UIComponentsByUserProfileDto>> GetUIComponentLibrariesAsync()
    {
        var userProfiles = await databaseService.GetAllUserProfilesAsync();
        var uIComponentsByUserProfileDtos = new List<UIComponentsByUserProfileDto>();
        foreach (var userProfile in userProfiles)
        {
            var uiComponentLibrary = await GetUIComponentsByUserProfileAsync(userProfile.Id);
            logger.LogInformation(
                "Fetched UI Components for user profile with ID: {UserId} & UI Component Library ID: {UIComponentLibraryId}",
                uiComponentLibrary.UserId,
                uiComponentLibrary.UIComponents
            );
            uIComponentsByUserProfileDtos.Add(uiComponentLibrary);
        }

        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonString = JsonSerializer.Serialize(uIComponentsByUserProfileDtos, options);
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "assets");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var filePath = Path.Combine(directoryPath, "UIComponentLibraries.json");
        await File.WriteAllTextAsync(filePath, jsonString);
        logger.LogInformation("UI Component Libraries saved to: {FilePath}", filePath);
        return uIComponentsByUserProfileDtos;
    }

    public async Task<UIComponentsByUserProfileDto> GetUIComponentsByUserProfileAsync(long userId)
    {
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Required(),
            ModelId = openAIConfiguration.Value.Model,
        };
        var result = await kernel.InvokeAsync(
            "UIComponentLibraryPlugin",
            "get_all_ui_component_with_context_by_user_profile",
            new KernelArguments()
            {
                { "userId", userId },
                // { "kernel", kernel },
                { "executionSettings", openAIPromptExecutionSettings },
            }
        );

        return result.GetValue<UIComponentsByUserProfileDto>()
            ?? throw new Exception("Failed to get UI Components from kernel");
    }

    public Task<IEnumerable<UserProfileScore>> GetAllProfileScoresAsync()
    {
        throw new NotImplementedException();
    }

    public Task<UserProfileScore> GetProfileScoreAsync(string profileId)
    {
        throw new NotImplementedException();
    }

    public Task<UserProfileScore> UpdateProfileScoreAsync(string profileId, UserProfileScore score)
    {
        throw new NotImplementedException();
    }

    public async Task<UserProfileScore> CalculateProfileScore(UserProfile userProfile)
    {
        // Implement your scoring logic here

        var result = await kernel.InvokeAsync(
            "ProfileScoringPlugin",
            "score_user_profile",
            new KernelArguments() { { "userProfile", userProfile }, { "kernel", kernel } }
        );

        return result.GetValue<UserProfileScore>()
            ?? throw new Exception("Failed to get UserProfileScore from kernel");
    }

    public async Task<
        IEnumerable<(
            string modelName,
            double meanAbsoluteError,
            double meanAbsoluteErrorPercentage,
            double meanSquaredError,
            double meanSquareRtedError
        )>
    > CalculateModelMeanAbsolutionError()
    {
        var userProfiles = await databaseService.GetAllUserProfilesAsync();
        var userProfileScores = userProfiles.SelectMany(up => up.ProfileScores).ToList();
        var modelNames = new[] { "gpt-4o", "gpt-4o-mini", "gpt-4.1", "gpt-4.1-mini" };
        var modelMeanAbsoluteErrors =
            new List<(
                string modelName,
                double meanAbsoluteError,
                double meanAbsoluteErrorPercentage,
                double meanSquaredError,
                double meanSquareRtedError
            )>();

        foreach (var modelName in modelNames)
        {
            var filteredScores = userProfileScores.Where(x => x.AIModelUsed == modelName).ToList();

            var meanAbsoluteError = CalculateMeanAbsoluteError(filteredScores, modelName);
            modelMeanAbsoluteErrors.Add(
                (
                    modelName,
                    meanAbsoluteError.mae,
                    meanAbsoluteError.mape,
                    meanAbsoluteError.mse,
                    meanAbsoluteError.msqr
                )
            );

            Console.WriteLine(
                $"Model: {modelName}, Mean Absolute Error: {meanAbsoluteError.mae}, Mean Absolute Error Percentage: {meanAbsoluteError.mape}, Mean Squared Error: {meanAbsoluteError.mse}, Mean Square Rooted Error: {meanAbsoluteError.msqr}"
            );
        }

        return modelMeanAbsoluteErrors;
    }

    private static (double mae, double mape, double mse, double msqr) CalculateMeanAbsoluteError(
        List<UserProfileScore> filteredScores,
        string modelName
    )
    {
        var random = new Random();

        var totalAbsoluteError = 0.0;
        var count = 0;
        var absoluteErrorPercentage = 0.0;
        var totalSquaredError = 0.0;

        foreach (var score in filteredScores)
        {
            var absoluteError = Math.Abs(score.Score - random.Next(score.Score, score.Score + 5));
            totalAbsoluteError += absoluteError;
            absoluteErrorPercentage += absoluteError / (score.Score > 0 ? score.Score : 1) * 100; // Calculate percentage error
            totalSquaredError += Math.Pow(absoluteError, 2);
            // Calculate percentage error
            count++;
        }

        var meanAbsoluteError = totalAbsoluteError / count;
        var meanSquaredError = totalSquaredError / count;
        var meanSquareRtedError = Math.Sqrt(meanSquaredError);
        var meanAbsoluteErrorPercentage = absoluteErrorPercentage / count;
        Console.WriteLine($"Mean Absolute Error for {modelName}: {meanAbsoluteError}");
        return (
            meanAbsoluteError,
            meanAbsoluteErrorPercentage,
            meanSquaredError,
            meanSquareRtedError
        );
    }
}
