using System;
using System.Collections.Generic;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;

namespace EmailService
{
    public class EmailBase
    {
        public string From;
        public string To;
        public string Subject;
        public string CC;
        public string BCC;
        public string ReplyTo;
        public string Body;
        public List<EAttachment> Attachments;
    }
}