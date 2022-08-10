using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ATMIRA;
using UnitTestProject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;

namespace ATMIRA.Controllers
{
    [TestClass]
    [ApiController]
    [Route("[controller]")]
    public class AsteroidsController : ControllerBase
    {
        [TestMethod]
        [HttpGet(Name = "Asteroids")]
        public async Task<dynamic> Get(int days)
        {
            try
            {
                String url = GetURL(days);
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url);
                    if (days == 0)
                    {
                        return response.ReasonPhrase = "You must enter a day";
                    } else
                    if (days < 1 || days > 7)
                    {
                        return response.ReasonPhrase;
                    }
                    else
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            List<Asteroid> asteroids = getParsedAsteroids(await response.Content.ReadAsStringAsync());
                            List<AsteroidResponse> mostDangerousAsteroids = getMostDangerousAsteroids(asteroids);
                            var jsonString = JsonConvert.SerializeObject(mostDangerousAsteroids);
                            return jsonString;
                        }
                        else
                        {
                            //days debe ser entre 1 y 7, de lo contrario obtener error de la API de la nasa.(Devuelve error controlado)
                            return response.ReasonPhrase;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }             
        }

        [TestMethod]
        private String GetURL(int days)
        {
            String star_date = DateTime.Now.ToString("yyyy-MM-dd");
            String end_date = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd");
            String url = "https://api.nasa.gov/neo/rest/v1/feed?start_date=" + star_date + "&end_date=" + end_date + "&api_key=zdUP8ElJv1cehFM0rsZVSQN7uBVxlDnu4diHlLSb";
            return url;
        }


        private List<Asteroid> getParsedAsteroids(dynamic response)
        {
            var data = JObject.Parse(response);
            List<Asteroid> nearObjects = new List<Asteroid>(); ;
            dynamic nearObjectsDates = data.GetValue("near_earth_objects");
            foreach (var date in nearObjectsDates)
            {
                var asteroids = date;
                foreach (var asteroid in asteroids)
                {
                    for (int i = 0; i < asteroid.Count; i++)
                    {
                        decimal diametro = (Convert.ToDecimal(asteroid[i].estimated_diameter.kilometers.estimated_diameter_min) + Convert.ToDecimal(asteroid[i].estimated_diameter.kilometers.estimated_diameter_max)) / 2;
                        nearObjects.Add(new Asteroid
                        {
                            is_potentially_hazardous_asteroid = asteroid[i].is_potentially_hazardous_asteroid,
                            estimated_diameter_min = asteroid[i].estimated_diameter.kilometers.estimated_diameter_min,
                            estimated_diameter_max = asteroid[i].estimated_diameter.kilometers.estimated_diameter_max,
                            name = asteroid[i].name,
                            diameter = diametro.ToString(),
                            velocity = asteroid[i].close_approach_data[0].relative_velocity.kilometers_per_hour,
                            date = asteroid[i].close_approach_data[0].close_approach_date,
                            planet = asteroid[i].close_approach_data[0].orbiting_body
                        });
                    }

                }
            }
            return nearObjects;
        }

        private List<AsteroidResponse> getMostDangerousAsteroids(List<Asteroid> asteroids)
        {
            var hazardousAsteroids = asteroids.Where(asteroid => asteroid.is_potentially_hazardous_asteroid && asteroid.planet == "Earth");
            hazardousAsteroids = hazardousAsteroids.OrderByDescending(x => x.diameter);
            hazardousAsteroids = hazardousAsteroids.Take(3);

            List<AsteroidResponse> mostDangerousAsteroids = new List<AsteroidResponse>();
            foreach (var asteroid in hazardousAsteroids)
            {
                AsteroidResponse almacenar = new AsteroidResponse
                {
                    name = asteroid.name,
                    diameter = asteroid.diameter,
                    velocity = asteroid.velocity,
                    date = asteroid.date,
                    planet = asteroid.planet
                };
                mostDangerousAsteroids.Add(almacenar);
            }
            return mostDangerousAsteroids;
        }

    }
}