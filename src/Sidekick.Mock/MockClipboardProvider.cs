using System;
using System.Threading.Tasks;
using Sidekick.Common.Platform;

namespace Sidekick.Mock;

public class MockClipboardProvider : IClipboardProvider
{
    public Task<string?> Copy()
    {
        throw new NotImplementedException();
    }

    public Task<string> CopyAdvanced()
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetText()
    {
        throw new NotImplementedException();
    }

    public Task SetText(string? text)
    {
        return Task.CompletedTask;
    }
}
