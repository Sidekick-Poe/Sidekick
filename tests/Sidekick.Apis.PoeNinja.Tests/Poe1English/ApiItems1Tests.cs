using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Sidekick.Apis.Poe.Trade.Trade.Models;
using Xunit;

namespace Sidekick.Apis.PoeNinja.Tests.Poe1English;

[Collection(Collections.NinjaTestCollection)]
public class ApiItems1Tests(NinjaTestFixture fixture)
{
    private const string Data = "[{\"Id\":\"50ede3febbbbe07f018dc24c3122fded7f4147d1edc9663c89f7d6eaf70ef6a3\",\"Name\":\"\",\"typeLine\":\"Shock Nova of Procession\",\"baseType\":\"Shock Nova of Procession\",\"Type\":\"Shock Nova of Procession\",\"Identified\":true,\"ilvl\":0,\"frameType\":4,\"foilVariation\":null,\"Corrupted\":true,\"Mutated\":false,\"Scourged\":{\"Tier\":0},\"Fractured\":false,\"Sanctified\":false,\"duplicated\":false,\"IsRelic\":false,\"Influences\":{\"Crusader\":false,\"Elder\":false,\"Hunter\":false,\"Redeemer\":false,\"Shaper\":false,\"Warlord\":false},\"Verified\":false,\"w\":1,\"h\":1,\"StackSize\":null,\"Icon\":\"https://web.poecdn.com/gen/image/WzMwLDE0LHsiZiI6IjJESXRlbXMvR2Vtcy9TaG9ja05vdmEiLCJ3IjoxLCJoIjoxLCJzY2FsZSI6MSwiZ2QiOjEzfV0/e9aff3d3a8/ShockNova.png\",\"Note\":null,\"BuiltInSupport\":\"Supported by Level 1 Intensify\",\"Requirements\":[{\"Name\":\"Level\",\"Icon\":null,\"values\":[[\"70\",0]],\"DisplayMode\":0,\"type\":62},{\"Name\":\"Int\",\"Icon\":null,\"values\":[[\"155\",0]],\"DisplayMode\":1,\"type\":65}],\"Properties\":[{\"Name\":\"Spell, AoE, Lightning, Nova\",\"Icon\":null,\"values\":[],\"DisplayMode\":0,\"type\":0},{\"Name\":\"Level\",\"Icon\":null,\"values\":[[\"20 (Max)\",0]],\"DisplayMode\":0,\"type\":5},{\"Name\":\"Cost\",\"Icon\":null,\"values\":[[\"23 Mana\",0]],\"DisplayMode\":0,\"type\":0},{\"Name\":\"Cast Time\",\"Icon\":null,\"values\":[[\"0.25 sec\",0]],\"DisplayMode\":0,\"type\":0},{\"Name\":\"Critical Strike Chance\",\"Icon\":null,\"values\":[[\"6.00%\",0]],\"DisplayMode\":0,\"type\":0},{\"Name\":\"Effectiveness of Added Damage\",\"Icon\":null,\"values\":[[\"95%\",0]],\"DisplayMode\":0,\"type\":0},{\"Name\":\"Quality\",\"Icon\":null,\"values\":[[\"\\u002B20%\",1]],\"DisplayMode\":0,\"type\":6}],\"AdditionalProperties\":[],\"implicitMods\":[],\"craftedMods\":[],\"explicitMods\":[\"Deals 263 to 790 Lightning Damage\",\"Repeats 2 times\",\"10% more Cast Speed\",\"30% less Area of Effect\"],\"utilityMods\":[],\"pseudoMods\":[],\"enchantMods\":[],\"runeMods\":[],\"fracturedMods\":[],\"desecratedMods\":[],\"scourgeMods\":[],\"sanctumMods\":[],\"mutatedMods\":[],\"GemSockets\":[],\"Sockets\":[],\"extended\":null,\"LogbookMods\":[],\"grantedSkills\":[],\"HasStats\":true},{\"Id\":\"9754bbb608b27b7359eb044796cd617ed14c14747dd7c10219c674cde420aadb\",\"Name\":\"Foulborn Voidbringer\",\"typeLine\":\"Conjurer Gloves\",\"baseType\":\"Conjurer Gloves\",\"Type\":\"Conjurer Gloves\",\"Identified\":true,\"ilvl\":82,\"frameType\":3,\"foilVariation\":null,\"Corrupted\":false,\"Mutated\":true,\"Scourged\":{\"Tier\":0},\"Fractured\":false,\"Sanctified\":false,\"duplicated\":false,\"IsRelic\":false,\"Influences\":{\"Crusader\":false,\"Elder\":false,\"Hunter\":false,\"Redeemer\":false,\"Shaper\":false,\"Warlord\":false},\"Verified\":false,\"w\":2,\"h\":2,\"StackSize\":null,\"Icon\":\"https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQXJtb3Vycy9HbG92ZXMvSGVsbGJyaW5nZXIiLCJ3IjoyLCJoIjoyLCJzY2FsZSI6MSwibXV0YXRlZCI6dHJ1ZX1d/c9887098f5/Hellbringer.png\",\"Note\":null,\"BuiltInSupport\":null,\"Requirements\":[{\"Name\":\"Level\",\"Icon\":null,\"values\":[[\"55\",0]],\"DisplayMode\":0,\"type\":62},{\"Name\":\"Int\",\"Icon\":null,\"values\":[[\"79\",0]],\"DisplayMode\":1,\"type\":65}],\"Properties\":[{\"Name\":\"Energy Shield\",\"Icon\":null,\"values\":[[\"147\",1]],\"DisplayMode\":0,\"type\":18}],\"AdditionalProperties\":[],\"implicitMods\":[],\"craftedMods\":[],\"explicitMods\":[\"\\u002B1 to Level of Socketed Elemental Gems\",\"249% increased Energy Shield\",\"Gain 16 Energy Shield per Enemy Killed\",\"Lose 70 Mana when you use a Skill\"],\"utilityMods\":[],\"pseudoMods\":[],\"enchantMods\":[],\"runeMods\":[],\"fracturedMods\":[],\"desecratedMods\":[],\"scourgeMods\":[],\"sanctumMods\":[],\"mutatedMods\":[\"13% increased Chaos Damage for each Corrupted Item Equipped\"],\"GemSockets\":[],\"Sockets\":[{\"Group\":0,\"sColour\":\"B\",\"type\":null,\"item\":null},{\"Group\":0,\"sColour\":\"B\",\"type\":null,\"item\":null},{\"Group\":0,\"sColour\":\"B\",\"type\":null,\"item\":null},{\"Group\":0,\"sColour\":\"B\",\"type\":null,\"item\":null}],\"extended\":null,\"LogbookMods\":[],\"grantedSkills\":[],\"HasStats\":true},{\"Id\":\"c505b96075471129c9457fcb3ea06b78f824800750413a4d573b8b91c40ff55b\",\"Name\":\"\",\"typeLine\":\"Large Cluster Jewel\",\"baseType\":\"Large Cluster Jewel\",\"Type\":\"Large Cluster Jewel\",\"Identified\":true,\"ilvl\":83,\"frameType\":0,\"foilVariation\":null,\"Corrupted\":false,\"Mutated\":false,\"Scourged\":{\"Tier\":0},\"Fractured\":false,\"Sanctified\":false,\"duplicated\":false,\"IsRelic\":false,\"Influences\":{\"Crusader\":false,\"Elder\":false,\"Hunter\":false,\"Redeemer\":false,\"Shaper\":false,\"Warlord\":false},\"Verified\":false,\"w\":1,\"h\":1,\"StackSize\":null,\"Icon\":\"https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvSmV3ZWxzL05ld0dlbUJhc2UzIiwidyI6MSwiaCI6MSwic2NhbGUiOjF9XQ/db35e60885/NewGemBase3.png\",\"Note\":null,\"BuiltInSupport\":null,\"Requirements\":[],\"Properties\":[],\"AdditionalProperties\":[],\"implicitMods\":[],\"craftedMods\":[],\"explicitMods\":[],\"utilityMods\":[],\"pseudoMods\":[],\"enchantMods\":[\"Adds 8 Passive Skills\",\"2 Added Passive Skills are Jewel Sockets\",\"Added Small Passive Skills grant: 12% increased Chaos Damage\"],\"runeMods\":[],\"fracturedMods\":[],\"desecratedMods\":[],\"scourgeMods\":[],\"sanctumMods\":[],\"mutatedMods\":[],\"GemSockets\":[],\"Sockets\":[],\"extended\":null,\"LogbookMods\":[],\"grantedSkills\":[],\"HasStats\":true},{\"Id\":\"0e6a102b3d532a8837792df1dbd5b34e9de332083e70b28d7a40403139cc4daf\",\"Name\":\"Blight Ruin\",\"typeLine\":\"Small Cluster Jewel\",\"baseType\":\"Small Cluster Jewel\",\"Type\":\"Small Cluster Jewel\",\"Identified\":true,\"ilvl\":83,\"frameType\":2,\"foilVariation\":null,\"Corrupted\":false,\"Mutated\":false,\"Scourged\":{\"Tier\":0},\"Fractured\":false,\"Sanctified\":false,\"duplicated\":false,\"IsRelic\":false,\"Influences\":{\"Crusader\":false,\"Elder\":false,\"Hunter\":false,\"Redeemer\":false,\"Shaper\":false,\"Warlord\":false},\"Verified\":false,\"w\":1,\"h\":1,\"StackSize\":null,\"Icon\":\"https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvSmV3ZWxzL05ld0dlbUJhc2UxIiwidyI6MSwiaCI6MSwic2NhbGUiOjF9XQ/0eb1a9d981/NewGemBase1.png\",\"Note\":null,\"BuiltInSupport\":null,\"Requirements\":[{\"Name\":\"Level\",\"Icon\":null,\"values\":[[\"62\",0]],\"DisplayMode\":0,\"type\":62}],\"Properties\":[],\"AdditionalProperties\":[],\"implicitMods\":[],\"craftedMods\":[],\"explicitMods\":[\"Added Small Passive Skills also grant: \\u002B6% to Fire Resistance\",\"Added Small Passive Skills also grant: 8% increased Mana Regeneration Rate\",\"Added Small Passive Skills also grant: \\u002B7 to Maximum Life\"],\"utilityMods\":[],\"pseudoMods\":[],\"enchantMods\":[\"Adds 3 Passive Skills\",\"Added Small Passive Skills grant: 6% increased Mana Reservation Efficiency of Skills\"],\"runeMods\":[],\"fracturedMods\":[],\"desecratedMods\":[],\"scourgeMods\":[],\"sanctumMods\":[],\"mutatedMods\":[],\"GemSockets\":[],\"Sockets\":[],\"extended\":null,\"LogbookMods\":[],\"grantedSkills\":[],\"HasStats\":true}]";

    [Fact]
    public async Task ShockNovaOfProcession()
    {
        var items = JsonSerializer.Deserialize<List<ApiItem>>(Data);
        Assert.NotNull(items);
        Assert.Equal(4, items.Count);
        await AssertItem(items[0], "shock-nova-of-procession-20-20c");
    }

    [Fact]
    public async Task FoulbornGloves()
    {
        var items = JsonSerializer.Deserialize<List<ApiItem>>(Data);
        Assert.NotNull(items);
        Assert.Equal(4, items.Count);
        Assert.Equal(4, items[1].MaxLinks);
        await AssertItem(items[1], "foulborn-voidbringer-corrupted-chaos-conjurer-gloves");
    }

    [Fact]
    public async Task LargeCluster()
    {
        var items = JsonSerializer.Deserialize<List<ApiItem>>(Data);
        Assert.NotNull(items);
        Assert.Equal(4, items.Count);
        await AssertItem(items[2], "12-increased-chaos-damage-8-passives-75");
    }

    [Fact]
    public async Task SmallCluster()
    {
        var items = JsonSerializer.Deserialize<List<ApiItem>>(Data);
        Assert.NotNull(items);
        Assert.Equal(4, items.Count);
        await AssertItem(items[3], "6-increased-mana-reservation-efficiency-of-skills-3-passives-75");
    }

    private async Task AssertItem(ApiItem item, string expectedDetailsId)
    {
        var itemDefinition = fixture.ItemDefinitionParser.Get(item);
        var invariantDefinition = itemDefinition?.Key != null ? fixture.ItemDefinitionParser.InvariantDictionary.GetValueOrDefault(itemDefinition.Key) : null;
        Assert.NotNull(invariantDefinition);

        var results = await fixture.NinjaStashProvider.GetInfo(invariantDefinition, item);
        Assert.Single(results);
        Assert.Equal(expectedDetailsId, results[0].DetailsId);
    }
}
