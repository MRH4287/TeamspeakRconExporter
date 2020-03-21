using System;
using System.Collections.Generic;
using System.Text;

namespace TeamspeakRconExporter
{
    public static class Consts
    {
        public const string Prefix = "teamspeak_";

        public static string GetMetricName(string name)
            => $"{Prefix}{name}";
    }
}
