using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityRetriever.Models
{
    public class CrmAccount
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AtcAccountNumber { get; set; }
        public List<CrmFinanceActivity> FinanceActivities { get; set; }
        //public DateTime LastAtcActivityInCrm { get; set; }
    }
}
