using System.Text.Json.Serialization;

namespace Models
{
    public class Park
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
    }
}