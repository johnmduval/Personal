using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroadMBTA
{
    public class JourneyLeg
    {
        public JourneyLeg(Route destinationRoute, Connection connection, JourneyLeg parent)
        {
            this.DestinationRoute = destinationRoute;
            this.Parent = parent;
            this.Connection = connection;
        }

        public Route DestinationRoute { get; init; }
        public JourneyLeg Parent { get; init; }
        public Connection Connection { get; init; }
    }

    public class PathFinder
    {
        public PathFinder(Map map)
        {
            this.Map = map;
            this.LegsToTry = new Queue<JourneyLeg>();
            this.RoutesAlreadyVisited = new List<string>();
        }

        private Map Map { get; init; }
        private Queue<JourneyLeg> LegsToTry { get; init; }
        private List<string> RoutesAlreadyVisited { get; init; }
        public JourneyLeg FindConnections(Route currentRoute, Route destinationRoute)
        {
            var alreadyVisited = new HashSet<string>();
            var q = new Queue<Route>();
            q.Enqueue(currentRoute);
            
            // Keep list of steps in the journey; the first step has no connection/parent
            var legs = new List<JourneyLeg>();
            legs.Add(new JourneyLeg(currentRoute, null, null));

            while (true)
            {
                if (q.Count == 0)
                    break;
                currentRoute = q.Dequeue();
                alreadyVisited.Add(currentRoute.ID);
                //Console.WriteLine($"Current route: {currentRoute.Name}");
                var connections = this.Map.GetConnectionsForRoute(currentRoute);
                foreach (var connection in connections)
                {
                    var childRoute = connection.GetOtherRoute(currentRoute);
                    if (alreadyVisited.Contains(childRoute.ID))
                        continue;
                    //Console.WriteLine($"\tNot yet visited: {childRoute.Name}");
                    var previousLeg = legs.Single(l => l.DestinationRoute == currentRoute);
                    var leg = new JourneyLeg(childRoute, connection, previousLeg);
                    legs.Add(leg);
                    if (childRoute.ID == destinationRoute.ID)
                        return leg;

                    if (!q.Contains(childRoute))
                        q.Enqueue(childRoute);
                }
            }

            return null;
        }
    }
}
