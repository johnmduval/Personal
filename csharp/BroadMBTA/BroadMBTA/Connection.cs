using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroadMBTA
{
    public class Connection
    {
        public Connection(Stop stop, Route route1, Route route2)
        {
            Stop = stop;
            Route1 = route1;
            Route2 = route2;
        }

        public Stop Stop { get; init; }
        public Route Route1 { get; init; }
        public Route Route2 { get; init; }

        public Route GetOtherRoute(Route route)
        {
            return route.ID == Route1.ID ? Route2 : Route1;
        }
        public override string ToString()
        {
            return $"Connection({this.Stop} [{this.Route1} <--> {this.Route2}])";
        }
    }
}
