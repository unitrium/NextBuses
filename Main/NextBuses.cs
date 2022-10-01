using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace NextBuses
{
    public static class AppFunction
    {
        [FunctionName("NextBuses")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Body data = JsonSerializer.Deserialize<Body>(requestBody);
            string responseMessage = "";
            foreach (RequestedStop stop in data.RequestedStops)
            {
                try
                {
                    QueryDepartureBoard query = new QueryDepartureBoard(stop.StopID, stop.ExcludedTransportTypes);
                    DepartureBoardWrapper departues = query.execute();
                    var display = new TTGODisplay();
                    responseMessage += departues.display(display, new HashSet<string>(stop.Lines), new HashSet<string>(stop.Directions));
                }
                catch (Exception ex)
                {
                    responseMessage += $"Error while querying stop {stop.StopID}.";
                    log.LogError(ex.Message);
                }
            }
            return new OkObjectResult(responseMessage);
        }
    }
}

