using System.Text.Json.Serialization;

namespace Models
{
    public class Room
    {
        public int Id { get; set; }
        public int ParkId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }
        
        [JsonIgnore]
        public Park? Park { get; set; }
    }
}