using ActivityRetriever.Models;
using ActivityRetriever.Models.AnytimeCollect;
using ActivityRetriever.Models.RetrieverSettings;
using ActivityRetriever.Services.AnytimeCollect;
using ActivityRetriever.Services.CRM;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityRetriever.Services.ActivitySync
{
    public class ActivitySyncService : IActivitySyncService
    {
        private ILog _log;
        private RetrieverSettings _settings;
        private ICrmService _crmService;
        private IAnytimeCollectService _anytimeCollectService;

        public ActivitySyncService(ILog log, RetrieverSettings settings, ICrmService crmService, IAnytimeCollectService anytimeCollectService)
        {
            _log = log;
            _settings = settings;
            _crmService = crmService;
            _anytimeCollectService = anytimeCollectService;
        }

        public async Task SyncCrmActivity()
        {
            List<CrmAccount> accountsToUpdate = _crmService.GetActiveAccounts();

            foreach (CrmAccount account in accountsToUpdate)
            {
                DateTime lastActivityDate = new DateTime(0001, 1, 1);

                var lastAccountActivity = account.FinanceActivities.OrderByDescending(c => c.ContactDate).FirstOrDefault();

                if (lastAccountActivity != null)
                {
                    lastActivityDate = lastAccountActivity.ContactDate;
                }

                AtcResponse response = await _anytimeCollectService.GetActivitiesByAccount(account.AtcAccountNumber, lastActivityDate);

                if (response.Success)
                {
                    _log.Info($"Found {response.Activities.Count} activities for {account.Name}");
                    foreach (AtcActivity activity in response.Activities)
                    {
                        var isActivityAlreadySaved = account.FinanceActivities.Where(x => x.AtcActivityId == activity.ActivityId).FirstOrDefault();

                        if (isActivityAlreadySaved == null)
                        {
                            _crmService.CreateNewActivity(account.Id, activity);
                        }
                    }
                }
            }
        }
    }
}
