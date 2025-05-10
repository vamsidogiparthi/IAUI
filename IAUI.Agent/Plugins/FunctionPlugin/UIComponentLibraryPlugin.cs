namespace IAUI.Agent.Plugins.FunctionPlugin;

public class UIComponentLibraryPlugin(
    IIAUIDatabaseService databaseService,
    ILogger<UIComponentLibraryPlugin> logger
)
{
    [KernelFunction("get_ui_component_library")]
    [Description("Get the UI component library.")]
    public static string GetUIComponentLibrary()
    {
        return "";
    }
}
