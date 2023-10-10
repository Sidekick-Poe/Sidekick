using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidekick.Common.Platform.Interprocess
{
    public interface IInterprocessClient
    {
        void SendMessage(string[] args);
        void Start();
    }
}
