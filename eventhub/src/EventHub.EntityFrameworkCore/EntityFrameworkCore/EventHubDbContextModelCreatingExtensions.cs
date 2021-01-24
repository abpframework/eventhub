using EventHub.Events;
using EventHub.Organizations;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

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

                b.HasIndex(x => x.Name);
                b.HasIndex(x => x.DisplayName);
            });

            builder.Entity<Event>(b =>
            {
                b.ToTable(EventHubConsts.DbTablePrefix + "Events", EventHubConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(x => x.Title).IsRequired().HasMaxLength(EventConsts.MaxTitleLength);
                b.Property(x => x.Description).IsRequired().HasMaxLength(EventConsts.MaxDescriptionLength);
                b.Property(x => x.UrlCode).IsRequired().HasMaxLength(EventConsts.UrlCodeLength);
                b.Property(x => x.Url).IsRequired().HasMaxLength(EventConsts.MaxUrlLength);

                b.HasIndex(x => new {x.OrganizationId, x.StartTime});
                b.HasIndex(x => x.StartTime);
                b.HasIndex(x => x.UrlCode);
            });
        }
    }
}
