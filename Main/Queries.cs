using System;
using System.Net.Http;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;

namespace NextBuses
{
    public interface IQuery<T>
    {
        public T? execute();
    }
    public abstract class Query<T> : IQuery<T>
    {
        private HttpClient client;
        public string url = "";
        public Query()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://xmlopen.rejseplanen.dk/bin/rest.exe/");
        }

        public T execute()
        {
            HttpResponseMessage result = client.GetAsync(url).Result;
            string response = result.Content.ReadAsStringAsync().Result;
            T? value = JsonSerializer.Deserialize<T>(response);
            if (value is null)
            {
                throw new InvalidDataException($"The response has not been deserialized properly. {result}");
            }
            return value;
        }
    }
    public class QueryDepartureBoard : Query<DepartureBoardWrapper>
    {
        Dictionary<TransportType, string> exclusionMap = new Dictionary<TransportType, string>()
        {
            { TransportType.Bus, "useBus=0" },
            { TransportType.Train, "useTrain=0" },
            { TransportType.Metro, "useMetro=0" }

        };
        public QueryDepartureBoard(string stopid, List<TransportType>? excludedTransportTypes) : base()
        {
            string use = "";
            if (excludedTransportTypes != null)
            {
                foreach (TransportType transport in excludedTransportTypes)
                {
                    use += exclusionMap[transport];
                }
            }
            url = $"departureBoard?id={stopid}{use}&format=json";
        }
    }
}