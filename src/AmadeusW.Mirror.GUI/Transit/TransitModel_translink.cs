using AmadeusW.Mirror.GUI.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace AmadeusW.Mirror.GUI.Transit
{
    internal class TransitModel_translink : TransitModel
    {
        private string _apiKey;

        public async Task<string> GetRawTransitData(string stopNumber, string routeNumber)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var request = new HttpRequestMessage(HttpMethod.Get, $"http://api.translink.ca/RTTIAPI/V1/stops/{stopNumber}/estimates?apiKey={_apiKey}&routeNo={routeNumber}");
                var response = await client.SendAsync(request);
                var message = await response.Content.ReadAsStringAsync();
                return message;
            }
        }

        public TransitModel_translink() : base()
        {
            try
            {
                _apiKey = SettingsController.Settings.TranslinkApi.ToString();
                dynamic routes = SettingsController.Settings.TransitRoutes;
                var transitLines = new List<TransitLine>();
                foreach (var route in routes)
                {
                    transitLines.Add(new TransitLine_translink(
                        route.apiStopNumber.ToString(),
                        route.apiRouteNumber.ToString(),
                        route.stopName.ToString(),
                        route.routeNumber.ToString(),
                        Int32.Parse(route.timeToWalk.ToString())));
                }
                this.Lines = transitLines;
            }
            catch (Exception ex)
            {
                var t = ex;
            }
        }

        public override async Task Update()
        {

            foreach (var line in Lines)
            {
                var translinkLine = line as TransitLine_translink;
                var rawData = await GetRawTransitData(translinkLine.ApiStopNumber, translinkLine.ApiRouteNumber);
                translinkLine.UpdateWithRawData(rawData);
            }
        }
    }
}
