using NUnit.Framework;
using BroadMBTA;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;

namespace BroadMBTATest
{
    public class RouteTests
    {
        private IEnumerable<Route> routeList; 
        [SetUp]
        public void Setup()
        {
            using (var client = new HttpClient())
            {
                var domain = "https://api-v3.mbta.com";

                this.routeList = Route.GetRoutes(client, domain);
                Stop.GetStops(client, domain, routeList);
            }
        }

        [Test]
        public void Route_GetStopsBetween_Forwards()
        {
            var redLine = this.routeList.First(e => e.Name == "Red Line");
            var alewife = redLine.Stops.First(s => s.Name == "Alewife");
            var harvard = redLine.Stops.First(s => s.Name == "Harvard");
            var stops = redLine.GetStopsBetween(alewife, harvard);
            Assert.AreEqual(4, stops.Count());
            Assert.AreEqual("Alewife", stops.First().Name);
            Assert.AreEqual("Harvard", stops.Last().Name);
        }

        [Test]
        public void Route_GetStopsBetween_Backwards()
        {
            var redLine = this.routeList.First(e => e.Name == "Red Line");
            var alewife = redLine.Stops.First(s => s.Name == "Harvard");
            var harvard = redLine.Stops.First(s => s.Name == "Alewife");
            var stops = redLine.GetStopsBetween(alewife, harvard);
            Assert.AreEqual(4, stops.Count());
            Assert.AreEqual("Harvard", stops.First().Name);
            Assert.AreEqual("Alewife", stops.Last().Name);
        }

    }
}