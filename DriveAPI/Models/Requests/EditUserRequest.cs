using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DriveAPI.Models.Requests
{
    public class EditUserRequest
    {
        public string Name { get; set; }

        public string Lastname { get; set; }

        [Required]
        public string Password { get; set; }

        public string NewPassword { get; set; }
    }
}
