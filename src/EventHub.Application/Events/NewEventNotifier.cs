using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using EventHub.Emailing;
using EventHub.Organizations;
using EventHub.Organizations.Memberships;
using EventHub.Users;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.TextTemplating;

namespace EventHub.Events
{
    public class NewEventNotifier : ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly ITemplateRenderer _templateRenderer;
        private readonly IRepository<OrganizationMembership, Guid>  _organizationMembershipsRepository;
        private readonly IRepository<AppUser, Guid> _userRepository;
        
        public NewEventNotifier(
            IEmailSender emailSender, 
            ITemplateRenderer templateRenderer, 
            IRepository<OrganizationMembership, Guid> organizationMembershipsRepository, 
            IRepository<AppUser, Guid> userRepository)
        {
            _emailSender = emailSender;
            _templateRenderer = templateRenderer;
            _organizationMembershipsRepository = organizationMembershipsRepository;
            _userRepository = userRepository;
        }
        
        public async Task NotifyAsync(
            Organization organization,
            Event @event)
        {
            if (organization is null || @event is null)
            {
                return;
            }

            var organizationMemberQueryable = await _organizationMembershipsRepository.GetQueryableAsync();

            var organizationMembers = await organizationMemberQueryable
                .Where(x => x.OrganizationId == organization.Id)
                .ToListAsync();

            foreach (var member in organizationMembers)
            {
                var user = await _userRepository.FindAsync(member.UserId);
                
                if (user is null)
                {
                    continue;
                }

                var model = new
                {
                    UserName = user.GetFullNameOrUsername(),
                    OrganizationName = organization.Name,
                    Title = @event.Title,
                    StartTime = @event.StartTime,
                    EndTime = @event.EndTime,
                    Url = @event.Url
                };
                
                await _emailSender.QueueAsync(
                    user.Email,
                    $"Do you attend the {@event.Title} event?",
                    await _templateRenderer.RenderAsync(EmailTemplates.NewEventCreated, model)
                );      
            }
        }
    }
}