using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TyphoonHilApi.Communication
{
    internal class PortsDto
    {
        public int SchematicApiPort { get; set; }
        public int HilApiPort { get; set; }
        public int ScadaApiPort { get; set; }
        public int PvGenApiPort { get; set; }
        public int FwApiPort { get; set; }
    }
}
