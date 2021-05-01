using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace NarenFitnessUsers.Class
{
    public class sendMail
    {
        public void SendMail(string To, string Subject, string Text)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(To);

            mail.From = new MailAddress("sourabh9303@gmail.com");
            mail.Subject = Subject;
            string Body = Text;
            mail.Body = Body;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com"; //Or Your SMTP Server Address
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("username", "password");//Or your Smtp Email ID and Password
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
    }
}