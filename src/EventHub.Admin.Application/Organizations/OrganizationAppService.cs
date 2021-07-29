using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using EventHub.Admin.Permissions;
using EventHub.Organizations;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace EventHub.Admin.Organizations
{
    [Authorize(EventHubPermissions.Organizations.Default)]
    public class OrganizationAppService : EventHubAdminAppService, IOrganizationAppService
    {
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly IRepository<IdentityUser, Guid> _identityUserRepository;
        private readonly IBlobContainer<OrganizationProfilePictureContainer> _organizationBlobContainer;

        public OrganizationAppService(
            IRepository<Organization, Guid> organizationRepository,
            IRepository<IdentityUser, Guid> identityUserRepository,
            IBlobContainer<OrganizationProfilePictureContainer> organizationBlobContainer)
        {
            _organizationRepository = organizationRepository;
            _identityUserRepository = identityUserRepository;
            _organizationBlobContainer = organizationBlobContainer;
        }

        public async Task<PagedResultDto<OrganizationInListDto>> GetListAsync(OrganizationListFilterDto input)
        {
            var query = await _organizationRepository.GetQueryableAsync();

            if (!input.Name.IsNullOrWhiteSpace())
            {
                input.Name = input.Name.ToLower();
                query = query.Where(o => o.Name.ToLower().Contains(input.Name));
            }

            if (!input.DisplayName.IsNullOrWhiteSpace())
            {
                input.DisplayName = input.DisplayName.ToLower();
                query = query.Where(o => o.DisplayName.ToLower().Contains(input.DisplayName));
            }

            if (input.MinMemberCount != null)
            {
                query = query.Where(o => o.MemberCount >= input.MinMemberCount);
            }

            if (input.MaxMemberCount != null)
            {
                query = query.Where(o => o.MemberCount <= input.MaxMemberCount);
            }

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = query.OrderBy(string.IsNullOrWhiteSpace(input.Sorting)
                ? OrganizationConsts.DefaultSorting
                : input.Sorting);

            query = query.PageBy(input);

            var organizationListDto = ObjectMapper
                .Map<List<Organization>, List<OrganizationInListDto>>(await AsyncExecuter.ToListAsync(query));

            return new PagedResultDto<OrganizationInListDto>(totalCount, organizationListDto);
        }

        public async Task<OrganizationProfileDto> GetAsync(Guid id)
        {
            var organization = await _organizationRepository.GetAsync(id);

            var dto = ObjectMapper.Map<Organization, OrganizationProfileDto>(organization);

            var user = await _identityUserRepository.GetAsync(organization.OwnerUserId);
            dto.OwnerUserName = user.UserName;
            dto.OwnerEmail = user.Email;

            dto.ProfilePictureContent = await GetCoverImageAsync(organization.Id);

            return dto;
        }

        [Authorize(EventHubPermissions.Organizations.Update)]
        public async Task<OrganizationProfileDto> UpdateAsync(Guid id, UpdateOrganizationDto input)
        {
            var organization = await _organizationRepository.GetAsync(id);

            organization.SetDisplayName(input.DisplayName);
            organization.SetDescription(input.Description);

            organization.Website = input.Website;
            organization.TwitterUsername = input.TwitterUsername;
            organization.GitHubUsername = input.GitHubUsername;
            organization.FacebookUsername = input.FacebookUsername;
            organization.InstagramUsername = input.InstagramUsername;
            organization.MediumUsername = input.MediumUsername;

            if (input.ProfilePictureContent?.Length > 0)
            {
                await SaveCoverImageAsync(organization.Id, input.ProfilePictureContent);
            }
            else
            {
                await DeleteCoverImageAsync(organization.Id);
            }

            await _organizationRepository.UpdateAsync(organization);

            return ObjectMapper.Map<Organization, OrganizationProfileDto>(organization);
        }

        [Authorize(EventHubPermissions.Organizations.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            await _organizationRepository.DeleteAsync(id);
        }

        private async Task<byte[]> GetCoverImageAsync(Guid id)
        {
            var blobName = id.ToString();

            return await _organizationBlobContainer.GetAllBytesOrNullAsync(blobName);
        }

        private async Task SaveCoverImageAsync(Guid id, byte[] coverImageContent)
        {
            var blobName = id.ToString();

            await _organizationBlobContainer.SaveAsync(blobName, coverImageContent, overrideExisting: true);
        }

        private async Task DeleteCoverImageAsync(Guid id)
        {
            var blobName = id.ToString();

            await _organizationBlobContainer.DeleteAsync(blobName);
        }
    }
}