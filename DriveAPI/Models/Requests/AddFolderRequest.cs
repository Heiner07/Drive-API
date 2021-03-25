using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DriveAPI.Models.Requests
{
    public class AddFolderRequest
    {
        [Required]
        public string Name { get; set; }

        public int? ParentFolder { get; set; }
    }
}
