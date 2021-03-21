using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace EventHub.Organizations
{
    public class OrganizationAppService : EventHubAppService, IOrganizationAppService
    {
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly OrganizationManager _organizationManager;
        private readonly IBlobContainer<OrganizationProfilePictureContainer> _organizationBlobContainer;

        public OrganizationAppService(
            IRepository<Organization, Guid> organizationRepository,
            OrganizationManager organizationManager,
            IBlobContainer<OrganizationProfilePictureContainer> organizationBlobContainer)
        {
            _organizationRepository = organizationRepository;
            _organizationManager = organizationManager;
            _organizationBlobContainer = organizationBlobContainer;
        }

        [Authorize]
        public async Task CreateAsync(CreateOrganizationDto input)
        {
            var organization = await _organizationManager.CreateAsync(
                CurrentUser.GetId(),
                input.Name,
                input.DisplayName,
                input.Description
            );

            await _organizationRepository.InsertAsync(organization);
        }

        public async Task<PagedResultDto<OrganizationInListDto>> GetListAsync(PagedResultRequestDto input)
        {
            var organizationCount = await _organizationRepository.GetCountAsync();
            var organizations = await _organizationRepository.GetPagedListAsync(
                input.SkipCount,
                input.MaxResultCount,
                nameof(Organization.DisplayName)
            );

            var organizationDto = ObjectMapper.Map<List<Organization>, List<OrganizationInListDto>>(organizations);
            
            foreach (var organization in organizationDto)
            {
                organization.ProfilePictureContent = await GetProfilePictureAsync(organization.Id);
            }

            return new PagedResultDto<OrganizationInListDto>(
                organizationCount,
                organizationDto
            );
        }

        public async Task<OrganizationProfileDto> GetProfileAsync(string name)
        {
            var organization = await _organizationRepository.GetAsync(o => o.Name == name);
            var organizationProfileDto = ObjectMapper.Map<Organization, OrganizationProfileDto>(organization);
           
            organizationProfileDto.ProfilePictureContent = await GetProfilePictureAsync(organizationProfileDto.Id);

            return organizationProfileDto;
        }

        [Authorize]
        public async Task<ListResultDto<OrganizationInListDto>> GetMyOrganizationsAsync()
        {
            var currentUserId = CurrentUser.GetId();
            var query = (await _organizationRepository.GetQueryableAsync()).Where(o => o.OwnerUserId == currentUserId);
            var organizations = await AsyncExecuter.ToListAsync(query);

            var organizationDto = ObjectMapper.Map<List<Organization>, List<OrganizationInListDto>>(organizations);

            foreach (var organization in organizationDto)
            {
                organization.ProfilePictureContent = await GetProfilePictureAsync(organization.Id);
            }
            
            return new ListResultDto<OrganizationInListDto>(organizationDto);
        }

        public async Task<bool> IsOrganizationOwnerAsync(Guid organizationId)
        {
            return CurrentUser.Id.HasValue && await _organizationRepository.AnyAsync(x => x.Id == organizationId && x.OwnerUserId == CurrentUser.Id.Value);
        }

        [Authorize]
        public async Task UpdateAsync(Guid id, UpdateOrganizationDto input)
        {
            var organization = await _organizationRepository.GetAsync(id);

            if (organization.OwnerUserId != CurrentUser.GetId())
            {
                //TODO: throw AbpAuthorizationException instead 
                throw new BusinessException(EventHubErrorCodes.NotAuthorizedToUpdateOrganizationProfile)
                    .WithData("OrganizationName", organization.DisplayName);
            }

            organization.SetDisplayName(input.DisplayName);
            organization.SetDescription(input.Description);
            organization.Website = input.Website;
            organization.TwitterUsername = input.TwitterUsername;
            organization.GitHubUsername = input.GitHubUsername;
            organization.InstagramUsername = input.InstagramUsername;
            organization.FacebookUsername = input.FacebookUsername;
            organization.MediumUsername = input.MediumUsername;
            
            await _organizationRepository.UpdateAsync(organization);
        }

        [Authorize]
        public async Task SaveProfilePictureAsync(Guid id, byte[] bytes)
        {
            var organization = await _organizationRepository.GetAsync(x => x.Id == id);
            
            if (organization.OwnerUserId != CurrentUser.GetId())
            {
                //TODO: throw AbpAuthorizationException instead
                throw new BusinessException(EventHubErrorCodes.NotAuthorizedToUpdateOrganizationProfile)
                    .WithData("Name", organization.DisplayName);
            }
            
            var blobName = id.ToString();
            
            await _organizationBlobContainer.SaveAsync(blobName, bytes, overrideExisting: true);
        }

        private async Task<byte[]> GetProfilePictureAsync(Guid id)
        {
            var blobName = id.ToString();

            return await _organizationBlobContainer.GetAllBytesOrNullAsync(blobName);
        }
    }
}
