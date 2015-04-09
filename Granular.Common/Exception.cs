using System;
using System.Collections.Generic;
using System.Linq;

namespace Granular
{
    public class Exception : System.Exception
    {
        public Exception(string format, params object[] args) :
            base(String.Format(format, args))
        {
            //
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
