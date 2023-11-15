namespace Sidekick.Apis.Poe.Stash.Models
{
    public class ApiStashTab
    {
        public required string id { get; set; }
        public string? parent { get; set; }
        public required string name { get; set; }
        public string? type { get; set; }
        public int Index { get; set; }
        public List<ApiStashTab>? children { get; set; }
        public List<APIStashItem>? items { get; set; }
        public APIStashMetadata? metadata { get; set; }

        public bool IsFolder => type == "Folder";
    }
}
