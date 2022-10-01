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
		public string display(IBoardDisplay display, HashSet<string> line, HashSet<string> direction)
		{
			return display.render(filterDepartures(line, direction));
		}
		private List<Departure> filterDepartures(HashSet<string> line, HashSet<string> direction)
        {
			if (Board.Departures.Count() == 0)
			{
				throw new InvalidDataException("Error no departure at this stop.");
			}
			return Board.Departures.Where(d => line.Contains(d.Line) && direction.Contains(d.Direction)).ToList();
        }
	}
	public class DepartureBoard {
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