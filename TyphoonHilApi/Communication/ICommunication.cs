﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TyphoonHilApi.Communication
{
    internal interface ICommunication
    {
        JObject Request(string operation, JObject parameters);
    }
}