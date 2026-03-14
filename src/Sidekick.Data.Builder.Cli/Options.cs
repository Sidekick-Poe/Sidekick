namespace Sidekick.Data.Builder.Cli;

public class Options
{
    public string Language { get; set; } = string.Empty;
    public bool Poe1 { get; set; } = true;
    public bool Poe2 { get; set; } = true;

    public bool Stats { get; set; }
    public bool Trade { get; set; }
    public bool Repoe { get; set; }
    public bool Pseudo { get; set; }
    public bool Ninja { get; set; }

    public bool HasSelectiveOptions => Stats || Trade || Repoe || Pseudo || Ninja;
}
