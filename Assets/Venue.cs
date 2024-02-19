using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VenueExtractor
{
    class Venue
    {
        
        static async Task Main(string[] args)
        {
            string apiUrl = "https://www.wmnf.org/api/events.php?ver=20160427";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Parse the JSON response
                    JObject json = JObject.Parse(responseBody);
                    JArray eventData = (JArray)json["data"];

                    // Extract venue data
                    foreach (JToken eventDataItem in eventData)
                    {
                        string venue = eventDataItem["venue"]?.ToString();
                        if (!string.IsNullOrEmpty(venue))
                        {
                            Console.WriteLine($"Venue: {venue}");
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request Exception: {e.Message}");
                }
            }
        }
    }
}
