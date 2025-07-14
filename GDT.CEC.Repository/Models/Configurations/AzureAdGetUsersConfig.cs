using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Repository.Models.Configurations
{
    public  class AzureAdGetUsersConfig
    {
        public string AzureADOuthTokenBaseUrl { get; set; }
        public string AzureADTenentID { get; set; }
        public string AzureADClientID { get; set; }
        public string AzureADClientSecret { get; set; }
    }
}
