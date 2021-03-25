using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DriveAPI.Database
{
    public partial class Register
    {
        public Register()
        {
            InverseParentFolderNavigation = new HashSet<Register>();
        }

        public int Id { get; set; }
        public bool IsFolder { get; set; }
        public string Name { get; set; }
        public string FileExtension { get; set; }
        public string PathOrUrl { get; set; }
        public int? Size { get; set; }
        public int Author { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int? ParentFolder { get; set; }

        public virtual UserDrive AuthorNavigation { get; set; }
        public virtual Register ParentFolderNavigation { get; set; }
        public virtual ICollection<Register> InverseParentFolderNavigation { get; set; }
    }
}
