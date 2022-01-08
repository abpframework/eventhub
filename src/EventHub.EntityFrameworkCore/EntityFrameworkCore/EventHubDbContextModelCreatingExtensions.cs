using EventHub.Countries;
using EventHub.Events;
using EventHub.Events.Registrations;
using EventHub.Organizations;
using EventHub.Organizations.Memberships;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;

namespace EventHub.EntityFrameworkCore
{
    public static class EventHubDbContextModelCreatingExtensions
    {
        public static void ConfigureEventHub(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure your own tables/entities inside here */

            builder.Entity<Organization>(b =>
            {
                b.ToTable(EventHubConsts.DbTablePrefix + "Organizations", EventHubConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(x => x.Name).IsRequired().HasMaxLength(OrganizationConsts.MaxNameLength);
                b.Property(x => x.DisplayName).IsRequired().HasMaxLength(OrganizationConsts.MaxDisplayNameLength);
                b.Property(x => x.Description).IsRequired().HasMaxLength(OrganizationConsts.MaxDescriptionNameLength);
                b.HasOne<IdentityUser>().WithMany().HasForeignKey(x => x.OwnerUserId).IsRequired().OnDelete(DeleteBehavior.NoAction);

                b.HasIndex(x => x.Name);
                b.HasIndex(x => x.DisplayName);
            });
            
            builder.Entity<OrganizationMembership>(b =>
            {
                b.ToTable(EventHubConsts.DbTablePrefix + "OrganizationMemberships", EventHubConsts.DbSchema);

                b.ConfigureByConvention();

                b.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).IsRequired().OnDelete(DeleteBehavior.NoAction);
                b.HasOne<IdentityUser>().WithMany().HasForeignKey(x => x.UserId).IsRequired().OnDelete(DeleteBehavior.NoAction);

                b.HasIndex(x => new {x.OrganizationId, x.UserId});
            });

            builder.Entity<Event>(b =>
            {
                b.ToTable(EventHubConsts.DbTablePrefix + "Events", EventHubConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(x => x.Title).IsRequired().HasMaxLength(EventConsts.MaxTitleLength);
                b.Property(x => x.Description).IsRequired().HasMaxLength(EventConsts.MaxDescriptionLength);
                b.Property(x => x.UrlCode).IsRequired().HasMaxLength(EventConsts.UrlCodeLength);
                b.Property(x => x.Url).IsRequired().HasMaxLength(EventConsts.MaxUrlLength);
                b.Property(x => x.OnlineLink).HasMaxLength(EventConsts.MaxOnlineLinkLength);
                b.Property(x => x.City).HasMaxLength(EventConsts.MaxCityLength);
                b.Property(x => x.Language).HasMaxLength(EventConsts.MaxLanguageLength);

                b.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).IsRequired().OnDelete(DeleteBehavior.NoAction);
               
                b.HasOne<Country>().WithMany().HasForeignKey(x => x.CountryId).OnDelete(DeleteBehavior.NoAction);

                b.Property(x => x.IsTimingChangeEmailSent).HasDefaultValue(true);

                b.HasIndex(x => new {x.OrganizationId, x.StartTime});
                b.HasIndex(x => x.StartTime);
                b.HasIndex(x => x.UrlCode);
                b.HasIndex(x => new {x.IsRemindingEmailSent, x.StartTime});
                b.HasIndex(x => x.IsEmailSentToMembers);

                b.HasMany(x => x.Tracks).WithOne().IsRequired().HasForeignKey(x => x.EventId);
            });

            builder.Entity<Track>(b =>
            {
                b.ToTable(EventHubConsts.DbTablePrefix + "EventTracks", EventHubConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(x => x.Name).IsRequired().HasMaxLength(TrackConsts.MaxNameLength);
                
                b.HasMany(x => x.Sessions).WithOne().IsRequired().HasForeignKey(x => x.TrackId);
            });
            
            builder.Entity<Session>(b =>
            {
                b.ToTable(EventHubConsts.DbTablePrefix + "EventSessions", EventHubConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(x => x.Title).IsRequired().HasMaxLength(SessionConsts.MaxTitleLength);
                b.Property(x => x.Description).IsRequired().HasMaxLength(SessionConsts.MaxDescriptionLength);
                b.Property(x => x.Language).IsRequired().HasMaxLength(SessionConsts.MaxLanguageLength);
                
                b.HasMany(x => x.Speakers).WithOne().IsRequired().HasForeignKey(x => x.SessionId);
            });
            
            builder.Entity<Speaker>(b =>
            {
                b.ToTable(EventHubConsts.DbTablePrefix + "EventSpeakers", EventHubConsts.DbSchema);

                b.ConfigureByConvention();

                b.HasKey(x => new {x.SessionId, x.UserId});

                b.HasOne<IdentityUser>().WithMany().HasForeignKey(x => x.UserId).IsRequired();
            });

            builder.Entity<EventRegistration>(b =>
            {
                b.ToTable(EventHubConsts.DbTablePrefix + "EventRegistrations", EventHubConsts.DbSchema);

                b.ConfigureByConvention();

                b.HasOne<Event>().WithMany().HasForeignKey(x => x.EventId).IsRequired().OnDelete(DeleteBehavior.NoAction);
                b.HasOne<IdentityUser>().WithMany().HasForeignKey(x => x.UserId).IsRequired().OnDelete(DeleteBehavior.NoAction);

                b.HasIndex(x => new {x.EventId, x.UserId});
            });
            
            builder.Entity<Country>(b =>
            {
                b.ToTable(EventHubConsts.DbTablePrefix + "Countries", EventHubConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(x => x.Name).IsRequired().HasMaxLength(CountryConsts.MaxNameLength);

                b.HasIndex(x => new {x.Name});
            });
        }
    }
}
