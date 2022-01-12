using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Zenzinger_Eshop_Web.Models.Extensions
{
    public static class SendEmailExtension
    {
        public static void SendEmail(string from, string password, string to, string message, string subject, string host, int port, string file)
        {
            try
            {
                MailMessage email = new MailMessage();
                email.From = new MailAddress(from);
                email.To.Add(to);
                email.Subject = subject;
                email.Body = message;
                SmtpClient smtp = new SmtpClient(host, port);
                smtp.UseDefaultCredentials = false;
                NetworkCredential nc = new NetworkCredential(from, password);
                smtp.Credentials = nc;
                smtp.EnableSsl = true;
                email.IsBodyHtml = true;
                email.Priority = MailPriority.Normal;
                email.BodyEncoding = Encoding.UTF8;

                if (file.Length > 0)
                {
                    Attachment attachment;
                    attachment = new Attachment(file);
                    email.Attachments.Add(attachment);
                }

                // smtp.Send(email);
                smtp.SendCompleted += SendCompletedCallBack;
                string userstate = "sending ...";
                smtp.SendAsync(email, userstate);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private static void SendCompletedCallBack(object sender,AsyncCompletedEventArgs e) {
            string result = "";
            if (e.Cancelled)
            {    
                Console.WriteLine(string.Format("{0} send canceled.", e.UserState),"Message");
            }
            else if (e.Error != null)
            {
                Console.WriteLine(string.Format("{0} {1}", e.UserState, e.Error), "Message");
            }
            else {
                Console.WriteLine("Your message is sended", "Message");
            }

        }

    }
}