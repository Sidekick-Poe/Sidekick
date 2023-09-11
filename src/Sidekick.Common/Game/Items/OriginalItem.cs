namespace Sidekick.Common.Game.Items
{
    public class OriginalItem
    {
        public string? Name { get; set; }

        public string? Type { get; set; }

        public string? Text { get; set; }

        /// <inheritdoc/>
        public override string? ToString()
        {
            if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Name))
            {
                return $"{Type}_{Name}";
            }

            if (!string.IsNullOrEmpty(Type))
            {
                return Type;
            }

            return Name;
        }

        public static OriginalItem Parse(string value)
        {
            if (value.Contains("_"))
            {
                var split = value.Split("_");
                return new OriginalItem()
                {
                    Name = split[1],
                    Type = split[0],
                };
            }

            return new OriginalItem()
            {
                Name = value,
            };
        }
    }
}
