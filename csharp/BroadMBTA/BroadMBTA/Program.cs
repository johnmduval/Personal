using BroadMBTA;

var map = new Map(loadFromApi: false);
//map.Dump();

var startStop = map.FindStop("Orange Line", "Oak Grove");
var endStop = map.FindStop("Green Line B", "Kenmore");

var pathFinder = new PathFinder(map);

var journeyLeg = pathFinder.FindConnections(startStop.ParentRoute, endStop.ParentRoute);

// move inside FindConnections?
var legsInOrder = new List<JourneyLeg>();
while (journeyLeg != null)
{
    legsInOrder.Insert(0, journeyLeg);
    journeyLeg = journeyLeg.Parent;
}

Stop currentStop = startStop;

foreach (var leg in legsInOrder)
{
    if (leg.Connection == null)
        continue;
    else
    {
        Console.WriteLine($"\t(Switch {leg.Connection.Route1.Name}/{leg.Connection.Route2.Name}");
        var stops = currentStop.ParentRoute.GetStopsBetween(currentStop, leg.Connection.Stop);
        foreach (var stop in stops)
        {
            Console.WriteLine($"\t\t{stop.Name}");
        }
        currentStop = leg.Connection.Stop;
    }
}
Console.WriteLine("----------------");
Console.WriteLine("SOLUTION:");
Console.WriteLine($"From: {startStop}");
Console.WriteLine($"To:   {endStop}");

var output = new List<string>();
while (journeyLeg != null)
{
    if (journeyLeg.Connection == null)
        output.Insert(0, $"Start at {journeyLeg.DestinationRoute.Name}");
    else
        output.Insert(0, $"Get to {journeyLeg.DestinationRoute.Name} from connection at {journeyLeg.Connection.Stop.Name}");
    
    journeyLeg = journeyLeg.Parent;
}

output.ForEach(e => Console.WriteLine(e));