using IAUI.Agent.Database_Layer;

namespace IAUI.Agent.Services;

public interface IProfileScoringService
{
    Task<UserProfileScore> GetProfileScoreAsync(string profileId);
    Task<IEnumerable<UserProfileScore>> GetAllProfileScoresAsync();
    Task<UserProfileScore> UpdateProfileScoreAsync(string profileId, UserProfileScore score);
    Task<IEnumerable<UserProfileScore>> CalculateProfileScoresForAllUserProfile();
}

public class ProfileScoringService(
    [FromKeyedServices("IAUIKKernel")] Kernel kernel,
    IIAUIDatabaseService databaseService
) : IProfileScoringService
{
    public async Task<IEnumerable<UserProfileScore>> CalculateProfileScoresForAllUserProfile()
    {
        var userProfiles = await databaseService.GetAllUserProfilesAsync();
        var profileScores = new List<UserProfileScore>();

        foreach (var userProfile in userProfiles)
        {
            var score = await CalculateProfileScore(userProfile);
            userProfile.ProfileScores = [.. userProfile.ProfileScores, score];
            await databaseService.UpdateUserProfileAsync(userProfile);
            profileScores.Add(score);
        }

        return profileScores;
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

    private async Task<UserProfileScore> CalculateProfileScore(UserProfile userProfile)
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
}
