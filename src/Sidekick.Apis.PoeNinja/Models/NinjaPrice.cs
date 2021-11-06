using System;
using Sidekick.Apis.PoeNinja.Api.Models;

namespace Sidekick.Apis.PoeNinja.Models
{
    /// <summary>
    /// Contains the result of a PriceFromNinjaQuery
    /// </summary>
    public class NinjaPrice
    {
        /// <summary>
        /// The name of the item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If the item is corrupted or not
        /// </summary>
        public bool Corrupted { get; set; }

        /// <summary>
        /// If it is a map, indicates the tier of the map
        /// </summary>
        public int MapTier { get; set; }

        /// <summary>
        /// If it is a gem, indicates the level of the gem
        /// </summary>
        public int GemLevel { get; set; }

        /// <summary>
        /// The price in chaos of the item
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// When was the data last updated from PoeNinja
        /// </summary>
        public DateTimeOffset LastUpdated { get; set; }

        public string DetailsId { get; set; }

        public ItemType ItemType { get; set; }

        public SparkLine SparkLine { get; set; }
    }
}
