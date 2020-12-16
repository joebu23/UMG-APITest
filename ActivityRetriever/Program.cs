using System.IO;
using System.Threading.Tasks;
using ActivityRetriever.Models.RetrieverSettings;
using ActivityRetriever.Services.ActivitySync;
using ActivityRetriever.Services.AnytimeCollect;
using ActivityRetriever.Services.CRM;
using log4net;
using Newtonsoft.Json;

namespace ActivityRetriever
{
    class Program
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static async Task Main(string[] args)
        {
            RetrieverSettings settings = new RetrieverSettings();
            using (var reader = new StreamReader(Directory.GetCurrentDirectory() + "/retrieversettings.json"))
            {
                settings = JsonConvert.DeserializeObject<RetrieverSettings>(reader.ReadToEnd());
            }

            var crmService = new CrmService(log, settings);
            var anytimeCollectService = new AnytimeCollectService(log);
            var activitySyncService = new ActivitySyncService(log, settings, crmService, anytimeCollectService);

            await activitySyncService.SyncCrmActivity();
        }
    }
}
