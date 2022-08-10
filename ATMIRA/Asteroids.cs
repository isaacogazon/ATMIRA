using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ATMIRA
{
    public class AsteroidResponse {
        public string name;
        public string diameter;
        public string velocity;
        public string date;
        public string planet;
    }

    class Asteroid : AsteroidResponse
    {
        public bool is_potentially_hazardous_asteroid;
        public string estimated_diameter_min;
        public string estimated_diameter_max;
    }

}





