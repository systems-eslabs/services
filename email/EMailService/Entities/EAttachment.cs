using System;

namespace EmailService
{
    public class EAttachment : EAttachmentRequest
    {
        public byte[] Data;
        public string localUrl;
        public string CloudUrl;
    }

}