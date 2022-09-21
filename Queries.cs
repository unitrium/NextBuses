using System;
using System.Net.Http;
using System.Text.Json;
using System.IO;

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
        public QueryDepartureBoard(string stopid, bool includeTrains = true, bool includeBus = true) : base()
        {
            string useBus = includeBus ? "" : "&useBus=0";
            string useTrains = includeTrains ? "" : "&includeTrain=0";
            url = $"departureBoard?id={stopid}{useBus}{useTrains}&format=json";
        }
    }
}