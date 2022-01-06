using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMI.Services.Common.Setting
{
        public class ConfigSettings
        {
            public JwtSettings JwtSettings { get; set; }
        }
    public class JwtSettings
    {
        public string Key { get; set; }
    }

}
