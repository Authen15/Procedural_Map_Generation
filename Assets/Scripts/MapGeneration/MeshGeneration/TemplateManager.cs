static class TemplateManager
{
    public static void InitializeTemplates()
    {
        IslandData.GenerateData();
        IslandTemplateGenerator.CreateTemplate();

        BridgeData.GenerateData();
        BridgeTemplateGenerator.CreateTemplate();
    }
}