using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ParkManagerWPF.Models
{
    public class Device
    {
        public int Id { get; set; }
        public int ParkId { get; set; }
        public int RoomId { get; set; }
        public string Name { get; set; }
        public string? Brand { get; set; }
        public string? Processor { get; set; }
        public long? RAM { get; set; }
        public long? Storage { get; set; }
        public string MacAddress { get; set; }
        public string? IpAddress { get; set; }
        public bool IsOnline { get; set; } = false;

        [JsonIgnore]
        public Park Park { get; set; }

        [JsonIgnore]
        public Room Room { get; set; }

        [JsonIgnore]
        public string OnlineLabel { get; set; }
    }
}
