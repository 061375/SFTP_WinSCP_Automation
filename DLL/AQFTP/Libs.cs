using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AQFTP
{
    public class Libs
    {
        public static readonly AQHelpers.Helpers Helpers = new AQHelpers.Helpers(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
    }
}
