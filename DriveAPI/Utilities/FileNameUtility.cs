using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DriveAPI.Utilities
{
    public class FileNameUtility
    {
        public static bool FileFolderNameIsValid(string name)
        {
            return Regex.Match(name, "^([a-zA-Z 0-9-_@&%=.áéíóúÁÉÍÓÚ!¡nÑ)(])+$").Success;
        }
    }
}
