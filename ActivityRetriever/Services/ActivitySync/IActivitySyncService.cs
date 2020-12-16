using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityRetriever.Services.ActivitySync
{
    public interface IActivitySyncService
    {
        Task SyncCrmActivity();
    }
}
