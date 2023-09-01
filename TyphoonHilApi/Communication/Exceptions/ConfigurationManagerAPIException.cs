using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TyphoonHilApi.Communication.Exceptions
{
    internal class ConfigurationManagerAPIException:Exception
    {
        public ConfigurationManagerAPIException()
        {
            
        }

        public ConfigurationManagerAPIException(string message):base(message) { }
    }
}
