using Belletrix.Entity.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Belletrix.Entity.ViewModel
{
    public class DocumentViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Size { get; set; }
        public string MimeType { get; set; }
        public byte[] Content { get; set; }

        public DateTime Created { get; set; }
        public UserModel CreatedBy { get; set; }

        public DateTime? LastModified { get; set; }
        public UserModel LastModifiedBy { get; set; }

        public DateTime? Deleted { get; set; }
        public UserModel DeletedBy { get; set; }
    }

    public class AddNewDocumentViewModel
    {
        /// <summary>
        /// File details from local machine uploading to the server.
        /// </summary>
        [Required]
        public HttpPostedFileBase File { get; set; }

        [Required]
        public int ActivityLogId { get; set; }
    }
}
