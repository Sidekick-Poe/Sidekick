namespace Sidekick.Apis.Poe.Trade.Models;

public static class LineContentTypeExtensions
{
    public static string GetColour(this LineContentType type)
    {
        return type switch
        {
            LineContentType.Augmented => "text-[#8888FF]",
            LineContentType.Fire => "text-[#960000]",
            LineContentType.Cold => "text-[#366492]",
            LineContentType.Chaos => "text-[#D02090]",
            LineContentType.Lightning => "text-[#FFD700]",
            LineContentType.BlockedIncursionRoom => "text-[#5A5A5A]",
            _ => "text-[#FFFFFF]",
        };
    }
}
