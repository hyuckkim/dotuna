public class Response
{
#pragma warning disable IDE1006 // Naming Styles: Because the JSON properties are in snake_case, we need to disable this rule for the properties.
    public int threadId { get; set; } = 0;
    public int sequence { get; set; } = 0;
    public string username { get; set; } = "";
    public string userId { get; set; } = "";
    public DateTime createdAt { get; set; } = new DateTime(0);
    public string content { get; set; } = "";
    public string attachment { get; set; } = "";
#pragma warning restore IDE1006 // Naming Styles
}

public class JsonThreadDocument
{
#pragma warning disable IDE1006 // Naming Styles: Because the JSON properties are in snake_case, we need to disable this rule for the properties.
    public string version { get; set; } = "";
    public string boardId { get; set; } = "";
    public int threadId { get; set; } = 0;
    public string title { get; set; } = "";
    public string username { get; set; } = "";
    public DateTime createdAt { get; set; } = new DateTime(0);
    public DateTime updatedAt { get; set; } = new DateTime(0);
    public int size { get; set; } = 0;
    public List<Response> responses { get; set; } = [];
#pragma warning restore IDE1006 // Naming Styles
}

