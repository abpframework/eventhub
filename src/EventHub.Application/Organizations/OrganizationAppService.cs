using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Organizations.Memberships;
using EventHub.Users;
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
        private readonly IRepository<OrganizationMembership, Guid>  _organizationMembershipsRepository;
        private readonly OrganizationManager _organizationManager;
        private readonly IBlobContainer<OrganizationProfilePictureContainer> _organizationBlobContainer;
        private readonly IRepository<AppUser, Guid> _userRepository;

        public OrganizationAppService(
            IRepository<Organization, Guid> organizationRepository,
            IRepository<OrganizationMembership, Guid> organizationMembershipsRepository,
            OrganizationManager organizationManager,
            IBlobContainer<OrganizationProfilePictureContainer> organizationBlobContainer, 
            IRepository<AppUser, Guid> userRepository)
        {
            _organizationRepository = organizationRepository;
            _organizationMembershipsRepository = organizationMembershipsRepository;
            _organizationManager = organizationManager;
            _organizationBlobContainer = organizationBlobContainer;
            _userRepository = userRepository;
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

            organization.Website = input.Website;
            organization.TwitterUsername = input.TwitterUsername;
            organization.GitHubUsername = input.GitHubUsername;
            organization.FacebookUsername = input.FacebookUsername;
            organization.InstagramUsername = input.InstagramUsername;
            organization.MediumUsername = input.MediumUsername;
            
            await _organizationRepository.InsertAsync(organization, true);
            
            if (input.ProfilePictureContent != null && input.ProfilePictureContent.Length > 0)
            {
                await SaveProfilePictureAsync(organization.Id, input.ProfilePictureContent);
            }
        }

        public async Task<PagedResultDto<OrganizationInListDto>> GetListAsync(OrganizationListFilterDto input)
        {
            var organizationQueryable = await _organizationRepository.GetQueryableAsync();
            var organizationMemberQueryable = await _organizationMembershipsRepository.GetQueryableAsync();

            var query = organizationQueryable;
            
            if (input.RegisteredUserId.HasValue)
            {
                var registeredOrganization = organizationMemberQueryable
                    .Where(x => x.UserId == input.RegisteredUserId)
                    .Select(x => x.OrganizationId);
                
                var organizationIds = await AsyncExecuter.ToListAsync(registeredOrganization);
                query = query.Where(x => organizationIds.Contains(x.Id));
            }

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = query.PageBy(input);

            var organizationDto = ObjectMapper
                .Map<List<Organization>, List<OrganizationInListDto>>(await AsyncExecuter.ToListAsync(query));
            
            foreach (var organization in organizationDto)
            {
                organization.ProfilePictureContent = await GetProfilePictureAsync(organization.Id);
            }

            return new PagedResultDto<OrganizationInListDto>(
                totalCount,
                organizationDto
            );
        }

        public async Task<OrganizationProfileDto> GetProfileAsync(string name)
        {
            var organization = await _organizationRepository.GetAsync(o => o.Name == name);
            var organizationProfileDto = ObjectMapper.Map<Organization, OrganizationProfileDto>(organization);

            var owner = await _userRepository.GetAsync(u => u.Id == organization.OwnerUserId);
            organizationProfileDto.OwnerUserName = owner.UserName;
            organizationProfileDto.OwnerEmail = owner.Email;
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
