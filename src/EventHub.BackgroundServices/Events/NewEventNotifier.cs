using System;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Emailing;
using EventHub.Organizations.Memberships;
using EventHub.Users;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.Linq;
using Volo.Abp.TextTemplating;

namespace EventHub.Events
{
    public class NewEventNotifier : ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly ITemplateRenderer _templateRenderer;
        private readonly IRepository<OrganizationMembership, Guid> _organizationMembershipsRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IAsyncQueryableExecuter _asyncExecuter;

        public NewEventNotifier(
            IEmailSender emailSender,
            ITemplateRenderer templateRenderer,
            IRepository<OrganizationMembership, Guid> organizationMembershipsRepository,
            IRepository<IdentityUser, Guid> userRepository,
            IAsyncQueryableExecuter asyncExecuter)
        {
            _emailSender = emailSender;
            _templateRenderer = templateRenderer;
            _organizationMembershipsRepository = organizationMembershipsRepository;
            _userRepository = userRepository;
            _asyncExecuter = asyncExecuter;
        }

        public async Task NotifyAsync(Event @event)
        {
            if (@event is null || @event.IsEmailSentToMembers || !@event.IsDraft)
            {
                return;
            }

            var userQueryable = await _userRepository.GetQueryableAsync();
            var organizationMembershipQueryable = await _organizationMembershipsRepository.GetQueryableAsync();
            
            var userQuery = from organizationMembership in organizationMembershipQueryable
                join user in userQueryable on organizationMembership.UserId equals user.Id
                where organizationMembership.OrganizationId == @event.OrganizationId
                select user;

            var users = await _asyncExecuter.ToListAsync(userQuery);

            foreach (var user in users)
            {
                var templateModel = new
                {
                    UserName = user.GetFullNameOrUsername(),
                    Title = @event.Title,
                    StartTime = @event.StartTime,
                    EndTime = @event.EndTime,
                    Url = @event.Url
                };

                await _emailSender.QueueAsync(
                    user.Email,
                    $"Do you attend the {@event.Title} event?",
                    await _templateRenderer.RenderAsync(EmailTemplates.NewEventCreated, templateModel)
                );
            }
        }
    }
}
