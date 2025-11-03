namespace Sidekick.Apis.PoeNinja.Items;

public interface INinjaPageProvider
{
    Task Download(string dataFolder);
}
