namespace Sidekick.Apis.Poe.Metadatas.Models
{
    /// <summary>
    /// Uniques
    /// Armour
    /// Cards
    /// Gems
    /// Jewels
    /// Maps
    /// Weapons
    /// Itemised Monsters
    /// </summary>
    public class ApiCategory
    {
        public string? Label { get; set; }
        public List<ApiItem> Entries { get; set; } = new();
    }
}
