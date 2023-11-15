namespace Sidekick.Common.Game.Items
{
    public class Header
    {
        public string? Name { get; set; }

        public string? Type { get; set; }

        public Class Class { get; set; }

        /// <inheritdoc/>
        public override string? ToString()
        {
            if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Name))
            {
                return $"{Type} - {Name}";
            }

            if (!string.IsNullOrEmpty(Type))
            {
                return Type;
            }

            return Name;
        }
    }
}
