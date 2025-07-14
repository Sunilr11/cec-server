using Amazon.Util.Internal;
using GDT.CEC.Repository.Models.AzureLabs;
using GDT.CEC.Repository.Models.PageTemplate;
using Microsoft.AspNetCore.Http;

namespace GDT.CEC.Service.DTOs
{
    public class LabDTO
    {
        public AzureLab Lab { get; set; }
        public int TotalSessions { get; set; }
        public TimeSpan TotalDuration { get; set; }
    }
    public class CreateLabDTO
    {
        public string AzureLabID { get; set; }
        public string Name { get; set; }
        public string HostPoolName { get; set; }
        public string Description { get; set; }
        public string LaunchLink { get; set; }
        public string ButtonText { get; set; }
        public string ButtonText2 { get; set; }
        public string Image { get; set; }
        public List<TabDataLab> TabsData { get; set; }
        public List<CategoryLab> Categories { get; set; }
        public bool IsActive { get; set; }
    }

    public class LabActiveDTO
    {
        public string Id { get; set; }
        public bool IsActive { get; set; }
    }


    public class CategoryDTO
    {
        public string Header { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public List<TabData> TabsData { get; set; }
    }
    public class CategoryTabDTO
    {
        public string Key { get; set; }
        public List<TabData> TabsData { get; set; }
    }



    public class TabDataDTO
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }

    }
}
