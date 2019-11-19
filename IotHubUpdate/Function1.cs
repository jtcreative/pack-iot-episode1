using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Newtonsoft.Json.Linq;

namespace IotHubUpdate
{
    public static class Function1
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("UpdateHub")]
        public static void Run([EventGridTrigger]JObject eventGridEvent, ILogger log)
        {
           // log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");
        }
    }
}