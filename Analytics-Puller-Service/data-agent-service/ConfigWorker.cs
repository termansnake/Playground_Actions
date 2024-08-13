namespace data_agent_service;
public class ConfigWorker
{
    public const string SectionName = "ConfigWorker";
    public string UrlAnalitycs { get; set; } = String.Empty;
    public int RequestInterval { get; set; } = 3600000;
    public int Offset { get; set; } = 10000;
    public String CloudProvider { get; set; } = "None";
}