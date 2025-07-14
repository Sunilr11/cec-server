using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Repository.Models.Configurations
{
    public class MailConfig
    {
        public string From { get; set; }
        public string  DisplayName { get; set; }
        public string SMTPServer { get; set; }
        public int SMTPPort { get; set; }
        public bool EnableSSL { get; set; }
        public string SMTPUsername { get; set; }
        public string SMTPPassword { get; set; }

        public string EmailTemplateImagePath { get; set; }
    }
}
