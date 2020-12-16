using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityRetriever.Models.AnytimeCollect
{
    public class AtcActivity
    {
        public string ActivityId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Contact { get; set; }
        public string Type { get; set; }
        public bool IsClosed { get; set; }
        public bool IsCall { get; set; }
        public DateTime ActivityDate { get; set; }
        public string Note { get; set; }
    }
}
