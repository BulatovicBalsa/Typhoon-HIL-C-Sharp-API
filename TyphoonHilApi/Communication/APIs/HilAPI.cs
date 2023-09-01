using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TyphoonHilApi.Communication.APIs
{
    internal class HilAPI : AbsractAPI
    {
        public override int ProperPort => Ports.HilApiPort;
        public HilAPI() { }

        public HilAPI(ICommunication communication):base(communication) { }


    }
}
