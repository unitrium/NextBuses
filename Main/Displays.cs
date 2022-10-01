using System;
using System.Linq;
using System.Collections.Generic;

namespace NextBuses
{
    public interface IBoardDisplay
    {
        public string render(List<Departure> departures);
        public int maxStopNameSize { get; }
        public int maxDepartures { get; }
    }
    public class GenericDisplay : IBoardDisplay
    {
        public int maxStopNameSize { get; }
        public int maxDepartures { get; }
        public GenericDisplay(int maxStopNameSize, int maxDepartures)
        {
            this.maxStopNameSize = maxStopNameSize;
            this.maxDepartures = maxDepartures;
        }
        public string render(List<Departure> departures)
        {
            return "";
        }
    }
    public class TTGODisplay : IBoardDisplay
    {
        // A display for the TTGO board displays lines horizontally.
        public int maxStopNameSize { get; } = 10;
        public int maxDepartures { get; } = 3;
        public string render(List<Departure> departures)
        {
            var first = departures.First();
            int stringLimit = first.Stop.Count() > maxStopNameSize ? maxStopNameSize : first.Stop.Count();
            string display = $"{first.Stop.Substring(0, stringLimit)} :\n";
            if (departures.Count() == 0)
            {
                return display + "No departure for line or direction.\n";
            }
            List<string> textMatrix = new List<string>() {""};
            var groupings = departures.Take(maxDepartures).GroupBy(d => d.Line);
            foreach (var grouping in groupings)
            {
                textMatrix[0] += $"{grouping.Key}:  ";
                int i = 1;
                foreach (Departure departure in grouping)
                {
                    if (textMatrix.Count() <= i)
                    {
                        textMatrix.Add("");
                    }
                    textMatrix[i] += $"{departure.GetTimeSpan().Minutes} min";
                    i++;
                }
            }
            foreach (string line in textMatrix)
            {
                display += line + "\n";
            }
            return display;
        }
    }
}
