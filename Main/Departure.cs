using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;
namespace NextBuses
{
	public class DepartureBoardWrapper
	{
		[JsonPropertyName("DepartureBoard")]
		public DepartureBoard Board { get; set; }
		// Display the next 5 departures
		public string display(HashSet<string> line, HashSet<string> direction, int stopNameLimit = 10, int maxDepartures = 5)
		{
			if (Board.Departures.Count() == 0)
            {
				throw new InvalidDataException("Error no departure at this stop.");
            }
			Departure first = Board.Departures.First();
			int stringLimit = first.Stop.Count() > stopNameLimit ? stopNameLimit : first.Stop.Count();
			string display = $"{first.Stop.Substring(0, stringLimit)} :\n";
			var departures = Board.Departures.Where(d => line.Contains(d.Line) && direction.Contains(d.Direction)).Take(maxDepartures).GroupBy(d => d.Line);
			if (departures.Count() == 0)
            {
				return display + "No departure for line or direction.\n";
            }
			foreach (var grouping in departures)
            {
				display+= $"{grouping.Key}: ";
				foreach (Departure departure in grouping)
                {
					display += $"{departure.Time} ";
				}
            }
			return display + "\n";
		}
	}
	public class DepartureBoard
	{
		[JsonPropertyName("Departure")]
		public List<Departure> Departures { get; set; }
	}

	public class Departure
	{
		[JsonPropertyName("name")]
		public string Name { get; set; }
		[JsonPropertyName("type")]
		public string Type { get; set; }
		[JsonPropertyName("time")]
		public string Time { get; set; }
		[JsonPropertyName("stop")]
		public string Stop { get; set; }
		[JsonPropertyName("line")]
		public string Line { get; set; }
		[JsonPropertyName("direction")]
		public string Direction { get; set; }
	}
}