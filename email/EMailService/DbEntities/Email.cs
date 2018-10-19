using System;
using System.Collections.Generic;

namespace EmailService.DbEntities
{
    public partial class Email
    {
        public Email()
        {
            Attachment = new HashSet<Attachment>();
            Reply = new HashSet<Reply>();
        }

        public int Id { get; set; }
        public string Mailid { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Date { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<Attachment> Attachment { get; set; }
        public ICollection<Reply> Reply { get; set; }
    }
}
