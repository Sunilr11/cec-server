using Amazon.Util;
using GDT.CEC.Repository.Models.AzureLabs;
using GDT.CEC.Repository.Models.PageTemplate;
using GDT.CEC.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Service.Implementation
{
    public class AzureLabService : IAzureLabService
    {
        private readonly IAzureLabsRepo _labRepo;
        private readonly IAnalyticsRepo _analyticsRepo;

        public AzureLabService(IAzureLabsRepo labRepo,IAnalyticsRepo analyticsRepo)
        {
            _labRepo = labRepo;
            _analyticsRepo = analyticsRepo;
        }

        public Task<List<AzureLab>> GetLabs(bool getActiveOnly, bool detailed)
        {
            return _labRepo.GetLabs(getActiveOnly, detailed);
        }

        public Task<List<AzureLab>> GetLabsForUser(string userOid)
        {
            return _labRepo.GetLabsForUser(userOid);
        }
        public async Task<LabDTO> GetLab(string id)
        {
            var lab=await  _labRepo.GetLab(id);
            var details =await  _analyticsRepo.GetLabAccessDetailByLab(lab.Name);
            var labDto = new LabDTO { Lab = lab, TotalDuration = details.Item2, TotalSessions = details.Item1 };
            return labDto;
        }

        public async Task<bool> CreateLabAsync(CreateLabDTO createLabDTO)
        {

            return await _labRepo.CreateLabAsync(new AzureLab
            {
                AzureLabID = Guid.NewGuid().ToString(),
                IsActive = createLabDTO.IsActive,
                ButtonText = createLabDTO.ButtonText,
                ButtonText2 = createLabDTO.ButtonText2,
                Categories =createLabDTO.Categories,
                Description =createLabDTO.Description,  
                Image=createLabDTO.Image,
                LaunchLink=createLabDTO.LaunchLink, 
                Name =createLabDTO.Name,    
                HostPoolName=createLabDTO.HostPoolName,
                TabsData =createLabDTO.TabsData
            });
        }
        public async Task<bool> UpdateLabActiveAsync(string azureid, bool isActive)
        {
            return await _labRepo.UpdateLabActiveAsync(azureid, isActive);
        }

        public async Task<bool> UpdateLabAsync(CreateLabDTO createLabDTO)
        {            
            return await _labRepo.UpdateLabAsync(new AzureLab
            {
                AzureLabID=createLabDTO.AzureLabID,
                IsActive = createLabDTO.IsActive,
                ButtonText = createLabDTO.ButtonText,
                ButtonText2 = createLabDTO.ButtonText2,
                Categories =createLabDTO.Categories,
                Description =createLabDTO.Description,
                Image =createLabDTO.Image,
                LaunchLink=createLabDTO.LaunchLink,
                Name =createLabDTO.Name,
                HostPoolName=createLabDTO.HostPoolName,
                TabsData=createLabDTO.TabsData                
            });
        }

    }
}
