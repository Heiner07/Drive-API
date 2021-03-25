using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveAPI.Models
{
    public class FileRegister
    {
        public string Name { get; set; }
        public string ExtensionFile { get; set; }
        public string MimeType { get; set; }
        public byte[] Bytes { get; set; }
    }
}
