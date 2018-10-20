using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Services;
using Google.Apis.Requests;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mail;
using CloudStorageService = Google.Apis.Storage.v1;
using Google.Cloud.Storage.V1;
using Common;
using StorageService;
using EmailEntities = EmailService.DbEntities;
namespace EmailService
{
    public class Mail : IDisposable
    {
        static Dictionary<string, string> _settings = null;
        GmailService _service = null;

        static readonly string[] StorageScope = { CloudStorageService.StorageService.Scope.DevstorageReadWrite };

        public static string[] Scopes = {
            GmailService.Scope.GmailReadonly,
            GmailService.Scope.GmailSend,
            GmailService.Scope.GmailCompose
            };

        public Mail()
        {
            _service = initailseEmailService();
        }

        GmailService initailseEmailService()
        {
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = Config.mailClientId,
                    ClientSecret = Config.mailClientSecret
                },
                Scopes = Scopes,
            });

            var credential = new UserCredential(flow, Environment.UserName, new TokenResponse
            {
                AccessToken = Config.mailAccessToken,
                RefreshToken = Config.mailRefreshToken
            });

            var service = new GmailService(new BaseClientService.Initializer()
            {
                ApplicationName = "Email API",
                HttpClientInitializer = credential
            });

            return service;
        }

        public BaseReturn<List<Email>> getUnreadEmailsByLabel(string label)
        {
            BaseReturn<List<Email>> baseObject = new BaseReturn<List<Email>>();
            List<Email> mails = new List<Email>();
            try
            {
                var inboxlistRequest = _service.Users.Messages.List("me");
                inboxlistRequest.LabelIds = label;
                inboxlistRequest.IncludeSpamTrash = true;
                inboxlistRequest.Q = "is:unread";

                var emailListResponse = inboxlistRequest.Execute();

                if (emailListResponse != null && emailListResponse.Messages != null)
                {
                    var messages = emailListResponse.Messages.ToList();
                    messages.ForEach(message =>
                    {
                        mails.Add(getMailInfo(message));
                    });
                }
                else
                {
                    baseObject.Message = "No new messages available";
                }
                baseObject.Success = true;
                baseObject.Data = mails;
            }
            catch (Exception ex)
            {
                baseObject.Success = false;
                baseObject.Message = "Error Occured.";
            }
            return baseObject;
        }

        public BaseReturn<bool> sendMailReply(Email mail, string replymessage)
        {
            BaseReturn<bool> baseObject = new BaseReturn<bool>();
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var enc1252 = Encoding.GetEncoding(1252);

                mail.To = mail.From;
                mail.From = Config.serviceMailId;
                mail.Body = replymessage;


                var msg = new AE.Net.Mail.MailMessage
                {
                    Subject = "RE : " + mail.Subject,
                    Body = mail.Body,
                    ContentType = "text/html",
                    From = new MailAddress(mail.From)
                };

                msg.To.Add(new MailAddress(mail.To));

                msg.ReplyTo.Add(new MailAddress(mail.From));

                mail.CC.Split(',').ToList().ForEach(addr =>
                {
                    if (addr.Trim() != "")
                    {
                        msg.Cc.Add(new MailAddress(addr));
                    }
                });

                mail.BCC.Split(',').ToList().ForEach(addr =>
                {
                    if (addr.Trim() != "")
                    {
                        msg.Bcc.Add(new MailAddress(addr));
                    }
                });

                var msgStr = new StringWriter();
                msg.Save(msgStr);

                var result = _service.Users.Messages.Send(new Message
                {
                    ThreadId = mail.MailId,
                    Id = mail.MailId,
                    Raw = CommonFunctions.Base64UrlEncode(msgStr.ToString())
                }, "me").Execute();

                saveReplyMailInfo(mail);
                baseObject.Success = true;
                baseObject.Data = true;
            }
            catch (Exception ex)
            {
                baseObject.Success = false;
                baseObject.Message = "Error Occured.";

            }
            return baseObject;
        }

        public BaseReturn<List<EAttachment>> getAttachments(Email mail)
        {
            List<EAttachment> attachments = null;
            BaseReturn<List<EAttachment>> baseObject = new BaseReturn<List<EAttachment>>();
            try
            {
                attachments = new List<EAttachment>();

                foreach (EAttachment attachment in mail.Attachments)
                {
                    var attachmentData = getAttachmentData(mail, attachment.AttachmentId, attachment.Filename);
                    attachments.Add(attachmentData);
                }
                baseObject.Success = true;
                baseObject.Data = attachments;
                saveMailAttachments(mail.TransactionId, attachments);

            }
            catch (Exception ex)
            {
                baseObject.Success = false;
                baseObject.Message = "Error Occured.";
            }
            return baseObject;
        }

        public BaseReturn<List<EAttachment>> getAttachments(Email mail, List<EAttachmentRequest> attachmentRequest)
        {
            List<EAttachment> attachments = null;
            BaseReturn<List<EAttachment>> baseObject = new BaseReturn<List<EAttachment>>();
            try
            {
                attachments = new List<EAttachment>();

                foreach (EAttachmentRequest attachment in attachmentRequest)
                {
                    var attachmentData = getAttachmentData(mail, attachment.AttachmentId, attachment.Filename);
                    attachments.Add(attachmentData);
                }
                baseObject.Success = true;
                baseObject.Data = attachments;
                saveMailAttachments(mail.TransactionId, attachments);

            }
            catch (Exception ex)
            {
                baseObject.Success = false;
                baseObject.Message = "Error Occured.";
            }
            return baseObject;
        }


        Email getMailInfo(Message message)
        {
            var emailInfoRequest = _service.Users.Messages.Get("me", message.Id);
            var emailInfoResponse = emailInfoRequest.Execute();
            var PayLoadHeader = emailInfoResponse.Payload.Headers;
            var PayloadPart = emailInfoResponse.Payload.Parts;

            List<EAttachment> attachments = new List<EAttachment>();

            var parts = PayloadPart.Where(part => Convert.ToString(part.Filename) != "").ToList();

            parts.ForEach(part =>
            {
                attachments.Add(new EAttachment
                {
                    AttachmentId = part.Body.AttachmentId,
                    Filename = part.Filename
                });
            });

            Email mail = new Email()
            {
                MailId = emailInfoResponse.Id,
                From = PayLoadHeader.Single(mPart => mPart.Name == "From").Value,
                To = Config.serviceMailId,
                Subject = PayLoadHeader.Single(mPart => mPart.Name == "Subject").Value,
                CC = PayLoadHeader.FirstOrDefault(mPart => mPart.Name == "Cc") == null ? "" : PayLoadHeader.FirstOrDefault(mPart => mPart.Name == "Cc").Value,
                // BCC = PayLoadHeader.FirstOrDefault(mPart => mPart.Name == "Bcc") == null ? "" : PayLoadHeader.FirstOrDefault(mPart => mPart.Name == "Bcc").Value,
                Date = PayLoadHeader.Single(mPart => mPart.Name == "Date").Value,
                Attachments = attachments
            };



            saveMailInfo(mail);
            markMailUnread(emailInfoResponse.Id);

            return mail;
        }

        // make method async
        EAttachment getAttachmentData(Email mail, string attachmentId, string filename)
        {
            EAttachment attachment = null;
            MessagePartBody attachPart = _service.Users.Messages.Attachments.Get("me", mail.MailId, attachmentId).Execute();
            byte[] attachmentData = CommonFunctions.FromBase64ForString(attachPart.Data);


            StorageService.Storage storageService = new StorageService.Storage("Email/" + mail.MailId);

            EStorageRequest request = new EStorageRequest
            {
                FileName = filename,
                isSaveLocal = true
            };

            BaseReturn<EStorageResponse> storageResponse = storageService.BinaryUpload(request, attachmentData);

            attachment = new EAttachment
            {
                AttachmentId = attachmentId,
                Filename = filename,
                localUrl = storageResponse.Data.LocalFilePath,
                CloudUrl = storageResponse.Data.BucketFilePath,
                Data = attachPart != null ? attachmentData : null
            };
            return attachment;
        }

        bool markMailUnread(string mailId)
        {
            var markAsReadRequest = new ModifyThreadRequest { RemoveLabelIds = new[] { "UNREAD" } };
            _service.Users.Threads.Modify(markAsReadRequest, "me", mailId).Execute();
            return true;
        }

        #region Async Operations

        protected async Task<List<Message>> getEmailsByLabelAsync(string label)
        {
            List<Message> messages = null;

            var inboxlistRequest = _service.Users.Messages.List("me");
            inboxlistRequest.LabelIds = label;
            inboxlistRequest.IncludeSpamTrash = true;
            inboxlistRequest.Q = "is:unread";

            var emailListResponse = await inboxlistRequest.ExecuteAsync();

            if (emailListResponse != null && emailListResponse.Messages != null)
            {
                messages = emailListResponse.Messages.ToList();
            }
            return messages;
        }

        public async Task markMailUnreadAsync(string id)
        {
            var markAsReadRequest = new ModifyThreadRequest { RemoveLabelIds = new[] { "UNREAD" } };
            await _service.Users.Threads.Modify(markAsReadRequest, "me", id).ExecuteAsync();
        }


        #endregion


        #region Save Mail details

        void saveMailInfo(Email mail)
        {
            var EmailEntity = new EMailRepository<EmailEntities.Email>().Add(new EmailEntities.Email()
            {
                Mailid = mail.MailId,
                From = mail.From,
                To = mail.To,
                Cc = mail.CC,
                Bcc = mail.BCC,
                Subject = mail.Subject,
                Date = mail.Date,
                CreatedBy = 0
            });

            mail.TransactionId = EmailEntity.Id;
        }

        void saveReplyMailInfo(Email mail)
        {
            new EMailRepository<EmailEntities.Reply>().Add(new EmailEntities.Reply()
            {
                TransactionId = mail.TransactionId,
                From = mail.From,
                To = mail.To,
                Cc = mail.CC,
                Bcc = mail.BCC,
                Subject = mail.Subject,
                Date = mail.Date,
                Body = mail.Body,
                CreatedBy = 0
            });


        }

        void saveMailAttachments(int transactionId, List<EAttachment> attachments)
        {
            List<EmailEntities.Attachment> lstEmailAttachments = attachments.Select(attachment => new EmailEntities.Attachment()
            {
                TransactionId = transactionId,
                MailAttachmentId = attachment.AttachmentId,
                CloudUrl = attachment.CloudUrl,
                LocalUrl = attachment.localUrl,
                CreatedBy = 0
            }).ToList();

            new EMailRepository<EmailEntities.Attachment>().AddRange(lstEmailAttachments);
        }

        #endregion


        public void Dispose()
        {
            // Console.WriteLine("mail service is disposed");
        }






    }
}