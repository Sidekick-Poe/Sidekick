using System.ComponentModel.DataAnnotations;

namespace Sidekick.Apis.PoeNinja.Models
{
    /// <summary>
    /// Contains translation data from the Poe ninja api
    /// </summary>
    public class NinjaTranslation
    {
        /// <summary>
        /// The value of the translation
        /// </summary>
        [Key]
        public string? Translation { get; set; }

        /// <summary>
        /// The english value
        /// </summary>
        public string? English { get; set; }
    }
}
