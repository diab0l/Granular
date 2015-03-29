using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.UnitTesting;

namespace Granular.Presentation.Tests.Web
{
    class Program
    {
        static void Main()
        {
            UnitTestRunner.Run(typeof(Program).Assembly);
        }
    }
}
