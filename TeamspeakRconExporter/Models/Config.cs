using System;
using System.Collections.Generic;
using System.Text;

namespace TeamspeakRconExporter.Models
{
    public class Config
    {
        public string Hostname { get; set; }
        public int? Port { get; set; }
        public int ServerId { get; set; } = 1;
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
