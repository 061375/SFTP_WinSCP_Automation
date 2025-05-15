using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AQ_FTP
{
    internal class Libs
    {
        public static readonly AQHelpers.Helpers Helpers = new AQHelpers.Helpers(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
    }
}
