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
                    Lines = new List<string>{"B" },
                    ExcludedTransportTypes = new List<TransportType>{0}
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
            TTGODisplay display = new TTGODisplay();
            responseMessage += departues.display(display, new HashSet<string>(stop.Lines), new HashSet<string>(stop.Directions));
        }
        Assert.AreNotEqual("", responseMessage);
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
}
