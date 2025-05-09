using System.Text.Json;
using System.Threading.Tasks;

namespace IAUI.Agent.Database_Layer;

public interface IDataSeeder
{
    Task SeedData();
}

public class DataSeeder(IIAUIDatabaseService iAUIDatabaseService, ILogger<IDataSeeder> logger)
    : IDataSeeder
{
    private readonly IIAUIDatabaseService _iAUIDatabaseService = iAUIDatabaseService;

    public async Task SeedData()
    {
        logger.LogInformation("Seeding data...");
        var random = new Random();
        var userProfiles = new List<UserProfile>();

        // Sample data for locations and time zones
        var locations = new List<Location>
        {
            new()
            {
                LocationId = 1,
                City = "New York",
                State = "NY",
                Country = "USA",
                ZipCode = "10001",
                Latitude = 40.7128,
                Longitude = -74.0060,
                TimeZone = "America/New_York",
                AddressLine1 = "123 Main St",
                AddressLine2 = "Apt 4B",
                Landmark = "Near Central Park",
            },
            new()
            {
                LocationId = 2,
                City = "London",
                State = "",
                Country = "UK",
                ZipCode = "EC1A",
                Latitude = 51.5074,
                Longitude = -0.1278,
                TimeZone = "Europe/London",
                AddressLine1 = "456 High St",
                AddressLine2 = null,
                Landmark = "Close to Big Ben",
            },
            new()
            {
                LocationId = 3,
                City = "Tokyo",
                State = "",
                Country = "Japan",
                ZipCode = "100-0001",
                Latitude = 35.6895,
                Longitude = 139.6917,
                TimeZone = "Asia/Tokyo",
                AddressLine1 = "789 Sakura Ave",
                AddressLine2 = "Building 10",
                Landmark = "Near Tokyo Tower",
            },
            new()
            {
                LocationId = 4,
                City = "Sydney",
                State = "NSW",
                Country = "Australia",
                ZipCode = "2000",
                Latitude = -33.8688,
                Longitude = 151.2093,
                TimeZone = "Australia/Sydney",
                AddressLine1 = "101 Harbour St",
                AddressLine2 = null,
                Landmark = "Opposite Sydney Opera House",
            },
            new()
            {
                LocationId = 5,
                City = "Mumbai",
                State = "MH",
                Country = "India",
                ZipCode = "400001",
                Latitude = 19.0760,
                Longitude = 72.8777,
                TimeZone = "Asia/Kolkata",
                AddressLine1 = "202 Marine Drive",
                AddressLine2 = "Flat 12A",
                Landmark = "Near Gateway of India",
            },
        };

        // Sample data for page activities
        var pages = new[]
        {
            "Home",
            "Profile",
            "Settings",
            "Dashboard",
            "Reports",
            "Help",
            "Logout",
        };
        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        for (int i = 1; i <= 100; i++)
        {
            var location = locations[random.Next(locations.Count)];
            var age = random.Next(18, 76); // Age between 18 and 75
            var dateOfBirth = DateTime.Now.AddYears(-age).AddDays(random.Next(-365, 365));
            var accountCreationDate = DateTime.Now.AddDays(-random.Next(0, 730)); // Past 2 years

            // Generate login history
            var loginHistory = GenerateLoginHistory(random, i, location, pages);

            var userProfile = new UserProfile
            {
                Id = i,
                Name = $"User{i}",
                Email = $"user{i}@example.com",
                PhoneNumber = $"+1{random.Next(1000000000, 1999999999)}",
                DateOfBirth = dateOfBirth,
                Address = location,
                AccountCreationDate = accountCreationDate,
                LoginHistory = [.. loginHistory],
                ProfileScores = [], // No profile scores
            };

            logger.LogInformation($"Adding User Profile: {userProfile}");
            userProfiles.Add(userProfile);
            var jsonOutput = JsonSerializer.Serialize(userProfile, jsonOptions);
            Console.WriteLine(jsonOutput);
            var userProfile1 = await _iAUIDatabaseService.GetUserProfileAsync(userProfile.Id);
            await _iAUIDatabaseService.AddUserProfileAsync(userProfile);
        }

        // Serialize to JSON

        // var jsonOutput = JsonSerializer.Serialize(userProfiles, jsonOptions);
        // Output to console
        // Console.WriteLine(jsonOutput);
    }

    private static List<UserLoginHistory> GenerateLoginHistory(
        Random random,
        int userId,
        Location location,
        string[] pages
    )
    {
        var loginHistories = new List<UserLoginHistory>();

        // Determine the number of login histories for the user
        int loginCount;
        var chance = random.Next(100);
        if (chance < 15) // 15% users with no login history
        {
            loginCount = 0;
        }
        else if (chance < 45) // 30% users with 2-3 login histories
        {
            loginCount = random.Next(2, 4);
        }
        else // Remaining users with 4-15 login histories
        {
            loginCount = random.Next(4, 16);
        }

        for (int i = 0; i < loginCount; i++)
        {
            var loginTime = DateTime
                .Now.AddDays(-random.Next(0, 730))
                .AddHours(random.Next(0, 24))
                .AddMinutes(random.Next(0, 60));
            var logoutTime = loginTime.AddMinutes(random.Next(5, 120)); // Logout after 5-120 minutes
            var pagesVisited = GeneratePageActivities(random, pages);

            var deviceTypes = new[] { "Desktop", "Mobile", "iPad" };
            var operatingSystems = new[] { "Windows", "iOS", "Android" };
            var browsers = new[] { "Chrome", "Safari", "Edge" };

            var loginHistory = new UserLoginHistory
            {
                UserId = userId,
                LoginId = random.Next(100000, 999999),
                LoginTime = loginTime,
                LogoutTime = logoutTime,
                SessionId = Guid.NewGuid().ToString(),
                IpAddress =
                    $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}",
                DeviceInfo = new DeviceInformation
                {
                    DeviceId = i + 1,
                    DeviceType = deviceTypes[random.Next(deviceTypes.Length)],
                    OperatingSystem = operatingSystems[random.Next(operatingSystems.Length)],
                    Browser = browsers[random.Next(browsers.Length)],
                    LastActive = loginTime,
                    DeviceName = $"Device-{i + 1}",
                },
                LoginLocation = location,
                PagesVisited = [.. pagesVisited],
            };

            loginHistories.Add(loginHistory);
        }

        return loginHistories;
    }

    private static List<PageInformation> GeneratePageActivities(Random random, string[] pages)
    {
        var pageActivities = new List<PageInformation>();
        var pageCount = random.Next(1, pages.Length + 1);

        for (int i = 0; i < pageCount; i++)
        {
            var page = pages[random.Next(pages.Length)];
            var duration = random.Next(10, 300); // Duration on page in seconds

            pageActivities.Add(
                new PageInformation
                {
                    Title = page,
                    PageId = i + 1,
                    PageUrl = $"https://example.com/{page.ToLower()}",
                }
            );
        }

        return pageActivities;
    }
}
