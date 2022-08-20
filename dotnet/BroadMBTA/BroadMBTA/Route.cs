using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroadMBTA
{
    public class Route
    {
        public Route(string id, string name)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("id");
            if (string.IsNullOrWhiteSpace(name))    
                throw new ArgumentNullException("name");

            this.ID = id;
            this.Name = name;
            this.Stops = new List<Stop>();
        }
        public string ID { get; init; }
        public string Name { get; init; }

        public List<Stop> Stops { get; init; }
        public override string ToString()
        {
            return $"Route({this.ID}, '{this.Name}')";
        }

        public static IEnumerable<Route> GetRoutes(HttpClient client, string domain)
        {
            var uriString = $"{domain}/routes?filter[type]=0,1";
            //var uriString = $"{domain}/routes?filter[type]=0,1&include=stop";

            var uri = new Uri(uriString);

            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("x-api-key", "ee00aba612f74416ab32125fe634621b");

            var result = client.SendAsync(request).GetAwaiter().GetResult();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Failed request: {result}");

            var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var contentJson = JObject.Parse(content);
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


        public IEnumerable<Stop> GetStopsBetween(Stop startStop, Stop endStop)
        {
            var startIndex = this.Stops.FindIndex(s => s.Equals(startStop));
            var endIndex = this.Stops.FindIndex(s => s.Equals(endStop));
            
            var minIndex = Math.Min(startIndex, endIndex);
            var count = Math.Abs(startIndex - endIndex) + 1;
            var stops = this.Stops.GetRange(minIndex, count);
            if (startIndex > endIndex)
            {
                stops.Reverse();
            }
            return stops;
        }
    }
}
