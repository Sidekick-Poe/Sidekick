using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidekick.Common.Platform.Interprocess
{
    public interface IInterprocessService
    {
        void CustomProtocolCallback(Action<string[]> callback);
        void CustomProtocol(String[] args);
        void Start();
    }
}
