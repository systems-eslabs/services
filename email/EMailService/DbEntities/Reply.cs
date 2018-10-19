using System;
using System.Collections.Generic;

namespace EmailService.DbEntities
{
    public partial class Reply
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Date { get; set; }
        public string Body { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public Email Transaction { get; set; }
    }
}
