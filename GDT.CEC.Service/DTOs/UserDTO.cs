using GDT.CEC.Repository.Models.AzureLabs;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Service.DTOs
{
    public class UserDTO
    {
    }
    public class ActiveDTO
    {
        public string Id { get; set; }
        public bool IsActive { get; set; }
    }

    public class RoleDTO
    {
        public List<string> Ids { get; set; }
        public int RoleId { get; set; }
    }
    public class AzureADUserDTO
    {
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string JobTitle { get; set; }
        public string Mail { get; set; }
        public string MobilePhone { get; set; }
        public string Surname { get; set; }
        public string UserPrincipalName { get; set; }
        public string ID { get; set; }
        public string Role { get; set; }
        
        public List<AzureLabDTO> Labs { get; set; }
    }
    public class AzureLabDTO
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }


    public class UserStatics
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int Disabled { get; set; }
        public int Rejected { get; set; }
        public int Pending { get; set; }
    }
}
