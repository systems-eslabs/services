
using System;
using EmailEntities = EmailService.DbEntities;

namespace EmailService
{
    public class MailService
    {

        public int getEmailCountByEmailId(string emailId)
        {
           return new EMailRepository<EmailEntities.Email>().GetCount(x => x.From.ToLower() == emailId.ToLower() && x.CreatedDate.ToString("dd-MMM-yy") == DateTime.Today.ToString("dd-MMM-yy"));
        }

    }
}