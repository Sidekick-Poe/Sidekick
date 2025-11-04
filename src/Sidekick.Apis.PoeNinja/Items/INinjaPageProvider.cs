namespace Sidekick.Apis.PoeNinja.Items;

public interface INinjaPageProvider
{
    Task Download(string dataFolder, string poe1League, string poe2League);
}
