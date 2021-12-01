using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using EventHub.Admin.Permissions;
using EventHub.Organizations;
using EventHub.Users;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;

namespace EventHub.Admin.Organizations
{
    [Authorize(EventHubPermissions.Organizations.Default)]
    public class OrganizationAppService : EventHubAdminAppService, IOrganizationAppService
    {
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly IUserRepository _identityUserRepository;
        private readonly IBlobContainer<OrganizationProfilePictureContainer> _organizationBlobContainer;

        public OrganizationAppService(
            IRepository<Organization, Guid> organizationRepository,
            IUserRepository identityUserRepository,
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

            return await CreateOrganizationProfileDto(organization);
        }

        public async Task<OrganizationProfileDto> GetByNameAsync(string name)
        {
            var organization = await _organizationRepository.FindAsync(x => x.Name.ToLower() == name.ToLower());
            if (organization is null)
            {
                throw new BusinessException(EventHubErrorCodes.OrganizationNotFound)
                    .WithData("OrganizationName", name);
            }

            return await CreateOrganizationProfileDto(organization);
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

            if (input.ProfilePictureStreamContent != null && input.ProfilePictureStreamContent.ContentLength > 0)
            {
                await SaveCoverImageAsync(organization.Id, input.ProfilePictureStreamContent);
            }
            else
            {
                await DeleteCoverImageAsync(blobName: id.ToString());
            }

            await _organizationRepository.UpdateAsync(organization);

            return ObjectMapper.Map<Organization, OrganizationProfileDto>(organization);
        }

        [Authorize(EventHubPermissions.Organizations.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            await _organizationRepository.DeleteAsync(id);
        }

        private async Task<OrganizationProfileDto> CreateOrganizationProfileDto(Organization organization)
        {
            var dto = ObjectMapper.Map<Organization, OrganizationProfileDto>(organization);

            var user = await _identityUserRepository.GetAsync(organization.OwnerUserId);
            dto.OwnerUserName = user.UserName;
            dto.OwnerEmail = user.Email;

            return dto;
        }

        [AllowAnonymous]
        public async Task<IRemoteStreamContent> GetCoverImageAsync(Guid id)
        {
            var blobName = id.ToString();
            var coverImageStream = await _organizationBlobContainer.GetOrNullAsync(blobName);

            if (coverImageStream is null)
            {
                return null;
            }

            return new RemoteStreamContent(coverImageStream, blobName);
        }

        private async Task SaveCoverImageAsync(Guid id, IRemoteStreamContent coverImageContent)
        {
            var blobName = id.ToString();

            await _organizationBlobContainer.SaveAsync(blobName, coverImageContent.GetStream(), overrideExisting: true);
        }

        private async Task DeleteCoverImageAsync(string blobName)
        {
            await _organizationBlobContainer.DeleteAsync(blobName);
        }
    }
}