using NextBuses;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System;

namespace Unittests.NextBuses;

[TestClass]
public class BaseTests
{
    Body body;

    [TestInitialize]
    public void Init()
    {
        body = new Body()
        {
            RequestedStops = new List<RequestedStop>()
            {
                new RequestedStop()
                {
                    StopID = "10",
                    Directions = new List<string>{"København H." },
                    Lines = new List<string>{"B" }
                },
                new RequestedStop()
                {
                    StopID = "10",
                    Directions = new List<string>{"Test" },
                    Lines = new List<string>{"A", "C" }
                }
            }
        };
    }
    [TestMethod]
    public void TestNormalRequest()
    {
        byte[] stream = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(body));
        var request = new Mock<HttpRequest>();
        request.Setup(r => r.Body).Returns(new MemoryStream(stream));
        var logger = new Mock<ILogger>();
        var result = AppFunction.Run(request.Object, logger.Object).Result;
    }
    [TestMethod]
    public void TestProcessing()
    {
        string testStop = "testStop";
        List<Departure> departures = GenerateDepartures(
                    new List<string>() { "A", "B", "C", "D" },
                    new List<string>() { "København H.", "Nørreport St.", "Test" },
                    stop:testStop);
        Mock<IQuery<DepartureBoardWrapper>> query = new Mock<IQuery<DepartureBoardWrapper>>();
        query.Setup(q => q.execute()).Returns(new DepartureBoardWrapper()
        {
            Board = new DepartureBoard()
            {
                Departures = departures
            }
        });
        string responseMessage = "";
        foreach (RequestedStop stop in body.RequestedStops)
        {
            DepartureBoardWrapper departues = query.Object.execute();
            responseMessage += departues.display(new HashSet<string>(stop.Lines), new HashSet<string>(stop.Directions));
        }
        var b_departures = departures.Where(d => d.Line == "B" && d.Direction == "København H.");
        var a_departures = departures.Where(d => (d.Line == "A" || d.Line == "C") && d.Direction == "Test");
        string b_message = GenerateMessage(b_departures.ToList(), testStop);
        string a_c_message = GenerateMessage(a_departures.ToList(), testStop);
        Assert.AreEqual($"{b_message}{a_c_message}", responseMessage);
    }
    public List<Departure> GenerateDepartures(
        List<string> lines, List<string> directions, int number = 19, string stop = "TestStop", string transportType = "Bus"
        )
    {
        Random random = new Random();
        List<Departure> departures = new List<Departure>();
        for (int i = 0; i< number; i++)
        {

            departures.Add(new Departure()
            {
                Stop = stop,
                Line = lines[random.Next(lines.Count())],
                Direction = directions[random.Next(directions.Count())],
                Type = transportType,
                Name = "SomeName",
                Time = $"{random.Next(2)}{random.Next(10)}:{random.Next(6)}{random.Next(10)}"
            });
        }
        return departures;

    }
    public string GenerateMessage(List<Departure> departures, string stopName)
    {
        string message = $"{stopName} :\n";
        if (departures.Count() == 0)
        {
            return message + "No departure for line or direction.\n";
        }
        foreach (var group in departures.GroupBy(d=>d.Line))
        {
            message += $"{group.Key}: ";
            foreach (Departure departure in group)
            {
                message += $"{departure.Time} ";
            }
        }
        return message + "\n";
    }
}
