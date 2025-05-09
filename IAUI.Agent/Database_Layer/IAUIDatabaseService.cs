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
}

public class IAUIDatabaseService : IIAUIDatabaseService
{
    private readonly IMongoCollection<UserProfile> _userProfilesCollection;

    public IAUIDatabaseService(IOptions<IAUIStoreDatabaseConfiguration> configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var mongoClient = new MongoClient(configuration.Value.ConnectionString);
        var database = mongoClient.GetDatabase(configuration.Value.DatabaseName);
        _userProfilesCollection = database.GetCollection<UserProfile>(
            configuration.Value.CollectionName
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
}
