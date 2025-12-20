using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe.Trade.Trade.Items.Models;

public class ExtendedMods
{
    public List<ExtendedMod> Implicit { get; set; } = [];
    public List<ExtendedMod> Explicit { get; set; } = [];
    public List<ExtendedMod> Crafted { get; set; } = [];
    public List<ExtendedMod> Enchant { get; set; } = [];
    public List<ExtendedMod> Rune { get; set; } = [];
    public List<ExtendedMod> Fractured { get; set; } = [];
    public List<ExtendedMod> Desecrated { get; set; } = [];
    public List<ExtendedMod> Scourge { get; set; } = [];
    public List<ExtendedMod> Sanctum { get; set; } = [];

    [JsonIgnore]
    public List<ExtendedMod> Pseudo { get; set; } = [];
}
