using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveAPI.Models
{
    public class JWTSettings
    {
        public const string JWT = "JWT";

        public string JWTKey { get; set; }
    }
}
