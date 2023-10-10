using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidekick.Common.Platform.Interprocess
{
    public interface IInterprocessService
    {
        static event Action<string[]> OnMessage;

        void ReceiveMessage(String[] args);

        void Start();
    }
}
