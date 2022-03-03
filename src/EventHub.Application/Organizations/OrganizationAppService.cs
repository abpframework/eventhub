using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Organizations.Memberships;
using EventHub.Organizations.PaymentRequests;
using EventHub.Organizations.Plans;
using EventHub.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace EventHub.Organizations
{
    public class OrganizationAppService : EventHubAppService, IOrganizationAppService
    {
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly IOrganizationMembershipRepository  _organizationMembershipsRepository;
        private readonly OrganizationManager _organizationManager;
        private readonly IBlobContainer<OrganizationProfilePictureContainer> _organizationBlobContainer;
        private readonly IUserRepository _userRepository;
        private readonly IOptions<PlanInfoOptions> _planInfoOptions;

        public OrganizationAppService(
            IRepository<Organization, Guid> organizationRepository,
            IOrganizationMembershipRepository organizationMembershipsRepository,
            OrganizationManager organizationManager,
            IBlobContainer<OrganizationProfilePictureContainer> organizationBlobContainer,
            IUserRepository userRepository,
            IOptions<PlanInfoOptions> planInfoOptions)
        {
            _organizationRepository = organizationRepository;
            _organizationMembershipsRepository = organizationMembershipsRepository;
            _organizationManager = organizationManager;
            _organizationBlobContainer = organizationBlobContainer;
            _userRepository = userRepository;
            _planInfoOptions = planInfoOptions;
        }

        [Authorize]
        public async Task<OrganizationDto> CreateAsync(CreateOrganizationDto input)
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

            if (input.ProfilePictureStreamContent != null && input.ProfilePictureStreamContent.ContentLength > 0)
            {
                await SaveProfilePictureAsync(organization.Id, input.ProfilePictureStreamContent);
            }
            
            return ObjectMapper.Map<Organization, OrganizationDto>(organization);
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

            return organizationProfileDto;
        }

        public async Task<ListResultDto<OrganizationInListDto>> GetOrganizationsByUserIdAsync(Guid userId)
        {
            var query = (await _organizationRepository.GetQueryableAsync()).Where(o => o.OwnerUserId == userId);
            var organizations = await AsyncExecuter.ToListAsync(query);

            var organizationDto = ObjectMapper.Map<List<Organization>, List<OrganizationInListDto>>(organizations);

            return new ListResultDto<OrganizationInListDto>(organizationDto);
        }

        public async Task<bool> IsOrganizationOwnerAsync(Guid organizationId)
        {
            return CurrentUser.Id.HasValue && await _organizationRepository
                .AnyAsync(x => x.Id == organizationId && x.OwnerUserId == CurrentUser.GetId());
        }

        [Authorize]
        public async Task UpdateAsync(Guid id, UpdateOrganizationDto input)
        {
            var organization = await _organizationRepository.GetAsync(id);

            if (organization.OwnerUserId != CurrentUser.GetId())
            {
                throw new AbpAuthorizationException(EventHubErrorCodes.NotAuthorizedToUpdateOrganizationProfile)
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

            if (input.ProfilePictureStreamContent != null && input.ProfilePictureStreamContent.ContentLength > 0)
            {
                await SaveProfilePictureAsync(organization.Id, input.ProfilePictureStreamContent);
            }

            await _organizationRepository.UpdateAsync(organization);
        }
        
        public async Task<IRemoteStreamContent> GetProfilePictureAsync(Guid id)
        {
            var blobName = id.ToString();

            var pictureContent = await _organizationBlobContainer.GetOrNullAsync(blobName);
            
            if (pictureContent is null)
            {
                return null;
            }

            return new RemoteStreamContent(pictureContent, blobName);
        }

        [Authorize]
        public async Task<List<PlanInfoDefinitionDto>> GetPlanInfosAsync()
        {
            var planInfoDefinitions = _planInfoOptions.Value.GetPlanInfos();
            return await Task.FromResult(ObjectMapper.Map<List<PlanInfoDefinition>, List<PlanInfoDefinitionDto>>(planInfoDefinitions));
        } 

        private async Task SaveProfilePictureAsync(Guid id, IRemoteStreamContent streamContent)
        {
            var blobName = id.ToString();

            await _organizationBlobContainer.SaveAsync(blobName, streamContent.GetStream(), overrideExisting: true);
        }
    }
}
