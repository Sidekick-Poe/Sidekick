namespace Sidekick.Common.Platform.Interprocess;

public class InterprocessService : IInterprocessService
{
#if DEBUG
    private const string APPLICATION_PROCESS_GUID = "a849007e-44a1-4eac-8cf4-721f3e34c1e8";
#else
    private const string APPLICATION_PROCESS_GUID = "93c46709-7db2-4334-8aa3-28d473e66041";
#endif

    private Mutex? mutex;
    private bool isMainInstance;

    public bool IsAlreadyRunning()
    {
        mutex ??= new Mutex(true, APPLICATION_PROCESS_GUID, out isMainInstance);
        return !isMainInstance;
    }

}
