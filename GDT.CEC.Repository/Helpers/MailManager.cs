using GDT.CEC.Repository.Models.Configurations;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Repository.Helpers
{
    public class MailManager
    {
        private    IOptions<MailConfig> _mailConfig;

        public MailManager(IOptions<MailConfig> mailConfig)
        {
            _mailConfig = mailConfig;
        }
        public   void SendMail(string to,string subject,string body)
        {
            using (MailMessage m = new MailMessage())
            {
                m.To.Add(to);
                m.From = new MailAddress(_mailConfig.Value.From,_mailConfig.Value.DisplayName);
                m.Subject = subject;
                m.Body = body;
                m.IsBodyHtml = true;
                
  

                int port = _mailConfig.Value.SMTPPort;

                string server = _mailConfig.Value.SMTPServer;
               

                System.Net.Mail.SmtpClient smtpClient = new SmtpClient(server, port);
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtpClient.EnableSsl = _mailConfig.Value.EnableSSL;

                if (_mailConfig.Value.SMTPUsername != "")
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(_mailConfig.Value.SMTPUsername, _mailConfig.Value.SMTPPassword);
                }

                smtpClient.Send(m);
            }
        }
    }
}
