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
using Google.Apis.Storage.v1;
using Google.Cloud.Storage.V1;


namespace mailLibrary
{
    public abstract class Mail
    {
        static Dictionary<string, string> _settings = null;
        GmailService _service = null;
        List<Email> _mails = null;

        static readonly string[] StorageScope = { StorageService.Scope.DevstorageReadWrite };

        public static string[] Scopes = {
            GmailService.Scope.GmailReadonly,
            GmailService.Scope.GmailSend,
            GmailService.Scope.GmailCompose
            };

        protected Mail()
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

        Email getMail(string mailId)
        {
            return _mails.Where(x => x.MailId == mailId).FirstOrDefault();
        }

        public BaseReturn<List<Email>> getUnreadEmailsByLabel(string label)
        {
            BaseReturn<List<Email>> baseObject = new BaseReturn<List<Email>>();
            List<Email> mails = null;
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

                    _mails = mails;
                    baseObject.Success = true;
                    baseObject.Data = mails;
                }
            }
            catch (Exception ex)
            {
                baseObject.Success = false;
                baseObject.Message = "Error Occured.";
                baseObject.Exception = ex;
            }
            return baseObject;
        }

        public BaseReturn<bool> sendMailReply(string mailId, string replymessage)
        {
            BaseReturn<bool> baseObject = new BaseReturn<bool>();
            try
            {
                var mail = getMail(mailId);
                if (mail != null)
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    var enc1252 = Encoding.GetEncoding(1252);

                    var msg = new AE.Net.Mail.MailMessage
                    {
                        Subject = "RE : " + mail.Subject,
                        Body = replymessage,
                        ContentType = "text/html",
                        From = new MailAddress(Config.serviceMailId)
                    };

                    msg.To.Add(new MailAddress(mail.From));

                    msg.ReplyTo.Add(new MailAddress(Config.serviceMailId));

                    mail.CC.Split(',').ToList().ForEach(addr =>
                    {
                        msg.Cc.Add(new MailAddress(addr));
                    });

                    mail.BCC.Split(',').ToList().ForEach(addr =>
                    {
                        msg.Bcc.Add(new MailAddress(addr));
                    });

                    var msgStr = new StringWriter();
                    msg.Save(msgStr);

                    var result = _service.Users.Messages.Send(new Message
                    {
                        ThreadId = mail.MailId,
                        Id = mail.MailId,
                        Raw = Common.Base64UrlEncode(msgStr.ToString())
                    }, "me").Execute();

                    baseObject.Success = true;
                    baseObject.Data = true;
                }
                else
                {
                    baseObject.Success = false;
                    baseObject.Message = "mailId not available";
                }
            }
            catch (Exception ex)
            {
                baseObject.Success = false;
                baseObject.Message = "Error Occured.";
                baseObject.Exception = ex;

            }
            return baseObject;
        }

        public BaseReturn<List<EAttachment>> getAttachments(string mailId)
        {
            List<EAttachment> attachments = null;
            BaseReturn<List<EAttachment>> baseObject = new BaseReturn<List<EAttachment>>();
            try
            {
                attachments = new List<EAttachment>();
                var mail = getMail(mailId);

                if (mail != null)
                {
                    foreach (EAttachment attachment in mail.Attachments)
                    {
                        var attachmentData = getAttachmentData(mailId, attachment.AttachmentId, attachment.Filename);
                        attachments.Add(attachmentData);
                    }
                     baseObject.Success = true;
                baseObject.Data = attachments;
                }
                else
                {
                    baseObject.Success = false;
                    baseObject.Message = "mailId not available";
                }
               
            }
            catch (Exception ex)
            {
                baseObject.Success = false;
                baseObject.Message = "Error Occured.";
                baseObject.Exception = ex;
            }
            return baseObject;
        }

        public BaseReturn<List<EAttachment>> getAttachments(string mailId, List<EAttachmentRequest> attachmentRequest)
        {
            List<EAttachment> attachments = null;
            BaseReturn<List<EAttachment>> baseObject = new BaseReturn<List<EAttachment>>();
            try
            {
                attachments = new List<EAttachment>();
                var mail = getMail(mailId);

                if (mail != null)
                {
                    foreach (EAttachment attachment in attachmentRequest)
                    {
                        var attachmentData = getAttachmentData(mailId, attachment.AttachmentId, attachment.Filename);
                        attachments.Add(attachmentData);
                    }
                     baseObject.Success = true;
                baseObject.Data = attachments;
                }
                else
                {
                    baseObject.Success = false;
                    baseObject.Message = "mailId not available";
                }
               
            }
            catch (Exception ex)
            {
                baseObject.Success = false;
                baseObject.Message = "Error Occured.";
                baseObject.Exception = ex;
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
                CC = PayLoadHeader.Single(mPart => mPart.Name == "Cc").Value, // handle if cc not mentioned
                BCC = PayLoadHeader.Single(mPart => mPart.Name == "Bcc").Value, // handle if cc not mentioned
                Attachments = attachments
            };
            markMailUnread(emailInfoResponse.Id);

            return mail;
        }

        // make method async
        EAttachment getAttachmentData(string mailId, string AttachmentId, string Filename)
        {
            EAttachment attachment = null;
            MessagePartBody attachPart = _service.Users.Messages.Attachments.Get("me", mailId, AttachmentId).Execute();

            attachment = new EAttachment
            {
                AttachmentId = AttachmentId,
                Filename = Filename,
                localUrl = "",
                CloudUrl = "",
                Data = attachPart != null ? Common.FromBase64ForString(attachPart.Data) : null
            };
            return attachment;
        }

        bool markMailUnread(string mailId)
        {
            var markAsReadRequest = new ModifyThreadRequest { RemoveLabelIds = new[] { "UNREAD" } };
            _service.Users.Threads.Modify(markAsReadRequest, "me", mailId).Execute();
            return true;
        }

        void sendMail(string mailId, string replymessage)
        {
            var mail = getMail(mailId);

            if (mail != null)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var enc1252 = Encoding.GetEncoding(1252);

                var msg = new AE.Net.Mail.MailMessage
                {
                    Subject = mail.Subject,
                    Body = mail.Body,
                    ContentType = "text/html",
                    From = new MailAddress(Config.serviceMailId)
                };

                mail.To.Split(',').ToList().ForEach(addr =>
                {
                    msg.To.Add(new MailAddress(addr));
                });

                mail.ReplyTo.Split(',').ToList().ForEach(addr =>
                {
                    msg.ReplyTo.Add(new MailAddress(addr));
                });

                mail.CC.Split(',').ToList().ForEach(addr =>
                {
                    msg.Cc.Add(new MailAddress(addr));
                });

                mail.BCC.Split(',').ToList().ForEach(addr =>
                {
                    msg.Bcc.Add(new MailAddress(addr));
                });

                var msgStr = new StringWriter();
                msg.Save(msgStr);

                var result = _service.Users.Messages.Send(new Message
                {
                    ThreadId = mail.MailId,
                    Id = mail.MailId,
                    Raw = Common.Base64UrlEncode(msgStr.ToString())
                }, "me").Execute();
            }
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


        public List<Email> getUnreadEmailsByLabelAsync(string label)
        {
            List<Email> mails = null;
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

                    _mails = mails;
                }

            }
            catch (Exception ex)
            {
                //Some error Occured -- return false in basereturn Issucess
            }
            return mails;
        }


        #endregion


        #region Save Mail details

        void saveMailInfo(Email mail)
        {
            //Save to firebase
        }

        #endregion








    }
}