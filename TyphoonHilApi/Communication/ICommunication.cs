using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TyphoonHilApi.Communication
{
    internal interface ICommunication
    {
        void Request(string operation, JObject parameters);
        JObject Response();
    }
}