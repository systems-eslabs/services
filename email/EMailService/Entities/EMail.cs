using System;
using System.Collections.Generic;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;

namespace EmailService
{
    public class Email : EmailBase
    {
        public string MailId;
        public int TransactionId;

        public bool HasValidAttachments = false;
        
    }
}
