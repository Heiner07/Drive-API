using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DriveAPI.Models.Requests
{
    public class ChangeRegisterNameRequest
    {
        [Required]
        public int RegisterId { get; set; }

        [Required]
        public string NewName { get; set; }
    }
}
