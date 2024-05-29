using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Helper
{
    public class AppSettings
    {
        public string SecretKey { get; set; }
        public int RefreshTokenTTL { get; set; }
    }
}
