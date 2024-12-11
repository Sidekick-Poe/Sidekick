namespace Sidekick.Apis.Poe.Modifiers.Models
{
    public record ApiModifier
    {
        public required string Id { get; init; }
        public required string Text { get; set; }
        public required string Type { get; init; }

        public ApiModifierOptions? Option { get; set; }
    }
}
