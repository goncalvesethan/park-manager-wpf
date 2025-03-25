using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkManagerWPF.Models
{
    public class Device
    {
        public string Brand { get; set; }
        public string Processor { get; set; }
        public long RAM { get; set; }
        public long Storage { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
        public int ParkId { get; set; }
        public int RoomId { get; set; }
    }
}
