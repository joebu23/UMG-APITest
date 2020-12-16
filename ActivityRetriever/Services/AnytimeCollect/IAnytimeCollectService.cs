using ActivityRetriever.Models.AnytimeCollect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ActivityRetriever.Services.AnytimeCollect
{
    public interface IAnytimeCollectService
    {
        Task<AtcResponse> GetActivitiesByAccount(string account, DateTime dateTimeFrom);
    }
}
