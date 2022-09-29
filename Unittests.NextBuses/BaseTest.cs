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
                    Directions = new List<string>{"Københaven H." },
                    Lines = new List<string>{"100" }
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
        Mock<IQuery<DepartureBoardWrapper>> query = new Mock<IQuery<DepartureBoardWrapper>>();
        query.Setup(q => q.execute()).Returns(new DepartureBoardWrapper()
        {
            Board = new DepartureBoard()
            {
                Departures = new List<Departure>()
                {
                    new Departure()
                    {
                        Name = "BusBus",
                        Direction = "Københaven H.",
                        Time = "11.05",
                        Type = "Bus",
                        Line = "100",
                        Stop = "TestStop"
                    }
                }
            }
        });
        string responseMessage = "";
        foreach (RequestedStop stop in body.RequestedStops)
        {
            DepartureBoardWrapper departues = query.Object.execute();
            responseMessage += departues.display(new HashSet<string>(stop.Lines), new HashSet<string>(stop.Directions));
        }

        Assert.AreEqual("TestStop :\n100:11.05 \n", responseMessage);
    }
}
