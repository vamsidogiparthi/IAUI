namespace IAUI.Agent.Models.Dtos;

public class UIComponentsByUserProfileDto
{
    [System.Text.Json.Serialization.JsonPropertyName("userId")]
    public long UserId { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("profileScore")]
    public int ProfileScore { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("uiComponents")]
    public List<UIComponentLibrary> UIComponents { get; set; } = [];

    [System.Text.Json.Serialization.JsonPropertyName("theme")]
    public string Theme { get; set; } = string.Empty;

    [System.Text.Json.Serialization.JsonPropertyName("languageCode")]
    public string LanguageCode { get; set; } = string.Empty;

    [System.Text.Json.Serialization.JsonPropertyName("localizationLanguageCode")]
    public string LocalizationLanguageCode { get; set; } = string.Empty;
}
