using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TyphoonHilApi.Communication.Exceptions
{
    internal class HilAPIException : Exception
    {
        public HilAPIException() { }

        public HilAPIException(string message) : base(message) { }
    }
}
