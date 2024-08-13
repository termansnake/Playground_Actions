using Nest;

namespace data_agent_service;

public class ResponseMetricData
{
    public List<Item> Value { get; set; }
    [JsonPropertyName("@odata.nextLink")]
    public string OdataNextLink { get; set; }
}

public class Item
{
    public string CompletedDate { get; set; }
    public string DateValue { get; set; }
    public string StateCategory { get; set; }
    public string State { get; set; }
    public string WorkItemType { get; set; }
    public int WorkItemId { get; set; }
    public decimal? StoryPoints { get; set; }
    public int Count { get; set; }
    public Iteration Iteration { get; set; }
    public Team Team { get; set; }
    public Project Project { get; set; }
    public Area Area { get; set; }
    public string Id { get; set; }
}

public class Iteration
{
    public string EndDate { get; set; }
    public string IterationName { get; set; }
    public string StartDate { get; set; }
    public string IterationPath { get; set; }
}

public class Team
{
    public string TeamName { get; set; }
}

public class Project
{
    public string ProjectName { get; set; }
}

public class Area
{
    public string AreaPath { get; set; }
}

public class ItemProccess
{
    private string _iterationName = string.Empty;

   [JsonProperty(PropertyName = "partitionKey")]
    public string PartitionKey => $"{ProjectName}.{WorkItemId}.{DateValue}";
    public string CompletedDate { get; set; }
    public string DateValue { get; set; }
    public string StateCategory { get; set; }
    public string State { get; set; }
    public string WorkItemType { get; set; }
    public int WorkItemId { get; set; }
    public decimal? StoryPoints { get; set; }
    public int Count { get; set; }
    public string IterationEndDate { get; set; }
    public string IterationName { get { return _iterationName.Replace("0", ""); } set { _iterationName = value; } }
    public string IterationStartDate { get; set; }
    public string IterationPath { get; set; }
    public string TeamName { get; set; }
    public string ProjectName { get; set; }
    public string AreaPath { get; set; }

    [JsonProperty(PropertyName = "id")]
    public string Id => $"{ProjectName}.{WorkItemId}.{DateValue}";

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
                           