namespace Sidekick.Apis.Poe.Stash.Models
{
    public class APIItemProperty
    {
        public required string name { get; set; }
        public int? type { get; set; }
        public List<List<object?>?>? values { get; set; }
    }
}
