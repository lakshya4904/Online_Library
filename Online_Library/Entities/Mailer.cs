using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Entities;
using NLog;

namespace Entities
{
    public sealed class Mailer
    {
        private readonly MailMessage _message;
        private readonly SmtpClient _smtp;
        private readonly bool isHtml;

        public Mailer()
        {
            _smtp = new SmtpClient();
            _smtp.Timeout = 10000;
            _message = new MailMessage();

            //_message.From = new MailAddress(Config.FromEmail);
            //_smtp.EnableSsl = Config.EnableSsl;
            //_smtp.UseDefaultCredentials = false;
            //_smtp.Credentials = new NetworkCredential(Config.FromEmail, Config.Password);
            //_smtp.Port = Config.Port;
            //_smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            //_smtp.Host = Config.Host;
            //isHtml = true;
            //if (!string.IsNullOrEmpty(Config.TargetName))
            //{
            //    _smtp.TargetName = Config.TargetName;

            //}
            //if (!string.IsNullOrEmpty(Config.DomainName))
            //{
            //    _smtp.Credentials = new NetworkCredential(Config.FromEmail, Config.Password, Config.DomainName);
            //}
            //else
            //{
            //    _smtp.Credentials = new NetworkCredential(Config.FromEmail, Config.Password);
            //}

        }

        public string From
        {
            get { return _message.From.ToString(); }
            set { _message.From = new MailAddress(value); }
        }

        public string ReplyTo
        {
            get { return string.Join("; ", _message.ReplyToList.Select(x => x.ToString())); }
            set { _message.ReplyToList.Add(value); }
        }

        public string Subject
        {
            get { return _message.Subject; }
            set { _message.Subject = value; }
        }

        public string Body
        {
            get { return _message.Body; }
            set { _message.Body = value; }
        }

        public string To
        {
            get
            {
#if DEBUG
                return "jharna@aavitechsolutions.com";
#endif
                return string.Join(",", _message.To.Select(x => x.Address));
            }
            set
            {
                _message.To.Clear();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _message.To.Add(value.Replace(";", ","));
                }
            }
        }

        public string CC
        {
            get
            {
#if DEBUG
                return "jharna@aavitechsolutions.com";
#endif
                return string.Join(",", _message.CC.Select(x => x.Address));
            }
            set
            {
                _message.CC.Clear();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _message.CC.Add(value.Replace(";", ","));
                }
            }
        }

        public string BCC
        {
            get
            {
#if DEBUG
                return "jharna@aavitechsolutions.com";
#endif
                return string.Join(",", _message.CC.Select(x => x.Address)); 
            }
            set
            {
                _message.Bcc.Clear();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _message.Bcc.Add(value.Replace(";", ","));
                }
            }
        }


        public bool Send()
        {
            try
            {
                _message.IsBodyHtml = isHtml;
                if (_message.To.Count > 0)
                {
                    _smtp.Send(_message);
                    _message.Dispose();
                }
                return true;
            }
            catch (Exception ex)
            {
                var logger = LogManager.GetLogger("databaseLogger");
                logger.Error(ex, $"{DateTime.Now:dd/MM/yyyy HH:mm:ss},_message={_message}");
            }
            return false;
        }

        public void AddAttachment(string fileName)
        {
            _message.Attachments.Add(new Attachment(fileName));
        }

        public void AddAttachment(Stream stream, string fileName)
        {
            _message.Attachments.Add(new Attachment(stream, fileName));
        }
    }
}