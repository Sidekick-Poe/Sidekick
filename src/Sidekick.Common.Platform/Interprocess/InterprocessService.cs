namespace Sidekick.Common.Platform.Interprocess;

public class InterprocessService : IInterprocessService
{
    private const string APPLICATION_PROCESS_GUID = "93c46709-7db2-4334-8aa3-28d473e66041";

    private Mutex? mutex;
    private bool isMainInstance;

    public bool IsAlreadyRunning()
    {
        mutex ??= new Mutex(true, APPLICATION_PROCESS_GUID, out isMainInstance);
        return !isMainInstance;
    }

}
