using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TyphoonHilApi.Communication.APIs
{
    internal class ConfigurationManagerAPI : AbsractAPI
    {
        public override int ProperPort => Ports.ScadaApiPort;
    }
}
