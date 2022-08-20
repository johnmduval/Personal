using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroadMBTA
{
    public class Stop
    {
        public Stop(Route parentRoute, string id, string name)
        {
            if (parentRoute == null)
                throw new ArgumentNullException("parentRoute");
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("id");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            this.ParentRoute = parentRoute;
            this.ID = id;
            this.Name = name;
        }
        public Route ParentRoute { get; init; }
        public string ID { get; init; }
        public string Name { get; init; }
        public override string ToString()
        {
            return $"Stop({this.ID}, '{this.Name}')";
        }

        public override bool Equals(object? obj)
        {
            var otherStop = obj as Stop;

            if (otherStop == null)
            {
                return false;
            }
            return this.ID == otherStop.ID;
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }
        public static void GetStops(HttpClient client, string domain, IEnumerable<Route> routeList)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (string.IsNullOrWhiteSpace(domain)) throw new ArgumentNullException("domain");
            if (routeList == null) throw new ArgumentNullException("routeList");

            // NOTE: strange but true: can't seem to include=route unless there is only 1 route in the route filter, so this does not work:
            // var routeFilter = string.Join(",", routeList.Select(e => e.ID));
            // var uriString = $"{domain}//stops?filter[route]={routeFilter}&include=route";
            // Instead, need to be chatty & issue multiple requests in loop...

            foreach (var route in routeList)
            {
                var uriString = $"{domain}//stops?filter[route]={route.ID}&include=route";
                var uri = new Uri(uriString);

                var result = client.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri)).GetAwaiter().GetResult();
                if (!result.IsSuccessStatusCode)
                    throw new Exception($"Failed request: {result}");

                var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
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

    }
}
