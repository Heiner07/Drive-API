using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DriveAPI.Models.Requests
{
    public class MoveRegisterRequest
    {
        [Required]
        public int RegisterId { get; set; }

        public int? DestinyFolderId { get; set; }
    }
}
