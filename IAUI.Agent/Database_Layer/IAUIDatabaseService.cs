using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace IAUI.Agent.Database_Layer;

public interface IIAUIDatabaseService
{
    Task<IEnumerable<UserProfile>> GetAllUserProfilesAsync();
    Task<UserProfile?> GetUserProfileAsync(long userId);
    Task AddUserProfileAsync(UserProfile userProfile);
    Task UpdateUserProfileAsync(UserProfile userProfile);
    Task DeleteUserProfileAsync(long userId);
    Task<UserProfileScore> GetProfileScoreAsync(long profileId);
    Task<IEnumerable<UIComponentLibrary>> GetUIComponentLibraryAsyncByProfileScore(int score);
    Task<UIComponentLibrary> CreateUIComponentLibraryAsync(UIComponentLibrary uiComponentLibrary);
    Task<IEnumerable<UIComponentLibrary>> GetAllUIComponentLibrariesAsync();
}

public class IAUIDatabaseService : IIAUIDatabaseService
{
    private readonly IMongoCollection<UserProfile> _userProfilesCollection;
    private readonly IMongoCollection<UIComponentLibrary> _uiComponentLibraryCollection;

    public IAUIDatabaseService(IOptions<IAUIStoreDatabaseConfiguration> configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var mongoClient = new MongoClient(configuration.Value.ConnectionString);
        var database = mongoClient.GetDatabase(configuration.Value.DatabaseName);
        _userProfilesCollection = database.GetCollection<UserProfile>(
            configuration.Value.CollectionName
        );

        _uiComponentLibraryCollection = database.GetCollection<UIComponentLibrary>(
            configuration.Value.UIComponentLibraryCollectionName
        );
    }

    public async Task<IEnumerable<UserProfile>> GetAllUserProfilesAsync()
    {
        return await _userProfilesCollection.Find(_ => true).ToListAsync();
    }

    public async Task<UserProfile?> GetUserProfileAsync(long userId)
    {
        var filter = Builders<UserProfile>.Filter.Eq(up => up.Id, userId);
        return await _userProfilesCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task AddUserProfileAsync(UserProfile userProfile)
    {
        ArgumentNullException.ThrowIfNull(userProfile);

        await _userProfilesCollection.InsertOneAsync(userProfile);
    }

    public async Task UpdateUserProfileAsync(UserProfile userProfile)
    {
        ArgumentNullException.ThrowIfNull(userProfile);

        var filter = Builders<UserProfile>.Filter.Eq(up => up.Id, userProfile.Id);
        await _userProfilesCollection.ReplaceOneAsync(filter, userProfile);
    }

    public async Task DeleteUserProfileAsync(long userId)
    {
        var filter = Builders<UserProfile>.Filter.Eq(up => up.Id, userId);
        await _userProfilesCollection.DeleteOneAsync(filter);
    }

    public async Task<UserProfileScore> GetProfileScoreAsync(long profileId)
    {
        var filter = Builders<UserProfile>.Filter.Eq(up => up.Id, profileId);
        var userProfile = await _userProfilesCollection.Find(filter).FirstOrDefaultAsync();
        return userProfile.ProfileScores.FirstOrDefault(defaultValue: new());
    }

    public async Task<IEnumerable<UIComponentLibrary>> GetUIComponentLibraryAsyncByProfileScore(
        int score
    )
    {
        var filter =
            Builders<UIComponentLibrary>.Filter.Gte(up => up.MatchingUserProfileScoreMin, score)
            & Builders<UIComponentLibrary>.Filter.Lt(up => up.MatchingUserProfileScoreMax, score);
        return await _uiComponentLibraryCollection.Find(filter).ToListAsync();
    }

    public async Task<UIComponentLibrary> CreateUIComponentLibraryAsync(
        UIComponentLibrary uiComponentLibrary
    )
    {
        ArgumentNullException.ThrowIfNull(uiComponentLibrary);

        await _uiComponentLibraryCollection.InsertOneAsync(uiComponentLibrary);
        return uiComponentLibrary;
    }

    public async Task<IEnumerable<UIComponentLibrary>> GetAllUIComponentLibrariesAsync()
    {
        return await _uiComponentLibraryCollection.Find(_ => true).ToListAsync();
    }
}
