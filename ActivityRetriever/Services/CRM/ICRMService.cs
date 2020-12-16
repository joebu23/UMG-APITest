using ActivityRetriever.Models;
using ActivityRetriever.Models.AnytimeCollect;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityRetriever.Services.CRM
{
    public interface ICrmService
    {
        List<CrmAccount> GetActiveAccounts();
        void CreateNewActivity(string accountId, AtcActivity activity);
    }
}
