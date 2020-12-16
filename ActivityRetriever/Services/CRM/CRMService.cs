using ActivityRetriever.Models;
using ActivityRetriever.Models.AnytimeCollect;
using ActivityRetriever.Models.RetrieverSettings;
using log4net;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ActivityRetriever.Services.CRM
{
    public class CrmService : ICrmService
    {
        private IOrganizationService _crmServiceClient;
        private ILog _log;
        private RetrieverSettings _settings;

        /// <summary>
        /// Instantiates instance of the CrmService class
        /// </summary>
        /// <param name="log">ILog object for logging</param>
        /// <param name="settings">RetrieverSettings object to use</param>
        public CrmService(ILog log, RetrieverSettings settings)
        {
            _log = log;
            _settings = settings;

            string conn = $@"
                    Url = {_settings.Login.Url};
                    AuthType = Office365;
                    UserName = {_settings.Login.Username};
                    Password = {_settings.Login.Password};
                    RequireNewInstance = True";

            log.Info($"Connecting with {_settings.Login.Url} and username {_settings.Login.Username}");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            CrmServiceClient svc = new CrmServiceClient(conn);
            _crmServiceClient = svc.OrganizationServiceProxy;
        }

        /// <summary>
        /// Gets Active Accounts in CRM - Active defined by an active account in CRM with an account number for Anytime Collect. Also gets all Finance Activity objects for the account
        /// </summary>
        /// <returns>List of CrmAccount objects</returns>
        public List<CrmAccount> GetActiveAccounts()
        {
            List<CrmAccount> returnAccounts = new List<CrmAccount>();
            try
            {
                var query = new QueryExpression("account")
                {
                    ColumnSet = new ColumnSet("name", _settings.CrmInfo.AtcIdFieldOnAccount),
                    Criteria = new FilterExpression(LogicalOperator.And)
                };

                query.Criteria.AddCondition(_settings.CrmInfo.AtcIdFieldOnAccount, ConditionOperator.NotNull);
                query.Criteria.AddCondition("statecode", ConditionOperator.Equal, "Active");

                EntityCollection results = _crmServiceClient.RetrieveMultiple(query);
                _log.Info($"Found {results.TotalRecordCount} accounts that could be updated");

                returnAccounts.AddRange(results.Entities.Select(x => new CrmAccount()
                {
                    AtcAccountNumber = x.Attributes[_settings.CrmInfo.AtcIdFieldOnAccount].ToString(),
                    Id = x.Attributes["accountid"].ToString(),
                    Name = x.Attributes["name"].ToString()
                }));

                foreach (CrmAccount account in returnAccounts)
                {
                    account.FinanceActivities = new List<CrmFinanceActivity>();
                    _log.Info($"Looking for saved ATC activities for account: {account.Name}, ATC account Number: {account.AtcAccountNumber}");
                    var activityQuery = new QueryExpression("new_financeactivity")
                    {
                        ColumnSet = new ColumnSet("activityid", "new_contactdate", "new_atcactivityid")
                    };

                    activityQuery.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, account.Id);

                    EntityCollection activityResults = _crmServiceClient.RetrieveMultiple(activityQuery);
                    foreach (Entity activity in activityResults.Entities)
                    {
                        account.FinanceActivities.Add(new CrmFinanceActivity()
                        {
                            AtcActivityId = activity["new_atcactivityid"].ToString(),
                            ContactDate = DateTime.Parse(activity["new_contactdate"].ToString())
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Error in Getting active accounts - Error: {ex.Message}");
            }

            return returnAccounts;
        }

        /// <summary>
        /// Creates a new Finance Activity object in CRM for an account
        /// </summary>
        /// <param name="accountId">Guid Id for Account in CRM</param>
        /// <param name="activity">AtcActivity object to save in CRM</param>
        public void CreateNewActivity(string accountId, AtcActivity activity)
        {
            _log.Info($"Creating new activity for account: {accountId} from: {activity.ActivityDate}");
            var newActivity = new Entity("new_financeactivity");

            newActivity["subject"] = activity.Subject;
            newActivity["new_actiontype"] = activity.Type == "Email" ? new OptionSetValue(100000000) : new OptionSetValue(100000001);
            newActivity["new_contactdate"] = activity.ActivityDate;
            newActivity["new_contactperson"] = activity.Contact;
            newActivity["new_financeactivitytype"] = new OptionSetValue(100000000);
            newActivity["new_note"] = activity.Note;
            newActivity["new_atcactivityid"] = activity.ActivityId;
            newActivity["regardingobjectid"] = new EntityReference("account", new Guid(accountId));

            Guid activityId = _crmServiceClient.Create(newActivity);

            SetStateRequest setStateRequest = new SetStateRequest();
            setStateRequest.EntityMoniker = new EntityReference("new_financeactivity", activityId);
            setStateRequest.State = new OptionSetValue(1);
            setStateRequest.Status = new OptionSetValue(2);
            SetStateResponse setStateResponse = (SetStateResponse)_crmServiceClient.Execute(setStateRequest);
        }
    }
}
