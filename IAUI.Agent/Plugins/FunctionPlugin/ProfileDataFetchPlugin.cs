namespace IAUI.Agent.Plugins.FunctionPlugin;

public class ProfileDataFetchPlugin(
    ILogger<ProfileDataFetchPlugin> logger,
    IIAUIDatabaseService databaseService,
    IProfileScoringService profileScoringService
)
{
    [KernelFunction("fetch_user_profile")]
    [Description("Fetch user profile from the database.")]
    public async Task<UserProfile> GetUserProfile(
        [Description("user profile information for scoring")] long userId
    )
    {
        logger.LogInformation("Fetching user profile for ID: {UserId}", userId);
        var userProfile =
            await databaseService.GetUserProfileAsync(userId)
            ?? throw new Exception($"User profile with ID {userId} not found.");
        return userProfile;
    }

    [KernelFunction("fetch_latest_user_profile_score")]
    [Description("Fetch latest user profile score from the database.")]
    public async Task<UserProfileScore> GetRecentUserProfileScore(
        [Description("user profile information for scoring")] long userId
    )
    {
        logger.LogInformation("Fetching user profile with {userId}", userId);
        var userProfile =
            await databaseService.GetUserProfileAsync(userId)
            ?? throw new Exception($"User profile score with ID {userId} not found.");

        var userProfileScore = userProfile
            .ProfileScores.OrderByDescending(s => s.CreatedAt)
            .FirstOrDefault();

        return userProfileScore ?? await profileScoringService.CalculateProfileScore(userProfile);
    }
}
