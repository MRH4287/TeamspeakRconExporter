using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TeamspeakRconExporter
{
    interface ICollector
    {
        Task Collect();
    }
}
