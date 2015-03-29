using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.UnitTesting;

namespace Granular.Host.Tests.Web
{
    class Program
    {
        static void Main()
        {
            UnitTestRunner.Run(typeof(Program).Assembly);
        }
    }
}
