using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace EventHub.Organizations
{
    public class OrganizationAppService : EventHubAppService, IOrganizationAppService
    {
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly OrganizationManager _organizationManager;

        public OrganizationAppService(
            IRepository<Organization, Guid> organizationRepository,
            OrganizationManager organizationManager)
        {
            _organizationRepository = organizationRepository;
            _organizationManager = organizationManager;
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

            return new PagedResultDto<OrganizationInListDto>(
                organizationCount,
                ObjectMapper.Map<List<Organization>, List<OrganizationInListDto>>(organizations)
            );
        }

        public async Task<OrganizationProfileDto> GetProfileAsync(string name)
        {
            var organization = await _organizationRepository.GetAsync(o => o.Name == name);
            return ObjectMapper.Map<Organization, OrganizationProfileDto>(organization);
        }

        [Authorize]
        public async Task<ListResultDto<OrganizationInListDto>> GetMyOrganizationsAsync()
        {
            var currentUserId = CurrentUser.GetId();
            var query = (await _organizationRepository.GetQueryableAsync()).Where(o => o.OwnerUserId == currentUserId);
            var organizations = await AsyncExecuter.ToListAsync(query);

            return new ListResultDto<OrganizationInListDto>(
                ObjectMapper.Map<List<Organization>, List<OrganizationInListDto>>(organizations)
            );
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
                throw new AbpAuthorizationException();
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
    }
}
