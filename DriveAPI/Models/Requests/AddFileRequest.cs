using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DriveAPI.Models.Requests
{
    public class AddFileRequest
    {
        public int? ParentFolder { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}
