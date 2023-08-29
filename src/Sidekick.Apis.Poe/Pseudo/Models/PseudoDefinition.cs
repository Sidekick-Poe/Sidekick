namespace Sidekick.Apis.Poe.Pseudo.Models
{
    public class PseudoDefinition
    {
        public PseudoDefinition(string id, string text)
        {
            Id = id;
            Text = text;
        }

        public string Id { get; }

        public string Text { get; }

        public List<PseudoDefinitionModifier> Modifiers { get; set; } = new();

        public override string ToString()
        {
            if (Text == null)
            {
                return Id;
            }

            return $"{Text} - {Id}";
        }
    }
}
