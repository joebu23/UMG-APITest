using ActivityRetriever.Models.AnytimeCollect;
using log4net;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ActivityRetriever.Services.AnytimeCollect
{
    public class AnytimeCollectService : IAnytimeCollectService
    {
        private ILog _log;

        /// <summary>
        /// Instantiates a new instance of the AnytimeCollectService class
        /// </summary>
        /// <param name="log"></param>
        public AnytimeCollectService(ILog log)
        {
            _log = log;
        }

        /// <summary>
        /// Gets all activities via an ATC account number and a date filter
        /// </summary>
        /// <param name="account">ATC account value to search for</param>
        /// <param name="dateTimeFrom">DateTime to start searching from</param>
        /// <returns>Response object showing success/failure and list of Activity objects returned</returns>
        public async Task<AtcResponse> GetActivitiesByAccount(string account, DateTime dateTimeFrom)
        {
            AtcResponse response = new AtcResponse();

            HttpClient client = new HttpClient();
            string clientCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("testtest" + ":" + "testtesttest"));

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", clientCredentials);

            //string path = $"http://zjq4d.mocklab.io/12345asdf/activities?datestart=8-7-2020";
            string formattedDate = dateTimeFrom.ToString("M-d-yyyy");

            string path = $"http://zjq4d.mocklab.io/{account}/activities?datestart={formattedDate}";

            _log.Info($"Getting ATC Activities for: {account} starting on {dateTimeFrom}");

            HttpResponseMessage httpResponse = await client.GetAsync(path);
            if (httpResponse.IsSuccessStatusCode)
            {
                var responseString = await httpResponse.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<AtcResponse>(responseString);
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.Response = "Error: There was a problem getting data from the AnytimeCollectService";
            }

            return response;
        }
    }
}
