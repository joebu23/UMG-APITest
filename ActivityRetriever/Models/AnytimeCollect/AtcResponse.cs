using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityRetriever.Models.AnytimeCollect
{
    public class AtcResponse
    {
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public List<AtcActivity> Activities { get; set; }
        public bool Success { get; set; }
        public string Response { get; set; }
    }
}
