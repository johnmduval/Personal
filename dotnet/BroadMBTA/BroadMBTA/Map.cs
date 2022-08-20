using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroadMBTA
{
    public class Map
    {
        private class RouteAndStopJsonString
        {
            public RouteAndStopJsonString(Route route, string stopJsonString)
            {
                Route = route;
                StopJsonString = stopJsonString;
            }
            public Route Route;
            public string StopJsonString;
        }

        private string mbtaApiDomain = "https://api-v3.mbta.com";
        private string dataDir = @"C:\Users\johnm\source\repos\BroadMBTA\MapData";
        public List<Route> Routes { get; init; }
        public List<Connection> Connections { get; init; }
        public Map(bool loadFromApi)
        {
            // read info about routes/stops from API (or file)
            var routeJsonString = LoadRoutes(loadFromApi);
            this.Routes = ConvertToRoutes(routeJsonString);
            LoadStops(loadFromApi, this.Routes);

            this.Connections = GetConnections(this.Routes);

        }

        public void Dump()
        {
            Console.WriteLine("ROUTES AND STOPS");
            foreach (var route in this.Routes)
            {
                Console.WriteLine("----------------");
                Console.WriteLine(route);
                foreach (var stop in route.Stops)
                {
                    Console.WriteLine($"\t{stop}");
                }
            }

            Console.WriteLine("CONNECTIONS");
            this.Connections.ForEach(c => Console.WriteLine(c));
        }

        private string LoadRoutes(bool loadFromApi)
        {
            if (loadFromApi)
                return LoadRoutesFromApi();
            else    
                return LoadRoutesFromFile();
        }

        private void LoadStops(bool loadFromApi, List<Route> routeList)
        {
            var routeAndStopContentList = loadFromApi ?
                LoadStopsFromApi(routeList) :
                LoadStopsFromFile(routeList);
            ConvertToStops(routeAndStopContentList);
        }

        private string LoadRoutesFromFile()
        {
            var routesFile = Path.Combine(this.dataDir, "Routes.json");
            using (var sr = new StreamReader(routesFile))
            {
                var content = sr.ReadToEnd();
                return content;
            }
        }

        private List<RouteAndStopJsonString> LoadStopsFromFile(List<Route> routeList)
        {
            var ret = new List<RouteAndStopJsonString>();
            foreach (var route in routeList)
            {
                var stopsFile = Path.Combine(this.dataDir, $"Stop_{route.ID}.json");
                using (var sr = new StreamReader(stopsFile))
                {
                    var content = sr.ReadToEnd();
                    ret.Add(new RouteAndStopJsonString(route, content));
                }
            }

            return ret;
        }

        private string LoadRoutesFromApi()
        {
            using (var client = new HttpClient())
            {
                var uriString = $"{this.mbtaApiDomain}/routes?filter[type]=0,1";
                //var uriString = $"{domain}/routes?filter[type]=0,1&include=stop";

                var uri = new Uri(uriString);

                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Add("x-api-key", "ee00aba612f74416ab32125fe634621b");

                var result = client.SendAsync(request).GetAwaiter().GetResult();
                if (!result.IsSuccessStatusCode)
                    throw new Exception($"Failed request: {result}");

                var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                var routesFile = Path.Combine(this.dataDir, "Routes.json");
                using (var sw = new StreamWriter(routesFile))
                {
                    sw.Write(content);
                }
                return content;
            }
        }

        private List<Route> ConvertToRoutes(string routeJsonContent)
        {
            var contentJson = JObject.Parse(routeJsonContent);
            if (contentJson == null) throw new Exception("Routes content is empty");
            var dataJson = contentJson["data"];
            if (dataJson == null) throw new Exception("Routes content 'data' is empty");

            var routeList = new List<Route>();
            foreach (var routeJson in dataJson)
            {
                var id = routeJson["id"]?.ToString();
                var name = routeJson["attributes"]?["long_name"]?.ToString();

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name))
                    throw new Exception("Empty id or name");

                var route = new Route(id, name);
                routeList.Add(route);
            }

            return routeList;
        }

        private List<RouteAndStopJsonString> LoadStopsFromApi(List<Route> routeList)
        {
            using (var client = new HttpClient())
            {
                var routeAndStopJsonList = new List<RouteAndStopJsonString>();
                foreach (var route in routeList)
                {
                    var uriString = $"{this.mbtaApiDomain}//stops?filter[route]={route.ID}&include=route";
                    var uri = new Uri(uriString);

                    var result = client.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri)).GetAwaiter().GetResult();
                    if (!result.IsSuccessStatusCode)
                        throw new Exception($"Failed request: {result}");

                    var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var stopsFile = Path.Combine(this.dataDir, $"Stop_{route.ID}.json");
                    using (var sw = new StreamWriter(stopsFile))
                    {
                        sw.Write(content);
                    }

                    routeAndStopJsonList.Add(new RouteAndStopJsonString(route, content));
                }

                return routeAndStopJsonList;
            }
        }

        private void ConvertToStops(List<RouteAndStopJsonString> routeAndStopContentList)
        {
            foreach (var routeAndStopContent in routeAndStopContentList)
            {
                var route = routeAndStopContent.Route;
                var content = routeAndStopContent.StopJsonString;

                var contentJson = JObject.Parse(content);
                if (contentJson == null) throw new Exception("Stops content is empty");
                var dataJson = contentJson["data"];
                if (dataJson == null) throw new Exception("Stops content 'data' is empty");

                foreach (var stopJson in dataJson)
                {
                    var id = stopJson["id"]?.ToString();
                    var name = stopJson["attributes"]?["name"]?.ToString();

                    if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name))
                        throw new Exception("Empty id or name");

                    var stop = new Stop(route, id, name);

                    route.Stops.Add(stop);
                }
            }

        }

        public static List<Connection> GetConnections(List<Route> routeList)
        {
            var connectionList = new List<Connection>();
            var workingList = new List<Route>(routeList);
            while (workingList.Count > 0)
            {
                // remove first route from list, compare with the remaining routes in list
                var routeA = workingList[0];
                workingList.RemoveAt(0);
                foreach (var routeB in workingList)
                {
                    var connections = routeA.Stops.Intersect(routeB.Stops).Select(s => new Connection(s, routeA, routeB));
                    connectionList.AddRange(connections);
                }
            }

            return connectionList;
        }

        public Stop FindStop(string routeName, string stopName)
        {
            var route = this.Routes.Single(e => e.Name == routeName);
            var stop = route.Stops.Single(s => s.Name == stopName);
            return stop;
        }

        public List<Connection> GetConnectionsForRoute(Route route)
        {
            return this.Connections.Where(c => c.Route1 == route || c.Route2 == route).ToList();
        }
    }
}
