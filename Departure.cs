using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace NextBuses
{
	public class DepartureBoardWrapper
	{
		[JsonPropertyName("DepartureBoard")]
		public DepartureBoard Board { get; set; }
		// Display the next 5 departures
		public string display(HashSet<string> line, HashSet<string> direction)
		{
			var next = Board.Departures.Where(d => line.Contains(d.Line) && direction.Contains(d.Direction)).Take(5);
			Departure first = next.First();
			string display = $"{first.Stop} ";
			foreach (Departure departure in next)
			{
				display += $"{departure.Line}:{departure.Time} ";
			}
			return display;
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