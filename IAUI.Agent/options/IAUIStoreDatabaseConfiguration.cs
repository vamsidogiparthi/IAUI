namespace IAUI.Agent.Options;

public class IAUIStoreDatabaseConfiguration
{
    public const string SectionName = "IAUIStoreDatabaseConfiguration";
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string CollectionName { get; set; } = string.Empty;
    public string UIComponentLibraryCollectionName { get; set; } = string.Empty;
}
