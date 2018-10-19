using System;
using System.Collections.Generic;

namespace EmailService.DbEntities
{
    public partial class Attachment
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string MailAttachmentId { get; set; }
        public string CloudUrl { get; set; }
        public string LocalUrl { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public Email Transaction { get; set; }
    }
}
