using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace NUMLPay_Api.CreateModel
{
    public class emailService
    {
        public bool Send(string receiverEmail, string subject, string body, string attachmentPath = null)
        {
            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("numlfeepay@gmail.com"); 
                    mailMessage.To.Add(receiverEmail);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;

                    if (!string.IsNullOrEmpty(attachmentPath))
                    {
                        Attachment attachment = new Attachment(attachmentPath);
                        mailMessage.Attachments.Add(attachment);
                    }

                    using (SmtpClient smtpClient = new SmtpClient())
                    {
                        smtpClient.Host = "smtp.gmail.com";
                        smtpClient.Port = 587; 
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.EnableSsl = true;
                        smtpClient.Credentials = new NetworkCredential("numlfeepay@gmail.com", "picu pymj tzms vchk");

                        smtpClient.Send(mailMessage);
                    }
                }

                return true; 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false; 
            }
        }
    }
}