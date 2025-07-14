using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Repository.Models.Configurations
{
    public class AzureADConfig
    {
        public string AzureADOuthTokenBaseUrl { get; set; }
        public string AzureADOuthTokenEndpoint { get; set; }
        public string AzureADTenentID { get; set; }
        public string AzureADClientID { get; set; }
        public string AzureADClientSecret { get; set; }
        public string AzureADGrantType { get; set; }
        public string AzureADResource { get; set; }
        public string AzureADGraphBaseUrl { get; set; }
        public string AzureADUserEndpoint { get; set; }
        public string AzureADManagementTokenEndpoint { get; set; }
        public string AzureADScope { get; set; }
        public string AzureADAnalyticsScope { get; set; }
        public string AzureADSubscriptionID { get; set; }
        public string AzureADResourceGroupName { get; set; }
        public string AzureADApplicationGroupName { get; set; }
        public string AzureADRoleAssignmentID { get; set; }
        public string AzureADManagementBaseUrl { get; set; }
        public string AzureADManagementEndpoint { get; set; }
        public string AzureADRoleDefinitionID { get; set; }
        public string AzureADManagementVMroleEndpoint { get; set; }
        public string AzureADVMRoleDefinitionId { get; set; }
        public string AzureAdVMScope { get; set; }
        public string CompanyDomain { get; set; }
        public string AzureAnalyticsBaseUrl { get; set; }
        public string AzureAnalyticsEndpoint { get; set; }
    }
}
