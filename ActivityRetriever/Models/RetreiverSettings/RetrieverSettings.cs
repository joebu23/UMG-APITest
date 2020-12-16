using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityRetriever.Models.RetrieverSettings
{
    public class RetrieverSettings
    {
        public Login Login { get; set; }
        public CrmInfo CrmInfo { get; set; }
    } 
}
